﻿using CommandWheelForms.Forms;
using CommandWheelOverlay.Controller;
using CommandWheelOverlay.View.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommandWheelForms.Editors
{
    public class ElementsEditor : IElementsEditor
    {
        public IWheelEditor WheelEditor { get; set; }
        public IButtonEditor ButtonEditor { get; set; }
        public IList<IActionEditor> ActionEditors { get; set; }

        public IWheelElements Edit(IWheelElements wheelElementsCopy)
        {
            ElementsEditorForm editor = new ElementsEditorForm(wheelElementsCopy);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                var buttons = new List<IWheelButton>();
                foreach (IWheel wheel in wheelElementsCopy.Wheels)
                {
                    buttons.AddRange(wheel.Buttons);
                }
                wheelElementsCopy.Buttons = buttons;
                return wheelElementsCopy;
            }
            return null;
        }
    }
}
