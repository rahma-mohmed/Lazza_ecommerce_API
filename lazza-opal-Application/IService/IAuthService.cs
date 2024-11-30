namespace Lazza.opal.Application.Service
{
    public interface IAuthService
    {
        Task LogoutAsync();
        Task<IEnumerable<IdentityUser>> GetAllAsync();
		Task<ApplicationUser> RegisterAsync(RegisterModel model , IFormFile Image);
        Task<string> LoginAsync(LoginModel model);
        Task<bool> SendVerificationCodeAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(string email, string newPassword, string ConfirmPassword);
        Task<GooglePayload> VerifyGoogleTokenAsync(string idToken);
        Task<FacebookUser> VerifyFacebookTokenAsync(string accessToken);
        Task<ApplicationUser> VerifyTwitterTokenAsync(string oauthToken, string oauthVerifier);
        Task<string> GenerateJwtToken(ApplicationUser user);
        Task<ApplicationUser> GetOrCreateExternalLoginUserAsync(string email, string name, string provider);
        Task<ApplicationUser?> GetUserProfileAsync(string token);
        Task<ApplicationUser> EditProfileAsync(string userId, EditProfileModel model);
        Task<IdentityResult> CheckCode(string verificationCode);
	}
}
