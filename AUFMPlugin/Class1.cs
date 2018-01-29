using Autodesk.Navisworks.Api.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AUFMPlugin
{
    [Plugin("AUFMPlugin", "AUFM", DisplayName = "AUFM Safety Pane")]
    [DockPanePlugin(200, 400, AutoScroll = true, MinimumHeight = 100, MinimumWidth = 200)]
    public class AUFM : DockPanePlugin
    {
        public override Control CreateControlPane()
        {
            return new ControlPane();
        }

        public override void DestroyControlPane(System.Windows.Forms.Control pane)
        {
            base.DestroyControlPane(pane);
        }
    }
}
