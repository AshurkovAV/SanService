using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

namespace SanatoriumEntities.Helpers
{
    public static class ObjectCopier<From, To>
    {
        public static To CopyProperies(From src, To dst)
        {
            PropertyInfo[] fromProperties = src.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy
            );

            PropertyInfo[] toProperties = dst.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy
            );

            foreach (var fromP in fromProperties)
            {
                var ind = ObjectCopier<From, To>.containPropertyName(dst, fromP.Name);

                if (ind >= 0 && (null != fromP.GetValue(src))) {
                    
                    toProperties[ind].SetValue(
                        dst,
                        fromP.GetValue(src)
                    );
                }
            }

            return dst;
        }

        public static To UpdateModelProperies(From src, To dst)
        {
            PropertyInfo[] fromProperties = src.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy
            );

            PropertyInfo[] toProperties = dst.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy
            );

            List<string>serviceFields = new List<string>() {"id", "active", "ip4", "created_at", "created_by", "updated_at", "updated_by"};
            
            foreach (var fromP in fromProperties)
            {
                if (serviceFields.Contains(fromP.Name)){
                    continue;
                }

                var ind = ObjectCopier<From, To>.containPropertyName(dst, fromP.Name);

                if (ind >= 0 && (null != fromP.GetValue(src))) {
                    
                    toProperties[ind].SetValue(
                        dst,
                        fromP.GetValue(src)
                    );
                }
            }

            return dst;
        }

        public static int containPropertyName(To obj, string name)
        {
            PropertyInfo[] objProperties = obj.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy
            );
            
            int index = 0;

            foreach (var objP in objProperties)
            {
                if (name == objP.Name) {
                    return index;
                }

                index++;
            }

            return -1;
        }
    }
}