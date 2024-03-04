// See https://aka.ms/new-console-template for more information

using System;
using OpenCvSharp;

public class Test
{
    public int get_fps(string dir)
    {
        VideoCapture video = new VideoCapture(dir);
        return (int)video.Fps;
    }
}