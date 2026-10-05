using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ParseXml.Forms
{
    public partial class EditColumn : Form
    {
        public EditColumn()
        {
            InitializeComponent();
        }

        public string FieldName
        {
            get { return string.IsNullOrWhiteSpace(tbColumn.Text) ? null : tbColumn.Text.Trim(); }
            set { tbColumn.Text = value; }
        }

        public string XmlPath
        {
            get { return string.IsNullOrWhiteSpace(tbPath.Text) ? FieldName : tbPath.Text.Trim(); }
            set { tbPath.Text = value; }
        }
    }
}
