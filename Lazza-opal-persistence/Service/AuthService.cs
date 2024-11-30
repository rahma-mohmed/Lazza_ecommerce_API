namespace Lazza.opal.persistence.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        public readonly ITwitterAuthService _twitterService;
		private readonly IImageService _imageService;
		private readonly HttpClient _httpClient;

		public AuthService(IUserRepository userRepository, UserManager<ApplicationUser> userManager,
              SignInManager<ApplicationUser> signInManager, IConfiguration configuration ,IEmailSender emailSender
            , ITwitterAuthService twitterService , IImageService imageService , HttpClient httpClient)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSender = emailSender;
			_twitterService = twitterService;
            _imageService = imageService;
			_httpClient = httpClient;
		}

        public async Task<IEnumerable<IdentityUser>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

		public async Task<ApplicationUser> RegisterAsync(RegisterModel model , IFormFile Image)
        {
			var savedImagePath = Image != null && Image.Length > 0
		   ? await _imageService.SaveImageAsync(Image, @"Images/UserImages/")
		   : @"Images/UserImages/person.png";


			var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                UserImage = savedImagePath
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
				throw new InvalidOperationException("Registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
			}

			return user;
		}

        public async Task<string> LoginAsync(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userRepository.GetByEmailAsync(model.Email);
                return await GenerateJwtToken(user);  
            }

            throw new UnauthorizedAccessException("Invalid login attempt");
        }


        public async Task<ApplicationUser?> GetUserProfileAsync(string token)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Token == token);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid or expired token.");
            }
            return user;
        }

		public async Task<ApplicationUser> EditProfileAsync(string userId, EditProfileModel model)
		{
			var user = await _userManager.FindByIdAsync(userId);
			
			if (user == null)
			{
				throw new KeyNotFoundException("User not found.");
			}

			user.Name = model.Name ?? user.Name;
			user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;
			user.City = model.City ?? user.City;
			user.Country = model.Country ?? user.Country;
			user.Address = model.Address ?? user.Address;

			if (model.Image != null && model.Image.Length > 0)
			{
				user.UserImage = await _imageService.SaveImageAsync(model.Image, @"Images/UserImages/");
			}
			else if (string.IsNullOrEmpty(user.UserImage))
			{
				user.UserImage = @"Images/UserImages/person.png";
			}

			if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
			{
				user.Email = model.Email;
				user.UserName = model.Email;
			}

			if (!string.IsNullOrEmpty(model.Password))
			{
				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

				if (!result.Succeeded)
				{
					throw new InvalidOperationException("Failed to update password.");
				}
			}
			_userRepository.Update(user);
			return user;
		}


		public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); 
            }

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds);

			var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

			user.Token = tokenString;
			await _userManager.UpdateAsync(user);

			return tokenString;
        }

        public async Task<bool> SendVerificationCodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false; 
            }

            var verificationCode = new Random().Next(100000, 999999).ToString();

            user.VerificationCode = verificationCode;
            user.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(10);

            var emailSubject = "Password Reset Verification Code";
			var logoUrl = "https://lazza-opal-vercel-app.runasp.net/images/UserImages/logo2.jpg";

			var emailBody = $@"
    <div style='font-family: Arial, sans-serif;'>
        <div style='text-align: center;'>
            <img src='{logoUrl}' alt='Store Logo' style='
             max-width: 150px; 
             border-radius: 50%;  
             margin-bottom: 20px;'
            />
        </div>
        <h2 style='color: #333;'>Password Reset Verification Code</h2>
        <p style='color: #555;'>
            Hello, <br /><br />
            You recently requested to reset your password. Please use the following verification code to proceed:
        </p>
        <p style='font-size: 24px; font-weight: bold; color: #007bff;'>
            {verificationCode}
        </p>
        <p style='color: #555;'>
            This code will expire in 10 minutes. If you did not request this, please ignore this email.
        </p>
        <p style='color: #555;'>Thank you,<br />The Support Team</p>
    </div>";
			//var emailBody = $"Your verification code is: {verificationCode}";

            await _emailSender.SendEmailAsync(email, emailSubject, emailBody);

            await _userManager.UpdateAsync(user); // Save changes

            return true;
        }

		public async Task<IdentityResult> CheckCode(string verificationCode)
		{
			var user = await _userRepository.GetByCode(verificationCode);	
			if (user == null ||
			(user.VerificationCodeExpiry.HasValue && user.VerificationCodeExpiry < DateTime.UtcNow)) 
            {
				return IdentityResult.Failed(new IdentityError { Description = "Invalid or expired verification code." });
			}
			return IdentityResult.Success;
		}


		public async Task<IdentityResult> ResetPasswordAsync(string email, string newPassword , string confirmpassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid or expired verification code." });
            }

            if(newPassword == confirmpassword)
			{
				var removePasswordResult = await _userManager.RemovePasswordAsync(user);
				if (!removePasswordResult.Succeeded)
				{
					return IdentityResult.Failed(new IdentityError { Description = "Error removing existing password." });
				}

				var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
				if (!addPasswordResult.Succeeded)
				{
					return IdentityResult.Failed(new IdentityError { Description = "Error adding new password." });
				}

				user.VerificationCode = null;
				user.VerificationCodeExpiry = null;
				await _userManager.UpdateAsync(user);

				return IdentityResult.Success;
			}

			return IdentityResult.Failed(new IdentityError { Description = "New password does not match the confirmation password." });
		}

		public async Task<GooglePayload?> VerifyGoogleTokenAsync(string idToken)
		{
			var payloadUrl = $"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}";
			var response = await _httpClient.GetAsync(payloadUrl);
			if (!response.IsSuccessStatusCode)
			{
				return null;
			}

			var payloadJson = await response.Content.ReadAsStringAsync();
			return JObject.Parse(payloadJson).ToObject<GooglePayload>();
		}

		// Facebook Login
		/*public async Task<ApplicationUser> VerifyFacebookTokenAsync(string accessToken)
		{
			using var httpClient = new HttpClient();
			var fbApiUrl = $"https://graph.facebook.com/me?access_token={accessToken}&fields=id,name,email";
			var response = await httpClient.GetStringAsync(fbApiUrl);
			var fbUser = JObject.Parse(response);

			if (fbUser.ContainsKey("email"))
			{
				var email = fbUser["email"].ToString();
				var user = await GetOrCreateExternalLoginUserAsync(email, fbUser["name"].ToString(), "Facebook");
				return user;
			}
			throw new UnauthorizedAccessException("Facebook token validation failed.");
		}*/

		public async Task<FacebookUser?> VerifyFacebookTokenAsync(string accessToken)
		{
			var fields = "id,name,email";
			var fbUrl = $"https://graph.facebook.com/me?fields={fields}&access_token={accessToken}";
			var response = await _httpClient.GetAsync(fbUrl);
			if (!response.IsSuccessStatusCode)
			{
				return null;
			}

			var fbUserJson = await response.Content.ReadAsStringAsync();
			return JObject.Parse(fbUserJson).ToObject<FacebookUser>();
		}


		// Twitter Login
		public async Task<ApplicationUser> VerifyTwitterTokenAsync(string oauthToken, string oauthVerifier)
		{
			var twitterUser = await _twitterService.ValidateTwitterTokenAsync(oauthToken, oauthVerifier);
			if (twitterUser != null)
			{
				var user = await GetOrCreateExternalLoginUserAsync(twitterUser.Email, twitterUser.Name, "Twitter");
				return user;
			}
			throw new UnauthorizedAccessException("Twitter token validation failed.");
		}

		public async Task<ApplicationUser> GetOrCreateExternalLoginUserAsync(string email, string name, string provider)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				user = new ApplicationUser
				{
					UserName = email,
					Email = email,
					Name = name,
					Provider = provider,
					UserImage = @"Images/UserImages/person.png"
				};

				var createUserResult = await _userManager.CreateAsync(user);
				if (!createUserResult.Succeeded)
				{
					throw new Exception("User creation failed: " + string.Join(", ", createUserResult.Errors.Select(e => e.Description)));
				}

				var loginInfo = new UserLoginInfo(provider, email, provider);

				var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
				if (!addLoginResult.Succeeded)
				{
					throw new Exception("Failed to add external login: " + string.Join(", ", addLoginResult.Errors.Select(e => e.Description)));
				}
			}
			return user;
		}
	}
}

