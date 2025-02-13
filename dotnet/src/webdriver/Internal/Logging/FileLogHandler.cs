// <copyright file="FileLogHandler.cs" company="Selenium Committers">
// Licensed to the Software Freedom Conservancy (SFC) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The SFC licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>

using System;
using System.IO;

#nullable enable

namespace OpenQA.Selenium.Internal.Logging
{
    /// <summary>
    /// Represents a log handler that writes log events to a file.
    /// </summary>
    public class FileLogHandler : ILogHandler, IDisposable
    {
        // performance trick to avoid expensive Enum.ToString() with fixed length
        private static readonly string[] _levels = { "TRACE", "DEBUG", " INFO", " WARN", "ERROR" };

        private FileStream _fileStream;
        private StreamWriter _streamWriter;

        private readonly object _lockObj = new object();
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogHandler"/> class with the specified file path.
        /// </summary>
        /// <param name="filePath">The path of the log file.</param>
        /// <exception cref="ArgumentException">If <paramref name="filePath"/> is <see langword="null"/> or <see cref="string.Empty"/>.</exception>
        public FileLogHandler(string filePath)
            : this(filePath, overwrite: true)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogHandler"/> class with the specified file path.
        /// </summary>
        /// <param name="filePath">The path of the log file.</param>
        /// <param name="overwrite">Specifies whether the file should be overwritten if it exists on the disk.</param>
        /// <exception cref="ArgumentException">If <paramref name="filePath"/> is <see langword="null"/> or <see cref="string.Empty"/>.</exception>
        public FileLogHandler(string filePath, bool overwrite)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("File log path cannot be null or empty.", nameof(filePath));

            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var fileMode = overwrite ? FileMode.Create : FileMode.Append;

            _fileStream = File.Open(filePath, fileMode, FileAccess.Write, FileShare.Read);
            _streamWriter = new StreamWriter(_fileStream, System.Text.Encoding.UTF8)
            {
                AutoFlush = true
            };
        }

        /// <summary>
        /// Handles a log event by writing it to the log file.
        /// </summary>
        /// <param name="logEvent">The log event to handle.</param>
        public void Handle(LogEvent logEvent)
        {
            lock (_lockObj)
            {
                _streamWriter.WriteLine($"{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss.fff} {_levels[(int)logEvent.Level]} {logEvent.IssuedBy.Name}: {logEvent.Message}");
            }
        }

        /// <summary>
        /// Disposes the file log handler and releases any resources used.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes the file log handler instance.
        /// </summary>
        ~FileLogHandler()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the file log handler and releases any resources used.
        /// </summary>
        /// <param name="disposing">A flag indicating whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (_lockObj)
            {
                if (!_isDisposed)
                {
                    if (disposing)
                    {
                        _streamWriter?.Dispose();
                        _streamWriter = null!;
                        _fileStream?.Dispose();
                        _fileStream = null!;
                    }

                    _isDisposed = true;
                }
            }
        }
    }
}
