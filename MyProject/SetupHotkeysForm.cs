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
    public partial class SetupHotkeysForm : Form
    {
        public delegate void SetupHotkeysDelegate();
        public SetupHotkeysDelegate setup_hotkeys;

        private HotkeysHandler hotkeys_handler;
        private ComboBox[] comboBoxes;

        private enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }

        public SetupHotkeysForm(IntPtr hWnd)
        {
            InitializeComponent();

            this.hotkeys_handler = new HotkeysHandler(hWnd);

            comboBoxes = new ComboBox[] 
            {
                this.comboBox1,
                this.comboBox2,
                this.comboBox3,
                this.comboBox4,
                this.comboBox5,
                this.comboBox6,
                this.comboBox7,
                this.comboBox8,
            };

            this.comboBox2.Items.AddRange(new object[] { Keys.F1, Keys.F2, Keys.F3 });
            this.comboBox4.Items.AddRange(new object[] { Keys.F4, Keys.F5, Keys.F6 });
            this.comboBox6.Items.AddRange(new object[] { Keys.F7, Keys.F8, Keys.F9 });
            this.comboBox8.Items.AddRange(new object[] { Keys.F10, Keys.F11, Keys.F12 });

            for (int i = 0; i < comboBoxes.Length; i++)
            {
                if (i % 2 == 0)
                {
                    comboBoxes[i].Items.AddRange(new object[] { KeyModifier.Control});
                }

                comboBoxes[i].Text = comboBoxes[i].GetItemText(comboBoxes[i].Items[0]);
                comboBoxes[i].TextChanged += delegate { this.button1.Enabled = true;  };
            }

            this.SetHotkeys();
        }

        private void SetupHotkeysForm_Load(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetHotkeys()
        {
            if(comboBoxes == null)
                return;

            for (int i = 0; i < comboBoxes.Length; i += 2)
                hotkeys_handler.Register(i, (int)comboBoxes[i].SelectedItem, comboBoxes[i + 1].SelectedItem.GetHashCode());
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.SetHotkeys();

            this.button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.SetHotkeys();

            this.Close();
        }
    }
}
