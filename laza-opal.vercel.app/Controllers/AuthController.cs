namespace laza_opal.vercel.app.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IConfiguration _configuration;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, SignInManager<ApplicationUser> signInManager)
		{
			_authService = authService;
			_userManager = userManager;
			_roleManager = roleManager;
			_configuration = configuration;
			_signInManager = signInManager;
		}

		[HttpGet("getAllUsers")]
		public async Task<IActionResult> GetAllUsers()
		{
			var res = await _authService.GetAllAsync();
			return Ok(res);
		}

		[HttpGet("getuserbyid")]
		public async Task<IActionResult> GetUserByID(string userid)
		{
			if (string.IsNullOrEmpty(userid))
			{
				return BadRequest("User ID cannot be null or empty.");
			}

			ApplicationUser? user = await _userManager.FindByIdAsync(userid);

			if (user == null)
			{
				return NotFound($"User with ID {userid} not found.");
			}

			return Ok(user);
		}

		[HttpGet("getusernamebyid")]
		public async Task<IActionResult> GetUserNameByID(string userid)
		{
			if (string.IsNullOrEmpty(userid))
			{
				return BadRequest("User ID cannot be null or empty.");
			}

			ApplicationUser? user = await _userManager.FindByIdAsync(userid);

			if (user == null)
			{
				return NotFound($"User with ID {userid} not found.");
			}

			return Ok(user.Name);
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromForm]RegisterModel model)
		{
			var user = await _authService.RegisterAsync(model , model.Image);
			return Ok(new { message = "Registration successful", user });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginModel model)
		{
			var token = await _authService.LoginAsync(model);
			return Ok(new { token });
		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			await _authService.LogoutAsync();
			return Ok("User Logged out");
		}

		[HttpPost("assign-role")]
		public async Task<IActionResult> AssignRole(string email, string role)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return NotFound("User not found");
			}

			if (!await _roleManager.RoleExistsAsync(role))
			{
				var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
				if (!roleResult.Succeeded)
				{
					return BadRequest("Failed to create role");
				}
			}
			await _userManager.AddToRoleAsync(user, role);
			return Ok("Role assigned successfully");
		}

		[HttpGet("profile")]
		public async Task<IActionResult> GetUserProfile([FromHeader]string token)
		{
			var userProfile = await _authService.GetUserProfileAsync(token);

			if (userProfile == null)
			{
				return Unauthorized();
			}
			return Ok(userProfile);
		}

		[HttpPut("profile")]
		[Authorize]
		public async Task<IActionResult> EditProfile([FromForm] EditProfileModel model)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Unauthorized("User not found.");
			}

			var updatedUser = await _authService.EditProfileAsync(userId, model);

			return Ok(updatedUser);
		}


		//[HttpPut("EditProfile")]
		//[Authorize]
		//public async Task<IActionResult> EditProfile2(string token ,[FromForm] RegisterModel model)
		//{
		//	if (token == null)
		//	{
		//		return Unauthorized("User not found.");
		//	}

		//	var updatedUser = await _authService.EditProfileAsync2(token, model);

		//	return Ok(updatedUser);
		//}


		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
		{
			var result = await _authService.SendVerificationCodeAsync(request.Email);
			if (result)
			{
				return Ok(new { message = "Verification code sent to your email." });
			}
			return NotFound(new { message = "Email not found." });
		}

		[HttpPost("Check_code")]
		public async Task<IActionResult> CheckCode([FromBody]string verificationcode)
		{
			if(verificationcode == null)
			{
				return BadRequest("Please enter verification code.");
			}
			var res = await _authService.CheckCode(verificationcode);
			if (res.Succeeded)
			{
				return Ok(new { message = "Verification code is valid." });
			}

			return BadRequest(new
			{
				message = "Invalid verification code.",
				errors = res.Errors.Select(e => e.Description).ToArray()
			});
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
		{
			if (request == null || string.IsNullOrEmpty(request.Email)||
		string.IsNullOrEmpty(request.NewPassword))
			{
				return BadRequest("Invalid request data.");
			}

			var result = await _authService.ResetPasswordAsync(request.Email, request.NewPassword , request.ConfirmPassword);

			if (result.Succeeded)
			{
				return Ok(new { message = "Password reset successful." });
			}

			return BadRequest(new { message = "Error resetting password.", errors = result.Errors.Select(e => e.Description).ToArray() });
		}

		[HttpPost("login-google")]
		public async Task<IActionResult> LoginWithGoogle([FromHeader] string idToken)
		{
			var payload = await _authService.VerifyGoogleTokenAsync(idToken);
			if (payload == null)
			{
				return Unauthorized("Invalid Google token.");
			}

			var user = await _authService.GetOrCreateExternalLoginUserAsync(payload.Email, payload.Name, "Google");
			string token = await _authService.GenerateJwtToken(user);
			return Ok(new { token });
		}

		[HttpPost("login-facebook")]
		public async Task<IActionResult> LoginWithFacebook([FromHeader]string accessToken)
		{
			var fbUser = await _authService.VerifyFacebookTokenAsync(accessToken);
			if (fbUser == null)
			{
				return Unauthorized("Invalid Facebook token.");
			}

			var user = await _authService.GetOrCreateExternalLoginUserAsync(fbUser.Email, fbUser.Name, "Facebook");
			string token = await  _authService.GenerateJwtToken(user);
			return Ok(new { token });
		}

		//[HttpPost("login-twitter")]
		//public async Task<IActionResult> LoginWithTwitter([FromBody] string oauthToken, string oauthVerifier)
		//{
		//	var twitterUser = await _authService.VerifyTwitterTokenAsync(oauthToken, oauthVerifier);
		//	if (twitterUser == null)
		//	{
		//		return Unauthorized("Invalid Twitter credentials.");
		//	}

		//	var user = await _authService.GetOrCreateExternalLoginUserAsync(twitterUser.Email, twitterUser.Name, "Twitter");
		//	var token = _authService.GenerateJwtToken(user);
		//	return Ok(new { token });
		//}

	}
}