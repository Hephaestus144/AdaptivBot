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
        public static object[,] Read(string filePath, string readAfterThisLine = "")
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

            if (readLines.Count == 0)
            {
                return null;
            }

            if (Path.GetExtension(filePath) == ".csv")
            {
                var outputLists = readLines.Select(x => x.Split(',')).ToList();
                var maxListLength = outputLists.Select(x => x.Length).Max();
                var output = new object[outputLists.Count, maxListLength];

                for (var i = 0; i < outputLists.Count; i++)
                {
                    for (var j = 0; j < maxListLength; j++)
                    {
                        output[i, j] = j < outputLists[i].Length
                            ? outputLists[i][j]
                            : "";
                    }
                }

                return output;
            }
            else
            {
                var output = new object[readLines.Count, 1];
                for (var i = 0; i < readLines.Count; i++)
                {
                    output[i, 0] = readLines[i];
                }

                return output;
            }
        }

    }
}
