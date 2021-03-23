using System;

namespace PatchKit.Api.Models.Main
{
    [Serializable]
    public struct ContentUrl
    {
        /// <summary>
        /// Url to content file.
        /// </summary>
        public string url;

        /// <summary>
        /// Url to meta file if available.
        /// </summary>
        public string meta_url;

        /// <summary>
        /// Region name of this mirror server.
        /// </summary>
        public string region;

        /// <summary>
        /// Value of recent server load (usage). Servers with lower load should be prioritorized.
        /// </summary>
        public double load;
    }
}