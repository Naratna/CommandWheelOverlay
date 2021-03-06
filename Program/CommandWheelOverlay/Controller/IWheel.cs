﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using CommandWheelOverlay.View;

namespace CommandWheelOverlay.Controller
{
    public interface IWheel
    {
        IList<IWheelButton> Buttons { get; set; }
        Color AccentColor { get; set; }
        Color BgColor { get; set; }
        string Label { get; set; }
        IWheel Clone(IWheelElements elements);
    }
}
