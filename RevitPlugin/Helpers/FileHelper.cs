using System.IO;

namespace RevitPlugin.Helpers
{
    public class FileHelper
    {
        public static string GetUniqueFilePath(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            var extension = Path.GetExtension(filePath);

            int copyIndex = 1;
            string newFilePath = filePath;

            while (File.Exists(newFilePath))
            {
                newFilePath = Path.Combine(directory, $"{fileNameWithoutExtension} {copyIndex}{extension}");
                copyIndex++;
            }

            return newFilePath;
        }
    }
}
