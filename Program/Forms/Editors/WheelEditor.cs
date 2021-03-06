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
    class WheelEditor<CreationT> : IWheelEditor where CreationT : class, IWheel, new()
    {
        public CreationT AddWheel(IWheelElements elements)
        {
            CreationT wheel = new CreationT();
            bool accepted =  EditWheel(wheel, elements);
            return accepted ? wheel : null;
        }

        public bool EditWheel(IWheel wheel, IWheelElements elements)
        {
            WheelEditorForm form = new WheelEditorForm(wheel, elements);
            if (form.ShowDialog() == DialogResult.OK)
            {
                wheel.Label = form.Label;
                wheel.AccentColor = form.AccentColor;
                wheel.BgColor = form.BgColor;

                if (form.IsStartup)
                {
                    elements.StartupWheel = wheel;
                }

                if (wheel.Buttons != null)
                {
                    foreach (IWheelButton button in form.Buttons)
                    {
                        if (!wheel.Buttons.Remove(button)) elements.Buttons.Add(button);
                    }
                    foreach (IWheelButton button in wheel.Buttons)
                    {
                        elements.Buttons.Remove(button);
                    } 
                }

                wheel.Buttons = form.Buttons;
                return true;
            }
            return false;
        }

        public bool RemoveWheel(IWheel wheel, IWheelElements elements)
        {
            Form dialog = new ComfirmationDialog($"Are you sure you wish to delete wheel '{wheel.Label}'?", "Comfirm deletion");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int index = elements.Wheels.IndexOf(wheel);
                elements.Wheels.Remove(wheel);
                if (wheel == elements.StartupWheel)
                {
                    if (elements.Wheels.Count == 0)
                    {
                        elements.StartupWheel = null;
                    }
                    else
                    {
                        index = Math.Min(index, elements.Wheels.Count - 1);
                        elements.StartupWheel = elements.Wheels[index]; ;
                    }
                }
                return true;
            }
            return false;
        }

        IWheel IWheelEditor.AddWheel(IWheelElements elements) => AddWheel(elements);
    }
}
