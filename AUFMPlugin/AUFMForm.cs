using AUFMPlugin.Properties;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
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
    public partial class AUFMForm : Form
    {
        private AUFMDockPane dockPanePlugin;
        private Building[] buildings;
        private ModelItemCollection parts;
        public AUFMForm()
        {
            InitializeComponent();
            PluginRecord pluginRecord = Autodesk.Navisworks.Api.Application.Plugins.FindPlugin("AUFMPlugin.AUFM");
            dockPanePlugin = (AUFMDockPane)pluginRecord.LoadedPlugin;
            buildings = getBuildings();
            comboBox1.Text = Settings.Default["Building"].ToString();
            label2.Text = comboBox1.Text;
            button1.Click += new EventHandler(this.SelectBuilding);
            button2.Click += new EventHandler(this.uploadParts);
            comboBox1.TextChanged += new EventHandler(this.BuildingChanged);
            Autodesk.Navisworks.Api.Application.ActiveDocument.Models.CollectionChanged += fileChanged;
            populateComboBox();
            updateButton();
            getParts();

        }

        private void BuildingChanged(object sender, EventArgs e)
        {
            updateButton();
        }

        void uploadParts(object sender, EventArgs e)
        {
            if (!isBuildinginAPI(comboBox1.Text))
            {
                AddBuildingToApi(comboBox1.Text);
                updateButton();
            }
            AddPartsToBuildinginApi();
        }

        private void fileChanged(object sender, EventArgs e)
        {
            getParts();
        }

        void updateButton()
        {
            if (isBuildinginAPI(comboBox1.Text))
            {
                button1.Text = "Select Building";
            }
            else
            {
                button1.Text = "Add Building To API";
            }
        }
        
        bool isBuildinginAPI(String buildingName)
        {
            Building building = new Building();
            building.name = buildingName;
            return buildings.Contains(building);
        }

        void SelectBuilding(object sender, EventArgs e)
        {
            if (comboBox1.Text.Length > 0) {
                Settings.Default["Building"] = comboBox1.Text;
                Settings.Default.Save();
                if (dockPanePlugin != null)
                {
                    dockPanePlugin.updateBuilding(comboBox1.Text);
                }
                label2.Text = comboBox1.Text;
                if (!isBuildinginAPI(comboBox1.Text))
                {
                    AddBuildingToApi(comboBox1.Text);
                }
                buildings = getBuildings();
                populateComboBox();
                updateButton();
            }
        }

        void populateComboBox()
        {
            String url = "/api/building";
            String response = Library.getHttpRequest(url);
            comboBox1.Items.Clear();
            if (response != null && response != "error")
            {
                Building[] buildings = JsonConvert.DeserializeObject<Building[]>(response);
                foreach (Building building in buildings)
                {
                    comboBox1.Items.Add(building.name);
                }
            }
        }

        String AddBuildingToApi(String buildingName)
        {
            
            var result = Library.postHttpRequest("/api/building", new JObject(new JProperty("name", buildingName)).ToString());
            return result;
        }

        void AddPartsToBuildinginApi()
        {
            Building b = Array.Find(buildings, building => building.name == comboBox1.Text);
            var total = "";
            foreach (ModelItem modelItem in parts)
            {
                Part p = new Part
                {
                    building_id = b.building_id,
                    element_id = Int32.Parse(modelItem.PropertyCategories.FindPropertyByDisplayName("Element ID", "Value").Value.ToDisplayString()),
                    part_name = modelItem.DisplayName
                };
                var result = Library.getHttpRequest("/api/part/" + p.element_id.ToString());
                if (result == "error")
                {
                    
                    Library.postHttpRequest("/api/part", JsonConvert.SerializeObject(p));
                    total += p.part_name + " Added\r\n";
                    textBox1.Text = total;
                    textBox1.Update();
                }
            }
            textBox1.Text = total + "Done";

        }

        void getParts()
        {
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            parts = new ModelItemCollection();
            foreach (ModelItem item in doc.Models.RootItemDescendants)
            {
                textBox1.Update();
                if (item.Children.Count() == 0 && item.Parent != null && !parts.Contains(item))
                {
                    parts.Add(item.Parent);
                }
            }
            button2.Text = "Upload " + parts.Count.ToString() + " Parts to Database";

        }

        Building[] getBuildings()
        {
            var response = Library.getHttpRequest("/api/building");
            if (response != "error")
            {
                return JsonConvert.DeserializeObject<Building[]>(response);
            }
            return new Building[1];
        }

    }
}
