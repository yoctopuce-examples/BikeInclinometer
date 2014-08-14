using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Video.FFMPEG;
using System.Drawing;
using System.ComponentModel;


namespace BikeInclinometer
{

    class VideoOSD
    {
        private string _src_vid;
        private VideoFileReader _reader;
        private List<int> _angles;
        private Brush _bgcolor;
        private int _width;
        private int _height;
        private int _fps;


        public VideoOSD(Color bgcolor, string src_video)
        {
            _bgcolor = new System.Drawing.SolidBrush(bgcolor);
            _src_vid = src_video;
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
                _fps = _reader.FrameRate;
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


        public void setAngles(List<int> angles)
        {
            _angles =angles;
        }



        private void process_one_frame(Bitmap videoFrame, int angle, Brush bgcolor)
        {
            Graphics g = Graphics.FromImage(videoFrame);


            int center_x = 230;
            int center_y = 980;
            int radius_in = 78;
            int radius_out = 138;
            int abs,x,y;

            Bitmap bg = new Bitmap(Properties.Resources.bitmap);
            if (bgcolor != null)
                g.FillRectangle(bgcolor, 0, 0, _width, _height);
            g.DrawImage(bg, 0, 0);

            if (angle < 0)
            {
                abs = -angle;
                double radian = Math.PI * abs / 180.0;
                x = Convert.ToInt32(center_x - Math.Sin(radian) * radius_out);
                y = Convert.ToInt32(center_y - Math.Cos(radian) * radius_out);
            }
            else
            {
                abs = angle;
                double radian = Math.PI * abs / 180.0;
                x = Convert.ToInt32(center_x + Math.Sin(radian) * radius_out);
                y = Convert.ToInt32(center_y - Math.Cos(radian) * radius_out);
            }

            // draw the needle shadow
            Pen shadow = new Pen(Color.FromArgb(16, 0, 0, 00), 3);
            Point point1 = new Point(center_x-3, center_y+3);
            Point point2 = new Point(x - 3, y + 3);
            g.DrawLine(shadow, point1, point2);

            // draw the needle
            Pen red = new Pen(Color.FromArgb(255, 255, 0, 00), 3);
            point1 = new Point(center_x - 3, center_y + 3);
            point2 = new Point(x, y);
            g.DrawLine(red, point1, point2);

            g.DrawString(angle.ToString(), new System.Drawing.Font("Arial", 60), Brushes.White, center_x-radius_in, center_y-radius_in);
            g.Save();
            g.Dispose();
        }


        public void output(string out_video,BackgroundWorker worker)
        {

            VideoFileWriter writer = new VideoFileWriter();
            long nb_frame = _angles.Count;
            if (_reader != null)
            {
                nb_frame = _reader.FrameCount;
            }
            writer.Open(out_video, _width, _height, _fps, VideoCodec.Default, 17748000);


            for (int i = 0; i < nb_frame; i++)
            {
                Bitmap videoFrame = null;
                Brush brush;
                if (_reader != null && i < _reader.FrameCount)
                {
                    videoFrame = _reader.ReadVideoFrame();
                    if (videoFrame == null)
                        break;
                    brush = null;
                }
                else
                {
                    videoFrame = new Bitmap(_width, _height);
                    brush = _bgcolor;
                }
                if (i < _angles.Count)
                {
                    process_one_frame(videoFrame, _angles[i], brush);
                }
                writer.WriteVideoFrame(videoFrame);
                videoFrame.Dispose();
                if (worker != null)
                {
                    worker.ReportProgress((int)(50 + 50 * i / nb_frame));
                }

            }

            writer.Close();
        }

    }

}
