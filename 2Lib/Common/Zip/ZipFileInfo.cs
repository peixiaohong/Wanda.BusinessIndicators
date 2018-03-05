using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Wanda.HR.Common
{
    public sealed class ZipFileInfo
    {          
        private CompressionLevel _level = CompressionLevel.Default;
        private EncryptionAlgorithm _encryption = EncryptionAlgorithm.None;
        private CompressionMethod _method = CompressionMethod.Deflate;
        /// <summary>
        /// filename
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// encrytion
        /// </summary>
        public EncryptionAlgorithm Encryption
        {
            get { return _encryption; }
            set { _encryption = value; }
        }

        /// <summary>
        /// level
        /// </summary>
        public CompressionLevel CompressionLevel
        {
            get { return _level; }
            set { _level = value; }
        }

        /// <summary>
        /// Comment 
        /// </summary>
        public string Comment { get; set; }
        
        /// <summary>
        /// CompressionMethod
        /// </summary>
        public CompressionMethod CompressionMethod
        {
            get { return _method; }
            set { _method = value; }
        }
    }
}
