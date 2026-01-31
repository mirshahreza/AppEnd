using AppEndCommon;
using Google.Apis.Auth;
using System.Text.Json;

namespace AppEndServer
{
	public static class GoogleOAuthServices
	{
		public static string? ClientId => AppEndSettings.AAA?["GoogleOAuth"]?["ClientId"]?.ToString();
		public static string? ClientSecret => AppEndSettings.AAA?["GoogleOAuth"]?["ClientSecret"]?.ToString();
		public static bool IsEnabled => AppEndSettings.AAA?["GoogleOAuth"]?["Enabled"]?.ToBooleanSafe() ?? false;

		public static async Task<GoogleJsonWebSignature.Payload?> VerifyTokenAsync(string idToken)
		{
			if (!IsEnabled || string.IsNullOrEmpty(ClientId))
			{
				return null;
			}

			try
			{
				var settings = new GoogleJsonWebSignature.ValidationSettings
				{
					Audience = new[] { ClientId }
				};

				var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
				return payload;
			}
			catch
			{
				return null;
			}
		}
	}
}

