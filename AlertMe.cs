/*
Copyright (C) 2013 Jay Satiro <raysatiro@yahoo.com>
All rights reserved.

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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Fiddler;


namespace AlertMe
{
    public class AlertMe : IAutoTamper
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
        /// Sound to play on alert
        /// </summary>
        System.Media.SoundPlayer soundPlayer = new System.Media.SoundPlayer(@"C:\Windows\media\chord.wav");

        /// <summary>
        /// E-Mail information
        /// </summary>
        EmailInfo emailInfo = new EmailInfo("");

        /// <summary>
        /// List of hosts to watch
        /// </summary>
        HostList hlHostsToWatch = new HostList("*");

        /// <summary>
        /// Regular expression to match
        /// </summary>
        Regex regexPattern = new Regex(".*", RegexOptions.Compiled);

        public void ShowPreferences()
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

            sPrefs += "\nSound: " + soundPlayer.SoundLocation + (soundPlayer.IsLoadCompleted ? "" : " (LOAD FAILED)");
            sPrefs += "\nEmail info: " + emailInfo.ToString();
            sPrefs += "\nHosts to watch (semi-colon delimited): " + hlHostsToWatch.ToString();
            sPrefs += "\nRegular expression: " + regexPattern.ToString();

            MessageBox.Show(sPrefs, "Fiddler-AlertMe Preferences");
        }
        #endregion Preferences

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
        private MenuItem miShowPreferences;
        private MenuItem miSplit5;
        private MenuItem miAbout;
        #endregion UI fields

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

            miMonitorResponses = new MenuItem("Monitor responses");
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

            miShowPreferences = new MenuItem("&Show current preferences");
            miShowPreferences.Click += new System.EventHandler(miShowPreferences_Click);

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
                miShowPreferences,
                miSplit5,
                miAbout
            });
        }

        void miEnabled_Click(object sender, EventArgs e)
        {
            miEnabled.Checked = !miEnabled.Checked;
            bEnabled = miEnabled.Checked;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.extensions.AlertMe.Enabled", bEnabled);

            if (bEnabled)
            {
                ShowPreferences(); //rem blocking..
            }
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
            if (emailInfo.IsEmpty() && !miAlertByEmail.Checked)
            {
                MessageBox.Show("Alert by e-mail cannot be enabled until you add your e-mail info.", "Fiddler-AlertMe");
                return;
            }

            miAlertByEmail.Checked = !miAlertByEmail.Checked;
            bAlertByEmail = miAlertByEmail.Checked;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.extensions.AlertMe.AlertByEmail", bAlertByEmail);

            if (bAlertByEmail)
            {
                MessageBox.Show(
                    "WARNING: Alert by e-mail is an experimental feature. There is no rate limiting. Exercise extreme caution.",
                    "Fiddler-AlertMe"
                );
            }
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
            string sOldSound = soundPlayer.SoundLocation;

            string sNewSound = frmPrompt.GetUserString(
                "Edit sound",
                "Enter the absolute path of a .wav file sound to play on alert.",
                sOldSound,
                true
            );

            if (null == sNewSound)
            {
                return;
            }

            try
            {
                soundPlayer.SoundLocation = sNewSound;
                soundPlayer.Load();
            }
            catch (Exception)
            {
                MessageBox.Show("The sound that you specified was not accepted. Try again.", "Fiddler-AlertMe [Error]");
                try
                {
                    soundPlayer.SoundLocation = sOldSound;
                    soundPlayer.Load();
                }
                catch (Exception)
                {
                }
                return;
            }

            FiddlerApplication.Prefs.SetStringPref("fiddler.extensions.AlertMe.Sound", sNewSound);
        }

        void miEditEmail_Click(object sender, EventArgs e)
        {
            string sOldInfo = emailInfo.ToString();

            string sNewInfo = frmPrompt.GetUserString(
                "Edit e-mail info",
                "Enter the following: smtp.server.com; port; from@email.com; to@email.com             A test e-mail will be sent.",
                sOldInfo,
                true
            );

            if (null == sNewInfo)
            {
                return;
            }

            if (!emailInfo.AssignFromString(sNewInfo) || !emailInfo.Send("Fiddler-AlertMe: Test e-mail", "This is a test."))
            {
                MessageBox.Show("The email info that you specified was not accepted. Try again.", "Fiddler-AlertMe [Error]");
                emailInfo.AssignFromString(sOldInfo);
                return;
            }

            MessageBox.Show("It appears the test e-mail was sent successfully. Check your inbox to confirm.", "Fiddler-AlertMe");
            FiddlerApplication.Prefs.SetStringPref("fiddler.extensions.AlertMe.Email", emailInfo.ToString());
        }

        void miEditHostsToWatch_Click(object sender, EventArgs e)
        {
            string sOldHosts = hlHostsToWatch.ToString();

            string sNewHosts = frmPrompt.GetUserString(
                "Edit hosts to watch",
                "Enter a semi-colon delimited list of hosts to watch for request/response. Use an asterisk as a wildcard.",
                sOldHosts,
                true
            );

            if (null == sNewHosts)
            {
                return;
            }

            if (!hlHostsToWatch.AssignFromString(sNewHosts))
            {
                MessageBox.Show("The list of hosts that you specified was not accepted. Try again.", "Fiddler-AlertMe [Error]");
                hlHostsToWatch.AssignFromString(sOldHosts);
                return;
            }

            FiddlerApplication.Prefs.SetStringPref("fiddler.extensions.AlertMe.HostsToWatch", hlHostsToWatch.ToString());
        }

        void miEditRegex_Click(object sender, EventArgs e)
        {
            string sOldPattern = regexPattern.ToString();

            string sNewPattern = frmPrompt.GetUserString(
                "Edit Regular Expression",
                "Enter a regular expression to match. Options: http://msdn.microsoft.com/en-us/library/yd1hzczs(v=vs.100).aspx",
                sOldPattern,
                true
            );

            if (null == sNewPattern)
            {
                return;
            }

            try
            {
                regexPattern = new Regex(sNewPattern, RegexOptions.Compiled);
            }
            catch (Exception)
            {
                MessageBox.Show("The regular expression that you specified was not accepted. Try again.", "Fiddler-AlertMe [Error]");
                try
                {
                    regexPattern = new Regex(sOldPattern, RegexOptions.Compiled);
                }
                catch (Exception)
                {
                }
                return;
            }

            FiddlerApplication.Prefs.SetStringPref("fiddler.extensions.AlertMe.Regex", sNewPattern);
        }

        // Split4

        void miShowPreferences_Click(object sender, EventArgs e)
        {
            ShowPreferences();
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

            string sSound_Default = soundPlayer.SoundLocation;
            string sSound = FiddlerApplication.Prefs.GetStringPref("fiddler.extensions.AlertMe.Sound", sSound_Default);
            try
            {
                soundPlayer.SoundLocation = sSound;
                soundPlayer.Load();
            }
            catch (Exception)
            {
                try
                {
                    soundPlayer.SoundLocation = sSound_Default;
                    soundPlayer.Load();
                }
                catch (Exception)
                {
                }
            }

            string sEmail_Default = emailInfo.ToString();
            string sEmail = FiddlerApplication.Prefs.GetStringPref("fiddler.extensions.AlertMe.Email", sEmail_Default);
            if (!emailInfo.AssignFromString(sEmail))
            {
                emailInfo.AssignFromString(sEmail_Default);
            }

            string sHostsToWatch_Default = hlHostsToWatch.ToString();
            string sHostsToWatch = FiddlerApplication.Prefs.GetStringPref(
                "fiddler.extensions.AlertMe.HostsToWatch", sHostsToWatch_Default
            );
            if (!hlHostsToWatch.AssignFromString(sHostsToWatch))
            {
                hlHostsToWatch.AssignFromString(sHostsToWatch_Default);
            }

            string sRegexPattern_Default = regexPattern.ToString();
            string sRegexPattern = FiddlerApplication.Prefs.GetStringPref("fiddler.extensions.AlertMe.Regex", sRegexPattern_Default);
            try
            {
                regexPattern = new Regex(sRegexPattern, RegexOptions.Compiled);
            }
            catch (Exception)
            {
                try
                {
                    regexPattern = new Regex(sRegexPattern_Default, RegexOptions.Compiled);
                }
                catch (Exception)
                {
                }
                return;
            }

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
            if (!bEnabled || !bMonitorEventLog)
            {
                return;
            }

            Match match = regexPattern.Match(oLEA.LogString);
            if (match.Success && !oLEA.LogString.StartsWith("Fiddler-AlertMe [Exception]"))
            {
                Alert(match, "Event Log match:  " + oLEA.LogString);
            }
        }

        public void AutoTamperRequestBefore(Session oSession)
        {
        }

        public void AutoTamperRequestAfter(Session oSession)
        {
            if (!bEnabled
                || !bMonitorRequests
                || (null == oSession.oRequest)
                || (null == oSession.oRequest.headers)
                || !hlHostsToWatch.ContainsHost(oSession.host)
            )
            {
                return;
            }

            try
            {
                Match match = regexPattern.Match(oSession.oRequest.headers.ToString());
                if (match.Success)
                {
                    Alert(match, "Session " + oSession.id + " request header match.  " + oSession.fullUrl);
                }
            }
            catch (Exception ex)
            {
                FiddlerApplication.Log.LogFormat(
                    "Fiddler-AlertMe [Exception]: While examining session {0} request headers: {1}", oSession.id, ex.ToString()
                );
            }

            if (null == oSession.requestBodyBytes)
            {
                return;
            }

            //bool bDecoded = oSession.utilDecodeRequest();

            try
            {
                Match match = regexPattern.Match(oSession.GetRequestBodyAsString());
                if (match.Success)
                {
                    Alert(match, "Session " + oSession.id + " request body match.  " + oSession.fullUrl);
                }
            }
            catch (Exception ex)
            {
                FiddlerApplication.Log.LogFormat(
                    "Fiddler-AlertMe [Exception]: While examining session {0} request body: {1}", oSession.id, ex.ToString()
                );
            }
        }

        public void AutoTamperResponseBefore(Session oSession)
        {
        }

        public void AutoTamperResponseAfter(Session oSession)
        {
            if (!bEnabled
                || !bMonitorResponses
                || (null == oSession.oResponse)
                || (null == oSession.oResponse.headers)
                || !hlHostsToWatch.ContainsHost(oSession.host)
            )
            {
                return;
            }

            try
            {
                Match match = regexPattern.Match(oSession.oResponse.headers.ToString());
                if (match.Success)
                {
                    Alert(match, "Session " + oSession.id + " response header match.  " + oSession.fullUrl);
                }
            }
            catch (Exception ex)
            {
                FiddlerApplication.Log.LogFormat(
                    "Fiddler-AlertMe [Exception]: While examining session {0} response headers: {1}", oSession.id, ex.ToString()
                );
            }

            if (null == oSession.responseBodyBytes)
            {
                return;
            }

            //bool bDecoded = oSession.utilDecodeResponse();

            try
            {
                Match match = regexPattern.Match(oSession.GetResponseBodyAsString());
                if (match.Success)
                {
                    Alert(match, "Session " + oSession.id + " response body match.  " + oSession.fullUrl);
                }
            }
            catch (Exception ex)
            {
                FiddlerApplication.Log.LogFormat(
                    "Fiddler-AlertMe [Exception]: While examining session {0} response body: {1}", oSession.id, ex.ToString()
                );
            }
        }

        public void OnBeforeReturningError(Session oSession)
        {
        }

        private void Alert(Match match, string sWhere)//, Session oSession)
        {
            string sSubject = "Fiddler-AlertMe: " + sWhere;

            string sBody = sSubject + "\n\n" + "UTC timestamp: "
                + System.DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture) + "\n\n"
                + "Your regex pattern is: " + regexPattern.ToString();

            for (int ctr = 1; ctr < match.Groups.Count; ++ctr)
            {
                sBody += "\n\n" + "Group " + ctr + " found at position " + match.Groups[ctr].Index + ": " + match.Groups[ctr].Value;
            }

            if (sSubject.Length > 78)
            {
                sSubject = sSubject.Remove(74) + " ...";
            }

            if (bAlertBySound && soundPlayer.IsLoadCompleted)
            {
                soundPlayer.Play();
            }

            if (bAlertByEmail)
            {
                emailInfo.Send(sSubject, sBody);
            }

            if (bAlertByMsgBox)
            {
                MessageBox.Show(sBody, sSubject);
            }
        }
    }

    #region EmailInfo
    public class EmailInfo
    {
        public string sServer;
        public int iPort;
        public string sFrom, sTo;

        public void Clear()
        {
            sServer = "";
            iPort = 0;
            sFrom = "";
            sTo = "";
        }

        public bool IsEmpty()
        {
            return ((String.Empty == sServer) && (0 == iPort) && (String.Empty == sFrom) && (String.Empty == sTo));
        }

        public EmailInfo(string s)
        {
            AssignFromString(s);
        }

        public bool AssignFromString(string s)
        {
            if (null == s)
            {
                return false;
            }

            s = s.Trim();

            if (String.Empty == s)
            {
                Clear();
                return true;
            }

            string[] sArray = s.Split(';');

            if ((sArray.Length == 4)
                || ((sArray.Length == 5) && (sArray[4].Trim().Length == 0))
            )
            {
                sServer = sArray[0].Trim();
                Int32.TryParse(sArray[1].Trim(), out iPort);
                sFrom = sArray[2].Trim();
                sTo = sArray[3].Trim();
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            if (IsEmpty())
            {
                return "";
            }
            else
            {
                return sServer + "; " + iPort + "; " + sFrom + "; " + sTo;
            }
        }

        public bool Send(string sSubject, string sBody)
        {
            if (IsEmpty())
            {
                return false;
            }

            try
            {
                new System.Net.Mail.SmtpClient(sServer, iPort).Send(sFrom, sTo, sSubject, sBody);
            }
            catch (Exception ex)
            {
                FiddlerApplication.Log.LogFormat("Fiddler-AlertMe [Exception]: While sending e-mail: {0}", ex.ToString());
                return false;
            }

            return true;
        }
    }
    #endregion EmailInfo
}
