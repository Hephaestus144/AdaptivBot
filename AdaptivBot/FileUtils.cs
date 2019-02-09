using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptivBot
{
    public static class FileUtils
    {
        public static List<string> Read(string filePath, string readAfterThisLine = "")
        {
            var readLines = new List<string>();
            var retainCurrentLine = readAfterThisLine == "";
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (retainCurrentLine)
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

            if (readLines.Count == 0)
            {
                return null;
            }

            if (Path.GetExtension(filePath) == ".csv")
            {
  

                
            }
            else
            {
                var output = new object[readLines.Count, 1];
                for (var i = 0; i < readLines.Count; i++)
                {
                    output[i, 0] = readLines[i];
                }

        
            }
        }

    }
}
