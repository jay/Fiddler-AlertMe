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
        #region Internal-state fields
        /// <summary>
        /// True if the AlertMe extension is enabled
        /// </summary>
        bool bEnabled = true;
        #endregion

        #region UI fields
        // The MenuItems hold the UI entrypoints for the extension.
        private MenuItem mnuAlertMe;
        private MenuItem miEnabled;
        private MenuItem miSplit1;
        private MenuItem miRegex;
        private MenuItem miAbout;
        #endregion

        /// <summary>
        /// Regular expression to match
        /// </summary>
        string sRegex;

        public AlertMe()
        {
            bEnabled = FiddlerApplication.Prefs.GetBoolPref("fiddler.extensions.alertme.enabled", this.bEnabled);
            sRegex = FiddlerApplication.Prefs.GetStringPref("fiddler.extensions.alertme.regex", "*");

            // Create the UI
            InitializeMenu();
        }

        #region UI_HANDLERS
        private void InitializeMenu()
        {
            this.mnuAlertMe = new MenuItem("&AlertMe");
            this.miEnabled = new MenuItem("&Enabled");
            this.miEnabled.Checked = bEnabled;
            this.miEnabled.Click += new System.EventHandler(miEnabled_Click);
            this.miSplit1 = new MenuItem("-");
            this.miRegex = new MenuItem("Edit Regular E&xpression...");
            this.miRegex.Click += new EventHandler(miRegex_Click);
            this.miAbout = new MenuItem("&About...");
            this.miAbout.Click += new System.EventHandler(miAbout_Click);
            this.mnuAlertMe.MenuItems.AddRange(new MenuItem[] {
                this.miEnabled,
                this.miSplit1,
                this.miRegex,
                this.miAbout,
            });
        }

        void miEnabled_Click(object sender, System.EventArgs e)
        {
            this.miEnabled.Checked = !this.miEnabled.Checked;
            this.bEnabled = this.miEnabled.Checked;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.extensions.alertme.enabled", this.bEnabled);
        }

        void miRegex_Click(object sender, EventArgs e)
        {
            string sNewRegex = frmPrompt.GetUserString(
                "Edit Regular Expression",
                "Enter a regular expression. You will be alerted for every future event log entry that matches.",
                sRegex,
                true
            );

            if (null == sNewRegex)
            {
                return;
            }

            sRegex = sNewRegex;
        }

        void miAbout_Click(object sender, System.EventArgs e)
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

        public void OnBeforeUnload()
        {
            // Store the list of Hosts to ignore.
            FiddlerApplication.Prefs.SetStringPref("fiddler.extensions.alertme.regex", sRegex);

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
