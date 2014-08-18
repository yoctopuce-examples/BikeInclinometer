using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using AForge.Video.FFMPEG;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Imaging;

namespace BikeInclinometer
{

    class VideoOSD
    {
        private string _src_vid;
        private VideoFileReader _reader;
        private List<List<double>> _angles;
        private Brush _bgcolor;
        private int _width;
        private int _height;
        private int _fps;


        Brush _shadowBrush ;
        Pen _shadowPen;
        Brush _nidleBrush;
        Pen _nidlePen;
        Bitmap _gauge;


        public VideoOSD(Color bgcolor, string src_video)
        {

            _src_vid = src_video;
            _shadowBrush = new SolidBrush(Color.FromArgb(16, 0, 0, 00));
            _shadowPen = new Pen(_shadowBrush, 3);
            _nidleBrush = new SolidBrush(Color.Red);
            _nidlePen = new Pen(_nidleBrush, 3);
            _gauge = new Bitmap(Properties.Resources.bitmap);


            if (src_video != "")
            {
                _reader = new VideoFileReader();

                // open video file
                _reader.Open(_src_vid);

                
                // check some of its attributes
                Console.WriteLine("width:  " + _reader.Width);
                Console.WriteLine("height: " + _reader.Height);
                Console.WriteLine("fps:    " + _reader.FrameRate);
                Console.WriteLine("frames  " + _reader.FrameCount);
                Console.WriteLine("codec:  " + _reader.CodecName);
                _height = _reader.Height;
                _width = _reader.Width;
                //_fps = _reader.FrameRate;
                _fps = 30;

            }
            else
            {
                _reader = null;
                _height = 1080;
                _width = 1920;
                _fps = 30;
            }
        }

        
        public void release()
        {
            if (_reader != null)
            {
                _reader.Close();
                _reader = null;

            }
        }


        public void setAngles(List<List<double>> angles)
        {
            _angles =angles;
        }



        private void process_one_frame(Bitmap videoFrame, int frameno, Brush bgcolor)
        {



            int center_x = 230;
            int center_y = 980;
            int radius_in = 78;
            int radius_out = 128;
            int abs,x,y;
            double a;

            int pos = frameno * _angles.Count / (int)_reader.FrameCount;
            a = _angles[pos][0];

            Graphics g = Graphics.FromImage(videoFrame);
            
            if (bgcolor != null)
                g.FillRectangle(bgcolor, 0, 0, _width, _height);
            g.DrawImage(_gauge, 0, 0);

            if (a < 0)
            {
                abs = Convert.ToInt32(Math.Round(-a));
                double radian = Math.PI * abs / 180.0;
                x = Convert.ToInt32(center_x - Math.Sin(radian) * radius_out);
                y = Convert.ToInt32(center_y - Math.Cos(radian) * radius_out);
            }
            else
            {
                abs = Convert.ToInt32(Math.Round(a));
                double radian = Math.PI * abs / 180.0;
                x = Convert.ToInt32(center_x + Math.Sin(radian) * radius_out);
                y = Convert.ToInt32(center_y - Math.Cos(radian) * radius_out);
            }

            // draw the needle shadow
            Point point1 = new Point(center_x-3, center_y+3);
            Point point2 = new Point(x - 3, y + 3);
            g.DrawLine(_shadowPen, point1, point2);
            
            // draw the needle
            point1 = new Point(center_x - 3, center_y + 3);
            point2 = new Point(x, y);
            g.DrawLine(_nidlePen, point1, point2);


            int toDisp = abs;
            if (_maxangle < abs)
            {
                _maxangle = abs;
                if (_freezeFor>0 && _maxangle > 15)
                {
                    _freezeFor = 45;
                }
            }
            else
            {
                if (_freezeFor==0 && _maxangle > 15)
                {
                    _freezeFor = 45;
                }

            }
            if (_freezeFor>0) {
                toDisp = _maxangle;
                _freezeFor--;
                if (_freezeFor==0) {
                    _maxangle =0;
                }
            }


            int textpos_x = center_x - radius_in + 20;
            int textpos_y = center_y - radius_in;
            g.DrawString(toDisp.ToString("00"), new System.Drawing.Font("Arial", 60), _shadowBrush, textpos_x - 3, textpos_y + 3);
            if (_freezeFor > 0)
            {
                g.DrawString(toDisp.ToString("00"), new System.Drawing.Font("Arial", 60), Brushes.Red, textpos_x, textpos_y);
            }
            else {
                g.DrawString(toDisp.ToString("00"), new System.Drawing.Font("Arial", 60), Brushes.White, textpos_x, textpos_y);
            }
            g.Save();
            g.Dispose();





        }
        int _freezeFor = 0;

        int _maxangle = 0;


        public void output(string out_video,BackgroundWorker worker)
        {

            StreamWriter file = new StreamWriter("c:\\tmp\\debug_2.log");

            double duration = _reader.FrameCount / _fps;
            file.WriteLine("nb measure = " + _reader.FrameCount + " duration:" + duration  +"s (nb frame= " + _reader.FrameCount + ")");

            VideoFileWriter writer = new VideoFileWriter();
            long nb_frame = _angles.Count;
            if (_reader != null)
            {
                nb_frame = _reader.FrameCount;
            }
            writer.Open(out_video, _width, _height, _fps, VideoCodec.Default, 17748000);


            Brush brush = _bgcolor;
            for (int i = 0; i < nb_frame; i++)
            {
                Bitmap videoFrame = null;
                if (_reader != null && i < _reader.FrameCount)
                {
                    videoFrame = _reader.ReadVideoFrame();
                    if (videoFrame == null)
                        break;
                    //string name ="c:\\tmp\\frames\\f_" + i + ".jpg";
                    //videoFrame.Save(name, ImageFormat.Jpeg);
                    brush = null;
                }
                else
                {
                    videoFrame = new Bitmap(_width, _height);
                }
                process_one_frame(videoFrame, i, brush);
                writer.WriteVideoFrame(videoFrame);
                videoFrame.Dispose();
                
                if (worker != null)
                {
                    worker.ReportProgress((int)(10 + 90 * i / nb_frame));
                }

            }

            writer.Close();
            file.Close();

        }

    }

}
