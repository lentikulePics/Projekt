namespace GUINavrh
{
    partial class SettingsForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.applyButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.generalTab = new System.Windows.Forms.TabPage();
            this.metricsGroup = new System.Windows.Forms.GroupBox();
            this.languageGroup = new System.Windows.Forms.GroupBox();
            this.languageCombobox = new System.Windows.Forms.ComboBox();
            this.advancedTab = new System.Windows.Forms.TabPage();
            this.cancelButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.generalTab.SuspendLayout();
            this.languageGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Controls.Add(this.applyButton);
            this.panel1.Controls.Add(this.okButton);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // applyButton
            // 
            resources.ApplyResources(this.applyButton, "applyButton");
            this.applyButton.Name = "applyButton";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.generalTab);
            this.tabControl1.Controls.Add(this.advancedTab);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // generalTab
            // 
            this.generalTab.BackColor = System.Drawing.Color.WhiteSmoke;
            this.generalTab.Controls.Add(this.metricsGroup);
            this.generalTab.Controls.Add(this.languageGroup);
            resources.ApplyResources(this.generalTab, "generalTab");
            this.generalTab.Name = "generalTab";
            // 
            // metricsGroup
            // 
            resources.ApplyResources(this.metricsGroup, "metricsGroup");
            this.metricsGroup.Name = "metricsGroup";
            this.metricsGroup.TabStop = false;
            // 
            // languageGroup
            // 
            this.languageGroup.Controls.Add(this.label1);
            this.languageGroup.Controls.Add(this.languageCombobox);
            resources.ApplyResources(this.languageGroup, "languageGroup");
            this.languageGroup.Name = "languageGroup";
            this.languageGroup.TabStop = false;
            // 
            // languageCombobox
            // 
            this.languageCombobox.FormattingEnabled = true;
            this.languageCombobox.Items.AddRange(new object[] {
            resources.GetString("languageCombobox.Items"),
            resources.GetString("languageCombobox.Items1")});
            resources.ApplyResources(this.languageCombobox, "languageCombobox");
            this.languageCombobox.Name = "languageCombobox";
            // 
            // advancedTab
            // 
            this.advancedTab.BackColor = System.Drawing.Color.WhiteSmoke;
            resources.ApplyResources(this.advancedTab, "advancedTab");
            this.advancedTab.Name = "advancedTab";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // SettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.generalTab.ResumeLayout(false);
            this.languageGroup.ResumeLayout(false);
            this.languageGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage generalTab;
        private System.Windows.Forms.GroupBox metricsGroup;
        private System.Windows.Forms.GroupBox languageGroup;
        private System.Windows.Forms.ComboBox languageCombobox;
        private System.Windows.Forms.TabPage advancedTab;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;

    }
}