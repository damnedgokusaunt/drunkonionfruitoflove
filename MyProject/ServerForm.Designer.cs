﻿using System;
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
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.startButton = new System.Windows.Forms.Button();
            this.portBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sep = new System.Windows.Forms.ToolStripSeparator();
            this.item = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.quitButton = new System.Windows.Forms.Button();
            this.menu.SuspendLayout();
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
            // passwordBox
            // 
            this.passwordBox.Location = new System.Drawing.Point(76, 163);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.Size = new System.Drawing.Size(100, 20);
            this.passwordBox.TabIndex = 20;
            this.passwordBox.Text = "pass";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(76, 206);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(60, 23);
            this.startButton.TabIndex = 19;
            this.startButton.Text = "Avvia";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(76, 112);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(100, 20);
            this.portBox.TabIndex = 17;
            this.portBox.Text = "3000";
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
            this.notifyIcon1.ContextMenuStrip = this.menu;
            this.notifyIcon1.Icon = global::MyProject.Properties.Resources.SystemTrayApp;
            this.notifyIcon1.Text = "Viola-Costanzo PdS Project";
            this.notifyIcon1.Visible = true;
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sep,
            this.item});
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(93, 32);
            // 
            // sep
            // 
            this.sep.Name = "sep";
            this.sep.Size = new System.Drawing.Size(89, 6);
            // 
            // item
            // 
            this.item.Image = global::MyProject.Properties.Resources.Exit;
            this.item.Name = "item";
            this.item.Size = new System.Drawing.Size(92, 22);
            this.item.Text = "Exit";
            this.item.Click += new System.EventHandler(this.Exit_Click);
            // 
            // comboBox
            // 
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Location = new System.Drawing.Point(76, 37);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(173, 21);
            this.comboBox.TabIndex = 22;
            this.comboBox.Text = "< Select an IP Address >";
            this.comboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
            // 
            // quitButton
            // 
            this.quitButton.Enabled = false;
            this.quitButton.Location = new System.Drawing.Point(142, 206);
            this.quitButton.Name = "quitButton";
            this.quitButton.Size = new System.Drawing.Size(60, 23);
            this.quitButton.TabIndex = 23;
            this.quitButton.Text = "Termina";
            this.quitButton.UseVisualStyleBackColor = true;
            this.quitButton.Click += new System.EventHandler(this.quitButton_Click);
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.quitButton);
            this.Controls.Add(this.comboBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.passwordBox);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.portBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ServerForm";
            this.Text = "Progetto PDS";
            this.Load += new System.EventHandler(this.ServerForm_Load);
            this.Resize += new System.EventHandler(this.TrayMinimizerForm_Resize);
            this.menu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox passwordBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox portBox;
        private Button button1;
        private ContextMenuStrip menu;
        private ComboBox comboBox;
        private Button quitButton;
        private NotifyIcon notifyIcon1;
        private ToolStripMenuItem item;
        private ToolStripSeparator sep;
        
    }
}