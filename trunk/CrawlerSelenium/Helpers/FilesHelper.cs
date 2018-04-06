// <copyright file="FilesHelper.cs" company="Objectivity Bespoke Software Specialists">
// Copyright (c) Objectivity Bespoke Software Specialists. All rights reserved.
// </copyright>
// <license>
//     The MIT License (MIT)
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// </license>

namespace CrawlerSelenium
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using NLog;

    using CrawlerSelenium;

    /// <summary>
    /// Class for handling downloading files <see href="https://github.com/ObjectivityLtd/Test.Automation/wiki/Downloading%20files">More details on wiki</see>
    /// </summary>
    public static class FilesHelper
    {
        /// <summary>
        /// Directory separator
        /// </summary>
        public static readonly char Separator = Path.DirectorySeparatorChar;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Returns the file extension.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// Files extension
        /// </returns>
        /// <example>How to use it: <code>
        /// FilesHelper.ReturnFileExtension(FileType.Html);
        /// </code></example>
        public static string ReturnFileExtension(FileType type)
        {
            switch (type)
            {
                case FileType.Pdf:
                    return ".pdf";
                case FileType.Xls:
                    return ".xls";
                case FileType.Csv:
                    return ".csv";
                case FileType.Txt:
                    return ".txt";
                case FileType.Doc:
                    return ".doc";
                case FileType.Xlsx:
                    return ".xlsx";
                case FileType.Docx:
                    return ".docx";
                case FileType.Gif:
                    return ".gif";
                case FileType.Jpg:
                    return ".jpg";
                case FileType.Bmp:
                    return ".bmp";
                case FileType.Png:
                    return ".png";
                case FileType.Xml:
                    return ".xml";
                case FileType.Html:
                    return ".html";
                case FileType.Ppt:
                    return ".ppt";
                case FileType.Pptx:
                    return ".pptx";
                default:
                    return "none";
            }
        }

        /// <summary>
        /// Gets the files of given type.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// Collection of files
        /// </returns>
        /// <example>How to use it: <code>
        /// var files = GetFilesOfGivenType(folder, FileType.Txt);
        /// </code></example>
        public static ICollection<FileInfo> GetFilesOfGivenType(string folder, FileType type)
        {
            return GetFilesOfGivenType(folder, type, string.Empty);
        }

        /// <summary>
        /// Gets the files of given type, use postfixFilesName in search pattern.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="type">The type of files.</param>
        /// <param name="postfixFilesName">Postfix name of files for search pattern.</param>
        /// <returns>
        /// Collection of files
        /// </returns>
        /// <example>How to use it: <code>
        /// var files = GetFilesOfGivenType(folder, FileType.Txt, "live");
        /// </code></example>
        public static ICollection<FileInfo> GetFilesOfGivenType(string folder, FileType type, string postfixFilesName)
        {
            //Logger.Debug("Get Files '{0}' from '{1}', postfixFilesName '{2}'", type, folder, postfixFilesName);
            ICollection<FileInfo> files =
                new DirectoryInfo(folder)
                    .GetFiles("*" + postfixFilesName + ReturnFileExtension(type)).OrderBy(f => f.Name).ToList();

            return files;
        }

        /// <summary>
        /// Gets the files of given type from all sub folders.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="type">The type of files.</param>
        /// <returns>
        /// Collection of files
        /// </returns>
        /// <example>How to use it: <code>
        /// var files = GetFilesOfGivenTypeFromAllSubFolders(folder, FileType.Txt);
        /// </code></example>
        public static ICollection<FileInfo> GetFilesOfGivenTypeFromAllSubFolders(string folder, FileType type)
        {
            return GetFilesOfGivenTypeFromAllSubFolders(folder, type, string.Empty);
        }

        /// <summary>
        /// Gets the files of given type from all sub folders, use postfixFilesName in search pattern.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="type">The type of files.</param>
        /// <param name="postfixFilesName">Postfix name of files for search pattern.</param>
        /// <returns>
        /// Collection of files
        /// </returns>
        /// <example>How to use it: <code>
        /// var files = GetFilesOfGivenTypeFromAllSubFolders(folder, FileType.Txt, "live");
        /// </code></example>
        public static ICollection<FileInfo> GetFilesOfGivenTypeFromAllSubFolders(string folder, FileType type, string postfixFilesName)
        {
            //Logger.Debug("Get Files '{0}' from '{1}', postfixFilesName '{2}'", type, folder, postfixFilesName);
            List<FileInfo> files =
                new DirectoryInfo(folder)
                    .GetFiles("*" + postfixFilesName + ReturnFileExtension(type), SearchOption.AllDirectories).OrderBy(f => f.Name).ToList();

            return files;
        }

        /// <summary>
        /// Gets all files from folder, use postfixFilesName in search pattern.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="postfixFilesName">Postfix name of files for search pattern.</param>
        /// <returns>
        /// Collection of files
        /// </returns>
        /// <example>How to use it: <code>
        /// var files = GetAllFiles(folder, "live");
        /// </code></example>
        public static ICollection<FileInfo> GetAllFiles(string folder, string postfixFilesName)
        {
            //Logger.Debug("Get all files from '{0}', postfixFilesName '{1}'", folder, postfixFilesName);
            ICollection<FileInfo> files =
                new DirectoryInfo(folder)
                    .GetFiles("*" + postfixFilesName).OrderBy(f => f.Name).ToList();

            return files;
        }

        /// <summary>
        /// Gets all files from all sub folders, use postfixFilesName in search pattern.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="postfixFilesName">Postfix name of files for search pattern.</param>
        /// <returns>
        /// Collection of files
        /// </returns>
        /// <example>How to use it: <code>
        /// var files = GetAllFilesFromAllSubFolders(folder, "live");
        /// </code></example>
        public static ICollection<FileInfo> GetAllFilesFromAllSubFolders(string folder, string postfixFilesName)
        {
            Logger.Debug("Get all files from '{0}', postfixFilesName '{1}'", folder, postfixFilesName);
            ICollection<FileInfo> files =
                new DirectoryInfo(folder)
                    .GetFiles("*" + postfixFilesName, SearchOption.AllDirectories).OrderBy(f => f.Name).ToList();

            return files;
        }

        /// <summary>
        /// Gets all files from all sub folders.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>
        /// Collection of files
        /// </returns>
        /// <example>How to use it: <code>
        /// var files = GetAllFilesFromAllSubFolders(folder);
        /// </code></example>
        public static ICollection<FileInfo> GetAllFilesFromAllSubFolders(string folder)
        {
            return GetAllFilesFromAllSubFolders(folder, string.Empty);
        }

        /// <summary>
        /// Gets all files from folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>
        /// Collection of files
        /// </returns>
        /// <example>How to use it: <code>
        /// var files = GetAllFiles(folder);
        /// </code></example>
        public static ICollection<FileInfo> GetAllFiles(string folder)
        {
            return GetAllFiles(folder, string.Empty);
        }

        /// <summary>
        /// Get file by its name in given folder
        /// </summary>
        /// <param name="folder">The folder</param>
        /// <param name="fileName">The file name</param>
        /// <returns>FileInfo of file</returns>
        public static FileInfo GetFileByName(string folder, string fileName)
        {
            Logger.Debug("Get File '{0}' from '{1}'", fileName, folder);
            FileInfo file =
                new DirectoryInfo(folder)
                    .GetFiles(fileName).First();

            return file;
        }

        /// <summary>
        /// Counts the files of given type.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// Number of files in subfolder
        /// </returns>
        /// <example>How to use it: <code>
        /// var filesNumber = FilesHelper.CountFiles(this.DriverContext.DownloadFolder, FileType.Txt);
        /// </code></example>
        public static int CountFiles(string folder, FileType type)
        {
            Logger.Debug(CultureInfo.CurrentCulture, "Count {0} Files in '{1}'", type, folder);
            var fileNumber = GetFilesOfGivenType(folder, type).Count;
            Logger.Debug(CultureInfo.CurrentCulture, "Number of files in '{0}': {1}", folder, fileNumber);
            return fileNumber;
        }

        /// <summary>
        /// Counts the files.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>
        /// Number of files in subfolder
        /// </returns>
        /// <example>How to use it: <code>
        /// var filesNumber = FilesHelper.CountFiles(this.DriverContext.DownloadFolder);
        /// </code></example>
        public static int CountFiles(string folder)
        {
            Logger.Debug(CultureInfo.CurrentCulture, "Count all Files in '{0}'", folder);
            var fileNumber = GetAllFiles(folder).Count;
            Logger.Debug(CultureInfo.CurrentCulture, "Number of files in '{0}': {1}", folder, fileNumber);
            return fileNumber;
        }

        /// <summary>
        /// Gets the last file of given type.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="type">The type of file.</param>
        /// <returns>Last file of given type</returns>
        /// <example>How to use it: <code>
        /// FilesHelper.GetLastFile(this.DriverContext.ScreenShotFolder, FileType.Png);
        /// </code></example>
        public static FileInfo GetLastFile(string folder, FileType type)
        {
            Logger.Debug(CultureInfo.CurrentCulture, "Get Last File of given type {0}", type);
            var lastFile =
                new DirectoryInfo(folder).GetFiles()
                    .Where(f => f.Extension == ReturnFileExtension(type))
                    .OrderByDescending(f => f.LastWriteTime)
                    .First();
            Logger.Trace("Last File: {0}", lastFile);
            return lastFile;
        }

        /// <summary>
        /// Gets the last file.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <returns>
        /// Last file of given type
        /// </returns>
        /// <example>How to use it: <code>
        /// FilesHelper.GetLastFile(this.DriverContext.ScreenShotFolder);
        /// </code></example>
        public static FileInfo GetLastFile(string folder)
        {
            Logger.Debug("Get Last File");
            var lastFile = new DirectoryInfo(folder).GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .First();
            Logger.Trace("Last File: {0}", lastFile);
            return lastFile;
        }




        /// <summary>
        /// Gets the folder from app.config as value of given key.
        /// </summary>
        /// <param name="appConfigValue">The application configuration value.</param>
        /// <param name="currentFolder">Directory where assembly files are located</param>
        /// <returns>
        /// The path to folder
        /// </returns>
        /// <example>How to use it: <code>
        ///   FilesHelper.GetFolder(BaseConfiguration.DownloadFolder, this.CurrentDirectory);
        /// </code></example>
        public static string GetFolder(string appConfigValue, string currentFolder)
        {
            Logger.Trace(CultureInfo.CurrentCulture, "appConfigValue '{0}', currentFolder '{1}', UseCurrentDirectory '{2}'", appConfigValue, currentFolder, BaseConfiguration.UseCurrentDirectory);
            string folder;

            if (string.IsNullOrEmpty(appConfigValue))
            {
                folder = currentFolder;
            }
            else
            {
                if (BaseConfiguration.UseCurrentDirectory)
                {
                    folder = currentFolder + appConfigValue;
                }
                else
                {
                    folder = appConfigValue;
                }

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }

            Logger.Trace(CultureInfo.CurrentCulture, "Returned folder '{0}'", folder);
            return folder;
        }
    }
}