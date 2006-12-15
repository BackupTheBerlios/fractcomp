using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FractalCompression
{
    public partial class Settings : Form
    {
        private int bigDelta;
        private int a;

        public Settings()
        {
            InitializeComponent();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            if (Validate())
            {
                this.errorProvider1.SetError(this.OKBtn, "");
                Properties.Settings.Default.bigDelta = bigDelta;
                Properties.Settings.Default.a = a;
                Properties.Settings.Default.smallDelta = bigDelta / a;
                this.Close();
            }
            else
                this.errorProvider1.SetError(this.OKBtn, "Not all values are correct");
        }

        private bool Validate()
        {
            if (Int32.TryParse(this.domainTextBox.Text, out bigDelta))
                if (Int32.TryParse(this.regionTextBox.Text,out  a))
                    return true;
            return false;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            bigDelta = Properties.Settings.Default.bigDelta;
            a = Properties.Settings.Default.a;
            this.domainTextBox.Text = bigDelta.ToString();
            this.regionTextBox.Text = a.ToString;
        }
    }
}