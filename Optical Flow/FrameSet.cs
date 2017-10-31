using System.IO;

namespace Optical_Flow
{
    public class FrameSet
    {
        public BlackAndWhiteDoubleArray frameOne, frameTwo;
        public StreamWriter fileStream;
        public long BoxSize;

        public FrameSet(BlackAndWhiteDoubleArray frame1, BlackAndWhiteDoubleArray frame2, StreamWriter stream, long box)
        {
            frameOne = frame1;
            frameTwo = frame2;
            fileStream = stream;
            BoxSize = box;
        }
    }
}
