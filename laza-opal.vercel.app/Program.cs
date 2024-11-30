using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Google;
using System.Net.Mail;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace laza_opal.vercel.app
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// JSON Serializer Settings
			builder.Services.AddControllers(options =>
			{
				options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError));
			}
			).AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			});

			// Database Context
			builder.Services.AddDbContext<AppDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
			);

			builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

			// Identity
			builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<AppDbContext>()
				.AddDefaultTokenProviders();

			// SMTP Client for Email - Secure Credentials
			builder.Services.AddSingleton<SmtpClient>(provider => new SmtpClient
			{
				Host = "smtp.gmail.com",
				Port = 587,
				EnableSsl = true,
				Credentials = new NetworkCredential(
					builder.Configuration["Smtp:Username"],
					builder.Configuration["Smtp:Password"]
				)
			});
			builder.Services.AddTransient<IEmailSender, EmailSender>();

			// CORS Policy
			builder.Services.AddCors(options => options.AddPolicy("MyPolicy", policy =>
			{
				policy.WithOrigins("https://lazza-opal-vercel-app.runasp.net", "http://localhost:44395", "http://127.0.0.1:5500")
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials();
			}));

			// Session Configuration
			builder.Services.AddDistributedMemoryCache();
			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			// Authentication Configuration
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
			{
				options.SaveToken = true;
				options.RequireHttpsMetadata = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = builder.Configuration["Jwt:Issuer"],
					ValidAudience = builder.Configuration["Jwt:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
				};
				options.Events = new JwtBearerEvents
				{
					OnChallenge = context =>
					{
						context.HandleResponse();
						context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
						context.Response.ContentType = "application/json";

						var result = JsonConvert.SerializeObject(new
						{
							StatusCode = context.Response.StatusCode,
							Message = "Unauthorized access. Please check your token and try again.",
							Details = "Possible reasons: invalid token, expired token, or insufficient permissions."
						});

						return context.Response.WriteAsync(result);
					}
				};
			})
			//.AddCookie()
			.AddGoogle("Google", options =>
			{
				options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
				options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
				options.CallbackPath = new PathString("/auth/login-google");
				options.SaveTokens = true;
			}).
			AddFacebook("Facebook", options =>
			{
				options.AppId = builder.Configuration["Authentication:FacebookAuth:AppId"];
				options.AppSecret = builder.Configuration["Authentication:FacebookAuth:AppSecret"];
			});

			builder.Services.AddHttpClient();
			// Authorization Policies
			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
				options.AddPolicy("User", policy => policy.RequireRole("User"));
			});

			// Swagger Setup
			builder.Services.AddSwaggerGen(swagger =>
			{
				swagger.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "ASP.NET 8 Web API",
					Description = "lazza-opel.vercel"
				});
				swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Enter 'Bearer' [space] and then your valid token in the text input below. Example: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
				});
				swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						Array.Empty<string>()
					}
				});
			});

			// Repository and Service DI Registration
			builder.Services.AddScoped<ICartRepository, CartRepository>();
			builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddScoped<IBrandRepository, BrandRepository>();
			builder.Services.AddScoped<IProductRepository, ProductRepository>();
			builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
			builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
			builder.Services.AddScoped<IAuthService, AuthService>();
			builder.Services.AddScoped<ICardRepository, CardRepository>();
			builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
			builder.Services.AddScoped<IOrderRepository, OrderRepository>();
			builder.Services.AddScoped<IImageService, ImageService>();
			builder.Services.AddScoped<ITwitterAuthService, TwitterAuthService>();
			builder.Services.AddScoped<StripePaymentService>();

			// Build App
			var app = builder.Build();

			// Middleware Order
			app.UseSession();
			app.UseSwagger();
			app.UseSwaggerUI();
			app.UseHttpsRedirection();
			//app.UseRouting();
			StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
			app.UseHsts();
			app.UseStaticFiles();
			app.UseCors("MyPolicy");
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();
			//app.UseMiddleware<ExceptionMiddleware>();
			app.Run();
		}
	}
}
