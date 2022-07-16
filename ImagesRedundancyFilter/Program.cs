using System;
using System.Drawing;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ImagesRedundancyFilter
{
    class Program
    {

        private static List<byte[]> target_Hashes = new List<byte[]>();   
        private static SHA256 Sha256 = SHA256.Create();
        static void Main(string[] args)
        {
            string baseFolder = @"Target Base Folder";


            string source = @"Images Source Folder";
            string target = Path.Combine(baseFolder, DateTime.Now.ToString("yyyy_MM_dd"));

            if (!Directory.Exists(baseFolder))
            {
                Directory.CreateDirectory(baseFolder);
            }

            CopyFolder(source, target);
        }
        /// <summary>
        ///  Copy Files But delete Redundant Files 
        /// </summary>
        /// <param name="source">Source Folder To Copy From</param>
        /// <param name="destinationBase"> Target Folder To Copy to</param>
        private static void CopyFolder(string source, string destinationBase)
        {
            CopyFolder(new DirectoryInfo(source), destinationBase);
            foreach (DirectoryInfo di in new DirectoryInfo(source).GetDirectories("*.*", SearchOption.AllDirectories))
            {
                CopyFolder(di, destinationBase);
            }
        }

        private static void CopyFolder(DirectoryInfo di, string destinationBase)
        {
            string destinationFolderName = Path.Combine(destinationBase, di.Name.Replace(":", ""));
            if (!Directory.Exists(destinationFolderName))
            {
                Directory.CreateDirectory(destinationFolderName);
            }
            foreach (FileInfo sFi in di.GetFiles())
            {
                byte[] new_Hashed_file_ToCopy = GetHashSha256(sFi.FullName);
                if(target_Hashes.Where(f => BytesToString(f) == BytesToString(new_Hashed_file_ToCopy)).Count() < 1){
                    sFi.CopyTo(Path.Combine(destinationFolderName, sFi.Name), false);
                    target_Hashes.Add(new_Hashed_file_ToCopy);
                }
            }
        }
        public static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }

        private static byte[] GetHashSha256(string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                return Sha256.ComputeHash(stream);
            }
        }

        #region Bad Pereformance Code
        private static bool CompareImages(string path1, string path2)
        {
            string img1_ref, img2_ref;
            bool flag = false;
            Bitmap img1 = new Bitmap(path1);
            Bitmap img2 = new Bitmap(path2);

            if (img1.Width == img2.Width && img1.Height == img2.Height)
            {
                for (int i = 0; i < img1.Width; i++)
                {
                    for (int j = 0; j < img1.Height; j++)
                    {
                        img1_ref = img1.GetPixel(i, j).ToString();
                        img2_ref = img2.GetPixel(i, j).ToString();
                        if (img1_ref != img2_ref)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            return true;
        }
        #endregion
    }
}
