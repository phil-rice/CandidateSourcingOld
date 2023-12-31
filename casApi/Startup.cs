﻿namespace casApi
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using xingyi.cas.common;
    using xingyi.common;
    using Microsoft.OpenApi.Models;
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<CasDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("CAS"),
                    b => b.MigrationsAssembly("casApi")
                )
            );
            services.AddScoped<ICasRepository, CasRepository>();

            services.AddSingleton<IShaCodec, ShaCodec>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Content Addressable Storage", Version = "v1.0" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // Use HSTS if required
                // app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Content Addressable Storage");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // If using JWT, insert the JWT middleware here.
            // app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();  // Maps to the Controllers for routing.
                                             // Add other endpoint mappings as necessary.
            });
        }
    }
}
