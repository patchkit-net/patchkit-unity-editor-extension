using System;

namespace PatchKit.Api.Models.Main
{
    [Serializable]
    public struct AppDiffSummary
    {
        /// <summary>
        /// Version string. Format: MAJOR.MINOR. Present in >= 2.4
        /// </summary>
        public string version;

        /// <summary>
        /// Diff size.
        /// </summary>
        public long size;

        /// <summary>
        /// Uncompressed archive size. Present in >= 2.4.
        /// </summary>
        public long uncompressed_size;

        /// <summary>
        /// Encryption method.
        /// </summary>
        public string encryption_method;

        /// <summary>
        /// Compression method.
        /// </summary>
        public string compression_method;

        /// <summary>
        /// List of added files.
        /// </summary>
        public string[] added_files;

        /// <summary>
        /// List of modified files.
        /// </summary>
        public string[] modified_files;

        /// <summary>
        /// List of removed files.
        /// </summary>
        public string[] removed_files;

        public string hash_code;

        public Chunks chunks;
    }
}