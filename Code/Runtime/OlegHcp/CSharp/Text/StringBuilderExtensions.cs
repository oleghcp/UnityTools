﻿using System.Text;

namespace OlegHcp.CSharp.Text
{
    public static class StringBuilderExtensions
    {
        public static string Cut(this StringBuilder self)
        {
            string value = self.ToString();
            self.Clear();
            return value;
        }
    }
}
