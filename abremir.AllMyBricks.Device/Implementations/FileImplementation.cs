﻿using abremir.AllMyBricks.Device.Interfaces;
using abremir.AllMyBricks.Device.Models;
using System.IO;
using System.Threading.Tasks;

namespace abremir.AllMyBricks.Device.Implementations
{
    public class FileImplementation : IFile
    {
        public void Delete(string path)
        {
            File.Delete(path);
        }

        public void DeleteFileIfExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public FileAttributes GetAttributes(string path)
        {
            return File.GetAttributes(path);
        }

        public FileSize GetFileSize(string path)
        {
            var fileInfo = new FileInfo(path);

            if (fileInfo.Exists)
            {
                return new FileSize(fileInfo.Length);
            }

            return new FileSize();
        }

        public FileStream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        public FileStream OpenWrite(string path)
        {
            return File.OpenWrite(path);
        }

        public async Task WriteAllBytes(string path, byte[] bytes)
        {
            using(var stream = new FileStream(path, FileMode.Create))
            {
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
    }
}