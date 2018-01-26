using System.Net;
using System.Windows.Forms;
using System.Configuration;
using AUFM.Ctr;
using Autodesk.Navisworks.Api.Plugins;
using System;

namespace AUFM
{
    [PluginAttribute("AUFMPlugin", "AUFM", DisplayName = "HelloWorld")]
    [DockPanePlugin(200,400,AutoScroll = true, MinimumHeight = 100, MinimumWidth = 200)]
    public class AUFM : DockPanePlugin
    {
        public override Control CreateControlPane()
        {
                  

            return new UcUpdate();
        }

        public override void DestroyControlPane(Control pane)
        {
            try
            {
                var ctr = (UcUpdate)pane;
                ctr?.Dispose();
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}
