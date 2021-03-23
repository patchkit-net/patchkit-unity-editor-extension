using System;

namespace PatchKit.Api.Models.Main
{
    [Serializable]
    public struct Changelog
    {
        /// <summary>
        /// Version id.
        /// </summary>
        public int version_id;

        /// <summary>
        /// Human readable label.
        /// </summary>
        public string version_label;

        /// <summary>
        /// Changes description.
        /// </summary>
        public string changes;
    }
}