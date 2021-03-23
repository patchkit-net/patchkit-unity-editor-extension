using System;

namespace PatchKit.Api.Models.Main
{
    [Serializable]
    public struct AppContentSummaryFile
    {
        /// <summary>
        /// File path.
        /// </summary>
        public string path;

        /// <summary>
        /// File hash.
        /// </summary>
        public string hash;

        /// <summary>
        /// Uncompressed file size in bytes. Present in >= 2.3
        /// </summary>
        public long size;

        /// <summary>
        /// File flags, present in >= 2.3
        /// </summary>
        public string flags;
    }
}