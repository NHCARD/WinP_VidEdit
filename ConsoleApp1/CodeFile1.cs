using System;
using OpenCvSharp;

class vidinfo
{
    int get_fps(String dir)
    {
        VideoCapture capture = new VideoCapture(dir);
        return (int)capture.Fps;
    }
}
