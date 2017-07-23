using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win32Client.Helpers
{
    public static class StringExtensions
    {
        /// <summary>
        /// The Mac Results are in MacRoman encoding, return it back to UTF
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string MacRomanToString(this byte[] source)
        {
            return System.Text.Encoding.GetEncoding(10000).GetString(source);
        }

    }
}
