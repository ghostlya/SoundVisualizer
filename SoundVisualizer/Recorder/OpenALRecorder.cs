﻿using OpenAL;
using System;
using System.Collections.Generic;
using System.Threading;
using SoundVisualizer.Audio;

namespace SoundVisualizer.Recorder
{
    public sealed class OpenALRecorder
    {

        private const int DefaultBufferSize = 8192;



        private readonly object syncObj = new object();
        private EventHandler<RecordedEventArgs> recorded;

        private bool disposed;
        private Timer systemTimer;
        private AudioCapture capture;
        private byte[] buffer;

        private AudioQuality quality;
        private int samplesSize;



        public event EventHandler<RecordedEventArgs> Recorded
        {
            
            add { recorded += value; }
            remove { recorded -= value; }
        }



       
        public OpenALRecorder(string deviceName = null)
        {
            if (string.IsNullOrEmpty(deviceName) || IsInited)
                return;

            Initialize(deviceName, new AudioQuality(2, 16, 44100));
        }



        public bool IsInited
        {
            get { return Interlocked.CompareExchange(ref capture, null, null) != null; }
        }

        public IList<string> Devices
        {
            get
            {
                try
                {
                    return AudioCapture.AvailableDevices;
                }
                catch (Exception)
                {
                    return new List<string>();
                }
            }
        }



      
        private void Initialize(string deviceName, AudioQuality quality)
        {
            try
            {
                this.quality = quality;
                this.samplesSize = DefaultBufferSize;

                ALFormat format;

                if (quality.Channels == 1)
                    format = quality.Bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                else
                    format = quality.Bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;

                lock (syncObj)
                {
                    buffer = new byte[quality.Channels * (quality.Bits / 8) * samplesSize * 2];

                    if (string.IsNullOrEmpty(deviceName))
                        deviceName = AudioCapture.DefaultDevice;

                    if (!AudioCapture.AvailableDevices.Contains(deviceName))
                        deviceName = AudioCapture.DefaultDevice;

                    capture = new AudioCapture(deviceName, quality.Frequency, format, samplesSize * 2);
                }
            }
            catch (Exception e)
            {
                if (capture != null)
                    capture.Dispose();

                capture = null;

            }
        }

        
        public void Start()
        {
            if (capture == null || capture.IsRunning)
                return;

            lock (syncObj)
            {
                capture.Start();
                systemTimer = new Timer(OnRecording, null, GetTimerTimeOut(), -1);
            }
        }

       
        public void SetOptions(string deviceName, AudioQuality quality)
        {
            if (IsInited)
            {
                Stop();
                capture.Dispose();
            }

            Initialize(deviceName, quality);
        }

       
        private void OnRecording(object state)
        {
            lock (syncObj)
            {
                if (capture == null || !capture.IsRunning)
                    return;

                int availableSamples = capture.AvailableSamples;

                if (availableSamples > 0)
                {
                    int availableDataSize = availableSamples * quality.Channels * (quality.Bits / 8);
                    if (availableDataSize > buffer.Length)
                        buffer = new byte[availableDataSize * 2];

                    capture.ReadSamples(buffer, availableSamples);

                    var temp = Interlocked.CompareExchange(ref recorded, null, null);
                    if (temp != null)
                        temp(this, new RecordedEventArgs(buffer, availableSamples, quality.Channels, quality.Bits, quality.Frequency));
                }

                if (systemTimer != null && capture.IsRunning)
                    systemTimer.Change(GetTimerTimeOut(), -1);
            }
        }

       
        public void Stop()
        {
            if (!IsInited)
                return;

            lock (syncObj)
            {
                capture.Stop();

                if (systemTimer != null)
                    systemTimer.Dispose();
                systemTimer = null;
            }
        }

       
        private int GetTimerTimeOut()
        {
            double timeToBufferFilled = samplesSize / (quality.Frequency * 1000d);
            return (int)(timeToBufferFilled / 2);
        }



        
        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
            recorded = null;

            lock (syncObj)
            {
                if (systemTimer != null)
                    systemTimer.Dispose();

                systemTimer = null;

                if (capture != null)
                {
                    capture.Stop();
                    capture.Dispose();
                }

                capture = null;
            }
        }

    }
}
