using System;

namespace FR.CascadeShadows.Resources;

public interface IFileLoader
{
    object? Load(string uri);
    Type ResourceType { get; }
}
