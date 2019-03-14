using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;


namespace AdaptivBot
{
    public static class FileUtils
    {
        public static IFileSystem fileSystem { get; set; } = new FileSystem();


        public static string FileSize(string filePath)
        {
            var fileInfo = fileSystem.FileInfo.FromFileName(filePath);
            return fileInfo.Length >= 1048576
                ? $"{fileInfo.Length / 1048576:n}" + " MB"
                : $"{fileInfo.Length / 1024:n}"    + " KB";
        }


        public static List<string> Read(string filePath, string readAfterThisLine = "")
        {
            var readLines = new List<string>();
            var retainCurrentLine = readAfterThisLine == "";
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (retainCurrentLine && !line.StartsWith(",,,,,,,"))
                    {
                        readLines.Add(line);
                    }
                    else
                    {
                        retainCurrentLine = line.StartsWith(readAfterThisLine);
                    }
                }
            }

            return readLines;
        }
    }
}
