﻿using System;
using OlegHcp.Async;

namespace OlegHcp.SaveLoad
{
    public interface ISaver
    {
        void DeleteKey(string key);
        void Set(string key, object value);
        object Get(string key, object defaltValue);
        object Get(string key, Type type);

        bool LoadVersion(string version);
        bool DeleteVersion(string version);
        void SaveVersion(string version);
        TaskInfo SaveVersionAsync(string version);
    }

    public interface IKeyGenerator
    {
        string Generate(Type objectType, string fieldName, string objectID = null);
    }
}