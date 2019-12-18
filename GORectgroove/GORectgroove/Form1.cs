using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
namespace GORectgroove
{
    public partial class Form1 : Form
    {
        public BeamTracing beamTracing = new BeamTracing();
        public List<Complex[]> lc = new List<Complex[]>();
        List<double> ld = new List<double>();
        public double width, depth, wavelength;
        public double[] angle;
        public string[] sA;
        public int rayNumber, samplePoint;
        public double realPart, imagePart;
        public Form1()
        {
            InitializeComponent();
        }
        public double deg2gray(double d)
        {
            return d / 180 * Math.PI;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            comboBoxDataType.Items.Add("Magnitude");
            comboBoxDataType.Items.Add("Real part");
            comboBoxDataType.Items.Add("Image part");
            comboBoxDataType.Items.Add("All");
            comboBoxDataType.SelectedIndex = 0;
            textBoxWidth.Text = "100";
            textBoxDepth.Text = "20";
            textBoxWavlength.Text = "0.5";
            textBoxAngle.Text = "10:1:85";
            textBoxRayNumber.Text = "1001";
            textBoxSamplePointNumber.Text = "10001";
            textBoxRealPart.Text = "0";
            textBoxImagePart.Text = "10"; 
            chart1.Titles.Add("Electric field");

        }
        public void ShowData(int angleIndex, int dataType)
        {
            if (angleIndex >= lc.Count || angleIndex < 0)
                return;
            double[] dataTempReal = new double[samplePoint];
            double[] dataTempImage = new double[samplePoint];
            double[] dataTempMag = new double[samplePoint];
            double[] x = new double[samplePoint];
            chart1.Series.Clear();
            for (int i = 0; i < samplePoint; i++)
            {
                    dataTempMag[i] = (lc[angleIndex])[i].Mod();
                    dataTempReal[i] = (lc[angleIndex])[i].real;
                    dataTempImage[i] = (lc[angleIndex])[i].image;
                    x[i] = -width / 2 + i * width / (samplePoint - 1);
            }
            Series series0 = new Series();
            series0.ChartType = SeriesChartType.Line;
            series0.BorderWidth = 2;
            series0.Color = System.Drawing.Color.Red;
            series0.LegendText = "Magnitude";
            Series series1 = new Series();
            series1.ChartType = SeriesChartType.Line;
            series1.BorderWidth = 2;
            series1.Color = System.Drawing.Color.Blue;
            series1.LegendText = "Real part";
            Series series2 = new Series();
            series2.ChartType = SeriesChartType.Line;
            series2.BorderWidth = 2;
            series2.Color = System.Drawing.Color.Black;
            series2.LegendText = "Image part";

            chart1.ChartAreas[0].AxisX.Title = "x (um)";
            chart1.ChartAreas[0].AxisY.Title = "Electric field";

            for (int i = 0; i < samplePoint; i++)
            {
                series0.Points.AddXY(x[i] * 1e6, dataTempMag[i]);
                series1.Points.AddXY(x[i] * 1e6, dataTempReal[i]);
                series2.Points.AddXY(x[i] * 1e6, dataTempImage[i]);
            }
            if (dataType == 0)
            {
                chart1.Series.Add(series0);
                chart1.ChartAreas[0].AxisY.Maximum = Math.Round(dataTempMag.Max(), 1) + 0.1;
                chart1.ChartAreas[0].AxisY.Minimum = Math.Round(dataTempMag.Min(), 1) - 0.1;
            }
            else if (dataType == 1)
            {
                chart1.Series.Add(series1);
                chart1.ChartAreas[0].AxisY.Maximum = Math.Round(dataTempReal.Max(), 1) + 0.1;
                chart1.ChartAreas[0].AxisY.Minimum = Math.Round(dataTempReal.Min(), 1) - 0.1;
            }
            else if (dataType == 2)
            {
                chart1.Series.Add(series2);
                chart1.ChartAreas[0].AxisY.Maximum = Math.Round(dataTempImage.Max(), 1) + 0.1;
                chart1.ChartAreas[0].AxisY.Minimum = Math.Round(dataTempImage.Min(), 1) - 0.1;
            }
            else
            {
                chart1.Series.Add(series0);
                chart1.Series.Add(series1);
                chart1.Series.Add(series2);
                double[] maxTemp = new double[3];
                maxTemp[0] = dataTempMag.Max();
                maxTemp[1] = dataTempReal.Max();
                maxTemp[2] = dataTempImage.Max();
                double[] minTemp = new double[3];
                minTemp[0] = dataTempMag.Min();
                minTemp[1] = dataTempReal.Min();
                minTemp[2] = dataTempImage.Min();
                chart1.ChartAreas[0].AxisY.Maximum = Math.Round(maxTemp.Max(),1) + 0.1;
                chart1.ChartAreas[0].AxisY.Minimum = Math.Round(minTemp.Min(),1) - 0.1;

            }
            // 设置显示范围
            //ChartArea chartArea = chart1.ChartAreas[0];
            //chartArea.AxisX.Minimum = 0;
            //chartArea.AxisX.Maximum = 10;
            //chartArea.AxisY.Minimum = 0d;
            //chartArea.AxisY.Maximum = 100d;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            lc.Clear();
            ld.Clear();
            try
            {
                width = Convert.ToDouble(textBoxWidth.Text) * 1e-6;
                depth = Convert.ToDouble(textBoxDepth.Text) * 1e-6;
                wavelength = Convert.ToDouble(textBoxWavlength.Text) * 1e-6;
                rayNumber = Convert.ToInt32(textBoxRayNumber.Text);
                samplePoint = Convert.ToInt32(textBoxSamplePointNumber.Text);
                realPart = Convert.ToDouble(textBoxRealPart.Text);
                imagePart = Convert.ToDouble(textBoxImagePart.Text);
                string s = textBoxAngle.Text;
                if (s.Contains(":"))
                {
                    char splitChar;
                    splitChar = ':';
                    sA = s.Split(splitChar);
                    if (sA.Length != 3)
                    {
                        MessageBox.Show("The format of illumination angle is wrong.");
                    }
                    double start = Convert.ToDouble(sA[0]);
                    double step = Convert.ToDouble(sA[1]);
                    double end = Convert.ToDouble(sA[2]);

                    while (start <= end)
                    {
                        ld.Add(start);
                        start = start + step;
                    }
                }
                else
                {
                    char splitChar;
                    splitChar = ',';
                    sA = s.Split(splitChar);
                    foreach (string st in sA)
                    {
                        ld.Add(Convert.ToDouble(st));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            Complex[] c;
            for (int i = 0; i < ld.Count; i++)
            {
                //str = "amp" + (i).ToString() + ".txt";
                //beamTracing.InitParameter(width, depth, deg2gray(-ld[i]), rayNumber, 0.5e-6, samplePoint, new Complex(realPart, 0));
                beamTracing.InitParameter(width, depth, deg2gray(-ld[i]), rayNumber, wavelength, samplePoint, new Complex(realPart, imagePart));
                beamTracing.InitRays();
                c = beamTracing.BeamTrace();
                lc.Add(c);
                //beamTracing.OutputAmplitude(str);
            }
            comboBoxIlluminationAngle.Items.Clear();
            for (int i = 0; i < ld.Count; i++)
            {
                comboBoxIlluminationAngle.Items.Add(ld[i].ToString());
            }
            comboBoxIlluminationAngle.Items.Add("ALL for save");
            comboBoxIlluminationAngle.SelectedIndex = 0;
            ShowData(comboBoxIlluminationAngle.SelectedIndex, comboBoxDataType.SelectedIndex);
        }

        private void comboBoxDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowData(comboBoxIlluminationAngle.SelectedIndex, comboBoxDataType.SelectedIndex);

        }

        private void comboBoxIlluminationAngle_SelectedIndexChanged(object sender, EventArgs e)
        {

            ShowData(comboBoxIlluminationAngle.SelectedIndex, comboBoxDataType.SelectedIndex);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "ext files (*.txt)|*.txt|All files(*.*)|*>**";
            sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;
            DialogResult dr = sfd.ShowDialog();
            if (dr == DialogResult.OK && sfd.FileName.Length > 0)
            {
                FileStream fs = new FileStream(sfd.FileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                if (comboBoxIlluminationAngle.SelectedIndex < ld.Count)
                    for (int i = 0; i < samplePoint; i++)
                    {
                        sw.WriteLine(lc[comboBoxIlluminationAngle.SelectedIndex][i].real.ToString() + "    " + lc[comboBoxIlluminationAngle.SelectedIndex][i].image.ToString());
                    }
                else
                {
                    for (int j = 0; j < ld.Count; j++)
                        for (int i = 0; i < samplePoint; i++)
                        {
                            sw.WriteLine(lc[j][i].real.ToString() + "    " + lc[j][i].image.ToString());
                        }                
                }
                sw.Close();
                fs.Close();        
            }   
        }
    }
}
