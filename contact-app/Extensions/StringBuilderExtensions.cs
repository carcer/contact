using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace contact_app.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineFormat(this StringBuilder sb, string format, params object[] args)
        {
            return sb.AppendLine(string.Format(format, args));
        }
    }
}