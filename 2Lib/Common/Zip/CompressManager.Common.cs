using System;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;

namespace LJTH.HR.Common
{
    public partial class CompressManager
    {
        #region Common

        /// <summary>
        /// 
        /// </summary>
        /// <param name="existingArchive"></param>
        /// <param name="filePathInArchive"></param>
        /// <returns></returns>
        public static bool ExistItemInArchive(ZipFileInfo existingArchive, string filePathInArchive)
        {
            using (var zip = ZipFile.Read(existingArchive.Filename))
            {
                zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
                var entryQuery = zip.Entries.Where(o => o.FileName.Equals(filePathInArchive));
                if (!entryQuery.Any()) return false;
                return true;
            }
        }

        /// <summary>
        /// update archive file to use file or directory as item 
        /// </summary>
        /// <param name="existingArchive"></param>
        /// <param name="addFileOrDirectory"></param>
        /// <param name="directoryPathInArchive"></param>
        public static void UpdateArchiveItem(ZipFileInfo existingArchive,string addFileOrDirectory ,string directoryPathInArchive)
        {
            using (var zip = ZipFile.Read(existingArchive.Filename))
            {
                zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
                zip.UpdateItem(addFileOrDirectory, directoryPathInArchive);
                zip.Save();
            }
        }

        /// <summary>
        /// remove an entry form archive file
        /// </summary>
        /// <param name="existingZipFile"></param>
        /// <param name="filename"></param>
        public static void RemvoeEntry(string existingZipFile, string filename)
        {
            using (var zip = ZipFile.Read(existingZipFile))
            {
                zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
                var entryQuery = zip.Entries.Where(o => filename.Equals(Path.GetFileName(o.FileName)));
                if (!entryQuery.Any()) return;
                var zipEntry = entryQuery.FirstOrDefault();
                
                if (zipEntry == null || string.IsNullOrEmpty(zipEntry.FileName)) return;
                
                zip.RemoveEntry(zipEntry);
                zip.Save();
            }
        }

        /// <summary>
        /// rename a file from archive file
        /// </summary>
        /// <param name="existingZipFile"></param>
        /// <param name="newFilename"></param>
        /// <param name="oldFilename"></param>
        public static void RenameEntry(string existingZipFile, string newFilename, string oldFilename)
        {
            using (var zip = ZipFile.Read(existingZipFile))
            {
                zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
                var entryQuery = zip.Entries.Where(o => oldFilename.Equals(Path.GetFileName(o.FileName)));

                if (!entryQuery.Any()) return;
                var zipEntry = entryQuery.FirstOrDefault();
                if (zipEntry == null || string.IsNullOrEmpty(zipEntry.FileName)) return;
                zipEntry.FileName = Path.Combine(Path.GetDirectoryName(zipEntry.FileName), newFilename);
                zip.Save();
            }
        }

        /// <summary>
        /// Converts a string to a MemoryStream.
        /// </summary>
        public static MemoryStream StringToMemoryStream(string s)
        {
            var bytes = Encoding.Default.GetBytes(s);
            return new MemoryStream(bytes);
        }

        /// <summary>
        /// converts a MemorySteam to a string.
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static String MemoryStreamToString(MemoryStream ms)
        {
            var byteArray = ms.ToArray();
            return Encoding.Default.GetString(byteArray);
        }

        /// <summary>
        /// copy stream to dest stream
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void CopyStream(Stream src, Stream dest)
        {
            var buffer = new byte[1024];
            var len = src.Read(buffer, 0, buffer.Length);
            while (len > 0)
            {
                dest.Write(buffer, 0, len);
                len = src.Read(buffer, 0, buffer.Length);
            }
            dest.Flush();
        }
        #endregion
    }
}
