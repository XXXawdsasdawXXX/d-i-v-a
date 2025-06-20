﻿using System.Collections.Generic;
using System.Linq;
using CSCore.DSP;

namespace Code.Infrastructure.LoopbackAudio
{
    internal class LineSpectrum : SpectrumBase
    {
        public int BarCount
        {
            get { return SpectrumResolution; }
            set { SpectrumResolution = value; }
        }

        public LineSpectrum(FftSize fftSize)
        {
            FftSize = fftSize;
        }

        public float[] GetSpectrumData(double maxValue)
        {
            // Get spectrum data internal
            float[] fftBuffer = new float[(int)FftSize];

            UpdateFrequencyMapping();

            if (SpectrumProvider.GetFftData(fftBuffer, this))
            {
                SpectrumPointData[] spectrumPoints = CalculateSpectrumPoints(maxValue, fftBuffer);

                // Convert to float[]
                List<float> spectrumData = new();
                spectrumPoints.ToList().ForEach(point => spectrumData.Add((float)point.Value));
                return spectrumData.ToArray();
            }

            return null;
        }
    }
}