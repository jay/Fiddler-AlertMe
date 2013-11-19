/*
Copyright (C) 2013 Jay Satiro <raysatiro@yahoo.com>
All rights reserved.

-
Copyright (C) 2013 Telerik
Some of the project code came from Eric Lawrence's HTTPSNotary sample.
https://www.fiddler2.com/dl/HTTPSNotarySample.zip
-

This file is part of the AlertMe extension for Fiddler.
https://github.com/jay/Fiddler-AlertMe

AlertMe is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

AlertMe is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with AlertMe.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Fiddler;

namespace AlertMe
{
    public class AlertMe : IFiddlerExtension
    {
        #region Preferences
        /// <summary>
        /// True if the AlertMe extension is enabled
        /// </summary>
        bool bEnabled = true;

        /// <summary>
        /// True if requests should be monitored
        /// </summary>
        bool bMonitorRequests = false;

        /// <summary>
        /// True if responses should be monitored
        /// </summary>
        bool bMonitorResponses = false;

        /// <summary>
        /// True if Fiddler's event log should be monitored
        /// </summary>
        bool bMonitorEventLog = true;

        /// <summary>
        /// True if a sound should be played on alert
        /// </summary>
        bool bAlertBySound = true;

        /// <summary>
        /// True if an email should be sent on alert
        /// </summary>
        bool bAlertByEmail = false;

        /// <summary>
        /// True if a message box should be shown on alert
        /// </summary>
        bool bAlertByMsgBox = false;

        /// <summary>
        /// Absolute path of a sound to play on alert
        /// </summary>
        string sSound = "C:\\Windows\\chord.wav";

        /// <summary>
        /// E-Mail address to alert
        /// </summary>
        string sEmail;

        /// <summary>
        /// List of hosts to watch
        /// </summary>
        HostList hlHostsToWatch;

        /// <summary>
        /// Regular expression to match
        /// </summary>
        string sRegex = "*";
        #endregion

        #region UI fields
        // The MenuItems hold the UI entrypoints for the extension.
        private MenuItem mnuAlertMe;
        private MenuItem miEnabled;
        private MenuItem miSplit1;
        private MenuItem miMonitorRequests;
        private MenuItem miMonitorResponses;
        private MenuItem miMonitorEventLog;
        private MenuItem miSplit2;
        private MenuItem miAlertBySound;
        private MenuItem miAlertByEmail;
        private MenuItem miAlertByMsgBox;
        private MenuItem miSplit3;
        private MenuItem miEditSound;
        private MenuItem miEditEmail;
        private MenuItem miEditHostsToWatch;
        private MenuItem miEditRegex;
        private MenuItem miSplit4;
        private MenuItem miRuleSummary;
        private MenuItem miSplit5;
        private MenuItem miAbout;
        #endregion

        #region UI_HANDLERS
        private void InitializeMenu()
        {
            mnuAlertMe = new MenuItem("&AlertMe");

            miEnabled = new MenuItem("&Enabled");
            miEnabled.Checked = bEnabled;
            miEnabled.Click += new System.EventHandler(miEnabled_Click);

            miSplit1 = new MenuItem("-");

            miMonitorRequests = new MenuItem("Monitor requests");
            miMonitorRequests.Checked = bMonitorRequests;
            miMonitorRequests.Click += new System.EventHandler(miMonitorRequests_Click);

            miMonitorResponses = new MenuItem("Monitor replies");
            miMonitorResponses.Checked = bMonitorResponses;
            miMonitorResponses.Click += new System.EventHandler(miMonitorResponses_Click);

            miMonitorEventLog = new MenuItem("Monitor event log");
            miMonitorEventLog.Checked = bMonitorEventLog;
            miMonitorEventLog.Click += new System.EventHandler(miMonitorEventLog_Click);

            miSplit2 = new MenuItem("-");

            miAlertBySound = new MenuItem("Alert by sound");
            miAlertBySound.Checked = bAlertBySound;
            miAlertBySound.Click += new System.EventHandler(miAlertBySound_Click);

            miAlertByEmail = new MenuItem("Alert by e-mail");
            miAlertByEmail.Checked = bAlertByEmail;
            miAlertByEmail.Click += new System.EventHandler(miAlertByEmail_Click);

            miAlertByMsgBox = new MenuItem("Alert by message box");
            miAlertByMsgBox.Checked = bAlertByMsgBox;
            miAlertByMsgBox.Click += new System.EventHandler(miAlertByMsgBox_Click);

            miSplit3 = new MenuItem("-");

            miEditSound = new MenuItem("Edit sound");
            miEditSound.Click += new System.EventHandler(miEditSound_Click);

            miEditEmail = new MenuItem("Edit e-mail");
            miEditEmail.Click += new System.EventHandler(miEditEmail_Click);

            miEditHostsToWatch = new MenuItem("Edit &hosts to watch");
            miEditHostsToWatch.Click += new System.EventHandler(miEditHostsToWatch_Click);

            miEditRegex = new MenuItem("Edit regular e&xpression");
            miEditRegex.Click += new System.EventHandler(miEditRegex_Click);

            miSplit4 = new MenuItem("-");

            miRuleSummary = new MenuItem("&Show current preferences");
            miRuleSummary.Click += new System.EventHandler(miRuleSummary_Click);

            miSplit5 = new MenuItem("-");

            miAbout = new MenuItem("&About...");
            miAbout.Click += new System.EventHandler(miAbout_Click);

            mnuAlertMe.MenuItems.AddRange(new MenuItem[] {
                miEnabled,
                miSplit1,
                miMonitorRequests,
                miMonitorResponses,
                miMonitorEventLog,
                miSplit2,
                miAlertBySound,
                miAlertByEmail,
                miAlertByMsgBox,
                miSplit3,
                miEditSound,
                miEditEmail,
                miEditHostsToWatch,
                miEditRegex,
                miSplit4,
                miRuleSummary,
                miSplit5,
                miAbout
            });
        }

        void miEnabled_Click(object sender, EventArgs e)
        {
            miEnabled.Checked = !miEnabled.Checked;
            bEnabled = miEnabled.Checked;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.extensions.AlertMe.Enabled", bEnabled);
        }

        // Split1

        void miMonitorRequests_Click(object sender, EventArgs e)
        {
            miMonitorRequests.Checked = !miMonitorRequests.Checked;
            bMonitorRequests = miMonitorRequests.Checked;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.extensions.AlertMe.MonitorRequests", bMonitorRequests);
        }

        void miMonitorResponses_Click(object sender, EventArgs e)
        {
            miMonitorResponses.Checked = !miMonitorResponses.Checked;
            bMonitorResponses = miMonitorResponses.Checked;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.extensions.AlertMe.MonitorResponses", bMonitorResponses);
        }

        void miMonitorEventLog_Click(object sender, EventArgs e)
        {
            miMonitorEventLog.Checked = !miMonitorEventLog.Checked;
            bMonitorEventLog = miMonitorEventLog.Checked;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.extensions.AlertMe.MonitorEventLog", bMonitorEventLog);
        }

        // Split2

        void miAlertBySound_Click(object sender, EventArgs e)
        {
            miAlertBySound.Checked = !miAlertBySound.Checked;
            bAlertBySound = miAlertBySound.Checked;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.extensions.AlertMe.AlertBySound", bAlertBySound);
        }

        void miAlertByEmail_Click(object sender, EventArgs e)
        {
            miAlertByEmail.Checked = !miAlertByEmail.Checked;
            bAlertByEmail = miAlertByEmail.Checked;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.extensions.AlertMe.AlertByEmail", bAlertByEmail);
        }

        void miAlertByMsgBox_Click(object sender, EventArgs e)
        {
            miAlertByMsgBox.Checked = !miAlertByMsgBox.Checked;
            bAlertByMsgBox = miAlertByMsgBox.Checked;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.extensions.AlertMe.AlertByMsgBox", bAlertByMsgBox);
        }

        // Split3

        void miEditSound_Click(object sender, EventArgs e)
        {
            string sNewSound = frmPrompt.GetUserString(
                "Edit sound",
                "Enter the absolute path of a sound to play on alert.",
                sSound,
                true
            );

            if (null == sNewSound)
            {
                return;
            }

            sSound = sNewSound;
            FiddlerApplication.Prefs.SetStringPref("fiddler.extensions.AlertMe.Sound", sSound);
        }

        void miEditEmail_Click(object sender, EventArgs e)
        {
            string sNewEmail = frmPrompt.GetUserString(
                "Edit e-mail",
                "Enter the email address to alert.",
                sEmail,
                true
            );

            if (null == sNewEmail)
            {
                return;
            }

            sEmail = sNewEmail;
            FiddlerApplication.Prefs.SetStringPref("fiddler.extensions.AlertMe.Email", sEmail);
        }

        void miEditHostsToWatch_Click(object sender, EventArgs e)
        {
            string sNewHostsToWatch = frmPrompt.GetUserString(
                "Edit hosts to watch",
                "Enter the list of hosts to watch. Use a semi-colon to separate hosts.",
                hlHostsToWatch.ToString(),
                true
            );

            if (null == sNewHostsToWatch)
            {
                return;
            }

            hlHostsToWatch = new HostList(sNewHostsToWatch);
            FiddlerApplication.Prefs.SetStringPref("fiddler.extensions.AlertMe.HostsToWatch", hlHostsToWatch.ToString());
        }

        void miEditRegex_Click(object sender, EventArgs e)
        {
            string sNewRegex = frmPrompt.GetUserString(
                "Edit Regular Expression",
                "Enter the regular expression to match.",
                sRegex,
                true
            );

            if (null == sNewRegex)
            {
                return;
            }

            sRegex = sNewRegex;
            FiddlerApplication.Prefs.SetStringPref("fiddler.extensions.AlertMe.Regex", sRegex);
        }

        // Split4

        void miRuleSummary_Click(object sender, EventArgs e)
        {
            string sPrefs = String.Format("Enabled: {0}", (bEnabled ? "YES" : "NO"));

            sPrefs += "\nMonitor: ";
            sPrefs += (bMonitorRequests ? "requests, " : "");
            sPrefs += (bMonitorResponses ? "responses, " : "");
            sPrefs += (bMonitorEventLog ? "eventlog, " : "");
            if (sPrefs.EndsWith(", "))
            {
                sPrefs = sPrefs.Remove(sPrefs.Length - 2);
            }

            sPrefs += "\nAlert by: ";
            sPrefs += (bAlertBySound ? "sound, " : "");
            sPrefs += (bAlertByEmail ? "email, " : "");
            sPrefs += (bAlertByMsgBox ? "msgbox, " : "");
            if (sPrefs.EndsWith(", "))
            {
                sPrefs = sPrefs.Remove(sPrefs.Length - 2);
            }

            sPrefs += "\nSound: " + sSound;
            sPrefs += "\nEmail: " + sEmail;
            sPrefs += "\nHosts to watch (semi-colon delimited): " + hlHostsToWatch.ToString();
            sPrefs += "\nRegular expression: " + sRegex;

            MessageBox.Show(sPrefs, "Rule summary");
        }

        // Split5

        void miAbout_Click(object sender, EventArgs e)
        {
            string sLicenseNotice = "GPLv3 licensed.\nhttp://www.gnu.org/licenses/gpl-3.0.html";
            string sProjectNotice = "If you like it, help improve it.\nhttps://github.com/jay/Fiddler-AlertMe";
            string sAbout = String.Format(
                "{0}/{1} {2}\n\n{3}\n\n{4}\n\n{5}",
                Assembly.GetExecutingAssembly().GetName().Name,
                Assembly.GetExecutingAssembly().GetName().Version,
                FiddlerApplication.GetVersionString(),
                ((AssemblyDescriptionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(
                    typeof(AssemblyDescriptionAttribute), false)[0]).Description,
                sLicenseNotice,
                sProjectNotice
            );

            MessageBox.Show(sAbout, "About AlertMe");
        }
        #endregion UI_HANDLERS


        public AlertMe()
        {
            bEnabled = FiddlerApplication.Prefs.GetBoolPref("fiddler.extensions.AlertMe.Enabled", bEnabled);
            bMonitorRequests = FiddlerApplication.Prefs.GetBoolPref("fiddler.extensions.AlertMe.MonitorRequests", bMonitorRequests);
            bMonitorResponses = FiddlerApplication.Prefs.GetBoolPref("fiddler.extensions.AlertMe.MonitorResponses", bMonitorResponses);
            bMonitorEventLog = FiddlerApplication.Prefs.GetBoolPref("fiddler.extensions.AlertMe.MonitorEventLog", bMonitorEventLog);
            bAlertBySound = FiddlerApplication.Prefs.GetBoolPref("fiddler.extensions.AlertMe.AlertBySound", bAlertBySound);
            bAlertByEmail = FiddlerApplication.Prefs.GetBoolPref("fiddler.extensions.AlertMe.AlertByEmail", bAlertByEmail);
            bAlertByMsgBox = FiddlerApplication.Prefs.GetBoolPref("fiddler.extensions.AlertMe.AlertByMsgBox", bAlertByMsgBox);
            sSound = FiddlerApplication.Prefs.GetStringPref("fiddler.extensions.AlertMe.Sound", sSound);
            sEmail = FiddlerApplication.Prefs.GetStringPref("fiddler.extensions.AlertMe.Email", sEmail);
            hlHostsToWatch = new HostList(FiddlerApplication.Prefs.GetStringPref("fiddler.extensions.AlertMe.HostsToWatch", "*"));
            sRegex = FiddlerApplication.Prefs.GetStringPref("fiddler.extensions.AlertMe.Regex", "*");

            // Create the UI
            InitializeMenu();
        }

        public void OnBeforeUnload()
        {
            // TODO: Technically, we should clean up our menu items here. However, since Fiddler Add-ons
            //       don't currently unload until the application is exiting, this isn't strictly required.
        }

        // Note: This event callback does not fire until Fiddler's UI is fully loaded. One or more
        // requests may have already been processed.
        public void OnLoad()
        {
            // Add our menu item to the main menu
            FiddlerApplication.UI.mnuMain.MenuItems.Add(mnuAlertMe);

            // Add Event handler
            FiddlerApplication.Log.OnLogString += FiddlerApplication_Log_OnLogString;
        }

        private void FiddlerApplication_Log_OnLogString(object sender, LogEventArgs oLEA)
        {
            if (!bEnabled)
            {
                return;
            }
            //rem this is blocking..
            MessageBox.Show(oLEA.LogString, "Fiddler: " + sender.GetType().ToString());
        }

    }
}
