using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;


namespace Optical_Flow
{
    class Program
    {
        static readonly string Version = "0.1.1";
        static void Main(string[] args)
        {

            Console.WriteLine("Welcome to the optic flow tool Version {0}, Please note this ONLY works with .bmp images", Version);
            Console.WriteLine("Please specify the filepath for the video");

            Console.WriteLine("Running");
            //Hard Code the URL just for funsies
            var video = @"D:\sample.mp4"; //TODO: Make dynamic.
            VideoToBitmap.RunOnVideo(video, 4, 8);
            Console.Read();
        }
    }
}
