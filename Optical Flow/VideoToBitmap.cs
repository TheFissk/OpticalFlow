using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video.FFMPEG;
using System.Collections.Concurrent;

namespace Optical_Flow
{
    static class VideoToBitmap
    {
        private static VideoFileReader reader;

        public static void RunOnVideo (string filePath)
        {
            reader = new VideoFileReader();
            reader.Open(filePath);

            int boxSize = 8; //for now this is a magic number, in the future it may be allowed to change

            //Just Some data used to ensure compatibility
            Console.WriteLine("width:  " + reader.Width);
            Console.WriteLine("height: " + reader.Height);
            Console.WriteLine("fps:    " + reader.FrameRate);
            Console.WriteLine("codec:  " + reader.CodecName);
            Console.WriteLine("number of frames:  " + reader.FrameCount);

            OpticalFlowArray OA = new OpticalFlowArray();
            //default implementation is a fifo box
            BlockingCollection<BlackAndWhiteDoubleArray> blackWhiteImages = new BlockingCollection<BlackAndWhiteDoubleArray>(5); //Magic object that does all the consumer producer bs for me
            BlackAndWhiteDoubleArray firstFrame, secondFrame;

            //Consumer Thread
            Task.Run(() =>
            {
                //run outside of the while loop to preload the first frame
                firstFrame = blackWhiteImages.Take();
                //runs until there are no more frames
                while (!blackWhiteImages.IsCompleted)
                {
                    secondFrame = blackWhiteImages.Take();
                    if (secondFrame != null && firstFrame != null)
                    {
                        //in the future this data will be saved. for now I will ignore it.
                        OA.CalculateAverageOpticFlow(firstFrame, secondFrame, boxSize);
                    }
                    firstFrame = secondFrame;
                }
            });

            //Producer Thread
            Task.Run(() =>
            {
                //for some reason ~4-5 frames from the end I was getting out of index errors so I just said fuck it and quit 10 frames early
                //Literature implementations of this code only ran at 4 FPS, in the future I may look into allowing the user to choose a lower framerate. for now I will analyse as is.
                for (int x = 0; x < reader.FrameCount - 10; x++)
                {
                    try
                    {
                        blackWhiteImages.Add(new BlackAndWhiteDoubleArray(reader.ReadVideoFrame()));
                    }
                    catch (NullReferenceException)
                    {
                        Console.WriteLine(x);
                        throw;
                    }
                    if (x % 200 == 0) Console.WriteLine(x);
                }
                blackWhiteImages.CompleteAdding();
            });
        }
    }
}
