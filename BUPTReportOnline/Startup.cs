using BUPTReportOnline.IServices;
using BUPTReportOnline.Jobs;
using BUPTReportOnline.Models;
using BUPTReportOnline.Services;
using FluentScheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;

namespace BUPTReportOnline
{
    public class Startup
    {
        public static IServiceProvider serviceProvider { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var ConnectionString = Configuration.GetConnectionString("BROContext");
            GlobalConfig.InitConfig(Configuration.GetSection("Config"));
            var CorsTarget = Configuration.GetSection("CorsTarget").Value;
            services.AddDbContext<BROContext>(opt =>
                opt.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString)));
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AutoMapperConfig>();
            }, AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IUserManagement, UserManagement>();
            services.AddCors(options =>
            {
                options.AddPolicy("cors", p =>
                {
                    p.WithOrigins(CorsTarget)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BUPTReportOnline", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BUPTReportOnline v1"));
            }

            app.UseCors("cors");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            serviceProvider = app.ApplicationServices;
            InitJobs();
        }

        private void InitJobs()
        {
            var context = serviceProvider.CreateScope().ServiceProvider.GetService<BROContext>();
            context.Database.EnsureCreated();
            var users = context.User.Where(u => u.Registered).ToList();
            foreach (var user in users)
            {
                if (user.GUID != "REMOVE_INIT")
                {
                    JobManager.AddJob(new SaveJob(serviceProvider, user.GUID), t =>
                    {
                        t.WithName(user.GUID).ToRunEvery(1).Days().At(user.StartHour, user.StartMinute);
                    });
                }
            }
            JobManager.Start();
        }
    }
}
