﻿using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
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
                ToolStripMenuItem item = new ToolStripMenuItem {
                    Text = Res.Remember_for_comparison,
                    Image = Res.Remember
                };
                item.Click += this.OnRemember;
                menu.Items.Add(item);
            }
            else
            {
                ToolStripMenuItem item = new ToolStripMenuItem {
                    Text = String.Format(Res.Compare_to_X, System.IO.Path.GetFileName(mostRecentPath)),
                    Image = Res.Compare,
                    Tag = mostRecentPath
                };
                item.Click += this.OnCompare;
                menu.Items.Add(item);

                ToolStripMenuItem subitem = new ToolStripMenuItem {
                    Text = String.Format(Res.Remember_X, SelectedItemPaths.Single()),
                    Image = Res.Remember
                };
                subitem.Click += this.OnRemember;

                ToolStripMenuItem history = new ToolStripMenuItem() {
                    Text = Res.Comparison_history
                };
                history.DropDownItems.Add(subitem);

                menu.Items.Add(history);
            }

            return menu;
        }

        private void OnRemember(object sender, EventArgs eventArgs)
        {
            var path = SelectedItemPaths.Single();
            m_registry.Value.InsertMruItem(MRU_KEY, path);
        }

        private void OnCompare(object sender, EventArgs eventArgs)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            var left = item.Tag.ToString();
            var right = SelectedItemPaths.Single();

            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var gateDiffExe = Path.Combine(assemblyDir, "GateDiff.exe");

            ProcessStartInfo command = new ProcessStartInfo(gateDiffExe, String.Join(" ", left, right));
            Process.Start(command);
        }
    }
}
