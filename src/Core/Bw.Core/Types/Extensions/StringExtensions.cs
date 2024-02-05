﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace Bw.Core.Types.Extensions;


public static class StringExtensions
{
    public static T ConvertTo<T>(this object input)
    {
        return input.ToString()!.ConvertTo<T>();
    }

    public static T ConvertTo<T>(this string input)
    {
        try
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFromString(input)!;
        }
        catch (NotSupportedException)
        {
            return default!;
        }
    }

    public static bool IsValidJson(this string strInput)
    {
        if (string.IsNullOrWhiteSpace(strInput))
        {
            return false;
        }

        strInput = strInput.Trim();
        if (
            strInput.StartsWith("{", StringComparison.Ordinal) && strInput.EndsWith("}", StringComparison.Ordinal)
            || strInput.StartsWith("[", StringComparison.Ordinal) && strInput.EndsWith("]", StringComparison.Ordinal)
        )
        {
            try
            {
                var obj = JToken.Parse(strInput);
                return true;
            }
            catch (JsonReaderException jex)
            {
                Console.WriteLine(jex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        return false;
    }
}
