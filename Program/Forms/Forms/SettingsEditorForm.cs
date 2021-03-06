﻿using CommandWheelForms.Input;
using CommandWheelOverlay.Input;
using CommandWheelOverlay.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommandWheelForms.Forms
{
    public partial class SettingsEditorForm : Form
    {
        private IInputHandler inputHandler;

        public IList<int> ShowHotkey { get; private set; }
        public IList<int> MoveLeftHotkey { get; private set; }
        public IList<int> MoveRightHotkey { get; private set; }
        public float Sensitivity => (float)(sensitivityNumericUpDown.Value / 100);

        private KeyNameConverter converter = new KeyNameConverter();

        TextBox[] textBoxes;
        Button[] buttons;

        private CancellationTokenSource tokenSource;

        public SettingsEditorForm(IUserSettings userSettings, IInputHandler inputHandler)
        {
            InitializeComponent();

            this.inputHandler = inputHandler;

            ShowHotkey = userSettings.ShowHotkey;
            MoveLeftHotkey = userSettings.MoveLeftHotkey;
            MoveRightHotkey = userSettings.MoveRightHotkey;
            sensitivityNumericUpDown.Value = (int)(userSettings.Sensitivity * 100);

            textBoxes = new[] { showHotkeyTextbox, leftHotkeyTextbox, rightHotkeyTextbox };
            buttons = new[] { ShowHotkeyButton, leftHotkeyButton, rightHotkeyButton };

            buttonLayout1.OkButton.Text = "Save";

            UpdateTextBoxes();
        }

        private void ShowHotkeyButton_Click(object sender, EventArgs e)
        {
            RecordHotkey
            (
                () => showHotkeyTextbox.Text,
                value => showHotkeyTextbox.Text = value,
                () => ShowHotkey,
                value => ShowHotkey = value
            );
        }

        private void LeftHotkeyButton_Click(object sender, EventArgs e)
        {
            RecordHotkey
            (
                () => leftHotkeyTextbox.Text,
                value => leftHotkeyTextbox.Text = value,
                () => MoveLeftHotkey,
                value => MoveLeftHotkey = value
            );
        }

        private void RightHotkeyButton_Click(object sender, EventArgs e)
        {
            RecordHotkey
            (
                () => rightHotkeyTextbox.Text,
                value => rightHotkeyTextbox.Text = value,
                () => MoveRightHotkey,
                value => MoveRightHotkey = value
            );
        }

        private void RecordHotkey(Func<string> getTextboxText, Action<string> setTextboxText, Func<IList<int>> getHotkey, Action<IList<int>> setHotkey)
        {
            tokenSource = new CancellationTokenSource();
            setTextboxText("");
            SetRecordButtonsEnabled(false);
            inputHandler.RecordHotkey(AddKey, SetHotkey, tokenSource.Token);


            void AddKey(int key)
            {
                string separator = "";
                string currentValue = getTextboxText();
                if (currentValue.Length > 0)
                {
                    separator += " + ";
                }
                setTextboxText(currentValue + separator + converter.GetName(key));
            }

            void SetHotkey(IList<int> newHotkey)
            {
                try { tokenSource.Dispose(); }
                catch { }
                if (newHotkey != null && newHotkey.Count > 0)
                {
                    setHotkey(newHotkey);
                }
                else
                {
                    string hotkeyText = string.Join(" + ", getHotkey().Select(key => converter.GetName(key)));
                    setTextboxText(hotkeyText);
                }
                SetRecordButtonsEnabled(true);
            }
        }

        private void SetRecordButtonsEnabled(bool enabled)
        {
            foreach (Button button in buttons)
            {
                button.Enabled = enabled;
            }
        }

        private void UpdateTextBoxes()
        {
            var hotkeys = new[] { ShowHotkey, MoveLeftHotkey, MoveRightHotkey };
            for (int i = 0; i < textBoxes.Length; i++)
            {
                if (hotkeys[i] is null) continue;
                textBoxes[i].Text = string.Join(" + ", hotkeys[i].Select(key => converter.GetName(key)));
            }
        }
    }
}
