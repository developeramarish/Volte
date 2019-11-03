using System;

namespace Gommon
{
    public static partial class Extensions
    {
        //an extension on all `Type` instances, it's only applicable on typeparsers but since it's a Type extension that doesn't really matter
        public static string SanitizeParserName(this Type type)
            => type.Name.Replace("Parser", string.Empty);

    }
}
