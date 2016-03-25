using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownLoadYoutube
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
            Initstart();
            
        }

        private UtilClass Util { get; set; }

        private void Initstart()
        {
            label1.Text = Directory.GetCurrentDirectory();
            Util = new UtilClass();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var curentDir = Directory.GetCurrentDirectory();
            folderBrowserDialog1.SelectedPath = curentDir;
            folderBrowserDialog1.ShowDialog();
            if ((curentDir == folderBrowserDialog1.SelectedPath) || !Directory.Exists(folderBrowserDialog1.SelectedPath))
                return;
            Directory.SetCurrentDirectory(folderBrowserDialog1.SelectedPath);
            Initstart();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show(@"Не задан урл");
            }

            else
            {
                label2.ResetText();
                label4.ResetText();
                label5.ResetText();
                label6.ResetText();
                var id = textBox1.Text.Substring(textBox1.Text.IndexOf("v=", StringComparison.Ordinal) + 2).Trim();

                Util = new UtilClass(id);


                label7.Text = Util.VideoName;

                if ((Util.Url == string.Empty) || (Util.VideoName == string.Empty)) return;
                button2.Enabled = false;
                DownloadFile(Util.Url, Util.FullPathVideoFileName);
            }
        }


        private void DownloadFile(string urlAddress, string location)
        {
            using (UtilClass.WebClient = new WebClient())
            {
                UtilClass.WebClient.DownloadFileCompleted += Completed;
                UtilClass.WebClient.DownloadProgressChanged += ProgressChanged;
                try
                {

                    var url = !urlAddress.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                        ? new Uri("http://" + urlAddress)
                        : new Uri(urlAddress);
                    UtilClass.Sw.Start();
                    UtilClass.WebClient.DownloadFileAsync(url, location);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {

                var labelSpeed = (Convert.ToDouble(e.BytesReceived)/1024/UtilClass.Sw.Elapsed.TotalSeconds).ToString("0.00") +
                                 " kb/s";
                var progressBar = e.ProgressPercentage;
                var labelPerc = e.ProgressPercentage + "%";
                var labelDownloaded = (Convert.ToDouble(e.BytesReceived)/1024/1024).ToString("0.00") + " Mb's" + "  /  " +
                                      (Convert.ToDouble(e.TotalBytesToReceive)/1024/1024).ToString("0.00") + " Mb's";
                label2.Text = labelSpeed;
                label4.Text = labelPerc;
                label5.Text = labelDownloaded;
                progressBar1.Value = progressBar;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            UtilClass.Sw.Reset();
            label6.Text = e.Cancelled ? @"Canceled" : @"Download completed!";
            button2.Enabled = true;
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Util.FullPathVideoFileName))
            {
                MessageBox.Show(@"Не заданно имя файла");


                label7.Text = Path.GetFileNameWithoutExtension(Directory.GetFiles(Directory.GetCurrentDirectory() + "\\" + "Video").First());
            }
            button3.Enabled = false;
            var psi = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    FileName = Directory.GetCurrentDirectory() + "\\" + "ffmpeg.exe",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    Arguments =
                        " -i \"" + Util.FullPathVideoFileName + "\" -vn -ar 44100 -ac 2 -ab 192 -f mp3 \"" +
                        Util.FullPathMp3FileName + "\""
                },
                EnableRaisingEvents = true
            };

            psi.Exited += EndProcess;

            await Task.Run(() => psi.Start()).ConfigureAwait(false);
        }

        private void EndProcess(object sender, EventArgs eventArgs)
        {
            if (button3.InvokeRequired)
            {
                UtilClass.SetTextCallback d = EndProcess;
                Invoke(d, sender, eventArgs);
            }
            else
            {
                button3.Enabled = true;
            }
        }
    }
}