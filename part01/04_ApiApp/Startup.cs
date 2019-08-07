namespace commasoft.Workshop.ApiApp
{
    using System;
    using System.Linq;

    using Clients;

    using Interfaces;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        #region constants

        private const string CorsConfig = "Default";

        #endregion

        #region constructors and destructors

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        #region methods

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseCors(CorsConfig);
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(
                c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Comma API V1");
                });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(
                options =>
                {
                    options.AddPolicy(
                        CorsConfig,
                        builder =>
                        {
                            builder.WithOrigins(Configuration["Cors:Origins"]);
                        });
                });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            // Swagger
            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(
                        "v1",
                        new Info
                        {
                            Title = "Comma API",
                            Version = "v1"
                        });
                    c.DescribeAllEnumsAsStrings();
                    c.EnableAnnotations();
                    c.IncludeXmlComments($"{AppDomain.CurrentDomain.BaseDirectory}/Workshop.ApiApp.xml");
                });
            // DI
            services.AddHttpClient<IOpenWeatherMapClient, OpenWeatherMapClient>();
        }

        #endregion

        #region properties

        public IConfiguration Configuration { get; }

        #endregion
    }
}