using System.Reflection;
using System.Collections.Concurrent;

namespace AppEndCommon
{
	public static class ExtensionsForHashtable
    {

        public static void SetOrAdd(this System.Collections.Hashtable hashtable, object key, object? value)
        {
            ArgumentNullException.ThrowIfNull(hashtable);
            ArgumentNullException.ThrowIfNull(key);

            if (hashtable.ContainsKey(key))
            {
                hashtable[key] = value;
            }
            else
            {
                hashtable.Add(key, value);
            }
        }

    }
}