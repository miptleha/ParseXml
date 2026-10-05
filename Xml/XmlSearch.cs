using ParseXml.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace ParseXml.Xml
{
    public static class XmlSearch
    {
        static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ---------- Публичный API ----------

        /// <summary>
        /// Преобразует человекочитаемый запрос в XPath-строку (namespace-агностичную).
        /// Пример: "MessageKind=IPS%" -> "//*[local-name()='MessageKind' and starts-with(., 'IPS')]"
        /// </summary>
        public static string ToXPath(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return "true()";

            Node ast = new Parser(query).Parse();
            return XPathBuilder.BuildBoolean(ast);
        }

        /// <summary>
        /// Возвращает значение первого узла, найденного по XPath.
        /// null — если не найдено.
        /// </summary>
        public static string Find(string xmlPath, string xpath)
        {
            if (string.IsNullOrWhiteSpace(xpath))
                return null;

            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            return Find(doc, xpath);
        }

        public static string Find(XmlDocument doc, string xpath)
        {
            if (string.IsNullOrWhiteSpace(xpath)) 
                return null;

            XPathNavigator navigator = doc.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select(xpath);
            return iterator.MoveNext() ? iterator.Current.Value : null;
        }

        /// <summary>
        /// true, если XPath-выражение на документе вычисляется в true
        /// (или, если это node-set, набор непустой).
        /// </summary>
        public static bool Filter(string xmlPath, string xpath)
        {
            if (string.IsNullOrWhiteSpace(xpath)) 
                return true;

            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            return Filter(doc, xpath);
        }

        public static bool Filter(XmlDocument doc, string xpath)
        {
            if (string.IsNullOrWhiteSpace(xpath))
                return true;

            XPathNavigator navigator = doc.CreateNavigator();
            object result = navigator.Evaluate(xpath);

            if (result is bool) return (bool)result;
            if (result is XPathNodeIterator) return ((XPathNodeIterator)result).MoveNext();
            if (result is double)
            {
                double d = (double)result;
                return d != 0 && !double.IsNaN(d);
            }
            if (result is string) return !string.IsNullOrEmpty((string)result);
            return false;
        }

        // ---------- AST ----------

        internal abstract class Node { }

        internal sealed class Segment
        {
            private readonly string _name;
            private readonly bool _isAttribute;

            public string Name { get { return _name; } }
            public bool IsAttribute { get { return _isAttribute; } }

            public Segment(string name, bool isAttribute)
            {
                _name = name;
                _isAttribute = isAttribute;
            }
        }

        internal sealed class PathNode : Node
        {
            private readonly List<Segment> _segments;
            public List<Segment> Segments { get { return _segments; } }
            public PathNode(List<Segment> segments) { _segments = segments; }
        }

        internal sealed class ComparisonNode : Node
        {
            private readonly PathNode _path;
            private readonly string _pattern;

            public PathNode Path { get { return _path; } }
            public string Pattern { get { return _pattern; } }

            public ComparisonNode(PathNode path, string pattern)
            {
                _path = path;
                _pattern = pattern;
            }
        }

        internal sealed class AndNode : Node
        {
            private readonly Node _left;
            private readonly Node _right;

            public Node Left { get { return _left; } }
            public Node Right { get { return _right; } }

            public AndNode(Node l, Node r) { _left = l; _right = r; }
        }

        internal sealed class OrNode : Node
        {
            private readonly Node _left;
            private readonly Node _right;

            public Node Left { get { return _left; } }
            public Node Right { get { return _right; } }

            public OrNode(Node l, Node r) { _left = l; _right = r; }
        }

        // ---------- Парсер ----------

        internal sealed class Parser
        {
            private readonly string _s;
            private int _pos;

            public Parser(string s)
            {
                _s = s ?? string.Empty;
                _pos = 0;
            }

            public Node Parse()
            {
                SkipWs();
                if (_pos >= _s.Length)
                    throw new ArgumentException("Пустое выражение.");

                Node node = ParseOr();
                SkipWs();
                if (_pos < _s.Length)
                    throw new ArgumentException(string.Format(
                        "Неожиданный символ '{0}' на позиции {1}.", _s[_pos], _pos));
                return node;
            }

            private Node ParseOr()
            {
                Node left = ParseAnd();
                while (true)
                {
                    int save = _pos;
                    SkipWs();
                    if (MatchKeyword("or"))
                    {
                        Node right = ParseAnd();
                        left = new OrNode(left, right);
                    }
                    else
                    {
                        _pos = save;
                        break;
                    }
                }
                return left;
            }

            private Node ParseAnd()
            {
                Node left = ParseAtom();
                while (true)
                {
                    int save = _pos;
                    SkipWs();
                    if (MatchKeyword("and"))
                    {
                        Node right = ParseAtom();
                        left = new AndNode(left, right);
                    }
                    else
                    {
                        _pos = save;
                        break;
                    }
                }
                return left;
            }

            private Node ParseAtom()
            {
                SkipWs();
                if (_pos < _s.Length && _s[_pos] == '(')
                {
                    _pos++;
                    Node node = ParseOr();
                    SkipWs();
                    if (_pos >= _s.Length || _s[_pos] != ')')
                        throw new ArgumentException(string.Format(
                            "Ожидалась ')' на позиции {0}.", _pos));
                    _pos++;
                    return node;
                }
                return ParseComparison();
            }

            private Node ParseComparison()
            {
                PathNode path = ParsePath();
                SkipWs();
                if (_pos < _s.Length && _s[_pos] == '=')
                {
                    _pos++;
                    string pattern = ParsePattern();
                    return new ComparisonNode(path, pattern);
                }
                return path;
            }

            private PathNode ParsePath()
            {
                List<Segment> segments = new List<Segment>();
                segments.Add(ParseSegment());
                while (true)
                {
                    SkipWs();
                    if (_pos < _s.Length && _s[_pos] == '.')
                    {
                        _pos++;
                        segments.Add(ParseSegment());
                    }
                    else break;
                }
                return new PathNode(segments);
            }

            private Segment ParseSegment()
            {
                SkipWs();
                bool isAttr = false;
                if (_pos < _s.Length && _s[_pos] == '@')
                {
                    _pos++;
                    isAttr = true;
                }
                string name = ParseIdentifier();
                return new Segment(name, isAttr);
            }

            private string ParseIdentifier()
            {
                SkipWs();
                int start = _pos;
                while (_pos < _s.Length && IsIdentChar(_s[_pos])) _pos++;
                if (_pos == start)
                    throw new ArgumentException(string.Format(
                        "Ожидался идентификатор на позиции {0}.", _pos));
                return _s.Substring(start, _pos - start);
            }

            private string ParsePattern()
            {
                SkipWs();
                if (_pos >= _s.Length)
                    throw new ArgumentException("Ожидался шаблон после '='.");

                char c = _s[_pos];
                if (c == '"' || c == '\'')
                {
                    _pos++;
                    int start = _pos;
                    while (_pos < _s.Length && _s[_pos] != c) _pos++;
                    if (_pos >= _s.Length)
                        throw new ArgumentException("Не закрыта кавычка в шаблоне.");
                    string val = _s.Substring(start, _pos - start);
                    _pos++;
                    return val;
                }

                int pStart = _pos;
                while (_pos < _s.Length && IsPatternChar(_s[_pos])) _pos++;
                if (_pos == pStart)
                    throw new ArgumentException(string.Format(
                        "Ожидался шаблон на позиции {0}.", _pos));
                return _s.Substring(pStart, _pos - pStart);
            }

            private static bool IsIdentChar(char c)
            {
                return char.IsLetterOrDigit(c) || c == '_' || c == '-';
            }

            private static bool IsPatternChar(char c)
            {
                return !char.IsWhiteSpace(c) && c != '(' && c != ')' && c != '=';
            }

            private void SkipWs()
            {
                while (_pos < _s.Length && char.IsWhiteSpace(_s[_pos])) _pos++;
            }

            private bool MatchKeyword(string kw)
            {
                if (_pos + kw.Length > _s.Length) return false;
                for (int i = 0; i < kw.Length; i++)
                    if (_s[_pos + i] != kw[i]) return false;

                int end = _pos + kw.Length;
                if (end < _s.Length && IsIdentChar(_s[end])) return false;
                _pos = end;
                return true;
            }
        }

        // ---------- Построение XPath ----------

        internal static class XPathBuilder
        {
            public static string BuildBoolean(Node node)
            {
                PathNode p = node as PathNode;
                if (p != null) return BuildPath(p);

                ComparisonNode c = node as ComparisonNode;
                if (c != null) return BuildComparison(c);

                AndNode a = node as AndNode;
                if (a != null)
                    return "(" + BuildBoolean(a.Left) + " and " + BuildBoolean(a.Right) + ")";

                OrNode o = node as OrNode;
                if (o != null)
                    return "(" + BuildBoolean(o.Left) + " or " + BuildBoolean(o.Right) + ")";

                throw new InvalidOperationException("Неизвестный узел AST.");
            }

            public static string BuildPath(PathNode path)
            {
                List<Segment> segs = path.Segments;
                StringBuilder sb = new StringBuilder("//");
                for (int i = 0; i < segs.Count; i++)
                {
                    Segment seg = segs[i];
                    bool isLast = i == segs.Count - 1;

                    if (seg.IsAttribute && !isLast)
                        throw new ArgumentException(
                            "Атрибут (@...) может быть только последним сегментом.");

                    if (i > 0) sb.Append("//");   // было '/'

                    if (seg.IsAttribute)
                        sb.Append("@*[local-name()='").Append(EscapeName(seg.Name)).Append("']");
                    else
                        sb.Append("*[local-name()='").Append(EscapeName(seg.Name)).Append("']");
                }
                return sb.ToString();
            }

            private static string BuildComparison(ComparisonNode c)
            {
                List<Segment> segs = c.Path.Segments;
                StringBuilder sb = new StringBuilder("//");
                string cond = BuildPatternCondition(c.Pattern);

                for (int i = 0; i < segs.Count; i++)
                {
                    Segment seg = segs[i];
                    bool isLast = i == segs.Count - 1;

                    if (seg.IsAttribute && !isLast)
                        throw new ArgumentException(
                            "Атрибут (@...) может быть только последним сегментом.");

                    if (i > 0) sb.Append("//");   // было '/'

                    if (seg.IsAttribute)
                    {
                        sb.Append("@*[local-name()='")
                          .Append(EscapeName(seg.Name))
                          .Append("' and ").Append(cond).Append("]");
                    }
                    else
                    {
                        sb.Append("*[local-name()='")
                          .Append(EscapeName(seg.Name)).Append("'");
                        if (isLast)
                            sb.Append(" and ").Append(cond);
                        sb.Append("]");
                    }
                }
                return sb.ToString();
            }

            private static string BuildPatternCondition(string pattern)
            {
                if (pattern == "%") return "true()";

                int n = pattern.Length;
                bool startsPct = pattern[0] == '%';
                bool endsPct = pattern[n - 1] == '%';

                int mid = 0;
                int from = startsPct ? 1 : 0;
                int to = endsPct ? n - 1 : n;
                for (int i = from; i < to; i++)
                    if (pattern[i] == '%') mid++;

                if (mid > 0)
                    throw new NotSupportedException(string.Format(
                        "Шаблон с несколькими '%' в середине не поддерживается: '{0}'.", pattern));

                if (startsPct && endsPct)
                {
                    string inner = pattern.Substring(1, n - 2);
                    if (inner.Length == 0) return "true()";
                    return "contains(., " + Quote(inner) + ")";
                }
                if (startsPct)
                {
                    string inner = pattern.Substring(1);
                    return "(string-length(.) >= " + inner.Length +
                           " and substring(., string-length(.) - " + (inner.Length - 1) +
                           ") = " + Quote(inner) + ")";
                }
                if (endsPct)
                {
                    string inner = pattern.Substring(0, n - 1);
                    return "starts-with(., " + Quote(inner) + ")";
                }

                return ". = " + Quote(pattern);
            }

            private static string Quote(string s)
            {
                if (s.IndexOf('\'') < 0) return "'" + s + "'";
                if (s.IndexOf('"') < 0) return "\"" + s + "\"";

                string[] parts = s.Split('\'');
                StringBuilder sb = new StringBuilder("concat(");
                for (int i = 0; i < parts.Length; i++)
                {
                    if (i > 0) sb.Append(", \"'\", ");
                    sb.Append('\'').Append(parts[i]).Append('\'');
                }
                sb.Append(")");
                return sb.ToString();
            }

            private static string EscapeName(string name)
            {
                return name.Replace("'", "&apos;");
            }
        }
    }
}