using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GateShell
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    [SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
    public class GateContextMenu : SharpContextMenu
    {
        private static readonly string MRU_KEY = "mru";
        private Lazy<State> m_registry = new Lazy<State>(() => new State(8));

        protected override bool CanShowMenu()
        {
            try
            {
                List<string> paths = SelectedItemPaths.ToList();
                if (paths.Count == 1)
                {
                    return File.Exists(paths[0]);
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected override ContextMenuStrip CreateMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            var savedPaths = m_registry.Value.GetMruItems(MRU_KEY).ToList();
            String mostRecentPath = savedPaths.FirstOrDefault();

            if (mostRecentPath == null)
            {
                string menuLabel = "Remember";
                ToolStripMenuItem item = new ToolStripMenuItem { Text = menuLabel };
                item.Click += this.OnRemember;
                menu.Items.Add(item);
            }
            else
            {
                string menuLabel = String.Format("Compare to {0}", System.IO.Path.GetFileName(mostRecentPath));
                ToolStripMenuItem item = new ToolStripMenuItem { Text = menuLabel };
                item.Click += this.OnCompare;
                menu.Items.Add(item);
            }

            return menu;
        }

        private void OnRemember(object sender, EventArgs eventArgs)
        {
            var path = this.SelectedItemPaths.Single();
            m_registry.Value.InsertMruItem(MRU_KEY, path);
        }

        private void OnCompare(object sender, EventArgs eventArgs)
        {
            MessageBox.Show("Not implemented", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
