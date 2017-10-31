using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Accord.Video.FFMPEG;
using System.IO;

namespace Optical_Flow
{
    static class VideoToBitmap
    {
        public static BlockingCollection<FrameSet> framesToAnalyse = new BlockingCollection<FrameSet>(5); //The image to be analyzed and the file to write it too

        static VideoToBitmap()
        {
            
            Task.Run(() =>
            {
                OpticalFlowArray OpticFlowAnalysis = new OpticalFlowArray();
                while (!framesToAnalyse.IsCompleted)
                {
                    var frameSet = framesToAnalyse.Take();
                    var results = OpticFlowAnalysis.CalculateAverageOpticFlow(frameSet.frameOne, frameSet.frameTwo, frameSet.BoxSize);
                    frameSet.fileStream.WriteLine(results.CSVFormat);
                }
            });
        }

        //SLOW AVOID USING
        public static void RunOnVideo (string filePath, int desiredFramerate, int inputBoxSize) 
        {
            VideoFileReader reader = new VideoFileReader();
            reader.Open(filePath);
            
            int boxSize = inputBoxSize;

            //Printing to the console to provide metadata confirm
            Console.WriteLine("width:  " + reader.Width);
            Console.WriteLine("height: " + reader.Height);
            Console.WriteLine("fps:    " + (double)reader.FrameRate);
            Console.WriteLine("codec:  " + reader.CodecName);
            Console.WriteLine("number of frames:  " + reader.FrameCount);

            OpticalFlowArray OA = new OpticalFlowArray();
            //default implementation is a fifo box
            BlockingCollection<BlackAndWhiteDoubleArray> blackWhiteImages = new BlockingCollection<BlackAndWhiteDoubleArray>(5); //Magic object that does all the consumer producer bs for me
            BlackAndWhiteDoubleArray firstFrame, secondFrame;

            StreamWriter writer = new StreamWriter(@"D:\Results\results.csv");

            //Consumer Thread
            Task.Run(() =>
            {
                int count = 0;
                long ticks = 0;
                //run outside of the while loop to preload the first frame
                firstFrame = blackWhiteImages.Take();
                //runs until there are no more frames
                while (!blackWhiteImages.IsCompleted)
                {
                    secondFrame = blackWhiteImages.Take();
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    if (secondFrame != null && firstFrame != null)
                    {
                        var results = OA.CalculateAverageOpticFlow(firstFrame, secondFrame, boxSize);
                        writer.WriteLine(results.CSVFormat);
                    }
                    firstFrame = secondFrame;
                    stopwatch.Stop();
                    count++;
                    ticks += stopwatch.ElapsedTicks;
                }
                Console.WriteLine($"Consumer average elapsed ticks: {ticks / count}.");
            });

            //Producer Thread with framerate reductions
            Task.Run(() =>
            {
                var sw = new Stopwatch();
                var stopwatch = new Stopwatch();
                sw.Start();

                int count = 0;             //debug variable, delete in production code
                double frameToSkip = 1;    //the number of frames to skip
                double nextFrame = 0;      //the next frame to analyze, we always analyze the first frame
         
                if (desiredFramerate < reader.FrameRate) frameToSkip = (double)reader.FrameRate / desiredFramerate;


                //for some reason ~4 - 5 frames from the end I was getting out of index errors so I just said fuck it and quit 10 frames early
                for (int x = 0; x < reader.FrameCount - 10; x++)
                {
                    if ((int)nextFrame <= x) //we do <= to catch any potental double fuckery, shouldn't be a problem but BUGS
                    {
                        Console.WriteLine(x);
                        nextFrame += frameToSkip;
                        stopwatch.Start();
                        try
                        {
                            blackWhiteImages.Add(new BlackAndWhiteDoubleArray(reader.ReadVideoFrame()));
                        }
                        catch (NullReferenceException) //writes to the debug log the frame that caused problems
                        {
                            Console.WriteLine(x);
                            throw;
                        }
                        stopwatch.Stop();
                        count++;
                    }
                    else
                    {
                        stopwatch.Start();
                        reader.ReadVideoFrame().Dispose(); //we have to load this frame to skip it, this feels inefficient, but its the best I can do with this library
                        stopwatch.Stop();
                    }

                }
                blackWhiteImages.CompleteAdding();
                sw.Stop();
                Console.WriteLine($"Producer average elapsed ticks: {stopwatch.ElapsedTicks / count}.");
                Console.WriteLine($"Producer runtime: {sw.ElapsedMilliseconds}ms.");
            });
        }
    }
}
