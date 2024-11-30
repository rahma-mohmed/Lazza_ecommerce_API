﻿namespace Lazza.opal.Application.DTO
{
	public class EditProfileModel
	{
		public string? Email { get; set; }
		public string? Password { get; set; }
		public string? Name { get; set; }
		public IFormFile? Image { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Address { get; set; }
		public string? City { get; set; }
		public string? Country { get; set; }
	}
}
