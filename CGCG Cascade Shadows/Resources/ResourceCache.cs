using System;
using System.Collections.Generic;
using System.IO;

using Path = System.IO.Path;

namespace FR.CascadeShadows.Resources;

public static class ResourceCache
{
    static readonly Dictionary<(Type type, string path), object> cache = new();

    public static T Get<T>(string uri) where T : class
    {
        uri = RelativePath(uri);

        if (cache.TryGetValue((typeof(T), uri), out var cached))
            return (T)cached;
        else
        {
            var resource = ResourceLoader.Load<T>(uri);
            cache.Add((typeof(T), uri), resource);
            return resource;
        }
    }

    private static string RelativePath(string path) => Path.GetRelativePath(Directory.GetCurrentDirectory(), path);
}
