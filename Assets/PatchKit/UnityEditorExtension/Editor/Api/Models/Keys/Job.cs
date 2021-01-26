using System;

namespace PatchKit.Api.Models.Keys
{
    [Serializable]
    public struct Job
    {
        public string guid;

        public bool pending;

        public bool finished;
    }
}