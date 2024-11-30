namespace Lazza.opal.persistence.Repository
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserRepository(AppDbContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _context.Users.OfType<ApplicationUser>().ToListAsync();
        }

        public async Task<ApplicationUser> GetByCode(string code)
        {
            return await _context.Users.OfType<ApplicationUser>().Where(u=>u.VerificationCode == code).SingleOrDefaultAsync();

        }
    }
}
