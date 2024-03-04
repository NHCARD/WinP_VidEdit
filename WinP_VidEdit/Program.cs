using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinP_VidEdit
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string folderPath = "C:/VidEdit";
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

            if (directoryInfo.Exists == false)
            {
                directoryInfo.Create();
            }
            Application.Run(new Form1());

        }
    }
}

class vidinfo
{
    public int get_fps(String dir)
    {
        Console.WriteLine(dir);
        VideoCapture capture = new VideoCapture(dir);
        return (int)capture.Fps;
    }

    
}