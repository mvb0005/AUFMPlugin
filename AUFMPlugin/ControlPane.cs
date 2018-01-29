using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;
using Autodesk.Navisworks.Api;

namespace AUFMPlugin
{
    public partial class ControlPane : UserControl
    {
        public ControlPane()
        {
            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.Changed += new EventHandler<EventArgs>(Selection_Changed);
            InitializeComponent();

        }

        private void Selection_Changed(object sender, EventArgs e)
        {
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            if (doc.CurrentSelection.SelectedItems.Count >= 1)
            {
                ModelItem selectedItem = doc.CurrentSelection.SelectedItems.ElementAt<ModelItem>(0);
                this.label1.Text = selectedItem.PropertyCategories.FindCategoryByDisplayName("Element ID").Properties.FindPropertyByDisplayName("Value").Value.ToDisplayString().ToString();
                this.dataGridView1.Visible = true;
                this.dataGridView1.Rows.Clear();
                WebClient client = new WebClient();
                string json = client.DownloadString("https://pastebin.com/raw/aVwZDm7U");
                Dictionary<string, object> properties = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                foreach (string p in properties.Keys)
                {
                    Dictionary<string, string> valued = JsonConvert.DeserializeObject<Dictionary<string, string>>(properties[p].ToString());
                    foreach (string v in valued.Keys)
                    {
                        this.dataGridView1.Rows.Add(v, valued[v]);
                    }
                }
            }
            else
            {
                this.label1.Text = "Make A Selection";
                this.dataGridView1.Visible = false;
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Dock = DockStyle.Fill;
        }

    }
}
