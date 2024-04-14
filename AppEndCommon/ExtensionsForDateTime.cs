using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;

namespace AppEndCommon
{
    public static class ExtensionsForDateTime
	{
        public static string ToAppEndStandard(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:MM:ss t" + "M");
		}

        

    }
}