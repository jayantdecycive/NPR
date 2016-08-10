using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace cfacore.shared.modules.com.util
{
    public static class Util
    {
        public static string Urlify(string str)
        {
            return Regex.Replace(str.Replace('+', ' '), @"[^A-Za-z0-9 ]", "").Trim().Replace(' ', '-').Replace("--", "-");
        }
        public static string Domify(string str)
        {
            return Regex.Replace(Urlify(str).ToLower(), @"-", "_");
        }
    }
}
