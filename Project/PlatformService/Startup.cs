using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PlatformService.Data;
using PlatformService.SynchDataServices.Http;

namespace PlatformService
{
    public class Startup
    {
        public IConfiguration _configuration { get; }
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
            });

            if (_webHostEnvironment.IsProduction())
            {
                Console.WriteLine($"--> Using SQL Server DB");
                services.AddDbContext<AppDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("PlatformServiceConnection")));
            }
            else
            {
                Console.WriteLine($"--> Using InMemoryPlatformDb");
                services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMemoryPlatformDb"));
            }

            services.AddScoped<IPlatformRepository, PlatformRepository>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

            Console.WriteLine($"--> CommandService Endpoint: {_configuration["CommandService"]}");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            DbSetup.DbPopulation(app, _webHostEnvironment.IsProduction());
        }
    }
}
