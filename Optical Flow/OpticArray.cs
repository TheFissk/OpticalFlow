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

        public long Height { get; private set; }
        public long Width { get; private set; }
        private long topBuffer, leftBuffer, boxSize;
        private BlackAndWhiteDoubleArray firstFrame, secondFrame;

        public StatisticalBox CalculateAverageOpticFlow(BlackAndWhiteDoubleArray _firstFrame, BlackAndWhiteDoubleArray _secondFrame, long _boxSize)
        {
            //determine the size of the buffers on the outside of the frame and the number of iterations we need to go to

            firstFrame = _firstFrame;
            secondFrame = _secondFrame;
            boxSize = _boxSize;

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
                Width = firstFrame.width / boxSize -1;
            }
            else
            {
                leftBuffer = (firstFrame.width % boxSize) / 2;
                Width = firstFrame.width / boxSize;
            }

            List<Tuple<int, int>> arrs = new List<Tuple<int, int>>();

            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    arrs.Add(new Tuple<int, int>(h, w));
                }
            }

            ConcurrentBag<Vector2> opticFlow = new ConcurrentBag<Vector2>();

            OrderablePartitioner<Tuple<int, int>> op = Partitioner.Create(arrs, true);

            Parallel.ForEach(op,
                x => opticFlow.Add(findOpticVector(x.Item1 * boxSize + topBuffer, x.Item2 * boxSize + leftBuffer)));

            return new StatisticalBox(opticFlow.ToArray());
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

        private Vector2 findOpticVector(long top, long left)
        {
            //I used the Lucas–Kanade method and while I can't explain the math concisely in a comment. I recommend either checking the documentation or wikipedia.
            double sIx2 = 0, sIy2 = 0, sIxIy = 0, sIxIt = 0, sIyIt = 0;
            for (long x = left; x < left + boxSize; x++)
            {
                for (long y = top; y < top + boxSize; y++)
                {
                    double Ix = firstFrame.BrightnessArray[x, y] - firstFrame.BrightnessArray[x + 1, y];    //difference between this pixel and the one to the right
                    double Iy = firstFrame.BrightnessArray[x, y] - firstFrame.BrightnessArray[x, y + 1];    //difference between and the one below it
                    double It = firstFrame.BrightnessArray[x, y] - secondFrame.BrightnessArray[x, y];       //difference between this pixel and the same on the next frame
                    sIx2 += Math.Pow(Ix, 2);
                    sIy2 += Math.Pow(Iy,2);
                    sIxIy += Ix * Iy;
                    sIxIt += Ix * It;
                    sIyIt += Iy * It;
                }
            }
            double determinant = sIy2 * sIx2 - Math.Pow(sIxIy,2);
            if (determinant == 0) return new Vector2(0,0);
            double Vx = (sIxIy * sIyIt - sIy2 * sIxIt) / determinant;
            double Vy = (sIxIy * sIxIt - sIx2 * sIyIt) / determinant;
            return new Vector2(Vx, Vy);
        }
    }


    class StatisticalBox
    {
        public double Mean { get; set; }
        public double Variance { get; set; }
        public double Skewness { get; set; }
        public double Kurtosis { get; set; }

        private double sumSquared, sumCubed, sumQuad;

        public StatisticalBox(Vector2[] data)
        {
            CalculateMean(data);
            CalculateSums(data);
            Variance = sumSquared / (data.Length - 1);
            Skewness = sumCubed / (Math.Pow(Variance, 1.5f) * data.Length);
            Kurtosis = sumQuad / (Math.Pow(Variance, 1.5f) * data.Length);
        }

        private void CalculateMean(Vector2[] data)
        {
            double average = 0;
            foreach (Vector2 x in data) average += x.Magnitude;
            Mean =  average / data.Length;
        }

        private void CalculateSums(Vector2[] data)
        {
            sumSquared = sumQuad = sumCubed = 0;
            foreach (Vector2 x in data)
            {
                sumSquared = Math.Pow(x.Magnitude - Mean, 2);
                sumCubed = Math.Pow(x.Magnitude - Mean, 3);
                sumQuad = Math.Pow(x.Magnitude - Mean, 4);
            }
        }

        public string CSVFormat // In ({mean}, {Variance}, {skewness}, {kurtosis}) format
        {
            get
            {
                return String.Format("{0}, {1}, {2}, {3}", Mean, Variance, Skewness, Kurtosis);
            }
        }
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
