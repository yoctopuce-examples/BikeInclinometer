﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.FFMPEG;
using System.Threading;


namespace BikeInclinometer
{
    public partial class Form1 : Form
    {

        string fmt = "dd MMM yyyy hh:mm:ss,fff";

        public Form1()
        {
            InitializeComponent();
        }



        private void moduleInventory()
        {
            YModule m, currentmodule;
            string name;
            int index;

            comboBox1.Items.Clear();
            currentmodule = null;
            m = YModule.FirstModule();
            while (m != null)
            {
                if (m.get_productName() == "Yocto-3D")
                {
                    comboBox1.Items.Add(m);
                    //stop the datalogger
                    YDataLogger d = YDataLogger.FindDataLogger(m.get_serialNumber() + ".dataLogger");
                    d.set_recording(YDataLogger.RECORDING_OFF);
                }
                m = m.nextModule();
            }

            if (comboBox1.Items.Count == 0)
            {
                comboBox1.Enabled = false;
                reset.Enabled = false;
                loadButton.Enabled = false;
            }
            else
            {
                comboBox1.Enabled = true;
                reset.Enabled = true;
                loadButton.Enabled = true;
                index = 0;                
                comboBox1.SelectedIndex = index;
            }
        }


        public void devicelistchanged(YModule m)
        {
            moduleInventory();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // we wanna know when device list changes
            YAPI.RegisterDeviceArrivalCallback(devicelistchanged);
            YAPI.RegisterDeviceRemovalCallback(devicelistchanged);
            moduleInventory();
            UpdateListTimer.Interval = 1000;
            UpdateListTimer.Start();

        }



        private void ConfigureYocto3d(string serial)
        {
            YQt q1 = YQt.FindQt(serial + ".qt1");
            q1.set_logFrequency("25/s");
            YQt q2 = YQt.FindQt(serial + ".qt2");
            q2.set_logFrequency("25/s");
            YQt q3 = YQt.FindQt(serial + ".qt3");
            q3.set_logFrequency("25/s");
            YQt q4 = YQt.FindQt(serial + ".qt4");
            q4.set_logFrequency("25/s");
            YDataLogger logger = YDataLogger.FindDataLogger(serial + ".dataLogger");
            logger.set_recording(YDataLogger.RECORDING_OFF);
            logger.set_autoStart(YDataLogger.AUTOSTART_OFF);
            logger.set_beaconDriven(YDataLogger.BEACONDRIVEN_ON);
            logger.get_module().saveToFlash();
            logger.forgetAllDataStreams();
            System.Threading.Thread.Sleep(5000);
            MessageBox.Show("the Yocto-3D " + serial + " is now ready to record data");

        }




        private List<YMeasure> LoadQT(string hwid)
        {
            YQt qt = YQt.FindQt(hwid);
            YDataSet dataset = qt.get_recordedData(0, 0);
            int progress=0;
            do {
                progress = dataset.loadMore();
            } while(progress <100);


            Console.WriteLine("Using DataLogger of " + qt.get_friendlyName());
            YMeasure summary = dataset.get_summary();
            List<YMeasure> res_bad = dataset.get_measures();
            List<YMeasure> res = new List<YMeasure>();

            foreach(YMeasure m in res_bad){
                if (m.get_startTimeUTC() > 100)
                {
                    res.Add(m);
                }
            }

            
            String line = String.Format("from {0} to {1} : min={2:0.00} avg={3:0.00}  max={4:0.00}",
                    summary.get_startTimeUTC_asDateTime().ToString(fmt), summary.get_endTimeUTC_asDateTime().ToString(fmt), summary.get_minValue(), summary.get_averageValue(), summary.get_maxValue()                    
                    );
            Console.WriteLine(line);
            return res;

        }

        public virtual List<double> computeAngles(double w, double x, double y, double z, double defaultRoll,double t)
        {
            double sqw = 0;
            double sqx = 0;
            double sqy = 0;
            double sqz = 0;
            double norm = 0;
            double delta = 0;
            double head = 0;
            double pitch = 0;
            double roll = 0;
            List<double> res = new List<double>();
            // may throw an exception
            sqw = w * w;
            sqx = x * x;
            sqy = y * y;
            sqz = z * z;
            norm = sqx + sqy + sqz + sqw;
            delta = y * w - x * z;
            if (delta > 0.499 * norm)
            {
                roll = defaultRoll;
                pitch = 90.0;
                head = Math.Round(2.0 * 1800.0 / Math.PI * Math.Atan2(x, w)) / 10.0;
            }
            else
            {
                if (delta < -0.499 * norm)
                {
                    roll = defaultRoll;
                    pitch = -90.0;
                    head = Math.Round(-2.0 * 1800.0 / Math.PI * Math.Atan2(x, w)) / 10.0;
                }
                else
                {
                    roll = Math.Round(1800.0 / Math.PI * Math.Atan2(2.0 * (w * x + y * z), sqw - sqx - sqy + sqz)) / 10.0;
                    pitch = Math.Round(1800.0 / Math.PI * Math.Asin(2.0 * delta / norm)) / 10.0;
                    head = Math.Round(1800.0 / Math.PI * Math.Atan2(2.0 * (x * y + z * w), sqw + sqx - sqy - sqz)) / 10.0;
                }
            }
            res.Add(roll);
            res.Add(pitch);
            res.Add(head);
            res.Add(t);
            //Console.WriteLine("w:" + w + " x:" + x + " y:" + y + " z:" + z + "-> r:" + roll + "-> p:" + pitch + "-> h:" + head );
            return res;
        }



        private List<List<double>> GetRecordList(string serial)
        {
            StreamWriter file = new StreamWriter("c:\\tmp\\debug.log");

            YDataLogger logger = YDataLogger.FindDataLogger(serial + ".dataLogger");
            List<YMeasure> qt_w = LoadQT(serial + ".qt1");
            List<YMeasure> qt_x = LoadQT(serial + ".qt2");
            List<YMeasure> qt_y = LoadQT(serial + ".qt3");
            List<YMeasure> qt_z = LoadQT(serial + ".qt4");
            int count = qt_w.Count;
            double len = qt_w[qt_w.Count-1].get_startTimeUTC() - qt_w[0].get_startTimeUTC();
            double frame = len * 30;
            file.WriteLine("nb measure = "+count+" duration:"+len+"s (we should get "+frame+" at 30/s)" );


            List<List<double>> angles = new List<List<double>>(count);
            double start = qt_w[0].get_startTimeUTC();
            for (int i = 0; i < count; i++)
            {

                
                List<double> pos = computeAngles(qt_w[i].get_averageValue(),
                    qt_x[i].get_averageValue(),
                    qt_y[i].get_averageValue(),
                    qt_z[i].get_averageValue(),
                    0,
                    qt_w[i].get_startTimeUTC()-qt_w[0].get_startTimeUTC()
                    );


                 angles.Add(pos);
                 len = qt_w[i].get_endTimeUTC() - qt_w[i].get_startTimeUTC();
                 double abs = qt_w[i].get_startTimeUTC() - start;

                 String line = String.Format("{0} -> {1:0.000}s  len{2:0.000} : W={3:0.00} X={4:0.00} Y={5:0.00} Z={6:0.00} => r={7:0.00} p={8:0.00} h={9:0.00}",
                    i,abs, len, qt_w[i].get_averageValue(), qt_x[i].get_averageValue(), qt_y[i].get_averageValue(), qt_z[i].get_averageValue(), pos[0], pos[1], pos[2]);
                  file.WriteLine(line);
                 backgroundWorker1.ReportProgress(10*i/count);

            }
            file.Close();
            return angles;
        }





        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Enabled = true;
            YModule m = (YModule)comboBox1.Items[comboBox1.SelectedIndex];
            List<object> arguments = new List<object>();
            arguments.Add(inputFileTextbox.Text);
            arguments.Add(outFileTextBox.Text);
            arguments.Add(m.get_serialNumber());
            backgroundWorker1.RunWorkerAsync(arguments);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            YModule m = (YModule)comboBox1.Items[comboBox1.SelectedIndex];
            ConfigureYocto3d(m.get_serialNumber());
        }

     

        private void UpdateListTimer_Tick(object sender, EventArgs e)
        {
            string errmsg="";
            YAPI.UpdateDeviceList(ref errmsg);
        }




        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            List<object> genericlist = e.Argument as List<object>;
            string inFilename = (string)genericlist[0];
            string outFilename = (string)genericlist[1];
            string serial = (string)genericlist[2];
            YModule m = YModule.FindModule(serial);

            List<List<double>> angles = GetRecordList(m.get_serialNumber());
            VideoOSD osd = new VideoOSD(System.Drawing.Color.Aqua, inFilename);
            osd.setAngles(angles);
            osd.output(outFilename,backgroundWorker1);
            osd.release();

			e.Result = "done";//return temp 

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            string msg = e.Result.ToString();
            progressBar1.Value = 0;
            progressBar1.Enabled = false;
            MessageBox.Show(msg);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
           
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string name = openFileDialog1.FileName;

            inputFileTextbox.Text = name;
        }

        private void selectFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

        }

        private void selectOutFileButton_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string name = saveFileDialog1.FileName;
            outFileTextBox.Text = name;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

      
      
    }


}
