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
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.azioniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nuovaConnessioneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.esciToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.impostazioniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hotkeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visualizzaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guidaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualeUtenteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.progress_bar = new System.Windows.Forms.ProgressBar();
            this.listView = new System.Windows.Forms.ListView();
            this.ID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Indirizzo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Porta = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Stato = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip2
            // 
            this.menuStrip2.BackColor = System.Drawing.Color.DodgerBlue;
            this.menuStrip2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.azioniToolStripMenuItem,
            this.impostazioniToolStripMenuItem,
            this.visualizzaToolStripMenuItem,
            this.guidaToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(0, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(480, 24);
            this.menuStrip2.TabIndex = 48;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // azioniToolStripMenuItem
            // 
            this.azioniToolStripMenuItem.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.azioniToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nuovaConnessioneToolStripMenuItem,
            this.esciToolStripMenuItem});
            this.azioniToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.azioniToolStripMenuItem.ForeColor = System.Drawing.Color.MidnightBlue;
            this.azioniToolStripMenuItem.Name = "azioniToolStripMenuItem";
            this.azioniToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.azioniToolStripMenuItem.Text = "Azioni";
            // 
            // nuovaConnessioneToolStripMenuItem
            // 
            this.nuovaConnessioneToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.nuovaConnessioneToolStripMenuItem.Name = "nuovaConnessioneToolStripMenuItem";
            this.nuovaConnessioneToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.nuovaConnessioneToolStripMenuItem.Text = "Nuova connessione";
            this.nuovaConnessioneToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.nuovaConnessioneToolStripMenuItem.Click += new System.EventHandler(this.nuovaConnessioneToolStripMenuItem_Click);
            // 
            // esciToolStripMenuItem
            // 
            this.esciToolStripMenuItem.Name = "esciToolStripMenuItem";
            this.esciToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.esciToolStripMenuItem.Text = "Esci";
            this.esciToolStripMenuItem.Click += new System.EventHandler(this.esciToolStripMenuItem_Click);
            // 
            // impostazioniToolStripMenuItem
            // 
            this.impostazioniToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hotkeyToolStripMenuItem});
            this.impostazioniToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.impostazioniToolStripMenuItem.ForeColor = System.Drawing.Color.MidnightBlue;
            this.impostazioniToolStripMenuItem.Name = "impostazioniToolStripMenuItem";
            this.impostazioniToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.impostazioniToolStripMenuItem.Text = "Impostazioni";
            // 
            // hotkeyToolStripMenuItem
            // 
            this.hotkeyToolStripMenuItem.Name = "hotkeyToolStripMenuItem";
            this.hotkeyToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.hotkeyToolStripMenuItem.Text = "Hotkey";
            this.hotkeyToolStripMenuItem.Click += new System.EventHandler(this.hotkeyToolStripMenuItem_Click);
            // 
            // visualizzaToolStripMenuItem
            // 
            this.visualizzaToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.visualizzaToolStripMenuItem.ForeColor = System.Drawing.Color.MidnightBlue;
            this.visualizzaToolStripMenuItem.Name = "visualizzaToolStripMenuItem";
            this.visualizzaToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.visualizzaToolStripMenuItem.Text = "Visualizza";
            // 
            // guidaToolStripMenuItem
            // 
            this.guidaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manualeUtenteToolStripMenuItem,
            this.infoToolStripMenuItem});
            this.guidaToolStripMenuItem.ForeColor = System.Drawing.Color.MidnightBlue;
            this.guidaToolStripMenuItem.Name = "guidaToolStripMenuItem";
            this.guidaToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.guidaToolStripMenuItem.Text = "Guida";
            // 
            // manualeUtenteToolStripMenuItem
            // 
            this.manualeUtenteToolStripMenuItem.Name = "manualeUtenteToolStripMenuItem";
            this.manualeUtenteToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.manualeUtenteToolStripMenuItem.Text = "Manuale utente";
            // 
            // infoToolStripMenuItem
            // 
            this.infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            this.infoToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.infoToolStripMenuItem.Text = "Info";
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.Location = new System.Drawing.Point(3, 214);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(104, 23);
            this.DisconnectButton.TabIndex = 31;
            this.DisconnectButton.Text = "Chiudi server";
            this.DisconnectButton.UseVisualStyleBackColor = true;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // progress_bar
            // 
            this.progress_bar.Location = new System.Drawing.Point(113, 214);
            this.progress_bar.Name = "progress_bar";
            this.progress_bar.Size = new System.Drawing.Size(236, 23);
            this.progress_bar.TabIndex = 46;
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ID,
            this.Indirizzo,
            this.Porta,
            this.Stato});
            this.listView.Location = new System.Drawing.Point(113, 3);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(248, 205);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // ID
            // 
            this.ID.Text = "ID";
            // 
            // Indirizzo
            // 
            this.Indirizzo.Text = "Indirizzo di rete";
            // 
            // Porta
            // 
            this.Porta.Text = "Porta";
            // 
            // Stato
            // 
            this.Stato.Text = "Stato";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.96451F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.02714F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.00835F));
            this.tableLayoutPanel1.Controls.Add(this.listView, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.progress_bar, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.DisconnectButton, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(480, 271);
            this.tableLayoutPanel1.TabIndex = 51;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 295);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Remote Control Client";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip2;
        private ToolStripMenuItem azioniToolStripMenuItem;
        private ToolStripMenuItem impostazioniToolStripMenuItem;
        private ToolStripMenuItem hotkeyToolStripMenuItem;
        private ToolStripMenuItem visualizzaToolStripMenuItem;
        private ToolStripMenuItem nuovaConnessioneToolStripMenuItem;
        private ToolStripMenuItem esciToolStripMenuItem;
        private ToolStripMenuItem guidaToolStripMenuItem;
        private ToolStripMenuItem manualeUtenteToolStripMenuItem;
        private ToolStripMenuItem infoToolStripMenuItem;
        private Button DisconnectButton;
        public ProgressBar progress_bar;
        private ListView listView;
        private ColumnHeader ID;
        private ColumnHeader Indirizzo;
        private ColumnHeader Porta;
        private ColumnHeader Stato;
        private TableLayoutPanel tableLayoutPanel1;
    }
}

