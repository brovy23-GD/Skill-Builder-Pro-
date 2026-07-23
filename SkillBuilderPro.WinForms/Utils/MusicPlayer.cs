using System;
using System.IO;
using System.Windows.Forms;
using NAudio.Wave;

namespace SkillBuilderPro.WinForms
{
    /// <summary>App-wide looping background music. Owned by Program, not any form.</summary>
    public static class MusicPlayer
    {
        private static IWavePlayer _device;
        private static AudioFileReader _reader;
        private static LoopStream _loop;

        private static float _volume = 0.35f;   // 0.0 – 1.0

        public static bool IsMuted { get; private set; }
        public static bool IsLoaded => _device != null;

        public static void Start(string fileName = "Theme.mp3")
        {
            string path = Path.Combine(
                Application.StartupPath, "Music", fileName);

            if (!File.Exists(path))
            {
                MessageBox.Show("NOT FOUND:\n" + path);   // ← temp
                return;
            }

            try
            {
                _reader = new AudioFileReader(path) { Volume = _volume };
                _loop = new LoopStream(_reader);
                _device = new WaveOutEvent();
                _device.Init(_loop);
                _device.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("AUDIO FAILED:\n" + ex.Message);   // ← temp
                Stop();
            }
        }

        public static void ToggleMute()
        {
            if (_reader == null) return;

            IsMuted = !IsMuted;
            _reader.Volume = IsMuted ? 0f : _volume;
        }

        public static void SetVolume(float v)
        {
            _volume = Math.Max(0f, Math.Min(1f, v));
            if (_reader != null && !IsMuted) _reader.Volume = _volume;
        }

        public static void Stop()
        {
            try { _device?.Stop(); } catch { }
            _device?.Dispose();
            _reader?.Dispose();
            _device = null;
            _reader = null;
            _loop = null;
        }
    }

    /// <summary>Wraps a stream so it repeats forever.</summary>
    internal class LoopStream : WaveStream
    {
        private readonly WaveStream _source;

        public LoopStream(WaveStream source) => _source = source;

        public override WaveFormat WaveFormat => _source.WaveFormat;
        public override long Length => _source.Length;

        public override long Position
        {
            get => _source.Position;
            set => _source.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int total = 0;
            while (total < count)
            {
                int read = _source.Read(buffer, offset + total, count - total);
                if (read == 0)
                {
                    if (_source.Position == 0) break;
                    _source.Position = 0;
                }
                total += read;
            }
            return total;
        }
    }
}