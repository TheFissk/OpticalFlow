using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Optical_Flow
{
    class OpticalFlowArray
    {
        //public Vector2[,] opticflow;

        public int Height { get; private set; }
        public int Width { get; private set; }
        private int leftover, topBuffer, leftBuffer, boxSize;
        private BlackAndWhiteDoubleArray firstFrame, secondFrame;

        public StatisticalBox CalculateAverageOpticFlow(BlackAndWhiteDoubleArray _firstFrame, BlackAndWhiteDoubleArray _secondFrame, int _boxSize)
        {
            //determine the size of the buffers on the outside of the frame and the number of iterations we need to go to

            firstFrame = _firstFrame;
            secondFrame = _secondFrame;
            boxSize = _boxSize;

            if (firstFrame.height % boxSize == 0)
            {
                topBuffer = boxSize / 2;
                Height = firstFrame.height / boxSize -1;
            }
            else
            {
                topBuffer = (firstFrame.height % boxSize) / 2;
                Height = firstFrame.height / boxSize;
            }

            if (firstFrame.width % boxSize == 0)
            {
                leftBuffer = boxSize / 2;
                Width = firstFrame.width / boxSize -1;
            }
            else
            {
                leftBuffer = (firstFrame.width % boxSize) / 2;
                Width = firstFrame.width / boxSize;
            }
            //opticflow = new Vector2[Height, Width];

            //Item1 = Height, Item2 = Width
            List<Tuple<int, int>> arrs = new List<Tuple<int, int>>();
            //var opticFlow = new List<Vector2>();
            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    //opticFlow.Add(findOpticVector(h * boxSize + topBuffer, w * boxSize + leftBuffer));
                    arrs.Add(new Tuple<int, int>(h, w));
                }
            }
            /*Vector2[] opticFlow = arrs
                .AsParallel()
                .WithDegreeOfParallelism(512)
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .WithMergeOptions(ParallelMergeOptions.NotBuffered)
                .AsUnordered()
                .Select(
                    x => findOpticVector(x.Item1 * boxSize + topBuffer, x.Item2 * boxSize + leftBuffer))
                .ToArray();*/

            ConcurrentBag<Vector2> opticFlow = new ConcurrentBag<Vector2>();

            OrderablePartitioner<Tuple<int, int>> op = Partitioner.Create(arrs, true);

            Parallel.ForEach(op,
                x => opticFlow.Add(findOpticVector(x.Item1 * boxSize + topBuffer, x.Item2 * boxSize + leftBuffer)));

            /*Parallel.For(0, Height, yit =>
                Parallel.For(0, Width, xit =>
                    opticflow[yit, xit] = findOpticVector(yit * boxSize + topBuffer, xit * boxSize + leftBuffer)
                )
            );*///36ms
            return CalculateStatistics(opticFlow.ToArray());
        }

        /*
        // SLOW DO NOT USE
        #region threeThreadImplementation
        //rather than generating a new thread for every box, this one generates only 3 threads to be better optimized to run on a 4 thread CPU
        public StatisticalBox ThreeThreadOpticFlowAsync(BlackAndWhiteDoubleArray _firstFrame, BlackAndWhiteDoubleArray _secondFrame, int _boxSize)
        {

            firstFrame = _firstFrame;
            secondFrame = _secondFrame;
            boxSize = _boxSize;

            //determine the size of the buffers on the outside of the frame, and set the size of the output array
            if (firstFrame.height % boxSize == 0)
            {
                topBuffer = boxSize / 2;
                Height = firstFrame.height / boxSize - 1;
            }
            else
            {
                topBuffer = (firstFrame.height % boxSize) / 2;
                Height = firstFrame.height / boxSize;
            }

            if (firstFrame.width % boxSize == 0)
            {
                leftBuffer = boxSize / 2;
                Width = firstFrame.width / boxSize - 1;
            }
            else
            {
                leftBuffer = (firstFrame.width % boxSize) / 2;
                Width = firstFrame.width / boxSize;
            }
            opticflow = new Vector2[Height, Width];
            leftover = Height % 3;

            Parallel.Invoke(() => RunBlock(topBuffer, 0, Height / 3), () => RunBlock(topBuffer + (Height / 3) * boxSize, Height / 3, Height / 3), () => RunBlock(topBuffer + (Height / 3) * 2 * boxSize, (Height / 3) * 2, Height / 3 + leftover));

            return CalculateStatistics();
        }

        private void RunBlock(int pixelTop, int arrayTop, int iterations)
        {
            for (int inter = 0; inter < iterations; inter++)
            {
                for (int x = 0; x < Width; x++)
                {
                    opticflow[inter + arrayTop, x] = findOpticVector(pixelTop + inter * boxSize, x * boxSize + leftBuffer); // you need to account for the buffer on the X axis, but the buff on the top is accounted for in pixelTop
                }
            }
        }
        #endregion*/

        private Vector2 findOpticVector(int top, int left)
        {
            //I used the Lucas–Kanade method and while I can't explain the math concisely in a comment. I recommend either checking the documentation or wikipedia.
            double sIx2 = 0, sIy2 = 0, sIxIy = 0, sIxIt = 0, sIyIt = 0;
            for (int x = left; x < left + boxSize; x++)
            {
                for (int y = top; y < top + boxSize; y++)
                {
                    sIx2 += (firstFrame.BrightnessArray[x, y] - firstFrame.BrightnessArray[x + 1, y]) * (firstFrame.BrightnessArray[x, y] - firstFrame.BrightnessArray[x + 1, y]);
                    sIy2 += (firstFrame.BrightnessArray[x, y] - firstFrame.BrightnessArray[x, y + 1]) * (firstFrame.BrightnessArray[x, y] - firstFrame.BrightnessArray[x, y + 1]);
                    sIxIy += (firstFrame.BrightnessArray[x, y] - firstFrame.BrightnessArray[x, y + 1]) * (firstFrame.BrightnessArray[x, y] - firstFrame.BrightnessArray[x + 1, y]);
                    sIxIt += (firstFrame.BrightnessArray[x, y] - firstFrame.BrightnessArray[x + 1, y]) * (firstFrame.BrightnessArray[x, y] - secondFrame.BrightnessArray[x, y]);
                    sIyIt += (firstFrame.BrightnessArray[x, y] - firstFrame.BrightnessArray[x, y + 1]) * (firstFrame.BrightnessArray[x, y] - secondFrame.BrightnessArray[x, y]);
                }
            }
            double dividable = sIy2 * sIx2 - sIxIy * sIxIy;
            if (dividable == 0) return new Vector2(0,0);
            double Vx = sIy2 / (sIy2 * sIx2 - sIxIy * sIxIy) * (-sIxIt) + sIxIy / (sIy2 * sIx2 - sIxIy * sIxIy) * sIyIt;
            double Vy = sIxIy / (sIy2 * sIx2 - sIxIy * sIxIy) * sIxIt + sIx2 / (sIy2 * sIx2 - sIxIy * sIxIy) * (-sIyIt);
            return new Vector2(Vx, Vy);
        }

        private StatisticalBox CalculateStatistics(Vector2[] opticFlow)
        {
            StatisticalBox results = new StatisticalBox();
            results.mean = Mean(opticFlow);//4ms
            //calculate the mean first, then 3 descriptive statistics
            Parallel.Invoke(
            () => { results.Variance = Variance(results.mean, opticFlow); },
            () => { results.Kurtosis = Kurtosis(results.mean, opticFlow); },
            () => { results.Skewness = Skewness(results.mean, opticFlow); }
            );//6ms
            return results;
        }

        private double Mean(Vector2[] vectors)
        {
            double average = 0;
            for (int i = 0; i < vectors.Length; i++)
            {
                average += vectors[i].Magnitude;
            }
            return average / vectors.Length;
        }

        private double Variance(double mean, Vector2[] opticFlow)
        {
            double sumSquared = 0;
            foreach(Vector2 x in opticFlow)
            {
                sumSquared += x.Magnitude * x.Magnitude;
            }
            return (sumSquared - (mean * mean * opticFlow.Length * opticFlow.Length) / opticFlow.Length) / (opticFlow.Length - 1);
        }

        private double Skewness(double mean, Vector2[] opticFlow)
        {
            double sumSquared = 0, sumCubed = 0;
            foreach (Vector2 x in opticFlow)
            {
                sumSquared += Math.Pow(x.Magnitude - mean, 2);
                sumCubed += Math.Pow(x.Magnitude - mean, 3);
            }
            double sample = (opticFlow.Length * Math.Sqrt(opticFlow.Length - 1)) / (opticFlow.Length - 2);
            return (sample * sumCubed) / Math.Sqrt(Math.Pow(sumSquared, 3));
        }
        
        private double Kurtosis(double mean, Vector2[] opticFlow)
        {
            double sumSquared = 0, sumQuad = 0;
            foreach (Vector2 x in opticFlow)
            {
                sumSquared += Math.Pow(x.Magnitude - mean, 2);
                sumQuad += Math.Pow(x.Magnitude - mean, 4);
            }
            double sample = (opticFlow.Length * (opticFlow.Length - 1) * (opticFlow.Length + 1)) / ((opticFlow.Length - 2) * (opticFlow.Length - 3));
            return (sample * sumQuad) / Math.Pow(sumSquared, 2);
        }
    }


    class StatisticalBox
    {
        public double mean { get; set; }
        public double Variance { get; set; }
        public double Skewness { get; set; }
        public double Kurtosis { get; set; }
    }

    class Vector2
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Magnitude => Math.Sqrt(X * X + Y * Y);

        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
