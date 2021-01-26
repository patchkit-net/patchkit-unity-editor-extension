using System;

namespace PatchKit.Api.Models.Main
{
    [Serializable]
    public struct StatsReport
    {
        public string event_name;

        /// <summary>
        /// Secret of game application if applicable.
        /// </summary>
        public string app_secret;

        /// <summary>
        /// Version of game application if applicable.
        /// </summary>
        public int app_version;

        /// <summary>
        /// Unique client id. Should remain the same for all applications launched on this machine + account. Used to identify the player.
        /// </summary>
        public long sender_id;

        /// <summary>
        /// Caller id the same as for caller GET parameters. More information: http://redmine.patchkit.net/projects/patchkit-documentation/wiki/Web_API_General_Info
        /// </summary>
        public string caller;

        public string operating_system_family;

        /// <summary>
        /// Operating system version without family name, for instance '10.11' for OSX.
        /// </summary>
        public string operating_system_version;
    }
}