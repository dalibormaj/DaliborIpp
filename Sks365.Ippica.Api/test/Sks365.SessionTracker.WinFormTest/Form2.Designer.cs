namespace Sks365.SessionTracker.WinFormTest
{
    partial class Form2
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnCreate = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSessionToken = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBookmakerId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAspNetSession = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSessionToken2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtBookmaker2 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtUserName2 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtAspSession2 = new System.Windows.Forms.TextBox();
            this.btnDecode = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(880, 299);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnCreate);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.txtSessionToken);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.txtBookmakerId);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.txtUsername);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.txtAspNetSession);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(872, 266);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Create token";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnDecode);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.txtBookmaker2);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.txtUserName2);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.txtAspSession2);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.txtSessionToken2);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(872, 266);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Decode token";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(145, 137);
            this.btnCreate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(328, 30);
            this.btnCreate.TabIndex = 17;
            this.btnCreate.Text = "Create token";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 215);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 20);
            this.label4.TabIndex = 16;
            this.label4.Text = "SessionToken";
            // 
            // txtSessionToken
            // 
            this.txtSessionToken.Location = new System.Drawing.Point(145, 212);
            this.txtSessionToken.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSessionToken.Name = "txtSessionToken";
            this.txtSessionToken.Size = new System.Drawing.Size(709, 26);
            this.txtSessionToken.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 20);
            this.label3.TabIndex = 14;
            this.label3.Text = "BookmakerId";
            // 
            // txtBookmakerId
            // 
            this.txtBookmakerId.Location = new System.Drawing.Point(144, 54);
            this.txtBookmakerId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtBookmakerId.Name = "txtBookmakerId";
            this.txtBookmakerId.Size = new System.Drawing.Size(329, 26);
            this.txtBookmakerId.TabIndex = 13;
            this.txtBookmakerId.Text = "7";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 20);
            this.label2.TabIndex = 12;
            this.label2.Text = "Username";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(144, 91);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(329, 26);
            this.txtUsername.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "AspNetSession";
            // 
            // txtAspNetSession
            // 
            this.txtAspNetSession.Location = new System.Drawing.Point(144, 18);
            this.txtAspNetSession.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtAspNetSession.Name = "txtAspNetSession";
            this.txtAspNetSession.Size = new System.Drawing.Size(329, 26);
            this.txtAspNetSession.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "SessionToken";
            // 
            // txtSessionToken2
            // 
            this.txtSessionToken2.Location = new System.Drawing.Point(151, 30);
            this.txtSessionToken2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtSessionToken2.Name = "txtSessionToken2";
            this.txtSessionToken2.Size = new System.Drawing.Size(608, 26);
            this.txtSessionToken2.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 158);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 20);
            this.label6.TabIndex = 24;
            this.label6.Text = "BookmakerId";
            // 
            // txtBookmaker2
            // 
            this.txtBookmaker2.Location = new System.Drawing.Point(152, 152);
            this.txtBookmaker2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtBookmaker2.Name = "txtBookmaker2";
            this.txtBookmaker2.Size = new System.Drawing.Size(329, 26);
            this.txtBookmaker2.TabIndex = 23;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 192);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 20);
            this.label7.TabIndex = 22;
            this.label7.Text = "Username";
            // 
            // txtUserName2
            // 
            this.txtUserName2.Location = new System.Drawing.Point(152, 189);
            this.txtUserName2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtUserName2.Name = "txtUserName2";
            this.txtUserName2.Size = new System.Drawing.Size(329, 26);
            this.txtUserName2.TabIndex = 21;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 120);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(119, 20);
            this.label8.TabIndex = 20;
            this.label8.Text = "AspNetSession";
            // 
            // txtAspSession2
            // 
            this.txtAspSession2.Location = new System.Drawing.Point(152, 116);
            this.txtAspSession2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtAspSession2.Name = "txtAspSession2";
            this.txtAspSession2.Size = new System.Drawing.Size(329, 26);
            this.txtAspSession2.TabIndex = 19;
            // 
            // btnDecode
            // 
            this.btnDecode.Location = new System.Drawing.Point(151, 69);
            this.btnDecode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDecode.Name = "btnDecode";
            this.btnDecode.Size = new System.Drawing.Size(330, 30);
            this.btnDecode.TabIndex = 25;
            this.btnDecode.Text = "Decode";
            this.btnDecode.UseVisualStyleBackColor = true;
            this.btnDecode.Click += new System.EventHandler(this.btnDecode_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 299);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSessionToken;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBookmakerId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAspNetSession;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtBookmaker2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtUserName2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtAspSession2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSessionToken2;
        private System.Windows.Forms.Button btnDecode;
    }
}