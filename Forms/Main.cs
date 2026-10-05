using ParseXml.Config;
using ParseXml.Db;
using ParseXml.Log;
using ParseXml.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace ParseXml.Forms
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            LogManager.Configure();
        }

        ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IDbExecuter db = null;

        private static object ConvertValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return DBNull.Value;

            DateTime dt;
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out dt))
                return dt;

            return value;
        }

        class InfoException : Exception
        {
            public InfoException(string info) : base(info)
            { }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            lblProgress.Text = "";
            tbXmlDir.Text = AppSettings.GetItem("XmlFolder");
            tbFilter.Text = AppSettings.GetItem("Filter");

            string fields = AppSettings.GetItem("Fields");
            var fieldsArray = fields.Split(',');
            foreach (var f in fieldsArray)
            {
                var value = f.Trim();
                var name = value;
                int index = value.IndexOf('=');
                if (index != -1)
                {
                    name = value.Substring(0, index);
                    value = value.Substring(index + 1);
                }

                if (string.IsNullOrEmpty(name) || FindColumn(name) != null)
                    continue;

                var item = new ListViewItem(name);
                item.SubItems.Add(value);
                lvColumns.Items.Add(item);
            }

            tbTable.Text = AppSettings.GetItem("Table");
            cbTableClear.Checked = AppSettings.GetItem("ClearTable") == "True";
        }

        private List<string> GetFields()
        {
            var parts = new List<string>();
            foreach (ListViewItem item in lvColumns.Items)
            {
                var name = item.Text;
                var value = item.SubItems[item.SubItems.Count > 1 ? 1 : 0].Text;

                if (string.IsNullOrEmpty(value) || name == value)
                    parts.Add(name);
                else
                    parts.Add(name + "=" + value);
            }
            
            return parts;
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            var parts = GetFields();
            var settings = new Dictionary<string, string>
            {
                { "XmlFolder",  tbXmlDir.Text.Trim() },
                { "Filter",     tbFilter.Text.Trim() },
                { "Fields",     string.Join(", ", parts) },
                { "Table",      tbTable.Text.Trim() },
                { "ClearTable", cbTableClear.Checked ? "True" : "False" }
            };

            AppSettings.SetItems(settings);
        }

        private void lvColumns_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            btnColumnEdit.Enabled = e.IsSelected;
            btnColumnDelete.Enabled = e.IsSelected;
        }

        private void btnColumnAdd_Click(object sender, EventArgs e)
        {
            var d = new EditColumn();

            while (d.ShowDialog(this) == DialogResult.OK)
            {
                if (d.FieldName == null)
                {
                    MessageBox.Show("Field name is required.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                if (FindColumn(d.FieldName) != null)
                {
                    MessageBox.Show(string.Format("Field \"{0}\" is already in the list.", d.FieldName),
                        "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                break;
            }

            if (d.DialogResult == DialogResult.OK)
            {
                var item = new ListViewItem(d.FieldName);
                item.SubItems.Add(d.XmlPath);
                lvColumns.Items.Add(item);
                SelectItem(lvColumns.Items.Count - 1);
            }

            d.Dispose();
        }

        private void btnColumnEdit_Click(object sender, EventArgs e)
        {
            var items = lvColumns.SelectedItems;
            if (items.Count == 0)
                return;
            var item = items[0];

            var d = new EditColumn();
            d.FieldName = item.Text;
            d.XmlPath = item.SubItems[item.SubItems.Count > 1 ? 1 : 0].Text;

            while (d.ShowDialog(this) == DialogResult.OK)
            {
                if (d.FieldName == null)
                {
                    MessageBox.Show("Field name is required.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                var existing = FindColumn(d.FieldName);
                if (existing != null && existing != item)
                {
                    MessageBox.Show(string.Format("Field \"{0}\" is already in the list.", d.FieldName),
                        "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                break;
            }

            if (d.DialogResult == DialogResult.OK)
            {
                item.Text = d.FieldName;
                if (item.SubItems.Count > 1)
                    item.SubItems[1].Text = d.XmlPath;
                else
                    item.SubItems.Add(d.XmlPath);
            }

            d.Dispose();
        }

        private void btnColumnDelete_Click(object sender, EventArgs e)
        {
            var items = lvColumns.SelectedItems;
            if (items.Count == 0)
                return;
            
            int index = lvColumns.SelectedIndices[0];
            lvColumns.Items.RemoveAt(index);

            if (!SelectItem(index))
                SelectItem(index - 1);
        }

        private bool SelectItem(int index)
        {
            if (index < 0 || index >= lvColumns.Items.Count)
                return false;

            lvColumns.Focus();
            lvColumns.Items[index].Selected = true;
            lvColumns.Items[index].EnsureVisible();
            return true;
        }

        private ListViewItem FindColumn(string name)
        {
            foreach (ListViewItem it in lvColumns.Items)
            {
                if (string.Equals(it.Text, name, StringComparison.OrdinalIgnoreCase))
                    return it;
            }
            return null;
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            if (!btnParse.Enabled)
                return;

            _stopwatch.Restart();

            try
            {
                string folder = tbXmlDir.Text.Trim();
                if (string.IsNullOrEmpty(folder))
                    throw new InfoException("Xml folder is not specified");
                if (!Directory.Exists(folder))
                    throw new InfoException("The specified xml folder does not exist");

                string filter = tbFilter.Text.Trim();
                string xFilter = XmlSearch.ToXPath(filter);
                log.Debug("Filter: " + filter + " -> " + xFilter);

                var fields = GetFields();
                if (fields.Count == 0)
                    throw new InfoException("Exported data is not filled in");
                var parsedFields = new List<Tuple<string, string>>();
                foreach (var f in fields)
                {
                    var index = f.IndexOf('=');
                    string field = f;
                    string path = f;
                    if (index != -1)
                    {
                        field = f.Substring(0, index);
                        path = f.Substring(index + 1);
                    }
                    try
                    {
                        var t = Tuple.Create(field, XmlSearch.ToXPath(path));
                        log.Debug("Field: " + t.Item1 + ", path: " + path + " -> " + t.Item2);
                        parsedFields.Add(t);
                    }
                    catch (Exception ex)
                    {
                        throw new InfoException("For field: " + field + ", error parsing path: " + path + ". " + ex.Message);
                    }
                }

                int total = 0;
                try
                {
                    foreach (string _ in Directory.EnumerateFiles(folder, "*.xml", SearchOption.TopDirectoryOnly))
                        total++;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    return;
                }
                log.Debug("Found " + total + " files in folder " + folder);

                string tabName = tbTable.Text.Trim();
                if (string.IsNullOrEmpty(tabName))
                    throw new InfoException("Table name is not specified");

                string sqlCols = string.Join(", ", parsedFields.Select(t => t.Item1));
                string sqlValues = string.Join(", ", parsedFields.Select(t => "@" + t.Item1));
                string sqlCommand = "INSERT INTO " + tabName + "(" + sqlCols + ") VALUES(" + sqlValues + ")";

                try
                {
                    db = new DbExecuter(AppSettings.ConnectionString);
                }
                catch (Exception ex)
                {
                    throw new InfoException("Database connection string is not configured in the config file. " + ex.Message);
                }

                var job = new ParseJob
                {
                    Folder = folder,
                    XPathFilter = xFilter,
                    Fields = parsedFields,
                    SqlCommand = sqlCommand,
                    Table = tabName,
                    ClearTable = cbTableClear.Checked,
                    Total = total
                };

                var worker = new BackgroundWorker { WorkerReportsProgress = true };
                worker.DoWork += worker_DoWork;
                worker.ProgressChanged += worker_ProgressChanged;
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                worker.RunWorkerAsync(job);

                btnParse.Enabled = false;
                progressBar.Minimum = 0;
                progressBar.Maximum = 100;
                progressBar.Value = 0;
                _parseTotal = total;
                //lblProgress.Text = "Processed 0 of " + total + ", saved 0";
            }
            catch (DbException ex)
            {
                log.Error(ex);
                if (ex.Type == DbExceptionType.Connection)
                    MessageBox.Show(this, "Database connection error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (ex.Type == DbExceptionType.Query)
                    MessageBox.Show(this, "Query execution error:\n" + ex.CommandText + "\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InfoException ex)
            {
                log.Error(ex);
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class ParseJob
        {
            public string Folder;
            public string XPathFilter;
            public List<Tuple<string, string>> Fields;
            public string SqlCommand;
            public string Table;
            public bool ClearTable;
            public int Total;
        }

        private int _parseTotal;
        private Stopwatch _stopwatch = new Stopwatch();


        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            var job = (ParseJob)e.Argument;

            if (job.ClearTable)
                db.Execute("delete from " + job.Table);

            int count = 0;
            int okCount = 0;

            foreach (string file in Directory.EnumerateFiles(job.Folder, "*.xml", SearchOption.TopDirectoryOnly))
            {
                count++;
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(file);
                }
                catch (Exception ex)
                {
                    log.Debug("Error loading xml file: " + Path.GetFileName(file) + ". " + ex.Message);
                }

                bool isOk = XmlSearch.Filter(doc, job.XPathFilter);
                if (isOk)
                {
                    okCount++;
                    log.Debug(count + " of " + job.Total + " file " + Path.GetFileName(file) + " matches the filter");
                    var result = new List<string>();
                    var dbParams = new List<DbParam>();
                    foreach (var t in job.Fields)
                    {
                        string name = t.Item1;
                        string value = XmlSearch.Find(doc, t.Item2);
                        result.Add(name + "=" + value);
                        dbParams.Add(new DbParam { Name = name, Value = ConvertValue(value) });
                    }
                    log.Debug("Result: " + string.Join(", ", result));
                    db.Execute(job.SqlCommand, dbParams.ToArray());
                }

                int percent = job.Total > 0 ? count * 100 / job.Total : 100;
                worker.ReportProgress(percent, Tuple.Create(count, okCount));
            }

            if (job.Total == 0 || count * 100 / job.Total != 100)
                worker.ReportProgress(100, Tuple.Create(count, okCount));

            e.Result = Tuple.Create(count, okCount);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var stat = (Tuple<int, int>)e.UserState;

            lblProgress.Text = "Processed " + stat.Item1 + " of " + _parseTotal + ", saved " + stat.Item2 + ", time: " + _stopwatch.Elapsed.ToString("hh\\:mm\\:ss");

            if (progressBar.Value != e.ProgressPercentage)
                progressBar.Value = e.ProgressPercentage;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnParse.Enabled = true;

            if (e.Error != null)
            {
                var dbEx = e.Error as DbException;
                if (dbEx != null)
                {
                    log.Error(dbEx);
                    if (dbEx.Type == DbExceptionType.Connection)
                        MessageBox.Show(this, "Database connection error:\n" + dbEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (dbEx.Type == DbExceptionType.Query)
                        MessageBox.Show(this, "Query execution error:\n" + dbEx.CommandText + "\n" + dbEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show(this, dbEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    log.Error(e.Error);
                    MessageBox.Show(this, e.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            var stat = (Tuple<int, int>)e.Result;
            _stopwatch.Stop();
            MessageBox.Show(this, "Processed " + stat.Item1 + " files, saved " + stat.Item2 + ", time: " + _stopwatch.Elapsed.ToString("hh\\:mm\\:ss"),
                "Processing completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnXmlDir_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            dlg.SelectedPath = tbXmlDir.Text.Trim();

            if (dlg.ShowDialog() == DialogResult.OK)
                tbXmlDir.Text = dlg.SelectedPath;
        }
    }
}
