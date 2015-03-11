namespace ProgettoPdS
{
    partial class ControlForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(284, 261);
            this.label1.TabIndex = 0;
            this.label1.Text = "Premi CTRL + Q per uscire da questa sessione";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ControlForm_MouseDown);
            this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMoveIntercepter);
            this.label1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ControlForm_MouseUp);
            
            // 
            // ControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.label1);
            this.KeyPreview = true;
            this.Name = "ControlForm";
            this.Text = "ControlForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ControlForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ControlForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ControlForm_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ControlForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMoveIntercepter);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ControlForm_MouseUp);

            // Associo metodo all'evento MouseWheel
            this.MouseWheel += ControlForm_MouseWheel;

            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;


    }
}