using SharpShell.Attributes;
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
    [SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
    public abstract class GateContextMenu : SharpContextMenu
    {
        private Lazy<State> m_registry = new Lazy<State>(() => new State(8));

        protected abstract string MruKey { get; }

        protected abstract bool IsPathAcceptable(string path);

        protected override bool CanShowMenu()
        {
            try
            {
                List<string> paths = SelectedItemPaths.ToList();
                if (paths.Count <= 2)
                {
                    return paths.All(IsPathAcceptable);
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
            List<string> selectionSet = SelectedItemPaths.ToList();

            if (selectionSet.Count == 1)
            {
                return CreateSingleSelectionMenu(selectionSet[0]);
            }
            else if (selectionSet.Count == 2)
            {
                return CreatePairSelectionMenu(selectionSet[0], selectionSet[1]);
            }

            return null;
        }

        private ContextMenuStrip CreateSingleSelectionMenu(string currentPath)
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            IEnumerable<string> savedPaths = m_registry.Value.GetMruItems(MruKey).ToList();
            string mostRecentPath = savedPaths.FirstOrDefault();

            savedPaths = savedPaths.Where(path => path != currentPath);

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
                ToolStripMenuItem history = new ToolStripMenuItem()
                {
                    Text = Res.Comparison_history,
                    Image = Res.History
                };

                if (currentPath != mostRecentPath)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem {
                        Text = String.Format(Res.Compare_to_X, Path.GetFileName(mostRecentPath)),
                        Image = Res.Compare,
                        Tag = mostRecentPath
                    };
                    item.Click += this.OnCompare;
                    menu.Items.Add(item);

                    ToolStripMenuItem subitem = new ToolStripMenuItem
                    {
                        Text = String.Format(Res.Remember_X, currentPath),
                        Image = Res.Remember
                    };
                    subitem.Click += this.OnRemember;
                    history.DropDownItems.Add(subitem);
                    history.DropDownItems.Add(new ToolStripSeparator());
                }

                foreach (string path in savedPaths)
                {
                    ToolStripMenuItem historyItem = new ToolStripMenuItem {
                        Text = String.Format(Res.Compare_to_X, path),
                        Image = Res.Compare,
                        Tag = path
                    };
                    historyItem.Click += this.OnCompare;
                    history.DropDownItems.Add(historyItem);
                }

                menu.Items.Add(history);
            }

            return menu;
        }

        private ContextMenuStrip CreatePairSelectionMenu(string path1, string path2)
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            ToolStripMenuItem item = new ToolStripMenuItem {
                Text = String.Format(Res.Compare_X_to_Y, Path.GetFileName(path2), Path.GetFileName(path1)),
                Image = Res.Compare,
                Tag = path2
            };
            item.Click += this.OnCompare;
            menu.Items.Add(item);

            return menu;
        }

        private void OnRemember(object sender, EventArgs eventArgs)
        {
            var path = SelectedItemPaths.Single();
            m_registry.Value.InsertMruItem(MruKey, path);
        }

        private void OnCompare(object sender, EventArgs eventArgs)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            var left = ArgumentEscape(item.Tag.ToString());
            var right = ArgumentEscape(SelectedItemPaths.First());

            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var gateDiffExe = Path.Combine(assemblyDir, "GateDiff.exe");

            ProcessStartInfo command = new ProcessStartInfo(gateDiffExe, String.Join(@" ", left, right, "--swapping"));
            Process.Start(command);
        }

        private static string ArgumentEscape(string arg)
        {
            if (arg.Contains(@" "))
                return @"""" + arg + @"""";
            return arg;
        }

    }

    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class GateFileContextMenu : GateContextMenu
    {
        protected override string MruKey { get { return "files"; } }

        protected override bool IsPathAcceptable(string path)
        {
            return File.Exists(path);
        }
    }

    [ComVisible(true)]
    [COMServerAssociation(AssociationType.Directory)]
    public class GateDirecoryContextMenu : GateContextMenu
    {
        protected override string MruKey { get { return "dirs"; } }

        protected override bool IsPathAcceptable(string path)
        {
            return Directory.Exists(path);
        }
    }

}
