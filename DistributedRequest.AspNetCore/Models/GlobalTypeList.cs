using System;
using System.Collections.Concurrent;

namespace DistributedRequest.AspNetCore.Models
{
    internal class GlobalTypeList
    {
        private readonly ConcurrentDictionary<string, Type> types = new ConcurrentDictionary<string, Type>();
        public GlobalTypeList()
        {
        }

        public void AddTypes(string key, Type type)
        {
            types.TryAdd(key, type);
        }

        public Type GetType(string key)
        {
            types.TryGetValue(key, out var type);
            return type;
        }
    }
}
