using AUFMPlugin.Properties;
using Autodesk.Navisworks.Api.Plugins;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AUFMPlugin
{
    public partial class AUFMLogin : Form
    {
        private AUFMDockPane dockPanePlugin;
        public AUFMLogin()
        {
            InitializeComponent();
            userbox.Text = Settings.Default.Username;
            url.Text = Settings.Default.Url;
            PluginRecord pluginRecord = Autodesk.Navisworks.Api.Application.Plugins.FindPlugin("AUFMPlugin.AUFM");
            dockPanePlugin = (AUFMDockPane)pluginRecord.LoadPlugin();
            if (dockPanePlugin == null)
            {
                dockPanePlugin = new AUFMDockPane();
                dockPanePlugin.ActivatePane();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.Url = webbox.Text;
            url.Text = webbox.Text;
            Settings.Default.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label4.Text = Library.login(userbox.Text, passbox.Text);
            Settings.Default.Username = userbox.Text;
            Settings.Default.Save();
            if (Library.LoggedIn)
            {
                if (dockPanePlugin != null)
                {
                    dockPanePlugin.updateBuilding(Settings.Default.Building);
                }
                Visible = false; 
            } else
            {
                dockPanePlugin.updateBuilding("Login Required");
            }
            
        }
    }
}
