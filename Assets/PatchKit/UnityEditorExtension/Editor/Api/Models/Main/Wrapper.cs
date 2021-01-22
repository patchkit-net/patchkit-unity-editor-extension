using System;

namespace PatchKit.Api.Models.Main
{

[Serializable]
public struct Wrapper<T>
{
    public T[] array;
}
}