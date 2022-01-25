using LightDetector.Helpers;

namespace LightDetector
{
    public partial class Form1 : Form
    {
        Detector detector;
        public Form1()
        {
            InitializeComponent();
            var cfg = new DetectorConfig() { MaxBlue = 256, MinBlue = 255, PercentThreshold = 12 };
            detector = new Detector(new Rectangle(0,0,pictureBox1.Width,pictureBox1.Height), cfg);
            detector.LightDetected += (x, y) =>
            {
                if (y.Phase == Detector.DetectionPhase.End)
                {
                    GvData.DataSource = detector.GetDataAsTable();
                }
            };
            detector.FrameReceived += (x, y) =>
            {
                //CvInvoke.Imshow("pic", y.Data);
                pictureBox1.Image = y.Data;
                pictureBox2.Image = y.Extracted;

            };
            BtnExport.Click += async (a, b) =>
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "CSV |*.csv";
                saveFileDialog1.Title = "Export to a CSV File";
                saveFileDialog1.ShowDialog();

                // If the file name is not an empty string open it for saving.
                if (saveFileDialog1.FileName != "")
                {
                    detector.ExportAsCSV(saveFileDialog1.FileName);
                }
            };
                BtnReset.Click += async (a, b) =>
            {
                detector.ResetData();
                GvData.DataSource = null;
            };
                BtnOpen.Click += async (a, b) =>
            {
                var fname = OpenFileDialogForm();
                if (!string.IsNullOrEmpty(fname))
                {
                    if (detector.IsCapturing)
                    {
                        MessageBox.Show("Tidak bisa memproses video lain, masih capturing.", "Warning");
                    }
                    else
                        await detector.ReadFrame(fname);
                }
            };
            pictureBox1.Paint += (object? sender, PaintEventArgs e) =>
            {
                if (SelectionArea.Width > 0)
                {
                    var pen = new Pen(Color.LightGreen, 2);
                    e.Graphics.DrawRectangle(pen,SelectionArea);
                }
            };
            pictureBox1.MouseDown += (object? sender, MouseEventArgs e)=>
            {
                if(e.Button == MouseButtons.Left)
                {
                    IsSelect = true;
                    StartLocation = e.Location;
                }
            };
            pictureBox1.MouseMove += (object? sender, MouseEventArgs e) =>
            {
                if(e.Button == MouseButtons.Left && IsSelect)
                {
                    SelectionArea.X = Math.Min(StartLocation.X, e.X);
                    SelectionArea.Y = Math.Min(StartLocation.Y, e.Y);
                    SelectionArea.Width = Math.Abs(StartLocation.X - e.X);
                    SelectionArea.Height = Math.Abs(StartLocation.Y - e.Y);
                    pictureBox1.Invalidate();
                }
            };
            pictureBox1.MouseUp += (object? sender, MouseEventArgs e) =>
            {
                if (e.Button == MouseButtons.Left && IsSelect)
                {
                    IsSelect = false;
                    detector.SetSelectionArea(SelectionArea);
                } else if (e.Button == MouseButtons.Right)
                {
                    SelectionArea.X = 0;
                    SelectionArea.Y = 0;
                    SelectionArea.Width = 0;
                    SelectionArea.Height = 0;
                }
            };
        }
        Point StartLocation;
        bool IsSelect = false;
        Rectangle SelectionArea = new Rectangle(0, 0, 0, 0);
        

        public string? OpenFileDialogForm()
        {
            var openFileDialog1 = new OpenFileDialog()
            {
                FileName = "",
                Filter = "Video files (*.mp4)|*.mp4",
                Title = "Open video file"
            };
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (File.Exists(openFileDialog1.FileName))
                    {
                        return openFileDialog1.FileName;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");

                }
            }
            return null;
        }

    }
}