using System;

namespace PatchKit.Api.Models.Main
{
    [Serializable]
    public struct AppContentUrl
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
        /// ISO code of origin country.
        /// </summary>
        public string country;
    }
}