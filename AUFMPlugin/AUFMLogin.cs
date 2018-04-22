using AUFMPlugin.Properties;
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
        public AUFMLogin()
        {
            InitializeComponent();
            userbox.Text = Settings.Default.Username;
            url.Text = Settings.Default.Url;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.Url = webbox.Text;
            url.Text = webbox.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Library.login(userbox.Text, passbox.Text);
            Settings.Default.Username = userbox.Text;
        }
    }
}
