using System;

namespace PatchKit.Api.Models.Main
{
    [Serializable]
    public struct Chunks
    {
        public int size;

        public string[] hashes;
    }
}