namespace AppEndCommon
{
    public static class ExtensionsForStream
    {
        public static byte[] ToByteArray(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new();
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }

        public static string ToText(this Stream input)
        {
            byte[] bytes = input.ToByteArray();
            return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
    }
}