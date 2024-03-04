using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using WMPLib;
using FFmpeg.AutoGen;
using FFmpeg;
using OpenCvSharp;
using NReco.VideoInfo;
using System.Security.AccessControl;
using System.Diagnostics;
using WinP_VidEdit;
using System.Windows.Interop;
using MetadataExtractor;
using System.IO.Packaging;
using NReco.VideoConverter;
using System.Xml;

namespace WinP_VidEdit
{
    public partial class Form1 : Form
    {
        WindowsMediaPlayer Player = new WindowsMediaPlayer();
        public Form1()
        {
            InitializeComponent();
            axWindowsMediaPlayer1.uiMode = "None";
            timer1.Enabled = false;
            timer1.Interval = 1000;
            label8.Text = "";
            label2.Enabled = false;
            label3.Enabled = false;
            trackBar2.Enabled = false;
            trackBar3.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;

        }

        private void 파일열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string strFile = openFileDialog1.FileName;
                axWindowsMediaPlayer1.URL = strFile;

                FFProbe ffprobe = new FFProbe();
                ffprobe.IncludeStreams = true;
                ffprobe.IncludeFormat = true;
                var vidinfo = ffprobe.GetMediaInfo(axWindowsMediaPlayer1.URL);

                trackBar1.Enabled = true;

                int vid_duration = get_duration();

                trackBar1.Maximum = vid_duration;
                trackBar1.Minimum = 0;
                timer1.Enabled = true;
                trackBar1.Value = 0;

                trackBar2.Maximum = vid_duration;
                trackBar2.Minimum = 0;
                trackBar2.Value = 0;

                trackBar3.Maximum = vid_duration;
                trackBar3.Minimum = 0;
                trackBar3.Value = 0;

                textBox4.Text = GetMetaData(axWindowsMediaPlayer1.currentMedia.sourceURL, "QuickTime Track Header", "Width");
                textBox3.Text = GetMetaData(axWindowsMediaPlayer1.currentMedia.sourceURL, "QuickTime Track Header", "Height");

                FileName_Label.Text = openFileDialog1.SafeFileName;
                Format_info.Text = axWindowsMediaPlayer1.currentMedia.sourceURL.Substring(axWindowsMediaPlayer1.currentMedia.sourceURL.Length - 3);
                Codec_info.Text = get_Vidinfo("codec_name", vidinfo);
                string width = get_Vidinfo("width", vidinfo);
                Width_info.Text = width;
                textBox3.Text = width;
                string height = get_Vidinfo("height", vidinfo);
                textBox4.Text = height;
                Height_info.Text = height;
                string bitrate = get_Vidinfo("bit_rate", vidinfo);
                int bit_rate = (int)Convert.ToInt64(bitrate) / 1024;
                bitrate_info.Text = Convert.ToString(bit_rate);
                string framerate = get_Vidinfo("avg_frame_rate", vidinfo);
                textBox7.Text = Convert.ToString((Convert.ToDouble(framerate.Substring(0, framerate.IndexOf("/") - 1)) / Convert.ToDouble(framerate.Substring(framerate.IndexOf("/") + 1))) * 10).Substring(0, 4);
                Framerate_info.Text = Convert.ToString((Convert.ToDouble(framerate.Substring(0, framerate.IndexOf("/")-1)) / Convert.ToDouble(framerate.Substring(framerate.IndexOf("/")+1)))*10).Substring(0, 4);
                string duration = get_Vidinfo("duration", vidinfo);
                Duration_info.Text = duration.Substring(0, duration.IndexOf("."));


                //Console.WriteLine(GetMetaData(axWindowsMediaPlayer1.currentMedia.sourceURL, "QuickTime Movie Header", "Preferred Rate"));

                
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.play();
            Console.WriteLine(axWindowsMediaPlayer1.status);
            timer1.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.pause();
            timer1.Enabled = false;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (trackBar1.Value < trackBar1.Maximum)
            {
                trackBar1.Value = (int)axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
                label1.Text = axWindowsMediaPlayer1.Ctlcontrols.currentPositionString;
            }
            else if ((int)axWindowsMediaPlayer1.Ctlcontrols.currentPosition == (int)trackBar1.Maximum)
            {
                timer1.Enabled = false;
                axWindowsMediaPlayer1.Ctlcontrols.pause();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = trackBar1.Value;
            label1.Text = axWindowsMediaPlayer1.Ctlcontrols.currentPositionString;
        }

        private int get_duration()
        {
            FFProbe ffprobe = new FFProbe();
            ffprobe.IncludeStreams = true;
            ffprobe.IncludeFormat = true;
            var vidinfo = ffprobe.GetMediaInfo(axWindowsMediaPlayer1.URL);

            Console.WriteLine(vidinfo.Streams.ToString());
            double vidDuration = Math.Floor(vidinfo.Duration.TotalSeconds);
            return (int)vidDuration;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            vidinfo vidinfo = new vidinfo();
            int fps = vidinfo.get_fps(axWindowsMediaPlayer1.URL);
            Console.WriteLine(fps);
            RunCmd("ffmpeg.exe", $" -vsync 2 -ss {axWindowsMediaPlayer1.Ctlcontrols.currentPosition} -t {axWindowsMediaPlayer1.Ctlcontrols.currentPosition + 1} -i {axWindowsMediaPlayer1.URL} -an -vf thumbnail=20 C:/VidEdit/{axWindowsMediaPlayer1.currentMedia.name}_{axWindowsMediaPlayer1.Ctlcontrols.currentPosition}.png");
        }

        private static string RunCmd(string exe, string args)

        {
            ProcessStartInfo process = new ProcessStartInfo();

            Process pro = new Process();

            process.FileName = exe; // 환경 변수 사용시 ffmpeg.exe로 호출 가능

            process.CreateNoWindow = true;  // cmd창을 띄우지 안도록 하기

            process.UseShellExecute = false;

            //process.RedirectStandardOutput = true; // cmd창에서 데이터를 가져오기

            process.RedirectStandardInput = true;  // cmd창으로 데이터 보내기

            process.RedirectStandardError = true;  // cmd창에서 오류 내용 가져오기

            pro.EnableRaisingEvents = true;

            process.Arguments = args;

            process.StandardErrorEncoding = Encoding.UTF8;

            pro.StartInfo = process;

            pro.Start();

            //pro.StandardInput.WriteLine(args);

            pro.StandardInput.Close();

            string result = pro.StandardError.ReadToEnd().ToLower();

            pro.WaitForExit();

            pro.StandardError.Close();

            return result;
        }

        public String get_Vidinfo(String data,  MediaInfo vidinfo)
        {
            


            var rawxml = vidinfo.Result.CreateNavigator().OuterXml;
            Console.WriteLine(rawxml.GetType());
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(rawxml);

            XmlNode ctg_node = xml.SelectSingleNode("ffprobe/streams/stream");

            string ctg_id = ctg_node.Attributes[$"{data}"].Value;
            return (String)ctg_id;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                if (trackBar2.Value == trackBar3.Value && checkBox1.Checked == true)
                {
                    label8.Text = "시작시간과 끝 시간은 같을 수 없습니다.";
                }
                else if (trackBar2.Value > trackBar3.Value && checkBox1.Checked)
                {
                    label8.Text = "시작 시간은 끝 시간보다 작아야 합니다.";
                }
                else
                {
                    Console.WriteLine("aaa");
                    RunCmd("ffmpeg.exe", $"-i {axWindowsMediaPlayer1.currentMedia.sourceURL} -ss {trackBar2.Value} -to {trackBar3.Value} -r {textBox7.Text} -s {textBox3.Text}x{textBox4.Text} -c copy C:/VidEdit/{axWindowsMediaPlayer1.currentMedia.name}_{trackBar2.Value}~{trackBar3.Value}.{comboBox1.SelectedItem.ToString()}");
                    label8.Text = "완료!";
                }
                
            }
            else
            {
                Console.WriteLine("bbb");
                RunCmd("ffmpeg.exe", $"-i {axWindowsMediaPlayer1.currentMedia.sourceURL} -r {textBox7.Text} -s {textBox3.Text}x{textBox4.Text} -c:a copy C:/VidEdit/{axWindowsMediaPlayer1.currentMedia.name}_{textBox8.Text}.{comboBox1.SelectedItem.ToString()}");
                label8.Text = "완료!";
                Console.WriteLine($"-i {axWindowsMediaPlayer1.currentMedia.sourceURL} -r {textBox7.Text} -s scale={textBox3.Text}:{textBox4.Text} -c copy C:/VidEdit/{axWindowsMediaPlayer1.currentMedia.name}_{textBox8.Text}.{comboBox1.SelectedItem.ToString()}");
            }

            
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            int min = (int)trackBar2.Value / 60;
            int sec = (int)trackBar2.Value % 60;
            textBox1.Text = $"{min}";
            textBox2.Text = $"{sec}";
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = (min * 60) + sec;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            int min = (int)trackBar3.Value / 60;
            int sec = (int)trackBar3.Value % 60;
            textBox6.Text = $"{min}";
            textBox5.Text = $"{sec}";
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = (min * 60) + sec;
        }

        private string GetMetaData(string filePath, string directoryName, string tagName)
        {
            IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(filePath);
            MetadataExtractor.Directory directory = directories.Where(s => string.Equals(s.Name, directoryName)).FirstOrDefault();


            if (directory == null)
            {
                return string.Empty;
            }

            MetadataExtractor.Tag tag = directory.Tags.Where(s => string.Equals(s.Name, tagName)).FirstOrDefault();

            if (tag == null)
            {
                return string.Empty;
            }

            return tag.Description;
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void Format_info_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void axWindowsMediaPlayer1_PositionChange(object sender, AxWMPLib._WMPOCXEvents_PositionChangeEvent e)
        {

        }

        private void Height_info_Click(object sender, EventArgs e)
        {

        }

        private void FileName_Label_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label2.Enabled = true;
                label3.Enabled = true;
                trackBar2.Enabled = true;
                trackBar3.Enabled = true;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox5.Enabled = true;
                textBox6.Enabled = true;
            }
            else
            {
                label2.Enabled = false;
                label3.Enabled = false;
                trackBar2.Enabled = false;
                trackBar3.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox5.Enabled = false;
                textBox6.Enabled = false;
            }
        }
    }
}
