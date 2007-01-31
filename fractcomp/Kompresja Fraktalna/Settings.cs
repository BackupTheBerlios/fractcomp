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
        private int eps;

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
            if (ValidateData())
            {
                this.errorProvider1.SetError(this.regionTextBox, "");
                Properties.Settings.Default.bigDelta = bigDelta;
                a = (int)Math.Sqrt(a);
                Properties.Settings.Default.a = a;
                Properties.Settings.Default.smallDelta = bigDelta / a;
                Properties.Settings.Default.eps = eps;
                Properties.Settings.Default.Save();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }                
        }

        private bool ValidateData()
        {
            int tempBigDelta;
            int tempA;
            int tempEps;
            if (Int32.TryParse(this.domainTextBox.Text, out tempBigDelta) &&
                IsPowerOf(tempBigDelta, 2))
            {
                this.errorProvider1.SetError(this.domainTextBox, "");
                if (Int32.TryParse(this.regionTextBox.Text, out  tempA) &&
                    IsPowerOf(tempA, 4) && tempA <= tempBigDelta/2)
                {
                    if (Int32.TryParse(this.epstextBox1.Text, out  tempEps) &&
                    tempEps >= 0)
                    {
                        a = tempA;
                        bigDelta = tempBigDelta;
                        eps = tempEps;
                        return true;
                    }
                    this.errorProvider1.SetError(this.epstextBox1, "Value is incorrect"
                    + "it should be greater than 0");
                    return false;
                }
                this.errorProvider1.SetError(this.regionTextBox, "Value is incorrect"
                + "it should be power of 4, and no bigger than Number of domains divide by 2");
                return false;
            }
            this.errorProvider1.SetError(this.domainTextBox, "Value is incorrect, it should be power of 2");
            return false;
        }

        private bool IsPowerOf(int number, int power)
        {
            if (number < 1)
                throw new Exception("Invalid value");
            int val = 1;
            while (val < number)
                val *= power;
            if (val == number)
                return true;
            return false;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            bigDelta = Properties.Settings.Default.bigDelta;
            a = Properties.Settings.Default.a;
            eps = Properties.Settings.Default.eps;
            a = (int)Math.Pow(a, 2);
            this.domainTextBox.Text = bigDelta.ToString();
            this.regionTextBox.Text = a.ToString();
            this.epstextBox1.Text = eps.ToString();

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}