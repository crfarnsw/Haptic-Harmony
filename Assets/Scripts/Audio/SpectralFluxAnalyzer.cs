using DSPLib;
using System.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Collections;

public class SpectralFluxAnalyzer
{
    public int numSamples = 1024;

    // Sensitivity multiplier to scale the average threshold.
    // In this case, if a rectified spectral flux sample is > 1.5 times the average, it is a peak
    public float thresholdMultiplier = 1.5f;
    //public float thresholdMultiplier = 1.5f;
    //public float thresholdMultiplier = 2.5f;
    //public float thresholdMultiplier = 3f;

    // Number of samples to average in our window
    public int thresholdWindowSize = 50;

    private int numChannels;
    private int numTotalSamples;
    private int sampleRate;
    private float clipLength;
    private float[] multiChannelSamples;
    private double[] sampleChunk;
    public List<SpectralFluxInfo> SpectralFluxSamples;

    float[] curSpectrum;
    float[] prevSpectrum;

    int indexToProcess;

    public SpectralFluxAnalyzer()
    {
        SpectralFluxSamples = new List<SpectralFluxInfo>();

        // Start processing from middle of first window and increment by 1 from there
        indexToProcess = thresholdWindowSize / 2;

        curSpectrum = new float[numSamples];
        prevSpectrum = new float[numSamples];
    }

    public SpectralFluxAnalyzer(int numChannels, int numTotalSamples, int sampleRate, float clipLength, float[] multiChannelSamples)
    {
        SpectralFluxSamples = new List<SpectralFluxInfo>();
        thresholdMultiplier = GameManager.instance.difficulty;

        this.numChannels = numChannels;
        this.numTotalSamples = numTotalSamples;
        this.sampleRate = sampleRate;
        this.clipLength = clipLength;
        this.multiChannelSamples = multiChannelSamples;

        // Start processing from middle of first window and increment by 1 from there
        indexToProcess = thresholdWindowSize / 2;

        curSpectrum = new float[numSamples];
        prevSpectrum = new float[numSamples];
    }

    public void SetCurSpectrum(float[] spectrum)
    {
        curSpectrum.CopyTo(prevSpectrum, 0);
        spectrum.CopyTo(curSpectrum, 0);
    }

    public void GetFullSpectrumThreaded()
    {
        // We only need to retain the samples for combined channels over the time domain
        float[] preProcessedSamples = new float[numTotalSamples];

        int numProcessed = 0;
        float combinedChannelAverage = 0f;

        for (int i = 0; i < multiChannelSamples.Length; i++)
        {
            combinedChannelAverage += multiChannelSamples[i];

            // Each time we have processed all channels samples for a point in time, we will store the average of the channels combined
            if ((i + 1) % numChannels == 0)
            {
                preProcessedSamples[numProcessed] = combinedChannelAverage / numChannels;
                numProcessed++;
                combinedChannelAverage = 0f;
            }
        }

        Debug.Log("Combine Channels done");
        Debug.Log(preProcessedSamples.Length);

        // Once we have our audio sample data prepared, we can execute an FFT to return the spectrum data over the time domain
        int spectrumSampleSize = 1024;
        int iterations = preProcessedSamples.Length / spectrumSampleSize;

        FFT fft = new FFT();
        fft.Initialize((UInt32)spectrumSampleSize);

        Debug.Log(string.Format("Processing {0} time domain samples for FFT", iterations));
        sampleChunk = new double[spectrumSampleSize];

        for (int i = 0; i < iterations; i++)
        {
            // Grab the current 1024 chunk of audio sample data
            Array.Copy(preProcessedSamples, i * spectrumSampleSize, sampleChunk, 0, spectrumSampleSize);

            // Apply our chosen FFT Window
            double[] windowCoefs = DSP.Window.Coefficients(DSP.Window.Type.Hanning, (uint)spectrumSampleSize);
            double[] scaledSpectrumChunk = DSP.Math.Multiply(sampleChunk, windowCoefs);
            double scaleFactor = DSP.Window.ScaleFactor.Signal(windowCoefs);

            // Perform the FFT and convert output (complex numbers) to Magnitude
            Complex[] fftSpectrum = fft.Execute(scaledSpectrumChunk);
            double[] scaledFFTSpectrum = DSPLib.DSP.ConvertComplex.ToMagnitude(fftSpectrum);
            scaledFFTSpectrum = DSP.Math.Multiply(scaledFFTSpectrum, scaleFactor);

            // These 1024 magnitude values correspond (roughly) to a single point in the audio timeline
            float curSongTime = GetTimeFromIndex(i) * spectrumSampleSize;

            // Send our magnitude data off to our Spectral Flux Analyzer to be analyzed for peaks
            AnalyzeSpectrum(Array.ConvertAll(scaledFFTSpectrum, x => (float)x), curSongTime);
        }

        Debug.Log("Spectrum Analysis done");
        Debug.Log("Background Thread Completed");
    }

    public void AnalyzeSpectrum(float[] spectrum, float time)
    {
        // Set spectrum
        SetCurSpectrum(spectrum);

        // Get current spectral flux from spectrum
        SpectralFluxInfo curInfo = new SpectralFluxInfo
        {
            time = time,
            spectralFlux = CalculateRectifiedSpectralFlux()
        };

        SpectralFluxSamples.Add(curInfo);

        // We have enough samples to detect a peak
        if (SpectralFluxSamples.Count >= thresholdWindowSize)
        {
            // Get Flux threshold of time window surrounding index to process
            SpectralFluxSamples[indexToProcess].threshold = GetFluxThreshold(indexToProcess);

            // Only keep amp amount above threshold to allow peak filtering
            SpectralFluxSamples[indexToProcess].prunedSpectralFlux = GetPrunedSpectralFlux(indexToProcess);

            // Now that we are processed at n, n-1 has neighbors (n-2, n) to determine peak
            int indexToDetectPeak = indexToProcess - 1;

            bool curPeak = IsPeak(indexToDetectPeak);

            if (curPeak)
            {
                SpectralFluxSamples[indexToDetectPeak].isPeak = true;
            }

            indexToProcess++;
        }
        else
        {
            Debug.Log(string.Format("Not ready yet.  At spectral flux sample size of {0} growing to {1}", SpectralFluxSamples.Count, thresholdWindowSize));
        }
    }
    
    float CalculateRectifiedSpectralFlux()
    {
        float sum = 0f;

        // Aggregate positive changes in spectrum data
        for (int i = 0; i < numSamples; i++)
        {
            sum += Mathf.Max(0f, curSpectrum[i] - prevSpectrum[i]);
        }

        return sum;
    }

    float GetFluxThreshold(int spectralFluxIndex)
    {
        // How many samples in the past and future we include in our average
        int windowStartIndex = Mathf.Max(0, spectralFluxIndex - thresholdWindowSize / 2);
        int windowEndIndex = Mathf.Min(SpectralFluxSamples.Count - 1, spectralFluxIndex + thresholdWindowSize / 2);

        // Add up our spectral flux over the window
        float sum = 0f;
        for (int i = windowStartIndex; i < windowEndIndex; i++)
        {
            sum += SpectralFluxSamples[i].spectralFlux;
        }

        // Return the average multiplied by our sensitivity multiplier
        float avg = sum / (windowEndIndex - windowStartIndex);
        return avg * thresholdMultiplier;
    }

    float GetPrunedSpectralFlux(int spectralFluxIndex)
    {
        return Mathf.Max(0f, SpectralFluxSamples[spectralFluxIndex].spectralFlux - SpectralFluxSamples[spectralFluxIndex].threshold);
    }

    public float GetTimeFromIndex(int index)
    {
        return ((1f / (float)sampleRate) * index);
    }

    bool IsPeak(int spectralFluxIndex)
    {
        if (SpectralFluxSamples[spectralFluxIndex].prunedSpectralFlux > SpectralFluxSamples[spectralFluxIndex + 1].prunedSpectralFlux &&
            SpectralFluxSamples[spectralFluxIndex].prunedSpectralFlux > SpectralFluxSamples[spectralFluxIndex - 1].prunedSpectralFlux)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void LogSample(int indexToLog)
    {
        int windowStart = Mathf.Max(0, indexToLog - thresholdWindowSize / 2);
        int windowEnd = Mathf.Min(SpectralFluxSamples.Count - 1, indexToLog + thresholdWindowSize / 2);

        Debug.Log(string.Format(
            "Peak detected at song time {0} with pruned flux of {1} ({2} over thresh of {3}).\n" +
            "Thresh calculated on time window of {4}-{5} ({6} seconds) containing {7} samples.",
            SpectralFluxSamples[indexToLog].time,
            SpectralFluxSamples[indexToLog].prunedSpectralFlux,
            SpectralFluxSamples[indexToLog].spectralFlux,
            SpectralFluxSamples[indexToLog].threshold,
            SpectralFluxSamples[windowStart].time,
            SpectralFluxSamples[windowEnd].time,
            SpectralFluxSamples[windowEnd].time - SpectralFluxSamples[windowStart].time,
            windowEnd - windowStart
        ));
    }
}

public class SpectralFluxInfo
{
    public float time;
    public float spectralFlux;
    public float threshold;
    public float prunedSpectralFlux;
    public bool isPeak;
}
