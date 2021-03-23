using System;

namespace PatchKit.Api.Models.Main
{
    [Serializable]
    public struct Job
    {
        /// <summary>
        /// Job GUID to be used with Jobs API.
        /// </summary>
        public string job_guid;
    }
}