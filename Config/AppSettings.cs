using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace ParseXml.Config
{
    public class AppSettings
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
        }

        public static string GetItem(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        public static void SetItems(Dictionary<string, string> values)
        {
            string path = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(path);

            var appSettings = (XmlElement)doc.SelectSingleNode("/configuration/appSettings");

            foreach (var kv in values)
            {
                var node = (XmlElement)appSettings.SelectSingleNode("add[@key=\"" + kv.Key + "\"]");
                node.SetAttribute("value", kv.Value);
            }

            doc.Save(path);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static void SetItem(string name, string value)
        {
            SetItems(new Dictionary<string, string> { { name, value } });
        }
    }
}