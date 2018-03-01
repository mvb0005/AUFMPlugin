﻿using System;
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

namespace AUFMPlugin
{
    public partial class ControlPane : UserControl
    {
        public ControlPane()
        {
            Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.Changed += new EventHandler<EventArgs>(Selection_Changed);
            InitializeComponent();

        }

        private class Protocol
        {
            public int protocol_id { get; set;}
            public string value { get; set; }
        }

        private void Selection_Changed(object sender, EventArgs e)
        {
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            if (doc.CurrentSelection.SelectedItems.Count >= 1)
            {
                ModelItem selectedItem = doc.CurrentSelection.SelectedItems.ElementAt<ModelItem>(0);
                String partID = selectedItem.PropertyCategories.FindCategoryByDisplayName("Element ID").Properties.FindPropertyByDisplayName("Value").Value.ToDisplayString().ToString();
                WebClient client = new WebClient();
                //JObject json = JObject.Parse(client.DownloadString("/api/part/" + partID + "/protocol"));
                JObject json = JObject.Parse(client.DownloadString("https://pastebin.com/raw/rk8NEyFY")); //w0zSmW4R
                if (json["Error"] != null)
                {
                    this.label1.Text = json["Error"].Value<String>();
                }
                else
                {
                    IList<JToken> results = json["protocols"].Children().ToList();
                    this.dataGridView1.Rows.Clear();
                    foreach (JToken result in results)
                    {
                        Protocol p = result.ToObject<Protocol>();
                        this.dataGridView1.Rows.Add(p.protocol_id, p.value);
                    }
                    this.label1.Text = partID;
                    this.dataGridView1.Visible = true;
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
