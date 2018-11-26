namespace ISoft.Coder.Forms
{
    partial class frmMain
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
            this.gb_Startup = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_Namespace = new System.Windows.Forms.TextBox();
            this.b_Close = new System.Windows.Forms.Button();
            this.b_Go = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cb_Operations = new System.Windows.Forms.ComboBox();
            this.cb_Objects = new System.Windows.Forms.ComboBox();
            this.cb_Connections = new System.Windows.Forms.ComboBox();
            this.gb_Console = new System.Windows.Forms.GroupBox();
            this.l_Console = new System.Windows.Forms.ListBox();
            this.gb_Startup.SuspendLayout();
            this.gb_Console.SuspendLayout();
            this.SuspendLayout();
            // 
            // gb_Startup
            // 
            this.gb_Startup.Controls.Add(this.label4);
            this.gb_Startup.Controls.Add(this.tb_Namespace);
            this.gb_Startup.Controls.Add(this.b_Close);
            this.gb_Startup.Controls.Add(this.b_Go);
            this.gb_Startup.Controls.Add(this.label1);
            this.gb_Startup.Controls.Add(this.label3);
            this.gb_Startup.Controls.Add(this.label2);
            this.gb_Startup.Controls.Add(this.cb_Operations);
            this.gb_Startup.Controls.Add(this.cb_Objects);
            this.gb_Startup.Controls.Add(this.cb_Connections);
            this.gb_Startup.Location = new System.Drawing.Point(12, 12);
            this.gb_Startup.Name = "gb_Startup";
            this.gb_Startup.Size = new System.Drawing.Size(560, 112);
            this.gb_Startup.TabIndex = 0;
            this.gb_Startup.TabStop = false;
            this.gb_Startup.Text = "Start";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 20);
            this.label4.TabIndex = 4;
            this.label4.Text = "Namespace: ";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tb_Namespace
            // 
            this.tb_Namespace.Location = new System.Drawing.Point(95, 86);
            this.tb_Namespace.Name = "tb_Namespace";
            this.tb_Namespace.Size = new System.Drawing.Size(241, 21);
            this.tb_Namespace.TabIndex = 3;
            // 
            // b_Close
            // 
            this.b_Close.Location = new System.Drawing.Point(505, 20);
            this.b_Close.Name = "b_Close";
            this.b_Close.Size = new System.Drawing.Size(45, 87);
            this.b_Close.TabIndex = 2;
            this.b_Close.Text = "Exit";
            this.b_Close.UseVisualStyleBackColor = true;
            // 
            // b_Go
            // 
            this.b_Go.Location = new System.Drawing.Point(347, 20);
            this.b_Go.Name = "b_Go";
            this.b_Go.Size = new System.Drawing.Size(156, 87);
            this.b_Go.TabIndex = 2;
            this.b_Go.Text = "Go";
            this.b_Go.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Database: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 20);
            this.label3.TabIndex = 1;
            this.label3.Text = "Action: ";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Worker:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cb_Operations
            // 
            this.cb_Operations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_Operations.FormattingEnabled = true;
            this.cb_Operations.Location = new System.Drawing.Point(95, 64);
            this.cb_Operations.Name = "cb_Operations";
            this.cb_Operations.Size = new System.Drawing.Size(241, 20);
            this.cb_Operations.TabIndex = 0;
            // 
            // cb_Objects
            // 
            this.cb_Objects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_Objects.FormattingEnabled = true;
            this.cb_Objects.Location = new System.Drawing.Point(95, 42);
            this.cb_Objects.Name = "cb_Objects";
            this.cb_Objects.Size = new System.Drawing.Size(241, 20);
            this.cb_Objects.TabIndex = 0;
            // 
            // cb_Connections
            // 
            this.cb_Connections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_Connections.FormattingEnabled = true;
            this.cb_Connections.Location = new System.Drawing.Point(95, 20);
            this.cb_Connections.Name = "cb_Connections";
            this.cb_Connections.Size = new System.Drawing.Size(241, 20);
            this.cb_Connections.TabIndex = 0;
            // 
            // gb_Console
            // 
            this.gb_Console.Controls.Add(this.l_Console);
            this.gb_Console.Location = new System.Drawing.Point(12, 122);
            this.gb_Console.Name = "gb_Console";
            this.gb_Console.Size = new System.Drawing.Size(560, 317);
            this.gb_Console.TabIndex = 1;
            this.gb_Console.TabStop = false;
            this.gb_Console.Text = "Debug Info";
            // 
            // l_Console
            // 
            this.l_Console.FormattingEnabled = true;
            this.l_Console.ItemHeight = 12;
            this.l_Console.Location = new System.Drawing.Point(10, 20);
            this.l_Console.Name = "l_Console";
            this.l_Console.Size = new System.Drawing.Size(540, 280);
            this.l_Console.TabIndex = 0;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 451);
            this.Controls.Add(this.gb_Console);
            this.Controls.Add(this.gb_Startup);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "POCO - Class Generator";
            this.gb_Startup.ResumeLayout(false);
            this.gb_Startup.PerformLayout();
            this.gb_Console.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gb_Startup;
        private System.Windows.Forms.GroupBox gb_Console;
        private System.Windows.Forms.ComboBox cb_Connections;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cb_Objects;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cb_Operations;
        private System.Windows.Forms.Button b_Go;
        private System.Windows.Forms.Button b_Close;
        private System.Windows.Forms.ListBox l_Console;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_Namespace;


    }
}