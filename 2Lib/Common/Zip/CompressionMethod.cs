using Ionic.Zip;

namespace Wanda.HR.Common
{
    public enum EncryptionAlgorithm
    {
        /// <summary>
        /// No encryption at all.
        /// </summary>
        None = 0,

        /// <summary>
        /// Traditional or Classic pkzip encryption.
        /// </summary>
        PkzipWeak,
    }

    public enum CompressionMethod
    {
        /// <summary>
        /// No compression at all. For COM environments, the value is 0 (zero).
        /// </summary>
        None = 0,

        /// <summary>
        ///   DEFLATE compression, as described in <see
        ///   href="http://www.ietf.org/rfc/rfc1951.txt">IETF RFC
        ///   1951</see>.  This is the "normal" compression used in zip
        ///   files. For COM environments, the value is 8.
        /// </summary>
        Deflate = 8,
    }

    public enum CompressionLevel
    {
        /// <summary>
        /// None means that the data will be simply stored, with no change at all.
        /// If you are producing ZIPs for use on Mac OSX, be aware that archives produced with CompressionLevel.None
        /// cannot be opened with the default zip reader. Use a different CompressionLevel.
        /// </summary>
        None = 0,
        /// <summary>
        /// Same as None.
        /// </summary>
        Level0 = 0,

        /// <summary>
        /// The fastest but least effective compression.
        /// </summary>
        BestSpeed = 1,

        /// <summary>
        /// A synonym for BestSpeed.
        /// </summary>
        Level1 = 1,

        /// <summary>
        /// A little slower, but better, than level 1.
        /// </summary>
        Level2 = 2,

        /// <summary>
        /// A little slower, but better, than level 2.
        /// </summary>
        Level3 = 3,

        /// <summary>
        /// A little slower, but better, than level 3.
        /// </summary>
        Level4 = 4,

        /// <summary>
        /// A little slower than level 4, but with better compression.
        /// </summary>
        Level5 = 5,

        /// <summary>
        /// The default compression level, with a good balance of speed and compression efficiency.
        /// </summary>
        Default = 6,
        /// <summary>
        /// A synonym for Default.
        /// </summary>
        Level6 = 6,

        /// <summary>
        /// Pretty good compression!
        /// </summary>
        Level7 = 7,

        /// <summary>
        ///  Better compression than Level7!
        /// </summary>
        Level8 = 8,

        /// <summary>
        /// The "best" compression, where best means greatest reduction in size of the input data stream.
        /// This is also the slowest compression.
        /// </summary>
        BestCompression = 9,

        /// <summary>
        /// A synonym for BestCompression.
        /// </summary>
        Level9 = 9,
    }
    public enum ExtractExistingFileAction
    {
        /// <summary>
        /// Throw an exception when extraction would overwrite an existing file. (For
        /// COM clients, this is a 0 (zero).)
        /// </summary>
        Throw,

        /// <summary>
        /// When extraction would overwrite an existing file, overwrite the file silently.
        /// The overwrite will happen even if the target file is marked as read-only.
        /// (For COM clients, this is a 1.)
        /// </summary>
        OverwriteSilently,

        /// <summary>
        /// When extraction would overwrite an existing file, don't overwrite the file, silently. 
        /// (For COM clients, this is a 2.)
        /// </summary>
        DoNotOverwrite,

        /// <summary>
        /// When extraction would overwrite an existing file, invoke the ExtractProgress
        /// event, using an event type of <see
        /// cref="ZipProgressEventType.Extracting_ExtractEntryWouldOverwrite"/>.  In
        /// this way, the application can decide, just-in-time, whether to overwrite the
        /// file. For example, a GUI application may wish to pop up a dialog to allow
        /// the user to choose. You may want to examine the <see
        /// cref="ExtractProgressEventArgs.ExtractLocation"/> property before making
        /// the decision. If, after your processing in the Extract progress event, you
        /// want to NOT extract the file, set <see cref="ZipEntry.ExtractExistingFile"/>
        /// on the <c>ZipProgressEventArgs.CurrentEntry</c> to <c>DoNotOverwrite</c>.
        /// If you do want to extract the file, set <c>ZipEntry.ExtractExistingFile</c>
        /// to <c>OverwriteSilently</c>.  If you want to cancel the Extraction, set
        /// <c>ZipProgressEventArgs.Cancel</c> to true.  Cancelling differs from using
        /// DoNotOverwrite in that a cancel will not extract any further entries, if
        /// there are any.  (For COM clients, the value of this enum is a 3.)
        /// </summary>
        InvokeExtractProgressEvent,
    }
}
