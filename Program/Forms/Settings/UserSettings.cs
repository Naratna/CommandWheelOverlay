﻿using CommandWheelOverlay.Settings;
using CommandWheelOverlay.View.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandWheelForms.Settings
{
    class UserSettings : IUserSettings
    {
        public static UserSettings Instance { get => instance; }
        private static UserSettings instance = new UserSettings();

        private readonly Properties.Settings settings = Properties.Settings.Default;

        public ISettingsEditor Editor { get; set; }
        public IList<int> ShowHotkey { get => settings.ShowHotkey?.ToList(); set => settings.ShowHotkey = value?.ToArray(); }
        public IList<int> MoveLeftHotkey { get => settings.MoveLeftHotkey?.ToList(); set => settings.MoveLeftHotkey = value?.ToArray(); }
        public IList<int> MoveRightHotkey { get => settings.MoveRightHotkey?.ToList(); set => settings.MoveRightHotkey = value?.ToArray(); }
        public int Port { get => settings.Port; set => settings.Port = value; }
        public float Sensitivity { get => settings.Sensitivity; set => settings.Sensitivity = value; }

        private UserSettings()
        {
            if (settings.UpgradeRequired)
            {
                settings.Upgrade();
                settings.UpgradeRequired = false;
                settings.Save();
            }
        }

        public void Save()
            => settings.Save();

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
