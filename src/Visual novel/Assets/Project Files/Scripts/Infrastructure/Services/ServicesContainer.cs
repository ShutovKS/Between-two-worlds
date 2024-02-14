#region

using System;
using System.Collections.Generic;

#endregion

namespace Infrastructure.Services
{
    public static class ServicesContainer
    {
        private static readonly Dictionary<Type, object> _classesDictionary = new();

        public static void SetServices(params object[] objects)
        {
            _classesDictionary.Clear();

            foreach (var variable in objects)
            foreach (var interfaceType in variable.GetType().GetInterfaces())
            {
                _classesDictionary.Add(interfaceType, variable);
            }
        }

        public static T GetService<T>() where T : class
        {
            return _classesDictionary.TryGetValue(typeof(T), out var TClass)
                ? TClass as T
                : throw new Exception($"No class {typeof(T)}");
        }
    }
}