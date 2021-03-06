﻿using CommandWheelOverlay.Controller;
using CommandWheelOverlay.View.Editors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommandWheelForms.Forms
{
    public partial class ButtonEditorForm : Form
    {
        private IWheelButton button;
        private IWheelElements elements;
        public IWheelAction Action { get; private set; }
        public string Label => nameTextBox.Text;
        public string ImgPath => imageTextBox.Text;
        private bool comboboxChangedByUser = true;
        private int lastIndex = -1;

        public ButtonEditorForm(IWheelButton button, IWheelElements elements)
        {
            InitializeComponent();
            this.button = button;
            this.elements = elements;
            actionComboBox.Items.Add("None");

            foreach (var item in elements.Editor.ActionEditors)
            {
                actionComboBox.Items.Add(item.DisplayName);
            }

            actionComboBox.SelectedIndex = 0;
            if (button.Action != null)
            {
                var types = elements.Editor.ActionEditors.Select(item => item.Type).ToList();
                int index = types.IndexOf(button.Action.GetType());
                if (index > -1)
                {
                    comboboxChangedByUser = false;
                    actionTriggerButton.Enabled =
                    editActionButton.Enabled = true;
                    actionComboBox.SelectedIndex = index + 1;
                }
            }

            Action = button.Action;
            nameTextBox.Text = button.Label;
            imageTextBox.Text = button.ImgPath;
        }

        private void imageBrowseButton_Click(object sender, EventArgs e)
        {
            if (imageOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                imageTextBox.Text = imageOpenFileDialog.FileName;
            }
        }

        private void actionComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (actionComboBox.SelectedIndex == lastIndex) return;
            lastIndex = actionComboBox.SelectedIndex;
            if (!comboboxChangedByUser)
            {
                comboboxChangedByUser = true;
            }
            else if (actionComboBox.SelectedIndex == 0)
            {
                Action = null;
                actionTriggerButton.Enabled =
                editActionButton.Enabled = false;
            }
            else
            {
                Action = elements.Editor.ActionEditors[actionComboBox.SelectedIndex - 1].CreateAction(elements);
                actionTriggerButton.Enabled =
                editActionButton.Enabled = true;
                if (Action is null) actionComboBox.SelectedIndex = 0;
            }
        }

        private void editActionButton_Click(object sender, EventArgs e)
        {
            if (actionComboBox.SelectedIndex == 0) return;
            elements.Editor.ActionEditors[actionComboBox.SelectedIndex - 1].EditAction(Action, elements);
        }

        private void ActionTriggerButton_Click(object sender, EventArgs e)
        {
            try
            {
                Action.Perform();
            }
            catch (Exception)
            {
            }
        }
    }
}
