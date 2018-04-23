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
using System.Diagnostics;

namespace AUFMPlugin
{
    public partial class ControlPane : UserControl
    {
        private String PID;
        
        public ControlPane()
        {
            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.Changed += new EventHandler<EventArgs>(Selection_Changed);
            InitializeComponent();
            linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(linkClicked);
            if (Settings.Default.Cookie != "")
            {
                updateBuilding(Settings.Default.Building);
            } else
            {
                updateBuilding("Login Required");
            }
        }

        private void linkClicked(object sender, EventArgs e)
        {
            var json = Library.getHttpRequest("/api/building/" + Settings.Default.Building);
            if (json != "error") {
                Building b = JsonConvert.DeserializeObject<Building>(json);
                Process.Start(Settings.Default.Url + "/#parts/" + b.building_id);
            }
        }

        public void updateBuilding(String buildingName)
        {
            linkLabel1.Text = buildingName;
        }

        private void Selection_Changed(object sender, EventArgs e)
        {
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            if (doc.CurrentSelection.SelectedItems.Count == 1 && IsValidPart(doc.CurrentSelection.SelectedItems.ElementAt<ModelItem>(0)))
            {
                ModelItem selectedItem = doc.CurrentSelection.SelectedItems.ElementAt<ModelItem>(0);
                String partID = selectedItem.PropertyCategories.FindPropertyByDisplayName("Element ID", "Value").Value.ToDisplayString().ToString();
                String url = "/api/part/" + partID + "/protocol";
                String response = Library.getHttpRequest(url);
                response = response == "error" ? null: response ;
                JObject json = null;
                try
                {
                    json = JObject.Parse(response);
                    response = Library.getHttpRequest("/api/part/" + partID);
                    this.label1.Text = JsonConvert.DeserializeObject<Part>(response).part_name + " (" + partID + ")";
                    this.GotoWebsite.Visible = true;
                    PID = partID;
                }
                catch (ArgumentNullException)
                {
                    this.label1.Text = "Not In Database";
                    this.GotoWebsite.Visible = false;
                    this.dataGridView1.Visible = false;   
                }
                if (json != null)
                {
                    PartProtocol part = json.ToObject<PartProtocol>();
                    this.dataGridView1.Rows.Clear();
                    foreach (Protocol protocol in part.protocols)
                    {
                        this.dataGridView1.Rows.Add(this.dataGridView1.Rows.Count + 1, protocol.value);
                    }
                    if (this.dataGridView1.Rows.Count > 0)
                    { 
                        this.dataGridView1.Visible = true;
                    } else
                    {
                        this.label1.Text += "\r\nNo Protocols Attached";
                        this.dataGridView1.Visible = false;
                    }
                }
            }
            else
            {
                this.label1.Text = "Make A Selection";
                this.dataGridView1.Visible = false;
                this.GotoWebsite.Visible = false;
            }
        }

        private bool IsValidPart(ModelItem part)
        {
            if (part == null || part.PropertyCategories.FindPropertyByDisplayName("Element ID", "Value") == null)
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

        private void GotoWebsite_Click(object sender, EventArgs e)
        {
            Process.Start(Settings.Default.Url + "/#protocols/" + PID);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
