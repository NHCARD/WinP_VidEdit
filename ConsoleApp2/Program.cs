using System;

using System.Diagnostics;

using System.Text;



namespace WinP_VidEdit

{

    class Program

    {

        static void Main(string[] args)

        {

            Console.WriteLine("Hello World!");



            //var test = RunCmd("C:\\Users\\SEONGHYEON\\Desktop\\ffmpeg-4.3.1-2020-10-01-full_build\\bin\\ffmpeg.exe -i \"C:\\Users\\SEONGHYEON\\Desktop\\녹음\\녹음_원본\\통화 녹음 01098981057_201016_102317.m4a\" \"C:\\Users\\SEONGHYEON\\Desktop\\녹음\\test\\test2.wav\"");

            var test = RunCmd(" -loglevel debug -y -i \"C:\\Users\\SEONGHYEON\\Desktop\\녹음\\녹음_원본\\통화 녹음 01098981057_201016_102317.m4a\" \"C:\\Users\\SEONGHYEON\\Desktop\\녹음\\test\\test2.wav\"");



            Console.WriteLine(test);

            Console.ReadLine();

        }



        private static string RunCmd(string args)

        {

            ProcessStartInfo process = new ProcessStartInfo();

            Process pro = new Process();



            process.FileName = "ffmpeg.exe"; // 환경 변수 사용시 ffmpeg.exe로 호출 가능



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

    }

}