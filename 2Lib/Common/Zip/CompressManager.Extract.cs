using System.IO;
using System.Linq;
using Ionic.Zip;
using Ionic.Zlib;

namespace Wanda.HR.Common
{
    public partial class CompressManager
    {
        #region Extract
        /// <summary>
        /// extract a archive file to directory
        /// </summary>
        /// <param name="archiveFile"></param>
        /// <param name="targetDirectory"></param>
        /// <param name="overwrite"></param>
        public static void Extract(string archiveFile, string targetDirectory, ExtractExistingFileAction overwrite)
        {
            using (var zip = ZipFile.Read(archiveFile))
            {
                foreach (var entry in zip)
                {
                    entry.Extract(targetDirectory,(Ionic.Zip.ExtractExistingFileAction)((int)overwrite));
                }
            }
        }


        public static void Extract(string archiveFile, string targetDirectory, ExtractExistingFileAction overwrite, string password)
        {
            using (var zip = ZipFile.Read(archiveFile))
            {
                foreach (var entry in zip)
                {
                    entry.ExtractWithPassword(targetDirectory, (Ionic.Zip.ExtractExistingFileAction)((int)overwrite), password);
                }
            }
        }

        /// <summary>
        /// extract a stream
        /// </summary>
        /// <param name="decompressed"></param>
        /// <returns></returns>
        public static Stream ExtractStream(Stream decompressed)
        {
            decompressed.Seek(0, SeekOrigin.Begin);
            var result = new MemoryStream();
            using (
                var zlibStream = new ZlibStream(result, CompressionMode.Decompress, true)
                )
            {
                CopyStream(decompressed, zlibStream);
                return result;
            }
        }

        public static string ExtractString(byte[] byteArray)
        {
            using (var decompressed = new MemoryStream(byteArray))
            {
                using (var stream = ExtractStream(decompressed))
                {
                    return MemoryStreamToString(stream as MemoryStream);
                }
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="existingZipFile"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Stream GetStreamFromArchive(string existingZipFile, string filename)
        {
            using (var zip = ZipFile.Read(existingZipFile))
            {                
                var result = new MemoryStream();
                var entryQuery = zip.Entries.Where(o => filename.Equals(Path.GetFileName(o.FileName)));
                if (!entryQuery.Any()) return Stream.Null;
                var zipEntry = entryQuery.FirstOrDefault();
                if (zipEntry == null) return Stream.Null;
                zipEntry.Extract(result);                
                return result;
            }
        }

        public static Stream GetStreamFromArchive(string existingZipFile, string filename,string password)
        {
            using (var zip = ZipFile.Read(existingZipFile))
            {
                var result = new MemoryStream();
                var entryQuery = zip.Entries.Where(o => filename.Equals(Path.GetFileName(o.FileName)));
                if (!entryQuery.Any()) return Stream.Null;
                var zipEntry = entryQuery.FirstOrDefault();
                if (zipEntry == null) return Stream.Null;
                zipEntry.ExtractWithPassword(result,password);
                return result;
            }
        }

        #endregion
    }
}
