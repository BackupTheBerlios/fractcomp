using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FractalCompression.Tools;

namespace FractalCompression
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            myTestFun();
        }


        private void myTestFun()
        {
            Mapper map = new Mapper(0.9, new Point(10, 10), new Point(5, 5),
                4, 16, 10, 11, 12, 13, 14, 15, 16, 17);
            MappedPoint mp = map.MapPoint(11, 10, 20);

        }
    }
}