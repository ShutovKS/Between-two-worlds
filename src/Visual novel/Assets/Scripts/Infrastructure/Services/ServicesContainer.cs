using System;
using System.Collections.Generic;

namespace Infrastructure.Services
{
    public static class ServicesContainer
    {
        public static void SetServices(params object[] objects)
        {
            _classesDictionary.Clear();

            foreach (var variable in objects)
            foreach (var interfaceType in variable.GetType().GetInterfaces())
            {
                _classesDictionary.Add(interfaceType, variable);
            }
        }

        private readonly static Dictionary<Type, object> _classesDictionary = new();

        public static T GetService<T>() where T : class
        {
            return _classesDictionary.TryGetValue(typeof(T), out var TClass)
                ? TClass as T
                : throw new Exception($"No class {typeof(T)}");
        }
    }
}