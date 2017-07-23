using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Win32Client.Helpers
{
    public static class MacDeserialiser
    {
        const char CsvSeparator = '|';

        public static IEnumerable<T> Deserialize<T>(string input)
        {
            var fields =
                (from mi in typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                where new[] { MemberTypes.Field, MemberTypes.Property }.Contains(mi.MemberType)
                let orderAttr = (ColumnOrderAttribute)Attribute.GetCustomAttribute(mi, typeof(ColumnOrderAttribute))
                orderby orderAttr == null ? int.MaxValue : orderAttr.Order, mi.Name
                select mi).ToArray();

            var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<T>();
            foreach (var line in lines)
            {
                var item = line.Split(CsvSeparator);
                var record = (T)Activator.CreateInstance(typeof(T));

                for (int i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    SetValue(field, record, item[i]);
                }
                result.Add(record);
            }

            return result;
        }
        
        private static void SetValue<T>(MemberInfo field, T record, string value)
        {
            if (field is FieldInfo)
            {
                var fi = (FieldInfo)field;
                fi.SetValue(record, Convert.ChangeType(value, fi.FieldType));
            }
            else if (field is PropertyInfo)
            {
                var pi = (PropertyInfo)field;
                if (pi.PropertyType.IsEnum)
                    pi.SetValue(record, Enum.Parse(pi.PropertyType, value));
                else
                    pi.SetValue(record, Convert.ChangeType(value, pi.PropertyType));
            }
            else
            {
                throw new Exception("Unhandled case.");
            }
        }
        
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ColumnOrderAttribute : Attribute
    {
        public int Order { get; private set; }
        public ColumnOrderAttribute(int order) { Order = order; }
    }
}

