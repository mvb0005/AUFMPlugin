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
        public AUFMForm()
        {
            PluginRecord pluginRecord = Autodesk.Navisworks.Api.Application.Plugins.FindPlugin("AUFMPlugin.AUFM");
            dockPanePlugin = (AUFMDockPane)pluginRecord.LoadedPlugin;
            InitializeComponent();
            comboBox1.Text = Settings.Default["Building"].ToString();
            button1.Click += new EventHandler(this.SelectBuilding);
            populateComboBox();
        }

        void SelectBuilding(object sender, EventArgs e)
        {
            Settings.Default["Building"] = comboBox1.Text;
            Settings.Default.Save();
            dockPanePlugin.updateBuilding(comboBox1.Text);
            label1.Text = Library.getHttpRequest("building/" + comboBox1.Text);
            if (comboBox1.Text.Length > 0 && Library.getHttpRequest("building/" + comboBox1.Text) == "error")
            {
                AddBuildingToApi(comboBox1.Text);
            }
            populateComboBox();
        }

        void populateComboBox()
        {
            String url = "building";
            String response = Library.getHttpRequest(url);
            comboBox1.Items.Clear();
            if (response != null)
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
            
            var result = Library.postHttpRequest("building", new JObject(new JProperty("name", buildingName)).ToString());
            label1.Text = result;
            return result;
        }

        void AddPartsToBuildinginApi()
        {
            Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            Search s = new Search();
            s.Selection.SelectAll();
            s.SearchConditions.Add(SearchCondition.HasPropertyByDisplayName("Element ID", "Value"));
            s.SearchConditions.Add(SearchCondition.HasPropertyByDisplayName("Item", "GUID"));
            ModelItemCollection modelItemCollection = s.FindAll(doc, false);
            foreach (ModelItem modelItem in modelItemCollection)
            {
                Part p = new Part
                {
                    building_id = 1,
                    element_id = Int32.Parse(modelItem.PropertyCategories.FindPropertyByDisplayName("Element ID", "Value").Value.ToDisplayString())
                };
                label1.Text += "\n" + JsonConvert.SerializeObject(p);
            }
        }
    }
}
