using AUFMPlugin.Properties;
using Autodesk.Navisworks.Api.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AUFMPlugin
{
    
    [Plugin(name: "AUFMPlugin", developerId: "AUFM", DisplayName = "AUFM Safety Pane")]
    [DockPanePlugin(200, 400, AutoScroll = true, MinimumHeight = 100, MinimumWidth = 200)]
    [AddInPlugin(AddInLocation.AddIn)]
    public class AUFMDockPane : DockPanePlugin
    {
        ControlPane AUFMControlPane;
        public override Control CreateControlPane()
        {
            AUFMControlPane = new ControlPane();
            return AUFMControlPane;
        }

        public void updateBuilding(String buildingName)
        {
            AUFMControlPane.updateBuilding(buildingName);
        }

        public override void DestroyControlPane(Control pane)
        {
            base.DestroyControlPane(pane);
        }
        
        public void toggleVisibilty()
        {
            Visible = !Visible;
        }
    }

    [Plugin(name: "AUFMAddinPane", developerId: "AUFM", ToolTip = "AUFM Safety Pane",  DisplayName = "AUFM Safety Pane")]
    [AddInPlugin(AddInLocation.AddIn)]
    public class AUFMAddinPane : AddInPlugin
    {
        public override int Execute(params string[] parameters)
        {
            PluginRecord pluginRecord = Autodesk.Navisworks.Api.Application.Plugins.FindPlugin("AUFMPlugin.AUFM");
            AUFMDockPane dockPanePlugin = (AUFMDockPane)pluginRecord.LoadedPlugin;
            if (dockPanePlugin != null)
            {
                dockPanePlugin.toggleVisibilty();
            } else
            {
                dockPanePlugin = new AUFMDockPane();
            }
     
            return 0;
        }
    }

    [Plugin(name: "AUFMBuildingManagementPane", developerId: "AUFM", ToolTip = "AUFM Building Management Pane", DisplayName = "AUFM Building Management Pane")]
    [AddInPlugin(AddInLocation.AddIn)]
    public class AUFMBuildingManagementPane : AddInPlugin
    {
        private AUFMForm form;
        public override int Execute(params string[] parameters)
        {
            if (Settings.Default.Cookie == "") {
                System.Windows.Forms.MessageBox.Show("Login Required");
                return 0;
            }
            if (form != null && !form.IsDisposed)
            {
                form.Visible = !form.Visible;
            }
            else
            {
                form = new AUFMForm();
                form.Visible = true;
            }
            return 0;
        }
    }

    [Plugin(name: "AUFMLoginPane", developerId: "AUFM", ToolTip = "AUFM Login Pane", DisplayName = "AUFM Login Pane")]
    [AddInPlugin(AddInLocation.AddIn)]
    public class AUFMLoginPane : AddInPlugin
    {
        private AUFMLogin form = new AUFMLogin();
        public override int Execute(params string[] parameters)
        {
            if (!form.IsDisposed)
            {
                form.Visible = !form.Visible;
            }
            else
            {
                form = new AUFMLogin();
                form.Visible = true;
            }
            return 0;
        }
    }
}
