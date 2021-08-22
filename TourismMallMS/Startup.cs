using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismMallMS.Database;
using TourismMallMS.Models.Entities;
using TourismMallMS.Services;

namespace TourismMallMS
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var secretByte = Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]);
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true, 
                        ValidIssuer = Configuration["Authentication:Issuer"],

                        ValidateAudience = true,
                        ValidAudience = Configuration["Authentication:Audience"],

                        ValidateLifetime = true,

                        IssuerSigningKey = new SymmetricSecurityKey(secretByte)
                    };
                });

            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
            })
                .AddNewtonsoftJson(setupAction =>
                {
                    setupAction.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters()
                .ConfigureApiBehaviorOptions(setupAction => {
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetail = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "",
                            Title = "",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "",
                            Instance = context.HttpContext.Request.Path
                        };
                        problemDetail.Extensions.Add("tracedId", context.HttpContext.TraceIdentifier);
                        return new UnprocessableEntityObjectResult(problemDetail)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                });

            services.AddTransient<ITouristRouteRepository, TouristRouteRepository>(); 

            services.AddDbContextPool<AppDbContext>(options =>
            {
                options.UseMySql(Configuration["ConnectionStrings:MySql"], new MySqlServerVersion(new Version(8, 0, 25)));
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();

            services.AddTransient<IShoppingCartRepository, ShoppingCartRepository>();

            services.AddTransient<IOrderRepository, OrderRepository>();

            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.Configure<MvcOptions>(config =>
            {
                var outputFormatter = config.OutputFormatters
                .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if(outputFormatter != null)
                {
                    outputFormatter.SupportedMediaTypes.Add("application/vnd.bleso.hateoas+json");
                }
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseDataInitializer();

            app.UseRouting(); 

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
