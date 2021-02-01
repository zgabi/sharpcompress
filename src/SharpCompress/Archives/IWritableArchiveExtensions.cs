﻿using System;
using System.IO;
using System.Threading.Tasks;
using SharpCompress.Writers;

namespace SharpCompress.Archives
{
    public static class IWritableArchiveExtensions
    {
        public static void AddEntry(this IWritableArchive writableArchive,
                                                     string entryPath, string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("Could not AddEntry: " + filePath);
            }
            writableArchive.AddEntry(entryPath, new FileInfo(filePath).OpenRead(), true, fileInfo.Length,
                                     fileInfo.LastWriteTime);
        }

        public static Task SaveToAsync(this IWritableArchive writableArchive, string filePath, WriterOptions options)
        {
            return writableArchive.SaveToAsync(new FileInfo(filePath), options);
        }

        public static async Task SaveToAsync(this IWritableArchive writableArchive, FileInfo fileInfo, WriterOptions options)
        {
            await using var stream = fileInfo.Open(FileMode.Create, FileAccess.Write);
            await writableArchive.SaveToAsync(stream, options);
        }

        public static void AddAllFromDirectory(
            this IWritableArchive writableArchive,
            string filePath, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            using (writableArchive.PauseEntryRebuilding())
            {
                foreach (var path in Directory.EnumerateFiles(filePath, searchPattern, searchOption))
                {
                    var fileInfo = new FileInfo(path);
                    writableArchive.AddEntry(path.Substring(filePath.Length), fileInfo.OpenRead(), true, fileInfo.Length,
                                            fileInfo.LastWriteTime);
                }
            }
        }
        public static IArchiveEntry AddEntry(this IWritableArchive writableArchive, string key, FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                throw new ArgumentException("FileInfo does not exist.");
            }
            return writableArchive.AddEntry(key, fileInfo.OpenRead(), true, fileInfo.Length, fileInfo.LastWriteTime);
        }
    }
}