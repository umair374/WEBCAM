using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using Oracle.ManagedDataAccess.Client;

namespace WEBCAM
{
    public partial class MainWindow : Window
    {
        private Bitmap capturedImage = null!;
        private VideoCaptureDevice videoSource = null!;
        private FilterInfoCollection videoDevices = null!;
        private string connString = "User Id=hr;Password=123456;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.1.9)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)))";

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;
        }

        private void InitializeCamera()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                MessageBox.Show("No video sources found.");
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSource.Start();
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            BitmapImage bitmapImage = new BitmapImage();

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                cameraFeed.Source = bitmapImage; // Displaying the live feed
            }));

        }

        private void LoadImage(int imageId)
        {
            using (OracleConnection connection = new OracleConnection(connString))
            {
                connection.Open();
                using (OracleCommand cmd = new OracleCommand("SELECT ImageData FROM ImagesTable WHERE ImageID = :ImageID", connection))
                {
                    cmd.Parameters.Add(":ImageID", OracleDbType.Int32).Value = imageId;

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            byte[] imageBytes = (byte[])reader["ImageData"];
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                BitmapImage bitmapImage = new BitmapImage();
                                bitmapImage.BeginInit();
                                bitmapImage.StreamSource = ms;
                                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                bitmapImage.EndInit();
                                bitmapImage.Freeze();

                                loadedImageDisplay.Source = bitmapImage; 
                            }
                        }
                        else
                        {
                            MessageBox.Show("Image not found.");
                        }
                    }
                }
            }
        }


        private void btnShowImage_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter Image ID:", "Load Image", "2", -1, -1);

            if (int.TryParse(input, out int id))
            {
                LoadImage(id);
            }
            else
            {
                MessageBox.Show("Invalid ID entered. Please enter a valid number.");
            }
        }

        private void btnStartCamera_Click(object sender, RoutedEventArgs e)
        {
            if (capturedImage != null)
            {
                capturedImageDisplay.Source = null;
                CapturedImageGrid.Visibility = Visibility.Collapsed;
                capturedImage = null;
            }
            if (videoSource == null)
            {

                InitializeCamera();
            }
            

            if (videoSource != null && !videoSource.IsRunning)
            {
                videoSource.Start();
            }
            else if (videoSource == null)
            {
                MessageBox.Show("No video source available. Please ensure a webcam is connected.");
            }
        }

        private void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            if (cameraFeed.Source != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)cameraFeed.Source));

                    encoder.Save(ms);
                    ms.Position = 0;

                    capturedImage = new Bitmap(ms);

                    using (MemoryStream displayStream = new MemoryStream())
                    {
                        capturedImage.Save(displayStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        displayStream.Position = 0;

                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = displayStream;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();

                        capturedImageDisplay.Source = bitmapImage;
                        CapturedImageGrid.Visibility = Visibility.Visible; 
                    }

                    MessageBox.Show("Image Captured.");
                }
            }
            else
            {
                MessageBox.Show("CAMERA IS TURNED OFF");
            }
        }


        private async void btnSaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (capturedImage == null)
            {
                MessageBox.Show("No image captured to save.");
                return;
            }

            try
            {
                int employeeId = 0;
                string input = Microsoft.VisualBasic.Interaction.InputBox("Enter EMPLOYEE ID : ", "UPDATE Image", "1", -1, -1);

                if (int.TryParse(input, out int id))
                {
                    employeeId = id;
                }

                StopCameraFeed();

                using (OracleConnection connection = new OracleConnection(connString))
                {
                    
                    await connection.OpenAsync(); 

                    using (OracleCommand cmd = new OracleCommand("UPDATE EMPLOYEE_PROFILE SET PICTURE = :ImageData WHERE EMPLOYEE_ID = :EmployeeId", connection))
                    {
                        cmd.CommandTimeout = 30; 

                        using (MemoryStream ms = new MemoryStream())
                        {
                            capturedImage.Save(ms, ImageFormat.Jpeg);
                            cmd.Parameters.Add(":ImageData", OracleDbType.LongRaw).Value = ms.ToArray();
                            cmd.Parameters.Add(":EmployeeId", OracleDbType.Int32).Value = employeeId;

                           // MessageBox.Show("Saving image data...");
                            await cmd.ExecuteNonQueryAsync(); 
                        }
                    }
                }

                MessageBox.Show("Image saved to database.");
                capturedImageDisplay.Source = null;
                CapturedImageGrid.Visibility = Visibility.Collapsed;
                capturedImage = null;
            }
            catch (OracleException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image to database: {ex.Message}");
            }
            finally
            {
                StartCameraFeed();
            }
        }


        private void StopCameraFeed()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop(); 
                videoSource = null; 
            }
        }

        private void StartCameraFeed()
        {
            if (videoDevices == null)
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("No video sources found.");
                    return;
                }
            }

            if (videoSource == null)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            }

            if (!videoSource.IsRunning)
            {
                videoSource.Start();
            }
        }


        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();

                Task.Run(() =>
                {
                    int waitTime = 2000;
                    int interval = 100;
                    int totalWaited = 0;
                    while (videoSource.IsRunning && totalWaited < waitTime)
                    {
                        Thread.Sleep(interval);
                        totalWaited += interval;
                    }

                    if (videoSource.IsRunning)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("Camera did not stop in time. Closing the application anyway.");
                        });
                    }
                });
            }
        }
    }
}
