using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Text;
using UnityEngine;

public static class ExtensionMethods
{
    public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
    {
        var tcs = new TaskCompletionSource<object>();
        asyncOp.completed += obj => { tcs.SetResult(null); };
        return ((Task)tcs.Task).GetAwaiter();
    }

    public static string EncodeBase64(this string value)
    {
        var valueBytes = Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(valueBytes);
    }

    public static string DecodeBase64(this string value)
    {
        var valueBytes = System.Convert.FromBase64String(value);
        return Encoding.UTF8.GetString(valueBytes);
    }

    public static string removeAllNonAlphanumericCharsExceptDashes(this string value)
    {
        char[] arr = value.ToCharArray();
        arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c) || c == '-')));
        return new string(arr);
    }
}
