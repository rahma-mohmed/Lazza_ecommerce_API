using Tweetinvi;
using Tweetinvi.Models;

namespace Lazza.opal.persistence.Service
{
	public class TwitterAuthService : ITwitterAuthService
	{
		private readonly string _consumerKey;
		private readonly string _consumerSecret;

		public TwitterAuthService(IConfiguration configuration)
		{
			_consumerKey = configuration["Authentication:Twitter:ConsumerKey"];
			_consumerSecret = configuration["Authentication:Twitter:ConsumerSecret"];
		}

		public async Task<IAuthenticatedUser> ValidateTwitterTokenAsync(string oauthToken, string oauthVerifier)
		{
			try
			{
				var userClient = new TwitterClient(_consumerKey, _consumerSecret);
				var requestCredentials = new TwitterCredentials(_consumerKey, _consumerSecret, oauthToken, oauthVerifier);
				var authenticatedClient = new TwitterClient(requestCredentials);

				var user = await authenticatedClient.Users.GetAuthenticatedUserAsync();
				return user;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error validating Twitter token: {ex.Message}");
				return null;
			}
		}
	}
}



