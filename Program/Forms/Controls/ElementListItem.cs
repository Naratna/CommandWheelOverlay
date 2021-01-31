﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommandWheelForms.Controlls
{
    public partial class ElementListItem : UserControl
    {
        public object Element { get; set; }
        public ElementListItem()
        {
            InitializeComponent();
        }
        public ElementListItem(object element) : this()
        {
            Element = element;
        }
    }
}