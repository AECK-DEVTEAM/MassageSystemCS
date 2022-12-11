using System.Windows.Forms;

namespace MessageSystemCSDesktopApp
{
    partial class frm_main : Form
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_main));
            this.btn_connect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_uid = new System.Windows.Forms.TextBox();
            this.lb_clients = new System.Windows.Forms.ListBox();
            this.tb_log = new System.Windows.Forms.RichTextBox();
            this.tb_ip = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tc_conversations = new System.Windows.Forms.TabControl();
            this.tb_uid2 = new System.Windows.Forms.TextBox();
            this.tb_password2 = new System.Windows.Forms.TextBox();
            this.btn_register = new System.Windows.Forms.Button();
            this.tb_password = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_login = new System.Windows.Forms.Button();
            this.lable7 = new System.Windows.Forms.Label();
            this.lb_groups = new System.Windows.Forms.ListBox();
            this.tb_gid = new System.Windows.Forms.TextBox();
            this.btn_newGroup = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_connect
            // 
            this.btn_connect.Location = new System.Drawing.Point(539, 35);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(75, 23);
            this.btn_connect.TabIndex = 0;
            this.btn_connect.Text = "Connect";
            this.btn_connect.UseVisualStyleBackColor = true;
            this.btn_connect.Click += new System.EventHandler(this.btn_connect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(420, 296);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Online Clients:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // tb_uid
            // 
            this.tb_uid.Location = new System.Drawing.Point(94, 6);
            this.tb_uid.Name = "tb_uid";
            this.tb_uid.Size = new System.Drawing.Size(136, 20);
            this.tb_uid.TabIndex = 4;
            // 
            // lb_clients
            // 
            this.lb_clients.FormattingEnabled = true;
            this.lb_clients.Location = new System.Drawing.Point(420, 312);
            this.lb_clients.Name = "lb_clients";
            this.lb_clients.Size = new System.Drawing.Size(198, 95);
            this.lb_clients.TabIndex = 5;
            this.lb_clients.DoubleClick += new System.EventHandler(this.lb_clients_DoubleClick);
            // 
            // tb_log
            // 
            this.tb_log.Location = new System.Drawing.Point(423, 116);
            this.tb_log.Name = "tb_log";
            this.tb_log.ReadOnly = true;
            this.tb_log.Size = new System.Drawing.Size(195, 177);
            this.tb_log.TabIndex = 7;
            this.tb_log.Text = "";
            // 
            // tb_ip
            // 
            this.tb_ip.Location = new System.Drawing.Point(478, 11);
            this.tb_ip.Name = "tb_ip";
            this.tb_ip.Size = new System.Drawing.Size(136, 20);
            this.tb_ip.TabIndex = 9;
            this.tb_ip.Text = "153.17.132.191";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(437, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "IP:";
            // 
            // tc_conversations
            // 
            this.tc_conversations.Location = new System.Drawing.Point(12, 116);
            this.tc_conversations.Name = "tc_conversations";
            this.tc_conversations.SelectedIndex = 0;
            this.tc_conversations.Size = new System.Drawing.Size(405, 622);
            this.tc_conversations.TabIndex = 11;
            this.tc_conversations.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tc_conversations_DrawItem);
            this.tc_conversations.SelectedIndexChanged += new System.EventHandler(this.tc_conversations_SelectedIndexChanged);
            this.tc_conversations.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tc_conversations_MouseDown);
            // 
            // tb_uid2
            // 
            this.tb_uid2.Location = new System.Drawing.Point(94, 64);
            this.tb_uid2.Name = "tb_uid2";
            this.tb_uid2.Size = new System.Drawing.Size(136, 20);
            this.tb_uid2.TabIndex = 12;
            // 
            // tb_password2
            // 
            this.tb_password2.Location = new System.Drawing.Point(94, 90);
            this.tb_password2.Name = "tb_password2";
            this.tb_password2.Size = new System.Drawing.Size(136, 20);
            this.tb_password2.TabIndex = 13;
            this.tb_password2.TextChanged += new System.EventHandler(this.tb_password_TextChanged);
            // 
            // btn_register
            // 
            this.btn_register.Enabled = false;
            this.btn_register.Location = new System.Drawing.Point(266, 77);
            this.btn_register.Name = "btn_register";
            this.btn_register.Size = new System.Drawing.Size(75, 23);
            this.btn_register.TabIndex = 14;
            this.btn_register.Text = "Register";
            this.btn_register.UseVisualStyleBackColor = true;
            this.btn_register.Click += new System.EventHandler(this.btn_register_Click);
            // 
            // tb_password
            // 
            this.tb_password.Location = new System.Drawing.Point(94, 32);
            this.tb_password.Name = "tb_password";
            this.tb_password.Size = new System.Drawing.Size(136, 20);
            this.tb_password.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Password:";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Password:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Name:";
            // 
            // btn_login
            // 
            this.btn_login.Enabled = false;
            this.btn_login.Location = new System.Drawing.Point(266, 14);
            this.btn_login.Name = "btn_login";
            this.btn_login.Size = new System.Drawing.Size(75, 23);
            this.btn_login.TabIndex = 19;
            this.btn_login.Text = "Login";
            this.btn_login.UseVisualStyleBackColor = true;
            this.btn_login.Click += new System.EventHandler(this.btn_login_Click);
            // 
            // lable7
            // 
            this.lable7.AutoSize = true;
            this.lable7.Location = new System.Drawing.Point(420, 410);
            this.lable7.Name = "lable7";
            this.lable7.Size = new System.Drawing.Size(39, 13);
            this.lable7.TabIndex = 20;
            this.lable7.Text = "Group:";
            // 
            // lb_groups
            // 
            this.lb_groups.FormattingEnabled = true;
            this.lb_groups.Location = new System.Drawing.Point(420, 452);
            this.lb_groups.Name = "lb_groups";
            this.lb_groups.Size = new System.Drawing.Size(198, 95);
            this.lb_groups.TabIndex = 21;
            this.lb_groups.DoubleClick += new System.EventHandler(this.lb_groups_DoubleCLick);
            // 
            // tb_gid
            // 
            this.tb_gid.Location = new System.Drawing.Point(420, 426);
            this.tb_gid.Name = "tb_gid";
            this.tb_gid.Size = new System.Drawing.Size(136, 20);
            this.tb_gid.TabIndex = 22;
            // 
            // btn_newGroup
            // 
            this.btn_newGroup.Enabled = false;
            this.btn_newGroup.Location = new System.Drawing.Point(562, 424);
            this.btn_newGroup.Name = "btn_newGroup";
            this.btn_newGroup.Size = new System.Drawing.Size(56, 23);
            this.btn_newGroup.TabIndex = 23;
            this.btn_newGroup.Text = "Create";
            this.btn_newGroup.UseVisualStyleBackColor = true;
            this.btn_newGroup.Click += new System.EventHandler(this.btn_newGroup_Click);
            // 
            // frm_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 750);
            this.Controls.Add(this.btn_newGroup);
            this.Controls.Add(this.tb_gid);
            this.Controls.Add(this.lb_groups);
            this.Controls.Add(this.lable7);
            this.Controls.Add(this.btn_login);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tb_password);
            this.Controls.Add(this.btn_register);
            this.Controls.Add(this.tb_password2);
            this.Controls.Add(this.tb_uid2);
            this.Controls.Add(this.tc_conversations);
            this.Controls.Add(this.tb_ip);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tb_log);
            this.Controls.Add(this.lb_clients);
            this.Controls.Add(this.tb_uid);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_connect);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frm_main";
            this.Text = "MessageSystemCS | DesktopApp";
            this.Activated += new System.EventHandler(this.frm_main_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_main_FormClosing);
            this.Load += new System.EventHandler(this.frm_main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_connect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_uid;
        private System.Windows.Forms.ListBox lb_clients;
        private System.Windows.Forms.RichTextBox tb_log;
        private System.Windows.Forms.TextBox tb_ip;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabControl tc_conversations;
        private TextBox tb_uid2;
        private TextBox tb_password2;
        private Button btn_register;
        private TextBox tb_password;
        private Label label4;
        private Label label5;
        private Label label6;
        private Button btn_login;
        private Label lable7;
        private ListBox lb_groups;
        private TextBox tb_gid;
        private Button btn_newGroup;
    }
}

