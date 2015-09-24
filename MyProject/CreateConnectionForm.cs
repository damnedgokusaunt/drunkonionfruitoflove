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
    public partial class CreateConnectionForm : Form
    {
        public delegate void NewConnectionDelegate(string ip_address, string port, string password);
        public NewConnectionDelegate new_connection;

        public CreateConnectionForm()
        {
            InitializeComponent();

            this.ip_textbox.ValidatingType = typeof(System.Net.IPAddress);
        }

        private void connect_button_Click(object sender, EventArgs e)
        {
            if (ip_textbox.Text == string.Empty || port_textbox.Text == string.Empty || password_textbox.Text == string.Empty)
            {
                MessageBox.Show("Alcuni campi sono vuoti.");

                return;
            }

            new_connection(ip_textbox.Text, port_textbox.Text, password_textbox.Text);
            
            this.Close();
        }

        private void CreateConnectionForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }
    }
}
