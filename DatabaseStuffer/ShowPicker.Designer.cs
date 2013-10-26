namespace DatabaseStuffer {
    partial class ShowPicker {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.txtShow = new System.Windows.Forms.TextBox();
            this.lbResults = new System.Windows.Forms.ListBox();
            this.btnLookup = new System.Windows.Forms.Button();
            this.lblVideoPath = new System.Windows.Forms.Label();
            this.btnGetData = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Show:";
            // 
            // txtShow
            // 
            this.txtShow.Location = new System.Drawing.Point(53, 12);
            this.txtShow.Name = "txtShow";
            this.txtShow.Size = new System.Drawing.Size(273, 20);
            this.txtShow.TabIndex = 1;
            // 
            // lbResults
            // 
            this.lbResults.FormattingEnabled = true;
            this.lbResults.Location = new System.Drawing.Point(53, 38);
            this.lbResults.Name = "lbResults";
            this.lbResults.Size = new System.Drawing.Size(273, 173);
            this.lbResults.TabIndex = 2;
            // 
            // btnLookup
            // 
            this.btnLookup.Location = new System.Drawing.Point(332, 10);
            this.btnLookup.Name = "btnLookup";
            this.btnLookup.Size = new System.Drawing.Size(75, 23);
            this.btnLookup.TabIndex = 3;
            this.btnLookup.Text = "Look up";
            this.btnLookup.UseVisualStyleBackColor = true;
            this.btnLookup.Click += new System.EventHandler(this.btnLookup_Click);
            // 
            // lblVideoPath
            // 
            this.lblVideoPath.AutoSize = true;
            this.lblVideoPath.Location = new System.Drawing.Point(12, 397);
            this.lblVideoPath.Name = "lblVideoPath";
            this.lblVideoPath.Size = new System.Drawing.Size(35, 13);
            this.lblVideoPath.TabIndex = 4;
            this.lblVideoPath.Text = "label2";
            // 
            // btnGetData
            // 
            this.btnGetData.Location = new System.Drawing.Point(332, 39);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(126, 23);
            this.btnGetData.TabIndex = 5;
            this.btnGetData.Text = "Get Show Listing";
            this.btnGetData.UseVisualStyleBackColor = true;
            this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // ShowPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 419);
            this.Controls.Add(this.btnGetData);
            this.Controls.Add(this.lblVideoPath);
            this.Controls.Add(this.btnLookup);
            this.Controls.Add(this.lbResults);
            this.Controls.Add(this.txtShow);
            this.Controls.Add(this.label1);
            this.Name = "ShowPicker";
            this.Text = "ShowPicker";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtShow;
        private System.Windows.Forms.ListBox lbResults;
        private System.Windows.Forms.Button btnLookup;
        private System.Windows.Forms.Label lblVideoPath;
        private System.Windows.Forms.Button btnGetData;
    }
}