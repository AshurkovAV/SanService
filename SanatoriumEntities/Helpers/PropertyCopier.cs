using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

namespace SanatoriumEntities.Helpers
{
    public static class PropertiesCopy<T>
    {
        private static readonly Action<T, T> copier;
        private static readonly Action<T, T> updater;

        static PropertiesCopy()
        {
            var p1 = Expression.Parameter(typeof(T), "from");
            var p2 = Expression.Parameter(typeof(T), "to");

            var props = from property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        where property.CanRead && property.CanWrite
                        select Expression.Assign(Expression.Property(p2, property), Expression.Property(p1, property));

            var propsUpdate = from property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        where property.CanRead && property.CanWrite && !property.Equals(null)
                        select Expression.Assign(Expression.Property(p2, property), Expression.Property(p1, property));

            copier = Expression.Lambda<Action<T, T>>(Expression.Block(props), p1, p2).Compile();

            updater = Expression.Lambda<Action<T, T>>(Expression.Block(propsUpdate), p1, p2).Compile();
        }

        public static void CopyAllProperties(T from, T to) => copier(from, to);

        public static void UpdateAllProperties(T from, T to) => copier(from, to);
    }
}