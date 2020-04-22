using Accord.Video.FFMPEG;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Optical_Flow
{
    class FrameProducer
    {
        

        public long     CompletedFrames     { get; private set; } = 0;
        public long     TotalFrames         { get; private set; }
        public double   PercentCompletion   { get { return (double)CompletedFrames / TotalFrames; } }
        public bool     IsCompleted         { get; private set; } = false;

        private string  videoPath, resultsPath;
        private long    BoxSize, DesiredFramerate;

        private VideoFileReader reader = new VideoFileReader();
        private StreamWriter output;

        public FrameProducer(string path, long desiredFramerate, long boxSize, string resultsFile)
        {
            reader.Open(path);
            videoPath = path;
            BoxSize = boxSize;
            DesiredFramerate = desiredFramerate;
            TotalFrames = reader.FrameCount;
            resultsPath = resultsFile;
        }

        public async void StartAsync ()
        {
            await Task.Run(() =>
            {
                double frameToSkip = 1;    //the number of frames to skip, setting to 1 never skips a frame
                double nextFrame = 0;      //the next frame to analyze, we always analyze the first frame

                if (DesiredFramerate < reader.FrameRate) frameToSkip = (double)reader.FrameRate / DesiredFramerate;

                BlackAndWhiteDoubleArray firstFrame;
                var secondFrame = new BlackAndWhiteDoubleArray(reader.ReadVideoFrame()); //load the first frame outside of the loop as prep

                output = new StreamWriter(resultsPath);

                //for some reason ~4-5 frames from the end I was getting out of index errors so I just said fuck it and quit 10 frames early
                for (CompletedFrames = 1; CompletedFrames < reader.FrameCount - 10; CompletedFrames++)
                {
                    //if (CompletedFrames % 200 == 0) Console.WriteLine($"OPTS: {CompletedFrames}");  //write progress to console, not required, but nice for debug

                    if ((int)nextFrame <= CompletedFrames)                                          //we do <= to catch any potential double fuckery
                    {
                        nextFrame += frameToSkip;
                        try
                        {
                            firstFrame = secondFrame;
                            secondFrame = new BlackAndWhiteDoubleArray(reader.ReadVideoFrame());
                            var fs = new FrameSet(firstFrame, secondFrame, output, BoxSize);
                            VideoToBitmap.framesToAnalyse.Add(fs);
                        }
                        catch (NullReferenceException) //writes to the debug log the frame that caused problems but still throws the exception, I may want to catch this elsewhere
                        {
                            Console.WriteLine(CompletedFrames);
                            throw;
                        }
                        if (nextFrame > reader.FrameCount - 10) break; //if the next frame to analyse is outside of the 
                    }
                    else
                    {
                        reader.ReadVideoFrame().Dispose(); //reading and disposing of frames like this feels inefficient, but is more performant that putting the desired frame as a parameter
                    }
                }
                Console.WriteLine("Completed: " + videoPath);
            });
        }
    }
}
