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
using Newtonsoft.Json.Linq;
using System.IO;
using AUFMPlugin.Properties;

namespace AUFMPlugin
{
    public partial class ControlPane : UserControl
    {
        public ControlPane()
        {
            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.Changed += new EventHandler<EventArgs>(Selection_Changed);
            InitializeComponent();            
            
        }

        public void updateBuilding(String buildingName)
        {
            linkLabel1.Text = buildingName;
        }

        private void Selection_Changed(object sender, EventArgs e)
        {
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            if (doc.CurrentSelection.SelectedItems.Count == 1 && IsValidPart(doc.CurrentSelection.SelectedItems.ElementAt<ModelItem>(0).Parent))
            {
                ModelItem selectedItem = doc.CurrentSelection.SelectedItems.ElementAt<ModelItem>(0).Parent;
                String partID = selectedItem.PropertyCategories.FindPropertyByDisplayName("Element ID", "Value").Value.ToDisplayString().ToString();
                String url = "part/" + partID + "/protocol";
                String response = Library.getHttpRequest(url);
                response = response == "error" ? response : null;
                JObject json = null;
                try
                {
                    json = JObject.Parse(response);
                    this.label1.Text = partID;
                }
                catch (ArgumentNullException)
                {
                    this.label1.Text = "Part Not Found";
                    this.dataGridView1.Visible = false;   
                }
                if (json != null)
                {
                    PartProtocol part = json.ToObject<PartProtocol>();
                    this.dataGridView1.Rows.Clear();
                    foreach (Protocol protocol in part.protocols)
                    {
                        this.dataGridView1.Rows.Add(protocol.protocol_id, protocol.value);
                    }

                    this.dataGridView1.Visible = true;
                }
            }
            else
            {
                this.label1.Text = "Make A Selection";
                this.dataGridView1.Visible = false;
            }
        }

        private bool IsValidPart(ModelItem part)
        {
            if (part.PropertyCategories.FindPropertyByDisplayName("Element ID", "Value") == null)
            {
                return false;
            }
            return true;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Dock = DockStyle.Fill;
        }

    }
}
