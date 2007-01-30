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
        Settings set = new Settings();

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

        /*private void ComputeInterpolationPointsTest0(Bitmap bitmap, int domainsInRow, int a)
        {
            Console.WriteLine("bmp size: {0}x{1}", bitmap.Width, bitmap.Height);
            List<MappedPoint> intrpList = POTools.ComputeInterpolationPoints(bitmap, domainsInRow, a);
            Console.WriteLine("interpolation points: " + intrpList.Count);
            Console.WriteLine();
            for (int i = 0; i < intrpList.Count; i++)
            {
                Console.Write("({0},{1}), ", intrpList[i].X, intrpList[i].Y);
                //Console.Write(intrpList[i].X + ", " + intrpList[i].Y + ";  ");
                if ((i + 1) % 4 == 0)
                    Console.WriteLine();
            }
        }*/

        //nie jest uzywana, zostala podzielona na dwie funckje, Compress i Decompress
        //
        private Bitmap CompressionTest(Bitmap bitmap, int bigDelta, int a)
        {
            Console.WriteLine("bmp size: {0}x{1}, bigDelta={2}, a={3}", bitmap.Width, bitmap.Height, bigDelta, a);
            FractalCompression.Structure.Region[,] regions = null;
            FractalCompression.Structure.Domain[,] domains = null;
            List<MappedPoint> intrpList = null;
            //List<MappedPoint> intrpList = POTools.PrepareRegions(bitmap, bigDelta, a, out regions);
            POTools.PrepareStructures(bitmap, bigDelta, a, out regions, out domains, out intrpList);

            //POTools.PrintInterpolationPoints(intrpList);
            //POTools.PrintDomains(domains);
            //POTools.PrintRegions(regions);

            string filepath = @"C:\Documents and Settings\Mroczek\Pulpit\myfile.nofc";
            Compressor compressor = new Compressor(bigDelta, a, 10, 3, domains, regions, intrpList, bitmap);
            Console.WriteLine("Compression started...");
            compressor.Compress();
            Console.WriteLine("Image compressed, saving to file...");
            compressor.SaveToFile(filepath);

            Console.WriteLine("Reading file before decompression...");
            CompResult cr = POTools.DeserializeCompResult(filepath);
            Console.WriteLine("Preparing to decompression...");
            List<double> cfactors = new List<double>(cr.Cqueue);
            List<MappedPoint> interpPoints = new List<MappedPoint>(cr.Iqueue);
            List<int> addresses = new List<int>(cr.Aqueue);
            Decompressor decompressor = new Decompressor(cfactors, interpPoints, addresses,
                cr.SmallDelta, cr.BigDelta, cr.A, cr.ImageWidth, cr.ImageHeight, cr.DMax);

            Console.WriteLine("Decompressing image...");
            return decompressor.DecompressImage();
        }

        private void Compress(Bitmap bitmap, int bigDelta, int a, String filepath)
        {
            //try{
            Console.WriteLine("bmp size: {0}x{1}, bigDelta={2}, a={3}", bitmap.Width, bitmap.Height, bigDelta, a);
            FractalCompression.Structure.Region[,] regions = null;
            FractalCompression.Structure.Domain[,] domains = null;
            List<MappedPoint> intrpList = null;
            POTools.PrepareStructures(bitmap, bigDelta, a, out regions, 
                out domains, out intrpList);
            Compressor compressor = new Compressor(bigDelta, a, 1000, 3, 
                domains, regions, intrpList, bitmap);
            Console.WriteLine("Compression started...");
            compressor.Compress();
            Console.WriteLine("Image compressed, saving to file...");
            compressor.SaveToFile(filepath);
        }

        private Bitmap Decompress(String filepath)
        {
            Console.WriteLine("Reading file before decompression...");
            CompResult cr = POTools.DeserializeCompResult(filepath);
            Console.WriteLine("Preparing to decompression...");
            List<double> cfactors = new List<double>(cr.Cqueue);
            List<MappedPoint> interpPoints = new List<MappedPoint>(cr.Iqueue);
            List<int> addresses = new List<int>(cr.Aqueue);
            Decompressor decompressor = new Decompressor(cfactors, 
                interpPoints, addresses,
                cr.SmallDelta, cr.BigDelta, cr.A, cr.ImageWidth, 
                cr.ImageHeight, cr.DMax);
            Console.WriteLine("Decompressing image...");
            return decompressor.DecompressImage();
            Console.WriteLine("Decompression done.");
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
            if (this.imageOpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.bitmap = MNTools.RescaleBitmap(
                    Image.FromFile(imageOpenFileDialog1.FileName));
                this.originallPictureBox.Image = bitmap;
                this.Text = this.originallPictureBox.Image.Width.ToString()
                    + "x" + this.originallPictureBox.Image.Height.ToString();
                this.compressToolStripMenuItem1.Enabled = true;
            }
        }

        private void settinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (set.ShowDialog() == DialogResult.OK && bitmap != null)
            {
                bitmap = MNTools.RescaleBitmap((Image)bitmap);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filepath = this.saveFileDialog1.FileName;
                Compress((Bitmap)this.originallPictureBox.Image, 
                    Properties.Settings.Default.bigDelta,
                    Properties.Settings.Default.a, filepath);
                this.compresedPictureBox.Image = Decompress(filepath);
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (this.imageOpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.bitmap = MNTools.RescaleBitmap(
                    Image.FromFile(imageOpenFileDialog1.FileName));
                this.originallPictureBox.Image = bitmap;
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String filePath = openFileDialog1.FileName;
                this.compresedPictureBox.Image = Decompress(
                  filePath);
            }
        }

        private void imageOpenFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}