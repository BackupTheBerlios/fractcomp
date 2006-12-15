using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FractalCompression.Tools;
using FractalCompression.Structure;

namespace FractalCompression
{
    public partial class Form1 : Form
    {
        private Bitmap bitmap;

        public Form1()
        {
            InitializeComponent();
         //   myTestFun();
        }


        private void myTestFun()
        {
            Mapper map = new Mapper(0.9, new Point(10, 10), new Point(5, 5),
                4, 16, 10, 11, 12, 13, 14, 15, 16, 17);
            MappedPoint mp = map.MapPoint(11, 10, 20);
            Bitmap bitmap = (Bitmap)Bitmap.FromFile(@"C:\Documents and Settings\Marcin\Pulpit\lena.jpg");
            int sdelta = 4;
            int bdelta = 16;
            Domain d = new Domain(new Point(0,0),new Point(0,bdelta),
                new Point(bdelta,bdelta),new Point(bdelta,0), bdelta / sdelta);
            Structure.Region r = new Structure.Region(new Point(0,0),new Point(0,sdelta),
                new Point(sdelta,sdelta),new Point(sdelta,0));
            double s = MNTools.ComputeContractivityFactor(d, r, bitmap);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image temp = Bitmap.FromFile(openFileDialog1.FileName);
                int big = Properties.Settings.Default.bigDelta;
                int modulo   = temp.Size.Width % big;
                int times = temp.Size.Width / big;
                int xNewSize = (times + ((modulo >= big / 2) ? 1 : 0)) * big;
                modulo = temp.Size.Height % big;
                times = temp.Size.Height / big;
                int yNewSize = (times + ((modulo >= big / 2) ? 1 : 0)) * big;
                this.bitmap = new Bitmap(temp, xNewSize, yNewSize);
                this.originallPictureBox.Image = bitmap;
                this.compresedPictureBox.Image = bitmap;
            }
        }

        private void settinsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}