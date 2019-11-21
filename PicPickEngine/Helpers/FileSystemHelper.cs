using System;
using System.IO;

namespace PicPick.Helpers
{
    public static class FileSystemHelper
    {
        /// <summary>
        /// Compares to files and returns true if the files are identical or false if the files are different.
        /// If the files have the same size, it will check the content, limitted to the optional argument byteCountCheck.
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <param name="byteCountCheck">How many bytes to compare. Default: 1000000 (1MB)</param>
        /// <returns>True if the files are identical. False if the files are different</returns>
        public static bool AreSameFiles(string file1, string file2, int byteCountCheck = 1000000)
        {
            try
            {
                int file1byte;
                int file2byte;
                FileStream fs1;
                FileStream fs2;

                // Open the two files.
                fs1 = new FileStream(file1, FileMode.Open);
                fs2 = new FileStream(file2, FileMode.Open);

                // Check the file sizes. If they are not the same, the files 
                // are not the same.
                if (fs1.Length != fs2.Length)
                {
                    // Close the file
                    fs1.Close();
                    fs2.Close();

                    // Return false to indicate files are different
                    return false;
                }

                // Read and compare a byte from each file until either a
                // non-matching set of bytes is found or until the end of
                // file1 is reached.
                int i = 0;
                do
                {
                    i++;
                    // Read one byte from each file.
                    file1byte = fs1.ReadByte();
                    file2byte = fs2.ReadByte();
                }
                while ((file1byte == file2byte) && (file1byte != -1) && (i < byteCountCheck));

                // Close the files.
                fs1.Close();
                fs2.Close();

                // Return the success of the comparison. "file1byte" is 
                // equal to "file2byte" at this point only if the files are 
                // the same.
                return ((file1byte - file2byte) == 0);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while comparing files", ex);
            }
        }

        public static string GetNewFileName(string path, string fileName)
        {
            int count = 2;

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            string newFileName;

            do
            {
                newFileName = $"{fileNameWithoutExtension} ({count++}).{extension}";
            }
            while (File.Exists(Path.Combine(path, newFileName)));

            return newFileName;
        }
    }
}
