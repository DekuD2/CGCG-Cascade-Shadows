using System;

namespace FR.CascadeShadows.Resources;

public abstract class FileLoader<T> : IFileLoader
{
    Type IFileLoader.ResourceType => typeof(T);

    public abstract T? Load(string uri);

    object? IFileLoader.Load(string uri)
        => Load(uri);
}