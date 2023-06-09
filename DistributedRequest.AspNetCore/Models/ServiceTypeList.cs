﻿using DistributedRequest.AspNetCore.Enums;
using System;
using System.Collections.Concurrent;

namespace DistributedRequest.AspNetCore.Models
{
    internal class ServiceTypeList
    {
        private readonly ConcurrentDictionary<string, Type> jobTypes = new ConcurrentDictionary<string, Type>();
        private readonly ConcurrentDictionary<string, Type> requestTypes = new ConcurrentDictionary<string, Type>();
        private readonly ConcurrentDictionary<string, Type> responseTypes = new ConcurrentDictionary<string, Type>();

        public ServiceTypeList()
        {
        }

        private ConcurrentDictionary<string, Type> GetCurrentType(TypeEnum typeEnum)
        {
            return typeEnum switch
            {
                TypeEnum.Request => requestTypes,
                TypeEnum.Response => responseTypes,
                _ => jobTypes,
            };
        }

        public bool Contains(TypeEnum typeEnum, Type type)
        {
            var selfTypes = GetCurrentType(typeEnum);
            return selfTypes.ContainsKey(type.FullName);
        }

        public void AddTypes(TypeEnum typeEnum, params Type[] types)
        {
            var selfTypes = GetCurrentType(typeEnum);
            foreach (var item in types)
            {
                selfTypes.TryAdd(item.FullName, item);
            }
        }

        public Type GetType(TypeEnum typeEnum, string typeFullName)
        {
            var selfTypes = GetCurrentType(typeEnum);
            selfTypes.TryGetValue(typeFullName, out var type);
            return type;
        }
    }
}
