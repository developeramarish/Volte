﻿using System;
using Discord;
 

namespace Volte.Commands.TypeParsers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VolteTypeParserAttribute : Attribute
    {
        public VolteTypeParserAttribute(bool overridePrimitive = false)
        {
            OverridePrimitive = overridePrimitive;
        }

        public bool OverridePrimitive { get; set; }
    }
}