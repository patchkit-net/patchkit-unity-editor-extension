using System;

namespace PatchKit.Api.Models.Main
{
    [Serializable]
    public struct AppDiffUrl
    {
        /// <summary>
        /// Url to diff file.
        /// </summary>
        public string url;

        /// <summary>
        /// Url to meta file if available.
        /// </summary>
        public string meta_url;

        /// <summary>
        /// ISO code of origin country.
        /// </summary>
        public string country;
    }
}