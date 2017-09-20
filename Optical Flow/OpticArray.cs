using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Optical_Flow
{
    class OpticalFlowArray
    {
        public Vector2[,] opticflow;

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
            opticflow = new Vector2[Height, Width];


            Parallel.For(0, Height, yit =>
                Parallel.For(0, Width, xit =>
                    opticflow[yit, xit] = findOpticVector(yit * boxSize + topBuffer, xit * boxSize + leftBuffer)
                )
            );
            return CalculateStatistics();
        }

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
        #endregion

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

        private StatisticalBox CalculateStatistics()
        {
            StatisticalBox results = new StatisticalBox();
            results.mean = Mean();
            //calculate the mean first, then 3 descriptive statistics
            Parallel.Invoke(
            () => { results.Variance = Variance(results.mean); },
            () => { results.Kurtosis = Kurtosis(results.mean); },
            () => { results.Skewness = Skewness(results.mean); }
            );
            return results;
        }

        private double Mean()
        {
            double average = 0;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    average += opticflow[y, x].Magnitude;
                }
            }
            return average / opticflow.Length;
        }

        private double Variance(double mean)
        {
            double sumSquared = 0;
            foreach(Vector2 x in opticflow)
            {
                sumSquared += x.Magnitude * x.Magnitude;
            }
            return (sumSquared - (mean * mean * opticflow.Length * opticflow.Length) / opticflow.Length) / (opticflow.Length - 1);
        }

        private double Skewness(double mean)
        {
            double sumSquared = 0, sumCubed = 0;
            foreach (Vector2 x in opticflow)
            {
                sumSquared += Math.Pow(x.Magnitude - mean, 2);
                sumCubed += Math.Pow(x.Magnitude - mean, 3);
            }
            double sample = (opticflow.Length * Math.Sqrt(opticflow.Length - 1)) / (opticflow.Length - 2);
            return (sample * sumCubed) / Math.Sqrt(Math.Pow(sumSquared, 3));
        }
        
        private double Kurtosis(double mean)
        {
            double sumSquared = 0, sumQuad = 0;
            foreach (Vector2 x in opticflow)
            {
                sumSquared += Math.Pow(x.Magnitude - mean, 2);
                sumQuad += Math.Pow(x.Magnitude - mean, 4);
            }
            double sample = (opticflow.Length * (opticflow.Length - 1) * (opticflow.Length + 1)) / ((opticflow.Length - 2) * (opticflow.Length - 3));
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
        public double Magnitude
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
        }

        public Vector2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
