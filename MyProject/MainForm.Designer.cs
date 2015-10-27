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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.azioniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nuovaConnessioneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.esciToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comandiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importaAppuntiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.esportaAppuntiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importaAppuntiToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.esportaAppuntiVersoIlServerRemotoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.impostazioniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hotkeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView = new System.Windows.Forms.ListView();
            this.ID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Indirizzo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Porta = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Stato = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.info_label = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip2
            // 
            this.menuStrip2.BackColor = System.Drawing.Color.White;
            this.menuStrip2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.azioniToolStripMenuItem,
            this.comandiToolStripMenuItem,
            this.impostazioniToolStripMenuItem});
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
            // comandiToolStripMenuItem
            // 
            this.comandiToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importaAppuntiToolStripMenuItem,
            this.esportaAppuntiToolStripMenuItem,
            this.importaAppuntiToolStripMenuItem1,
            this.esportaAppuntiVersoIlServerRemotoToolStripMenuItem});
            this.comandiToolStripMenuItem.Name = "comandiToolStripMenuItem";
            this.comandiToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.comandiToolStripMenuItem.Text = "Comandi";
            // 
            // importaAppuntiToolStripMenuItem
            // 
            this.importaAppuntiToolStripMenuItem.Name = "importaAppuntiToolStripMenuItem";
            this.importaAppuntiToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.importaAppuntiToolStripMenuItem.Text = "Avvia/riprendi controllo";
            this.importaAppuntiToolStripMenuItem.Click += new System.EventHandler(this.importaAppuntiToolStripMenuItem_Click);
            // 
            // esportaAppuntiToolStripMenuItem
            // 
            this.esportaAppuntiToolStripMenuItem.Name = "esportaAppuntiToolStripMenuItem";
            this.esportaAppuntiToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.esportaAppuntiToolStripMenuItem.Text = "Sospendi controllo";
            this.esportaAppuntiToolStripMenuItem.Click += new System.EventHandler(this.esportaAppuntiToolStripMenuItem_Click);
            // 
            // importaAppuntiToolStripMenuItem1
            // 
            this.importaAppuntiToolStripMenuItem1.Name = "importaAppuntiToolStripMenuItem1";
            this.importaAppuntiToolStripMenuItem1.Size = new System.Drawing.Size(275, 22);
            this.importaAppuntiToolStripMenuItem1.Text = "Importa Appunti dal server remoto";
            this.importaAppuntiToolStripMenuItem1.Click += new System.EventHandler(this.importaAppuntiToolStripMenuItem1_Click);
            // 
            // esportaAppuntiVersoIlServerRemotoToolStripMenuItem
            // 
            this.esportaAppuntiVersoIlServerRemotoToolStripMenuItem.Name = "esportaAppuntiVersoIlServerRemotoToolStripMenuItem";
            this.esportaAppuntiVersoIlServerRemotoToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.esportaAppuntiVersoIlServerRemotoToolStripMenuItem.Text = "Esporta Appunti verso il server remoto";
            this.esportaAppuntiVersoIlServerRemotoToolStripMenuItem.Click += new System.EventHandler(this.esportaAppuntiVersoIlServerRemotoToolStripMenuItem_Click);
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
            this.hotkeyToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.hotkeyToolStripMenuItem.Text = "Modifica hotkey";
            this.hotkeyToolStripMenuItem.Click += new System.EventHandler(this.hotkeyToolStripMenuItem_Click);
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ID,
            this.Indirizzo,
            this.Porta,
            this.Stato});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.Location = new System.Drawing.Point(62, 74);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(357, 140);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // ID
            // 
            this.ID.Text = "ID";
            this.ID.Width = 100;
            // 
            // Indirizzo
            // 
            this.Indirizzo.Text = "Indirizzo di rete";
            this.Indirizzo.Width = 100;
            // 
            // Porta
            // 
            this.Porta.Text = "Porta";
            this.Porta.Width = 100;
            // 
            // Stato
            // 
            this.Stato.Text = "Stato";
            this.Stato.Width = 100;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.29167F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75.83333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.875F));
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.listView, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.info_label, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.880898F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.78489F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 53.97828F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.39011F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3.024372F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.941445F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(480, 271);
            this.tableLayoutPanel1.TabIndex = 51;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(62, 220);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(357, 22);
            this.progressBar1.TabIndex = 32;
            // 
            // info_label
            // 
            this.info_label.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.info_label.AutoSize = true;
            this.info_label.Location = new System.Drawing.Point(232, 245);
            this.info_label.Name = "info_label";
            this.info_label.Size = new System.Drawing.Size(16, 8);
            this.info_label.TabIndex = 33;
            this.info_label.Text = "...";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(62, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(357, 58);
            this.pictureBox1.TabIndex = 34;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 295);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Remote Control Client";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip2;
        private ToolStripMenuItem azioniToolStripMenuItem;
        private ToolStripMenuItem impostazioniToolStripMenuItem;
        private ToolStripMenuItem hotkeyToolStripMenuItem;
        private ToolStripMenuItem nuovaConnessioneToolStripMenuItem;
        private ToolStripMenuItem esciToolStripMenuItem;
        private ListView listView;
        private ColumnHeader ID;
        private ColumnHeader Indirizzo;
        private ColumnHeader Porta;
        private ColumnHeader Stato;
        private TableLayoutPanel tableLayoutPanel1;
        private ToolStripMenuItem comandiToolStripMenuItem;
        private ToolStripMenuItem importaAppuntiToolStripMenuItem;
        private ToolStripMenuItem esportaAppuntiToolStripMenuItem;
        private ToolStripMenuItem importaAppuntiToolStripMenuItem1;
        private ToolStripMenuItem esportaAppuntiVersoIlServerRemotoToolStripMenuItem;
        private ProgressBar progressBar1;
        private Label info_label;
        private PictureBox pictureBox1;
    }
}

