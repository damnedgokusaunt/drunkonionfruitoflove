using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProgettoPdS;
using ProgettoPdS.Properties;

//using ProgettoPdS.Properties;


namespace ProgettoPdS
{
	/// <summary>
	/// 
	/// </summary>
	class ProcessIcon : IDisposable
	{
		/// <summary>
		/// The NotifyIcon object.
		/// </summary>
		NotifyIcon ni;
        ServerForm frm;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessIcon"/> class.
		/// </summary>
		public ProcessIcon()
		{
			// Instantiate the NotifyIcon object.
			ni = new NotifyIcon();       
		}

        public void setFrm(ref ServerForm frm)
        {
            this.frm = frm;
        }
		/// <summary>
		/// Displays the icon in the system tray.
		/// </summary>
		public void Display()
		{
         /*   if (ni.Visible == false){
                ni.MouseClick += new MouseEventHandler(ni_MouseClick);
                ni.ContextMenuStrip = new ContextMenus().Create();                
                }else
            */
            ni.MouseClick += new MouseEventHandler(ni_MouseClick);
            ni.Icon = Resources.SystemTrayApp;
            ni.Text = "Viola-Costanzo PdS Project";
            ni.Visible = true;
            ni.ContextMenuStrip = new ContextMenus().Create(ref frm);
            ni.BalloonTipIcon = ToolTipIcon.Info;
            ni.BalloonTipText = "Fai click destro per selezionare l'opzione preferita!";
            ni.BalloonTipTitle = "Progetto PdS Viola-Costanzo";
        }
        
		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		public void Dispose()
		{
			// When the application closes, this will remove the icon from the system tray immediately.
			ni.Dispose();
            
		}

		/// <summary>
		/// Handles the MouseClick event of the ni control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		void ni_MouseClick(object sender, MouseEventArgs e)
		{
			// Handle mouse button clicks.
			if (e.Button == MouseButtons.Left)
			{
				// Start Windows Explorer.
                //Process.Start("explorer", null);

			}
		}
	}
}