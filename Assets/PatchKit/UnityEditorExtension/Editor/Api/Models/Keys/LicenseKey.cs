using System;

namespace PatchKit.Api.Models.Keys
{
    [Serializable]
    public struct LicenseKey
    {
        public string key;

        public string app_secret;

        public string key_secret;

        public int collection_id;

        /// <summary>
        /// Number of key registrations. This is a request wihout a app_secret.
        /// </summary>
        public int registrations;

        /// <summary>
        /// If set to true, this key is blocked for further use.
        /// </summary>
        public bool blocked;
    }
}