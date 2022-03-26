﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FR.CascadeShadows.Resources;

public static class ResourceLoader
{
    static readonly Dictionary<Type, IFileLoader> loaders = new();

    static ResourceLoader()
    {
        foreach (var type in Assembly.GetExecutingAssembly().DefinedTypes)
        {
            var iface = type.GetInterface(nameof(IFileLoader));
            if (iface == null || type.IsAbstract || iface != typeof(IFileLoader)) continue;

            var loader = Activator.CreateInstance(type) as IFileLoader;
            if (loader == null) continue;

            loaders.Add(loader.ResourceType, loader);
        }
    }

    public static T Load<T>(string uri) where T : class
    {
        if (!File.Exists(uri))
            throw new ArgumentException($"File '{uri}' not found.");

        if (!loaders.TryGetValue(typeof(T), out var loader))
            throw new ArgumentException($"No loader exists for the type {typeof(T)}.");

        T? result = loader.Load(uri) as T;
        if (result == null)
            throw new Exception($"The loader of type {loader.GetType()} couldn't succesfully load the file '{uri}' into {typeof(T)}.");

        return result;
    }
}
