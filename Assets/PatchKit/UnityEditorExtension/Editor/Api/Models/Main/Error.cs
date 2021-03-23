using System;

namespace PatchKit.Api.Models.Main
{
    [Serializable]
    public struct Error
    {
        /// <summary>
        /// Human-readable error message
        /// </summary>
        public string message;

        /// <summary>
        /// Error symbol
        /// </summary>
        public string symbol;
    }
}