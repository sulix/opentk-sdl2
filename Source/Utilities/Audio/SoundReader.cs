﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OpenTK.Audio
{
    /// <summary>
    /// Encapsulates a sound stream and provides decoding and streaming capabilities.
    /// </summary>
    /// <typeparam name="SampleType"></typeparam>
    public class SoundReader : IDisposable
    {
        static object reader_lock = new object();
        static List<SoundReader> readers = new List<SoundReader>();

        bool disposed;
        Stream stream;
        SoundReader implementation;

        #region --- Constructors ---

        #region static SoundReader()

        static SoundReader()
        {
            // TODO: Plugin architecture for sound formats. This is overkill now that we only have a WaveLoader (future proofing).
            readers.Add(new WaveReader());
        }

        #endregion

        #region protected SoundReader()

        protected SoundReader() { }

        #endregion

        #region public SoundReader(string filename)

        /// <summary>Creates a new SoundReader that can read the specified sound file.</summary>
        /// <param name="filename">The path to the sound file.</param>
        /// <returns>A new OpenTK.Audio.SoundReader, which can be used to read from the specified sound file.</returns>
        public SoundReader(string filename)
            : this(new FileStream(filename, FileMode.Open))
        { }

        #endregion

        #region public SoundReader(Stream s)

        /// <summary>Creates a new SoundReader that can read the specified soundstream.</summary>
        /// <param name="s">The System.IO.Stream to read from.</param>
        /// <returns>A new OpenTK.Audio.SoundReader, which can be used to read from the specified sound stream.</returns>
        public SoundReader(Stream s)
        {
            try
            {
                lock (reader_lock)
                {
                    foreach (SoundReader reader in readers)
                    {
                        long pos = s.Position;
                        if (reader.Supports(s))
                        {
                            s.Position = pos;
                            implementation = (SoundReader)
                                reader.GetType().GetConstructor(
                                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public |
                                    System.Reflection.BindingFlags.Instance,
                                    null,
                                    new Type[] { typeof(Stream) },
                                    null)
                                .Invoke(new object[] { s });
                            return;
                        }
                        s.Position = pos;
                    }
                }
            }
            catch (Exception e)
            {
                s.Close();
                throw;
            }
            throw new NotSupportedException("Could not find a decoder for the specified sound stream.");
        }

        #endregion

        #endregion

        #region --- Public Members ---

        #region public virtual bool Supports(Stream s)

        /// <summary>When overriden in a derived class, checks if the decoder supports the specified sound stream.</summary>
        /// <param name="s">The System.IO.Stream to check.</param>
        /// <returns>True if the sound stream is supported; false otherwise.</returns>
        public virtual bool Supports(Stream s)
        {
            if (implementation != null)
                return implementation.Supports(s);
            throw new NotImplementedException();
        }

        #endregion

        #region public virtual SoundData<SampleType> ReadSamples(long count)

        /// <summary>
        /// When overriden in a derived class, reads and decodes the specified number of samples from the sound stream.
        /// </summary>
        /// <param name="samples">The number of samples to read and decode.</param>
        /// <returns>An OpenTK.Audio.SoundData object that contains the decoded buffer.</returns>
        public virtual SoundData ReadSamples(long count)
        {
            if (implementation != null)
                return implementation.ReadSamples(count);
            throw new NotImplementedException();
        }

        #endregion

        #region public virtual SoundData<SampleType> ReadToEnd()

        /// <summary>
        /// When overriden in a derived class, reads and decodes the sound stream.
        /// </summary>
        /// <returns>An OpenTK.Audio.SoundData object that contains the decoded buffer.</returns>
        public virtual SoundData ReadToEnd()
        {
            if (implementation != null)
                return implementation.ReadToEnd();
            throw new NotImplementedException();
        }

        #endregion

        #region public virtual int Frequency

        /// <summary>
        /// 
        /// </summary>
        public virtual int Frequency
        {
            get
            {
                if (implementation != null)
                    return implementation.Frequency;
                else
                    throw new NotImplementedException();
            }
            protected set
            {
                if (implementation != null)
                    implementation.Frequency = value;
                else
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region public bool EndOfFile

        public bool EndOfFile
        {
            get { return stream.Position >= stream.Length; }
        }

        #endregion

        #endregion

        #region --- Protected Members ---

        protected virtual Stream Stream
        {
            get { return stream; }
            set { stream = value; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>Closes the underlying Stream and disposes of the SoundReader resources.</summary>
        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                    if (this.Stream != null)
                        this.Stream.Close();

                disposed = true;
            }
        }

        ~SoundReader()
        {
            this.Dispose(false);
        }

        #endregion
    }
}
