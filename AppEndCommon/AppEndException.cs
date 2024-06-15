using System.Reflection;
using System.Runtime.CompilerServices;

namespace AppEndCommon
{
    public class AppEndException : Exception
    {
	    private List<KeyValuePair<string, object>> _errorMetadata = [];

        public AppEndException(string message, MethodBase? methodBase) : base(message)
        {
            AddParam("Site", methodBase.GetPlaceInfo());
        }

        //public AppEndException(string message, Exception inner) : base(message, inner)
        //{
        //}


        public AppEndException AddParam(string name, object value)
        {
            _errorMetadata.Add(new KeyValuePair<string, object>(name, value));
            return this;
        }

        public string GetMetadata()
        {
            return string.Join(", ", _errorMetadata);
        }
    }

    public static class ExtensionsForAppEndException
	{
		public static Exception GetEx(this AppEndException appEndException)
		{
			return new Exception($"{appEndException.Message} : [{string.Join(", ", appEndException.GetMetadata())}]");
		}

	}

}
