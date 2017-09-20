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
            bool stay = true;
            string videoPath = null;

            Console.WriteLine("Welcome to the optic flow tool Version {0}, Please note this ONLY works with .bmp images", Version);
            Console.WriteLine("Please specify the filepath for the video");
            /*
            while (stay)
            {
                videoPath = Console.ReadLine();

                if (File.Exists(videoPath)) break;

                Console.WriteLine("File path invalid, {0} try again", videoPath);
            }
            */
            /*
            Console.WriteLine("Please specify the size of the box (too small will introduce error, to large takes to long to run) that is slightly larger than your subject");
            int boxSize = Int32.Parse(Console.ReadLine());
            */
            Console.WriteLine("Running");
            //Hard Code the URL just for funsies
            var video = @"C:\sample.mp4"; //TODO: Make dynamic.
            VideoToBitmap.RunOnVideo(video);
            Console.Read();
        }
    }
}
