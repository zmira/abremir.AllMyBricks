﻿using abremir.AllMyBricks.AssetManagement.Interfaces;
using abremir.AllMyBricks.Platform.Interfaces;
using Easy.MessageHub;
using SharpCompress.Common;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace abremir.AllMyBricks.AssetManagement.Implementations
{
    public class AssetUncompression : IAssetUncompression
    {
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IReaderFactory _readerFactory;
        private readonly IMessageHub _messageHub;

        public AssetUncompression(
            IFile file,
            IDirectory directory,
            IReaderFactory readerFactory,
            IMessageHub messageHub)
        {
            _file = file;
            _directory = directory;
            _readerFactory = readerFactory;
            _messageHub = messageHub;
        }

        public bool UncompressAsset(string sourceFilePath, string targetFolderPath, bool overwrite = true, string encryptionKey = null)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath)
                || !_file.Exists(sourceFilePath)
                || _file.GetAttributes(sourceFilePath).HasFlag(FileAttributes.Directory))
            {
                return false;
            }

            using var sourceFileStream = _file.OpenRead(sourceFilePath);

            return UncompressAsset(sourceFileStream, targetFolderPath, overwrite, encryptionKey);
        }

        public bool UncompressAsset(Stream sourceStream, string targetFolderPath, bool overwrite = true, string encryptionKey = null)
        {
            if (sourceStream is null
                || (!string.IsNullOrWhiteSpace(targetFolderPath)
                    && _directory.Exists(targetFolderPath)
                    && !_file.GetAttributes(targetFolderPath).HasFlag(FileAttributes.Directory)))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(targetFolderPath)
                && !_directory.Exists(targetFolderPath))
            {
                _directory.CreateDirectory(targetFolderPath);
            }

            using var workingStream = GetDecryptedStream(sourceStream, encryptionKey);
            using var sourceReader = _readerFactory.Open(workingStream);

            sourceReader.EntryExtractionProgress += SourceReader_EntryExtractionProgress;

            while (sourceReader.MoveToNextEntry())
            {
                if (!sourceReader.Entry.IsDirectory)
                {
                    var targetFilePath = Path.Combine(targetFolderPath ?? string.Empty, sourceReader.Entry.Key);

                    if (overwrite)
                    {
                        _file.DeleteFileIfExists(targetFilePath);
                    }

                    using var targetFileStream = _file.OpenWrite(targetFilePath);

                    sourceReader.WriteEntryTo(targetFileStream);
                }
            }

            return true;
        }

        private void SourceReader_EntryExtractionProgress(object sender, ReaderExtractionEventArgs<IEntry> entry)
        {
            _messageHub.Publish(entry);
        }

        private Stream GetDecryptedStream(Stream inputStream, string encryptionKey)
        {
            inputStream.Position = 0;

            if(encryptionKey is null)
            {
                return inputStream;
            }

            var hash = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(encryptionKey));
            var rijndael = new RijndaelManaged
            {
                Key = hash.Take(32).ToArray(),
                IV = hash.Take(16).ToArray()
            };

            using var outputStream = new MemoryStream();
            using var cryptoStreamDecryptor = new CryptoStream(inputStream, rijndael.CreateDecryptor(), CryptoStreamMode.Read);

            var byteArrayInput = new byte[inputStream.Length];

            cryptoStreamDecryptor.Read(byteArrayInput, 0, byteArrayInput.Length);
            outputStream.Write(byteArrayInput, 0, byteArrayInput.Length);

            outputStream.Flush();
            outputStream.Position = 0;

            return new MemoryStream(outputStream.ToArray());
        }
    }
}
