namespace eventApi
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using xingyi.cas.client;
    using xingyi.cas.common;
    using xingyi.common;
    using xingyi.common.http;
    using xingyi.events;

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

            services.AddDbContext<EventStoreDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("EventStore"),
                    b => b.MigrationsAssembly("eventApi")
                )
            );
            services.AddSingleton<IShaCodec, ShaCodec>();
            services.AddScoped<ICasJsonGetter, CasClient>();
            services.AddHttpClient(CasClient.HttpClientName, client =>
            {
                client.BaseAddress = new Uri("https://localhost/");
            });

            services.AddScoped<ICasAdder, CasClient>();
            services.AddScoped<IEventExecutor<Dictionary<string, object>>, JsonEventExecutor>();
            services.AddScoped<IEventStoreGetter, EventStoreRepository>();
            services.AddScoped<IEventStoreAdder, EventStoreRepository>();


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
