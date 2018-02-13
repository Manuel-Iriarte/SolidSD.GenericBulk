using GenericBulk.Core.Atributos;
using System;
using System.Linq;
using System.Reflection;

namespace GenericBulk.Core.Helpers
{
    public static class ObjetoHelper<T>
    {
        public static int Compara(T x, T y)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties()
                                            .Where(p => Attribute.IsDefined(p, typeof(IgnorarEnComparacionAttribute)) == false)
                                            .ToArray();

            FieldInfo[] fields = type.GetFields();
            int compareValue = 0;

            foreach (PropertyInfo property in properties)
            {
                IComparable valx = property.GetValue(x, null) as IComparable;
                if (valx == null)
                    continue;
                object valy = property.GetValue(y, null);
                compareValue = valx.CompareTo(valy);
                if (compareValue != 0)
                    return compareValue;
            }
            foreach (FieldInfo field in fields)
            {
                IComparable valx = field.GetValue(x) as IComparable;
                if (valx == null)
                    continue;
                object valy = field.GetValue(y);
                compareValue = valx.CompareTo(valy);
                if (compareValue != 0)
                    return compareValue;
            }

            return compareValue;
        }
    }
}
