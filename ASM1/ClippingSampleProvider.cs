using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM1
{
    public class ClippingSampleProvider : ISampleProvider
    {
        private ISampleProvider sourceProvider;

        public ClippingSampleProvider(ISampleProvider sourceProvider)
        {
            this.sourceProvider= sourceProvider;
        }  

        public WaveFormat WaveFormat
        {
            get
            {
                return sourceProvider.WaveFormat;
            }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = sourceProvider.Read(buffer, offset, count);
            for(int n = 0; n < samplesRead; n++)
            {
                if (buffer[offset + n] > 1.0f)
                {
                    buffer[offset + n] = 1.0f;
                }
                else if (buffer[offset + n] < -1.0f)
                {
                    buffer[offset + n] = -1.0f;
                }
            }
            return samplesRead;
        }
    }
}
