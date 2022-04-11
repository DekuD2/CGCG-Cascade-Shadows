using System;
using System.Collections.Generic;
using System.IO;

using Path = System.IO.Path;

namespace FR.CascadeShadows.Resources;

public static class ResourceCache
{
    static readonly Dictionary<(Type type, string path), object> cache = new();

    public static T Get<T>(string uri, bool forceReload = false) where T : class
    {
        uri = RelativePath(uri);

        if (!forceReload && cache.TryGetValue((typeof(T), uri), out var cached))
            return (T)cached;
        else
        {
            var resource = ResourceLoader.Load<T>(uri);

            if (forceReload)
                cache[(typeof(T), uri)] = resource;
            else
                cache.Add((typeof(T), uri), resource);

            return resource;
        }
    }

    private static string RelativePath(string path) => Path.GetRelativePath(Directory.GetCurrentDirectory(), path);
}
