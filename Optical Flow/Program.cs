using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;


namespace Optical_Flow
{
    class Program
    {
        static readonly string Version = "0.2.2";
        [STAThread]
        static void Main(string[] args)
        {



            Console.WriteLine("Running");
            //Hard Code the filepath just for funsies
            var video = @"D:\sample.mp4"; //TODO: Make dynamic.
            VideoToBitmap.RunOnVideo(video, 4, 8);

            /*
            MainForm form = new MainForm();
            form.ShowDialog();
            */
            Console.Read();
            
        }
    }
}
