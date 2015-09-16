using MyProject.Properties;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace MyProject
{
    partial class ServerForm
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


        

      // private System.ComponentModel.BackgroundWorker backgroundWorker1;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label5 = new System.Windows.Forms.Label();
            this.pwd2 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.portBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.item = new System.Windows.Forms.ToolStripMenuItem();
            this.sep = new System.Windows.Forms.ToolStripSeparator();

            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(103, 147);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Password";
            // 
            // pwd2
            // 
            this.pwd2.Location = new System.Drawing.Point(76, 163);
            this.pwd2.Name = "pwd2";
            this.pwd2.Size = new System.Drawing.Size(100, 20);
            this.pwd2.TabIndex = 20;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(76, 206);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(60, 23);
            this.button2.TabIndex = 19;
            this.button2.Text = "Avvia";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(76, 112);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(100, 20);
            this.portBox.TabIndex = 17;
            this.portBox.TextChanged += new System.EventHandler(this.portBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(103, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Porta server";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.Icon = global::MyProject.Properties.Resources.SystemTrayApp;
            this.notifyIcon1.Text = "Project";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.ContextMenuStrip = this.menu;
            
            // 
            // menu
            // 
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(61, 4);
            this.menu.Visible = true;
            
            //
            // item1
            // 
            this.item.Image = global::MyProject.Properties.Resources.Explorer;
            this.item.Name = "item1";
            this.item.Size = new System.Drawing.Size(191, 22);
            this.item.Text = "Vai alle info del server!";
            this.item.Click += new System.EventHandler(this.Explorer_Click);
            // 
            // item2
            // 
            this.item.Image = global::MyProject.Properties.Resources.About;
            this.item.Name = "item2";
            this.item.Size = new System.Drawing.Size(191, 22);
            this.item.Text = "About";
            this.item.Click += new System.EventHandler(this.About_Click);
            // 
            // sep
            // 
            this.sep.Name = "sep";
            this.sep.Size = new System.Drawing.Size(188, 6);
            // 
            // item3
            // 
            this.item.Image = global::MyProject.Properties.Resources.Exit;
            this.item.Name = "item3";
            this.item.Size = new System.Drawing.Size(191, 22);
            this.item.Text = "Exit";
            this.item.Click += new System.EventHandler(this.Exit_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(76, 37);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(173, 21);
            this.comboBox1.TabIndex = 22;
            this.comboBox1.Text = "< Select an IP Address >";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(142, 206);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(60, 23);
            this.button3.TabIndex = 23;
            this.button3.Text = "Termina";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pwd2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.portBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ServerForm";
            this.Text = "Progetto";
            this.Load += new System.EventHandler(this.Start_Form_Load);
            this.Resize += new System.EventHandler(this.TrayMinimizerForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

            // Windows Explorer.
            item = new ToolStripMenuItem();
            item.Text = "Vai alle info del server!";
            item.Click += new EventHandler(Explorer_Click);
            item.Image = Resources.Explorer;
            menu.Items.Add(item);

            // About.
            item = new ToolStripMenuItem();
            item.Text = "About";
            item.Click += new EventHandler(About_Click);
            item.Image = Resources.About;
            menu.Items.Add(item);

            // Separator.
            sep = new ToolStripSeparator();
            menu.Items.Add(sep);

            // Exit.
            item = new ToolStripMenuItem();
            item.Text = "Exit";
            item.Click += new System.EventHandler(Exit_Click);
            item.Image = Resources.Exit;
            menu.Items.Add(item);

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox pwd2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox portBox;
        private Button button1;
        private ContextMenuStrip menu;
        private ComboBox comboBox1;
        private Button button3;
        private NotifyIcon notifyIcon1;
        private ToolStripMenuItem item;
        private ToolStripSeparator sep;
        
    }
}