using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaptivBot;
using Xunit;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using rdg = RandomDataGenerator.TextData.Models;
using NLipsum;

namespace AdaptivBotTests
{
    public class FileUtilsTests : IDisposable
    {
        public FileUtilsTests()
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddFile(@"C:\fileSizeTestFile.txt",
                NLipsum.Core.Lipsums.TheRaven);
            FileUtils.fileSystem = mockFileSystem;
        }


        [Fact]
        public void FileSize()
        {
            Assert.Equal("6.00 KB", FileUtils.FileSize(@"C:\fileSizeTestFile.txt"));
        }


        public void Dispose()
        {
        }
    }
}
