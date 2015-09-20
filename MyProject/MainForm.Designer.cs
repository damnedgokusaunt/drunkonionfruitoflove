using System;
using System.Windows.Forms;

namespace MyProject
{
    partial class MainForm
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Liberare le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.portBox = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.listView = new System.Windows.Forms.ListView();
            this.label4 = new System.Windows.Forms.Label();
            this.pwd1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.serverPortBox = new System.Windows.Forms.TextBox();
            this.serverAddrBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.startClientButton = new System.Windows.Forms.Button();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(452, 188);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(110, 20);
            this.portBox.TabIndex = 10;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(644, 24);
            this.menuStrip1.TabIndex = 17;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // listView
            // 
            this.listView.AllowColumnReorder = true;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.LabelEdit = true;
            this.listView.Location = new System.Drawing.Point(12, 27);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(312, 139);
            this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView.TabIndex = 18;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(330, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Password";
            // 
            // pwd1
            // 
            this.pwd1.Location = new System.Drawing.Point(462, 105);
            this.pwd1.Name = "pwd1";
            this.pwd1.Size = new System.Drawing.Size(100, 20);
            this.pwd1.TabIndex = 27;
            this.pwd1.Text = "pass";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(330, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Porta";
            // 
            // serverPortBox
            // 
            this.serverPortBox.Location = new System.Drawing.Point(462, 75);
            this.serverPortBox.Name = "serverPortBox";
            this.serverPortBox.Size = new System.Drawing.Size(100, 20);
            this.serverPortBox.TabIndex = 25;
            this.serverPortBox.Text = "3000";
            // 
            // serverAddrBox
            // 
            this.serverAddrBox.Location = new System.Drawing.Point(462, 45);
            this.serverAddrBox.Name = "serverAddrBox";
            this.serverAddrBox.Size = new System.Drawing.Size(100, 20);
            this.serverAddrBox.TabIndex = 24;
            this.serverAddrBox.Text = "169.254.32.225";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(330, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Indirizzo IPv4";
            // 
            // startClientButton
            // 
            this.startClientButton.Location = new System.Drawing.Point(462, 143);
            this.startClientButton.Name = "startClientButton";
            this.startClientButton.Size = new System.Drawing.Size(100, 23);
            this.startClientButton.TabIndex = 22;
            this.startClientButton.Text = "Connetti";
            this.startClientButton.UseVisualStyleBackColor = true;
            this.startClientButton.Click += new System.EventHandler(this.startClientButton_Click);
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.Location = new System.Drawing.Point(13, 185);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(123, 23);
            this.DisconnectButton.TabIndex = 31;
            this.DisconnectButton.Text = "Chiudi server";
            this.DisconnectButton.UseVisualStyleBackColor = true;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 259);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 35;
            this.label3.Text = "Choose CTRL + ";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.AllowDrop = true;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(102, 256);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 36;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(229, 259);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 37;
            this.label5.Text = "to select a server";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 301);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 38;
            this.label6.Text = "Choose CTRL + ";
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(102, 298);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 21);
            this.comboBox2.TabIndex = 39;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(231, 301);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(124, 13);
            this.label7.TabIndex = 40;
            this.label7.Text = "to suspend a connection";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(134, 240);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 41;
            this.label8.Text = "HOTKEY";
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(333, 254);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(35, 23);
            this.button2.TabIndex = 42;
            this.button2.Text = "Ok";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(361, 296);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(35, 23);
            this.button4.TabIndex = 43;
            this.button4.Text = "Ok";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(420, 263);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(105, 68);
            this.button5.TabIndex = 44;
            this.button5.Text = "Importa appunti (server->client)";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(531, 262);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(102, 71);
            this.button6.TabIndex = 45;
            this.button6.Text = "Esporta appunti (client->server)";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 342);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.DisconnectButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pwd1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.serverPortBox);
            this.Controls.Add(this.serverAddrBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.startClientButton);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.portBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Progetto MyProject";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox pwd1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox serverPortBox;
        private System.Windows.Forms.TextBox serverAddrBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button startClientButton;
        private Button DisconnectButton;
        private Label label3;
        private ComboBox comboBox1;
        private Label label5;
        private Label label6;
        private ComboBox comboBox2;
        private Label label7;
        private Label label8;
        private Button button2;
        private Button button4;
        private Button button5;
        private Button button6;
    }
}

