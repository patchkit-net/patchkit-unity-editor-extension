using System;

namespace PatchKit.Api.Models.Main
{
    [Serializable]
    public struct AppVersion
    {
        /// <summary>
        /// Initial version id.
        /// </summary>
        public int id;

        /// <summary>
        /// Version label.
        /// </summary>
        public string label;

        /// <summary>
        /// Description of changes.
        /// </summary>
        public string changelog;

        /// <summary>
        /// Unix timestamp of publish date.
        /// </summary>
        public long publish_date;

        /// <summary>
        /// Guid of content file.
        /// </summary>
        public string content_guid;

        /// <summary>
        /// Guid of content meta file if available.
        /// </summary>
        public string content_meta_guid;

        /// <summary>
        /// Guid of diff file or null if there's no diff.
        /// </summary>
        public string diff_guid;

        /// <summary>
        /// Guid of diff meta file if available.
        /// </summary>
        public string diff_meta_guid;

        /// <summary>
        /// Set to true if this version is a draft version.
        /// </summary>
        public bool draft;

        public bool pending_publish;

        public bool published;

        /// <summary>
        /// If pending_publish is true, this number will indicate the publishing progress.
        /// </summary>
        public float publish_progress;

        /// <summary>
        /// Main executable relative path without slash at the beginning.
        /// </summary>
        public string main_executable;

        /// <summary>
        /// Main executable arguments
        /// </summary>
        public string[] main_executable_args;

        /// <summary>
        /// Relative list of paths that should be ignored for local data consistency.
        /// </summary>
        public string[] ignored_files;

        /// <summary>
        /// Set to true if version will be published after successfull processing.
        /// </summary>
        public bool publish_when_processed;

        public string processing_started_at;

        public string processing_finished_at;

        /// <summary>
        /// If true then this version can be imported to other application. Visible only for owners.
        /// </summary>
        public bool can_be_imported;
    }
}