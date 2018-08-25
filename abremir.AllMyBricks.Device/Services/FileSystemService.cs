﻿using abremir.AllMyBricks.Device.Configuration;
using abremir.AllMyBricks.Device.Interfaces;
using System.IO;
using Xamarin.Essentials.Interfaces;

namespace abremir.AllMyBricks.Device.Services
{
    public class FileSystemService : IFileSystemService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IFile _file;

        public FileSystemService(
            IFileSystem fileSystem,
            IFile file)
        {
            _fileSystem = fileSystem;
            _file = file;
        }

        public string ThumbnailCacheFolder => Path.Combine(_fileSystem.AppDataDirectory, Constants.AllMyBricksDataFolder, Constants.ThumbnailCacheFolder);

        public string GetLocalPathToFile(string filename, string subfolder = null)
        {
            return Path.Combine(_fileSystem.AppDataDirectory,
                Constants.AllMyBricksDataFolder,
                string.IsNullOrWhiteSpace(subfolder?.Trim()) ? string.Empty : subfolder.Trim(),
                (filename ?? string.Empty).Trim());
        }

        public string GetThumbnailFolder(string theme, string subtheme)
        {
            return Path.Combine(ThumbnailCacheFolder,
                string.IsNullOrWhiteSpace(theme?.Trim()) ? Constants.FallbackFolderName : theme.Trim(),
                string.IsNullOrWhiteSpace(subtheme?.Trim()) ? Constants.FallbackFolderName : subtheme.Trim());
        }

        public void SaveThumbnailToCache(string theme, string subtheme, string filename, byte[] thumbnail)
        {
            if(string.IsNullOrWhiteSpace(filename) || thumbnail == null || thumbnail.Length == 0)
            {
                return;
            }

            _file.WriteAllBytes(Path.Combine(GetThumbnailFolder(theme, subtheme), filename), thumbnail);
        }
    }
}