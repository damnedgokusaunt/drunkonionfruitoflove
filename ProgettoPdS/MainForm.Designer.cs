using System;
using System.Windows.Forms;

namespace ProgettoPdS
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
            this.label3 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.pwd2 = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.dsfdsdsdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nuovoServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chiudiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView = new System.Windows.Forms.ListView();
            this.label4 = new System.Windows.Forms.Label();
            this.pwd1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.serverPortBox = new System.Windows.Forms.TextBox();
            this.serverAddrBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.startClientButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(365, 202);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(100, 20);
            this.portBox.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(474, 205);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Porta server";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(365, 254);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "Avvia server";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(474, 231);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Password";
            // 
            // pwd2
            // 
            this.pwd2.Location = new System.Drawing.Point(365, 228);
            this.pwd2.Name = "pwd2";
            this.pwd2.Size = new System.Drawing.Size(100, 20);
            this.pwd2.TabIndex = 15;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dsfdsdsdfToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(573, 24);
            this.menuStrip1.TabIndex = 17;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // dsfdsdsdfToolStripMenuItem
            // 
            this.dsfdsdsdfToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nuovoServerToolStripMenuItem,
            this.chiudiToolStripMenuItem});
            this.dsfdsdsdfToolStripMenuItem.Name = "dsfdsdsdfToolStripMenuItem";
            this.dsfdsdsdfToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.dsfdsdsdfToolStripMenuItem.Text = "Impostazioni";
            this.dsfdsdsdfToolStripMenuItem.Click += new System.EventHandler(this.dsfdsdsdfToolStripMenuItem_Click);
            // 
            // nuovoServerToolStripMenuItem
            // 
            this.nuovoServerToolStripMenuItem.Name = "nuovoServerToolStripMenuItem";
            this.nuovoServerToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.nuovoServerToolStripMenuItem.Text = "Nuovo server";
            this.nuovoServerToolStripMenuItem.Click += new System.EventHandler(this.nuovoServerToolStripMenuItem_Click);
            // 
            // chiudiToolStripMenuItem
            // 
            this.chiudiToolStripMenuItem.Name = "chiudiToolStripMenuItem";
            this.chiudiToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.chiudiToolStripMenuItem.Text = "Chiudi";
            this.chiudiToolStripMenuItem.Click += new System.EventHandler(this.chiudiToolStripMenuItem_Click);
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
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView.DoubleClick += new System.EventHandler(this.item_DoubleClick);
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
            // 
            // serverAddrBox
            // 
            this.serverAddrBox.Location = new System.Drawing.Point(462, 45);
            this.serverAddrBox.Name = "serverAddrBox";
            this.serverAddrBox.Size = new System.Drawing.Size(100, 20);
            this.serverAddrBox.TabIndex = 24;
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
            this.startClientButton.Location = new System.Drawing.Point(333, 143);
            this.startClientButton.Name = "startClientButton";
            this.startClientButton.Size = new System.Drawing.Size(98, 23);
            this.startClientButton.TabIndex = 22;
            this.startClientButton.Text = "Connetti";
            this.startClientButton.UseVisualStyleBackColor = true;
            this.startClientButton.Click += new System.EventHandler(this.startClientButton_Click);
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(462, 143);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(100, 23);
            this.StopButton.TabIndex = 29;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(106, 182);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 23);
            this.button1.TabIndex = 30;
            this.button1.Text = "Seleziona server";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(573, 309);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pwd1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.serverPortBox);
            this.Controls.Add(this.serverAddrBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.startClientButton);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pwd2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.portBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Progetto PDS";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox pwd2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem dsfdsdsdfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nuovoServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chiudiToolStripMenuItem;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox pwd1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox serverPortBox;
        private System.Windows.Forms.TextBox serverAddrBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button startClientButton;
        private Button StopButton;
        private Button button1;
    }
}

