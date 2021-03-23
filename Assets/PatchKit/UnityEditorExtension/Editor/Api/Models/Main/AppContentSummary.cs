using System;

namespace PatchKit.Api.Models.Main
{
    [Serializable]
    public struct AppContentSummary
    {
        /// <summary>
        /// Version string. Format: MAJOR.MINOR. Present in >= 2.4
        /// </summary>
        public string version;

        /// <summary>
        /// Content size.
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
        /// List of content files.
        /// </summary>
        public AppContentSummaryFile[] files;

        public string hash_code;

        public Chunks chunks;
    }
}