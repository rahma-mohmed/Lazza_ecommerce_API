﻿namespace Lazza.opal.Application.DTO
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        //public string VerificationCode { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
	}
}
