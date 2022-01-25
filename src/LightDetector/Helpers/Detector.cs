using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightDetector.Helpers
{
    public class Detector:IDisposable
    {
        #region Vars and Events
        Rectangle selectionArea;
        Rectangle viewArea;
        bool IsRecording = false;
        List<LightRecord> Datas;
        LightRecord CurrentRecord;
        public bool IsCapturing { get; set; } = false;
        public delegate void FrameReceivedEventHandler(object source, FrameReceivedArgs args);
        public event FrameReceivedEventHandler? FrameReceived;
        public class FrameReceivedArgs : EventArgs
        {
            public Bitmap Extracted { get; set; }
            public Bitmap Data { get; set; }
            public DateTime DateExtract { get; set; }
        }

        public delegate void CaptureStoppedEventHandler(object source, CaptureStoppedArgs args);
        public event CaptureStoppedEventHandler? CaptureStopped;
        public class CaptureStoppedArgs : EventArgs
        {
            public DateTime DateStopped { get; set; }
        }

        public enum DetectionPhase { Start,End };
        public delegate void LightDetectedEventHandler(object source, LightDetectedArgs args);
        public event LightDetectedEventHandler? LightDetected;
        public class LightDetectedArgs : EventArgs
        {
            public DetectionPhase Phase { get; set; }
            public DateTime DateDetected { get; set; }
        }
        public DetectorConfig Config { get; set; }
      
        System.Windows.Forms.Timer My_Timer;
        VideoCapture _capture;

        #endregion
        public Detector(Rectangle VideoFrameSize)
        {
            viewArea = VideoFrameSize;
            Config = new DetectorConfig();
            Datas=new List<LightRecord>();
        }
        public Detector(Rectangle VideoFrameSize,DetectorConfig config)
        {
            viewArea = VideoFrameSize;
            Config = config;
            Datas=new List<LightRecord>();
        }

       
        public void SetSelectionArea(params Rectangle[] rects)
        {
            //for example we just take 1 selection area
            if(rects!=null && rects.Length>0)
                selectionArea = rects[0];
        }
        public async Task ReadFrame(string FName)
        {
            _capture = new Emgu.CV.VideoCapture(FName);
            
            if (My_Timer == null)
            {
                var FPS = 30;
                My_Timer = new System.Windows.Forms.Timer();
                My_Timer.Interval = 1000 / FPS;
                My_Timer.Tick += new EventHandler(My_Timer_Tick);
            }

            //Frame Rate
            IsCapturing = true;
            My_Timer.Start();
            
        }

        void My_Timer_Tick(object sender, EventArgs e)
        {
            var frame = _capture.QueryFrame();
            if (frame != null)
            {
                DetectLight(frame);
            }
            else
            {
                IsCapturing=false;
                My_Timer.Stop();
                CaptureStopped?.Invoke(this, new CaptureStoppedArgs() { DateStopped = DateTime.Now }); 
            }
        }
        
        void DetectLight(Mat nextFrame)
        {


            if (nextFrame != null)
            {

                // buat gambar jadi B/W untuk memudahkan lihat bagian yang terang
                Mat imgGrayscale = new Mat(nextFrame.Size, DepthType.Cv8U, 1);
                CvInvoke.CvtColor(nextFrame, imgGrayscale, ColorConversion.Bgr2Gray);

                Mat imgBlurred = new Mat(nextFrame.Size, DepthType.Cv8U, 1);
                CvInvoke.GaussianBlur(imgGrayscale, imgBlurred, new Size(11, 11), 0);

                Mat imgThreshold = new Mat(nextFrame.Size, DepthType.Cv8U, 1);
                var thresh = CvInvoke.Threshold(imgBlurred, imgThreshold, 200, 255, ThresholdType.Binary);

                Mat imgErode = new Mat(nextFrame.Size, DepthType.Cv8U, 1);
                var element = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(3, 3), new Point(-1, -1));

                CvInvoke.Erode(imgThreshold, imgErode, element, Point.Empty, 2,BorderType.Default, default(MCvScalar));

                Mat imgDilate = new Mat(nextFrame.Size, DepthType.Cv8U, 1);
                CvInvoke.Dilate(imgErode, imgDilate, element, Point.Empty, 4, BorderType.Default, default(MCvScalar));

                if (selectionArea.Width > 0 && selectionArea.Height > 0)
                {
                    //cropping sesuai selection area
                    var ratioX = (double)selectionArea.X / viewArea.Width;
                    var ratioY = (double)selectionArea.Y / viewArea.Height;
                    var ratioWidth = (double)selectionArea.Width / viewArea.Width;
                    var ratioHeight = (double)selectionArea.Height / viewArea.Height;

                    var selectRect = new Rectangle((int)(ratioX * nextFrame.Width), (int)(ratioY * nextFrame.Height), (int)(ratioWidth * nextFrame.Width), (int)(ratioHeight * nextFrame.Height));
                    Mat Selected = new Mat(imgDilate, selectRect);
                    CvInvoke.Imshow("cropped", Selected);

                    //extract light blue pixels                   
                    
                    //Image<Gray, Byte> grayImg = Selected.ToImage<Bgr,byte>().InRange(new Bgr(minBlue,0,0), new Bgr(maxBlue, 0, 0));
                    //var countBlue = CvInvoke.CountNonZero(grayImg);
                    var countBlue = 0;
                 
                    var img = Selected.ToImage<Bgr, byte>();
                    var allByte = img.Size.Height * img.Size.Width;
                    for (int i = 0; i < img.Size.Height; i++)
                    {
                        for (int j = 0; j < img.Size.Width; j++)
                        {
                           
                                Bgr currentColor = img[i, j];

                                if (currentColor.Blue >= Config.MinBlue && currentColor.Blue <= Config.MaxBlue)
                                {
                                    countBlue++;
                                }
                          
                        }
                    }
                    if (Selected != null)
                    {
                        var pct = (float) countBlue / allByte * 100;
                        Debug.WriteLine($"blue: {countBlue}/{allByte} = {pct.ToString("n2")}%");
                        if (pct > Config.PercentThreshold && !IsRecording)
                        {
                            IsRecording = true;
                            CurrentRecord = new LightRecord() { StartDate = DateTime.Now };
                            LightDetected?.Invoke(this, new LightDetectedArgs() { DateDetected = DateTime.Now, Phase = DetectionPhase.Start });
                        }else if(IsRecording && pct < Config.PercentThreshold)
                        {
                            IsRecording= false;
                            CurrentRecord.EndDate = DateTime.Now;
                            CurrentRecord.Duration = CurrentRecord.EndDate - CurrentRecord.StartDate;
                            CurrentRecord.RegionNo = 1;
                            //add to datas
                            Datas.Add(CurrentRecord);
                            LightDetected?.Invoke(this, new LightDetectedArgs() { DateDetected = DateTime.Now, Phase = DetectionPhase.End });

                        }
                    }
                }
                FrameReceived?.Invoke(this, new FrameReceivedArgs() { Data=nextFrame.ToBitmap(), Extracted = imgDilate.ToBitmap(), DateExtract = DateTime.Now });


            }

        }

        private Mat GetMatFromSDImage(System.Drawing.Image image)
        {
            int stride = 0;
            Bitmap bmp = new Bitmap(image);

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);

            System.Drawing.Imaging.PixelFormat pf = bmp.PixelFormat;
            if (pf == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                stride = bmp.Width * 4;
            }
            else
            {
                stride = bmp.Width * 3;
            }

            Image<Bgra, byte> cvImage = new Image<Bgra, byte>(bmp.Width, bmp.Height, stride, (IntPtr)bmpData.Scan0);

            bmp.UnlockBits(bmpData);

            return cvImage.Mat;
        }

        public void Dispose()
        {
            My_Timer.Stop();
            My_Timer.Dispose();

        }

        #region Data

        public bool ExportAsCSV(string FName)
        {
            try
            {
                var dt = GetDataAsTable();
                if (dt == null) return false;
                StringBuilder sb = new StringBuilder();

                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName);
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in dt.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    sb.AppendLine(string.Join(",", fields));
                }

                File.WriteAllText(FName, sb.ToString());
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error saat save csv:" + ex);
            }
            return false;
        }
        public void ResetData()
        {
            Datas.Clear();
        }
        public DataTable GetDataAsTable()
        {
            var dt = new DataTable("summary");
            dt.Columns.Add("Region");
            dt.Columns.Add("Start Time");
            dt.Columns.Add("End Time");
            dt.Columns.Add("Duration (seconds)");
            foreach(var item in Datas)
            {
                var rows = dt.NewRow();
                rows[0] = item.RegionNo; 
                rows[1] = item.StartDate.ToString("dd/MM/yyyy HH:mm:ss"); 
                rows[2] = item.EndDate.ToString("dd/MM/yyyy HH:mm:ss"); 
                rows[3] = item.Duration.TotalSeconds.ToString("n2"); 
                dt.Rows.Add(rows);
            }
            dt.AcceptChanges();
            return dt;
        }
        #endregion
    }

    public class LightRecord
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RegionNo { get; set; }
        public TimeSpan Duration { get; set; }
    }
    public class DetectorConfig
    {
        int _MinBlue = 255;
        public int MinBlue
        {
            get { return _MinBlue; }
            set
            {
                if (value < 0 || value > 255) throw new ArgumentOutOfRangeException("range 0 - 255");
                _MinBlue = value;
            }
        }
        int _MaxBlue = 256;
        public int MaxBlue
        {
            get { return _MaxBlue; }
            set
            {
                if (value < 1 || value > 256) throw new ArgumentOutOfRangeException("range 1 - 256");
                _MaxBlue = value;
            }
        }

        int _PercentThreshold = 15;
        public int PercentThreshold
        {
            get { return _PercentThreshold; }
            set
            {
                if (value <= 0 || value >= 100) throw new ArgumentOutOfRangeException("range 0 - 99");
                _PercentThreshold = value;
            }
        }
    }
}
