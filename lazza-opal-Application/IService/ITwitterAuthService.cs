namespace Lazza.opal.Application.Service
{
	public interface ITwitterAuthService
	{
		Task<IAuthenticatedUser> ValidateTwitterTokenAsync(string oauthToken, string oauthVerifier);
	}
}
