using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.FFMPEG;
using System.Threading;


namespace BikeInclinometer
{
    public partial class Form1 : Form
    {
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
            q1.set_logFrequency("30/s");
            YQt q2 = YQt.FindQt(serial + ".qt2");
            q2.set_logFrequency("30/s");
            YQt q3 = YQt.FindQt(serial + ".qt3");
            q3.set_logFrequency("30/s");
            YQt q4 = YQt.FindQt(serial + ".qt4");
            q4.set_logFrequency("30/s");
            YDataLogger logger = YDataLogger.FindDataLogger(serial + ".dataLogger");
            logger.forgetAllDataStreams();
            System.Threading.Thread.Sleep(5000);
            logger.set_autoStart(YDataLogger.AUTOSTART_ON);
            System.Threading.Thread.Sleep(5000);
            logger.get_module().saveToFlash();
        }




        private List<YMeasure> LoadQT(string hwid)
        {
            YQt qt = YQt.FindQt(hwid);
            YDataSet dataset = qt.get_recordedData(0, 0);
            int progress=0;
            do {
                progress = dataset.loadMore();
            } while(progress <100);


            string fmt = "dd MMM yyyy hh:mm:ss,fff";
            Console.WriteLine("Using DataLogger of " + qt.get_friendlyName());
            YMeasure summary = dataset.get_summary();
            List<YMeasure> res = dataset.get_measures();

            
            double count = (summary.get_endTimeUTC() - summary.get_startTimeUTC())*30;
            Console.WriteLine(count +" value expeded vs " + res.Count);        
            String line = String.Format("from {0} to {1} : min={2:0.00} avg={3:0.00}  max={4:0.00}",
                    summary.get_startTimeUTC_asDateTime().ToString(fmt), summary.get_endTimeUTC_asDateTime().ToString(fmt), summary.get_minValue(), summary.get_averageValue(), summary.get_maxValue()                    
                    );
            Console.WriteLine(line);
            return res;

        }

        public virtual List<double> computeAngles(double w, double x, double y, double z, double defaultRoll)
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
            Console.WriteLine("w:" + w + " x:" + x + " y:" + y + " z:" + z + "-> r:" + roll);
            return res;
        }



        private List<int> GetRecordList(string serial)
        {

            YDataLogger logger = YDataLogger.FindDataLogger(serial + ".dataLogger");
            List<YMeasure> qt_w = LoadQT(serial + ".qt1");
            List<YMeasure> qt_x = LoadQT(serial + ".qt2");
            List<YMeasure> qt_y = LoadQT(serial + ".qt3");
            List<YMeasure> qt_z = LoadQT(serial + ".qt4");
            int count = qt_w.Count;
            List<int> angles = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                 List<double> pos = computeAngles(qt_w[i].get_averageValue(),
                    qt_x[i].get_averageValue(),
                    qt_y[i].get_averageValue(),
                    qt_z[i].get_averageValue(),
                    0);
                 angles.Add((int)Math.Round(pos[0]));
                 backgroundWorker1.ReportProgress(50*i/count);
            }
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

            List<int> angles = GetRecordList(m.get_serialNumber());
            VideoOSD osd = new VideoOSD(System.Drawing.Color.Aqua, inFilename);
            osd.setAngles(angles);
            osd.output(outFilename,backgroundWorker1);
            osd.release();

			e.Result = "done";//return temp 

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            string msg = e.Result.ToString();
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
