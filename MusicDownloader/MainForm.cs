using MusicDownloader.Code;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace MusicDownloader
{
    public partial class MainForm : Form
    {
        private Downloader downloader = new Downloader();

        public MainForm()
        {
            InitializeComponent();
        }

        private void browse_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void download_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                var list = downloader.GetSongs(textBox1.Text);
                for (var i = 1; i < list.Length; i++)
                {
                    Update((i + 1) + "/" + list.Length, list[i]);

                    downloader.Download(list[i], ConfigurationManager.AppSettings["binaryFolder"], ConfigurationManager.AppSettings["outputFolder"]);
                }

                Update("-", "-");
            }
            else
            {
                MessageBox.Show("Please select song list file.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Cursor.Current = Cursors.Default;
        }

        private void Update(string str, string str2)
        {
            toolStripStatusLabel1.Text = str;
            toolStripStatusLabel2.Text = str2;
            Refresh();
            Update();
        }
    }
}
