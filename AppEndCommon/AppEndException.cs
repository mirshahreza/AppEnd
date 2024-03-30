namespace AppEndCommon
{
    public class AppEndException : Exception
    {
	    private List<KeyValuePair<string, object>> _errorMetadata = [];

        public AppEndException()
        {
        }

        public AppEndException(string message) : base(message)
        {
        }

        public AppEndException(string message, Exception inner) : base(message, inner)
        {
        }


        public AppEndException AddParam(string name, object value)
        {
            _errorMetadata.Add(new KeyValuePair<string, object>(name, value));
            return this;
        }

        public AppEndException SetMetaData(List<KeyValuePair<string, object>> errorMetadata)
        {
            _errorMetadata = errorMetadata;
            return this;
        }
    }

}
