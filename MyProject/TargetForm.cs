using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyProject
{
    public partial class TargetForm : Form
    {
       
        public TargetForm()
        {
            InitializeComponent();
            //TopMost = true;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.LightGreen;
            TransparencyKey = Color.LightGreen;
            Width = SystemInformation.VirtualScreen.Width;
            Height = SystemInformation.VirtualScreen.Height;
            TopMost = true;
            ShowInTaskbar = false;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.LightGreen;
            TransparencyKey = Color.LightGreen;
            Width = SystemInformation.VirtualScreen.Width;
            Height = SystemInformation.VirtualScreen.Height;
         }

   
        public void ShowActiveServer_Paint(object sender, PaintEventArgs e)
        {

            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(255, 0, 255, 0));
            pen.Width = 15;
            e.Graphics.DrawLine(pen, 0, 0, 0, Screen.PrimaryScreen.Bounds.Height);
            e.Graphics.DrawLine(pen, 0, 0, Screen.PrimaryScreen.Bounds.Width, 0);
            e.Graphics.DrawLine(pen, Screen.PrimaryScreen.Bounds.Width, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            e.Graphics.DrawLine(pen, 0, Screen.PrimaryScreen.Bounds.Height, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            //Image newImage = Image.FromFile("foto.jpg");

            // Create coordinates for upper-left corner of image.
            //float x = Screen.PrimaryScreen.Bounds.Width - newImage.Width - 25;
            float y = 30;

            // Draw image to screen.
            //g.DrawImage(newImage, x, y);
            e.Graphics.Dispose();

            //ReleaseDC(IntPtr.Zero, desktopPtr);

        }

        public void ExitTarget()
        {
            // Quit without further ado.
            Application.Exit();
        }




        private void TargetForm_Load(object sender, EventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}