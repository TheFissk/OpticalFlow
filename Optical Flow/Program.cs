using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;


namespace Optical_Flow
{
    class Program
    {
        static readonly string Version = "0.2.2";
        [STAThread]
        static void Main(string[] args)
        {



            Console.WriteLine("Hello, press anything to select the folder with which the videos to analyze are contained");
            Console.Read();

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string filepath in fileDialog.FileNames)
                {
                    new FrameProducer(filepath, 10, 10, Path.ChangeExtension(filepath,"csv")).StartAsync();
                }
            }

            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
        }
    }
}
