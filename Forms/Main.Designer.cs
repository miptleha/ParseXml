
namespace ParseXml.Forms
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.label1 = new System.Windows.Forms.Label();
            this.tbXmlDir = new System.Windows.Forms.TextBox();
            this.btnParse = new System.Windows.Forms.Button();
            this.tbFilter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lvColumns = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label3 = new System.Windows.Forms.Label();
            this.tbTable = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbTableClear = new System.Windows.Forms.CheckBox();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.btnColumnDelete = new System.Windows.Forms.Button();
            this.btnColumnEdit = new System.Windows.Forms.Button();
            this.btnColumnAdd = new System.Windows.Forms.Button();
            this.btnXmlDir = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Папка с xml файлами";
            // 
            // tbXmlDir
            // 
            this.tbXmlDir.Location = new System.Drawing.Point(15, 25);
            this.tbXmlDir.Name = "tbXmlDir";
            this.tbXmlDir.Size = new System.Drawing.Size(713, 20);
            this.tbXmlDir.TabIndex = 1;
            // 
            // btnParse
            // 
            this.btnParse.Location = new System.Drawing.Point(12, 413);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(94, 23);
            this.btnParse.TabIndex = 15;
            this.btnParse.Text = "Выгрузить";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // tbFilter
            // 
            this.tbFilter.Location = new System.Drawing.Point(15, 76);
            this.tbFilter.Name = "tbFilter";
            this.tbFilter.Size = new System.Drawing.Size(713, 20);
            this.tbFilter.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Фильтр";
            // 
            // lvColumns
            // 
            this.lvColumns.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvColumns.FullRowSelect = true;
            this.lvColumns.HideSelection = false;
            this.lvColumns.Location = new System.Drawing.Point(15, 131);
            this.lvColumns.MultiSelect = false;
            this.lvColumns.Name = "lvColumns";
            this.lvColumns.Size = new System.Drawing.Size(713, 164);
            this.lvColumns.TabIndex = 6;
            this.lvColumns.UseCompatibleStateImageBehavior = false;
            this.lvColumns.View = System.Windows.Forms.View.Details;
            this.lvColumns.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvColumns_ItemSelectionChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Колонка";
            this.columnHeader1.Width = 156;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Путь в xml";
            this.columnHeader2.Width = 539;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Выгружаемые сведения";
            // 
            // tbTable
            // 
            this.tbTable.Location = new System.Drawing.Point(15, 325);
            this.tbTable.Name = "tbTable";
            this.tbTable.Size = new System.Drawing.Size(270, 20);
            this.tbTable.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 309);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Таблица";
            // 
            // cbTableClear
            // 
            this.cbTableClear.AutoSize = true;
            this.cbTableClear.Location = new System.Drawing.Point(315, 327);
            this.cbTableClear.Name = "cbTableClear";
            this.cbTableClear.Size = new System.Drawing.Size(116, 17);
            this.cbTableClear.TabIndex = 12;
            this.cbTableClear.Text = "Очистить таблицу";
            this.cbTableClear.UseVisualStyleBackColor = true;
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Location = new System.Drawing.Point(112, 413);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(142, 23);
            this.btnSaveSettings.TabIndex = 16;
            this.btnSaveSettings.Text = "Сохранить настройки";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // btnColumnDelete
            // 
            this.btnColumnDelete.Enabled = false;
            this.btnColumnDelete.Image = global::ParseXml.Properties.Resources.delete;
            this.btnColumnDelete.Location = new System.Drawing.Point(734, 196);
            this.btnColumnDelete.Name = "btnColumnDelete";
            this.btnColumnDelete.Size = new System.Drawing.Size(36, 30);
            this.btnColumnDelete.TabIndex = 9;
            this.btnColumnDelete.UseVisualStyleBackColor = true;
            this.btnColumnDelete.Click += new System.EventHandler(this.btnColumnDelete_Click);
            // 
            // btnColumnEdit
            // 
            this.btnColumnEdit.Enabled = false;
            this.btnColumnEdit.Image = global::ParseXml.Properties.Resources.edit;
            this.btnColumnEdit.Location = new System.Drawing.Point(734, 163);
            this.btnColumnEdit.Name = "btnColumnEdit";
            this.btnColumnEdit.Size = new System.Drawing.Size(36, 30);
            this.btnColumnEdit.TabIndex = 8;
            this.btnColumnEdit.UseVisualStyleBackColor = true;
            this.btnColumnEdit.Click += new System.EventHandler(this.btnColumnEdit_Click);
            // 
            // btnColumnAdd
            // 
            this.btnColumnAdd.Image = global::ParseXml.Properties.Resources.add;
            this.btnColumnAdd.Location = new System.Drawing.Point(734, 131);
            this.btnColumnAdd.Name = "btnColumnAdd";
            this.btnColumnAdd.Size = new System.Drawing.Size(36, 30);
            this.btnColumnAdd.TabIndex = 7;
            this.btnColumnAdd.UseVisualStyleBackColor = true;
            this.btnColumnAdd.Click += new System.EventHandler(this.btnColumnAdd_Click);
            // 
            // btnXmlDir
            // 
            this.btnXmlDir.Image = global::ParseXml.Properties.Resources.loupe;
            this.btnXmlDir.Location = new System.Drawing.Point(734, 22);
            this.btnXmlDir.Name = "btnXmlDir";
            this.btnXmlDir.Size = new System.Drawing.Size(36, 26);
            this.btnXmlDir.TabIndex = 2;
            this.btnXmlDir.UseVisualStyleBackColor = true;
            this.btnXmlDir.Click += new System.EventHandler(this.btnXmlDir_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 362);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(716, 23);
            this.progressBar.TabIndex = 13;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(12, 388);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(58, 13);
            this.lblProgress.TabIndex = 14;
            this.lblProgress.Text = "lblProgress";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 448);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnColumnDelete);
            this.Controls.Add(this.btnColumnEdit);
            this.Controls.Add(this.btnColumnAdd);
            this.Controls.Add(this.btnSaveSettings);
            this.Controls.Add(this.cbTableClear);
            this.Controls.Add(this.tbTable);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lvColumns);
            this.Controls.Add(this.tbFilter);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnParse);
            this.Controls.Add(this.btnXmlDir);
            this.Controls.Add(this.tbXmlDir);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выгрузка данных из xml в таблицу";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbXmlDir;
        private System.Windows.Forms.Button btnXmlDir;
        private System.Windows.Forms.Button btnParse;
        private System.Windows.Forms.TextBox tbFilter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView lvColumns;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbTable;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbTableClear;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.Button btnColumnAdd;
        private System.Windows.Forms.Button btnColumnEdit;
        private System.Windows.Forms.Button btnColumnDelete;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblProgress;
    }
}

