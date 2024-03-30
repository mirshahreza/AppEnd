namespace AppEndDbIO
{
    public class ClientParam(string name, object? value = null)
	{
		public string Name { set; get; } = name;
		public object? Value { set; get; } = value;
	}
}
