﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace V2RayW
{
    public partial class MainForm : Form
    {

        public ConfigForm configForm;
        FormCoreOutput outputForm;

        public MainForm()
        {
            InitializeComponent();
        }

        public void updateMenu()
        {
            statusToolStripMenuItem.Text = Program.coreLoaded ? "V2Ray: loaded" : "V2Ray: unloaded";
            startStopToolStripMenuItem.Enabled = Program.profiles.Count > 0;
            startStopToolStripMenuItem.Text = Program.coreLoaded ? "Unload V2Ray" : "Load V2Ray";

            //v2RayRulesToolStripMenuItem.Checked = Program.proxyMode == Program.RULES_MODE;
            pacModeToolStripMenuItem.Checked = Program.proxyMode == Program.PAC_MODE;
            //globalModeToolStripMenuItem.Checked = Program.proxyMode == Program.GLOBAL_MODE;
            manualModeToolStripMenuItem.Checked = Program.proxyMode == Program.MANUAL_MODE;
            if (!Program.useCusProfile)
            {
                v2RayRulesToolStripMenuItem.Checked = Program.proxyMode == Program.RULES_MODE;
                globalModeToolStripMenuItem.Checked = Program.proxyMode == Program.GLOBAL_MODE;
                v2RayRulesToolStripMenuItem.Visible = true;
            } else
            {
                v2RayRulesToolStripMenuItem.Visible = false;
                globalModeToolStripMenuItem.Checked = Program.proxyMode == Program.GLOBAL_MODE || Program.proxyMode == Program.RULES_MODE;
            }


            serversToolStripMenuItem.DropDownItems.Clear();
            if (Program.profiles.Count == 0 && Program.cusProfiles.Count == 0)
            {
                ToolStripMenuItem noServer = new ToolStripMenuItem("no available servers.");
                serversToolStripMenuItem.DropDownItems.Add(noServer);
                return;
            }
            var serverMenuItems = Program.profiles.Select(p => new ToolStripMenuItem(p.remark == "" ? p.address : p.remark, null, switchToServer)).ToArray();
            var cusServerMenuItems = Program.cusProfiles.Select(p => new ToolStripMenuItem(p.Split('\\').Last(), null, switchToCusServer)).ToArray();
            if(Program.useCusProfile)
            {
                cusServerMenuItems[Program.selectedCusServerIndex].Checked = true;
            } else
            {
                serverMenuItems[Program.selectedServerIndex].Checked = true;
            }
            foreach(var p in serverMenuItems)
            {
                serversToolStripMenuItem.DropDownItems.Add(p);
            }
            if(serverMenuItems.Count() > 1)
            {
                serversToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("use all", null, switchToServer));
            }
            if (serverMenuItems.Count() > 0 && cusServerMenuItems.Count() > 0)
            {
                serversToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            }
            if(cusServerMenuItems.Count() > 0)
            {
                serversToolStripMenuItem.DropDownItems.AddRange(cusServerMenuItems);
            }
        }

        private void switchToServer(object sender, EventArgs e)
        {
            Program.selectedServerIndex = serversToolStripMenuItem.DropDownItems.IndexOf((ToolStripMenuItem)sender);
            if(Program.selectedServerIndex == Program.profiles.Count)
            {
                Program.useMultipleServer = true;
            } else
            {
                Program.useMultipleServer = false; 
            }
            Program.useCusProfile = false;
            Program.configurationDidChange();
        }

        private void switchToCusServer(object sender, EventArgs e)
        {
            Program.selectedCusServerIndex = Program.cusProfiles.Count - serversToolStripMenuItem.DropDownItems.Count + serversToolStripMenuItem.DropDownItems.IndexOf((ToolStripMenuItem)sender);
            Program.useCusProfile = true;
            Program.useMultipleServer = false;
            Program.configurationDidChange();
        }


        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.configForm == null || this.configForm.IsDisposed)
            {
                configForm = new ConfigForm();
                configForm.Show();
            }
            configForm.Focus();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://v2ray.com");
        }

        private void notifyIconMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            configureToolStripMenuItem_Click(sender, e);
        }

        private void startStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(!Program.coreLoaded && Program.proxyMode != 3)
            {
                Program.backUpProxy();
            }
            if(Program.coreLoaded && Program.proxyMode != 3)
            {
                Program.restoreProxy();
            }
            Program.coreLoaded = !Program.coreLoaded;
            Program.configurationDidChange();
        }

        private void v2RayRulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.coreLoaded && Program.proxyMode == 3)
            {
                Program.backUpProxy();
            }
            Program.proxyMode = Program.RULES_MODE;
            Program.configurationDidChange();
        }

        private void pacModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.coreLoaded && Program.proxyMode == 3)
            {
                Program.backUpProxy();
            }
            Program.proxyMode = Program.PAC_MODE;
            Program.configurationDidChange();
        }

        private void globalModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.coreLoaded && Program.proxyMode == 3)
            {
                Program.backUpProxy();
            }
            Program.proxyMode = Program.GLOBAL_MODE;
            Program.configurationDidChange();
        }

        private void manualModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Program.coreLoaded && Program.proxyMode != 3)
            {
                Program.restoreProxy();
            }
            Program.proxyMode = Program.MANUAL_MODE;
            Program.configurationDidChange();
        }

        internal void viewLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.outputForm == null || this.outputForm.IsDisposed)
            {
                outputForm = new FormCoreOutput();
                outputForm.Show();
            }
            outputForm.Focus();
        }
    }
}
