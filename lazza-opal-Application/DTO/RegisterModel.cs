namespace Lazza.opal.Application.DTO
{
    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public IFormFile? Image {  get; set; }
    }
}
