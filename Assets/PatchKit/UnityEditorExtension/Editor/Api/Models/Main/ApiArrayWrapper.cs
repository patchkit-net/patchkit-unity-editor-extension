using System;

namespace PatchKit.Api.Models.Main
{
    [Serializable]
    public struct ApiArrayWrapper<T>
    {
        public T[] array;
    }
}