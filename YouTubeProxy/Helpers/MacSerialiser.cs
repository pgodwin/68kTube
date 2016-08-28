using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy
{
    public static class MacSerialiser
    {
        public static void Serialize<T>(TextWriter output, IEnumerable<T> objects)
        {
            
            var fields =
                from mi in typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                where new[] { MemberTypes.Field, MemberTypes.Property }.Contains(mi.MemberType)
                let orderAttr = (ColumnOrderAttribute)Attribute.GetCustomAttribute(mi, typeof(ColumnOrderAttribute))
                orderby orderAttr == null ? int.MaxValue : orderAttr.Order, mi.Name
                select mi;
            //output.WriteLine(QuoteRecord(fields.Select(f => f.Name)));
            foreach (var record in objects)
            {
                output.WriteLine(string.Join(CsvSeparator, FormatObject(fields, record)));
            }
        }

        public static string Serialize<T>(this IEnumerable<T> objects)
        {
            var output = new StringWriter();
            Serialize(output, objects);
            return output.ToString(); 
        }


        static IEnumerable<string> FormatObject<T>(IEnumerable<MemberInfo> fields, T record)
        {
            foreach (var field in fields)
            {
                if (field is FieldInfo)
                {
                    var fi = (FieldInfo)field;
                    yield return Convert.ToString(fi.GetValue(record)).Replace(CsvSeparator, "/");
                }
                else if (field is PropertyInfo)
                {
                    var pi = (PropertyInfo)field;
                    yield return Convert.ToString(pi.GetValue(record, null)).Replace(CsvSeparator, "/");
                }
                else
                {
                    throw new Exception("Unhandled case.");
                }
            }
        }

        const string CsvSeparator = "|";
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ColumnOrderAttribute : Attribute
    {
        public int Order { get; private set; }
        public ColumnOrderAttribute(int order) { Order = order; }
    }
}
