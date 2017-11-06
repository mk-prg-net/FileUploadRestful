using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.IO;

namespace Server.AspNetCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // mko, 6.11.2017
            // DI- register class factory. Scoped means creating one instance available during the entire request. 

            services.AddScoped<FileUploadRestful.FileBuilder.IFileBuilder, FileUploadRestful.Impl.FilesystemFileBuilder>();
            services.AddScoped<FileUploadRestful.UploadServer.IUploadServer, FileUploadRestful.UploadServer.UploadServerV1>(sp => 
            {
                var tempDir = Environment.ExpandEnvironmentVariables("%TEMP%");
                var queuesDir = Path.Combine(tempDir, Guid.NewGuid().ToString("D"));
                Directory.CreateDirectory(queuesDir);
                var updSvr =  new FileUploadRestful.UploadServer.UploadServerV1(queuesDir);
                var fb = sp.GetRequiredService<FileUploadRestful.FileBuilder.IFileBuilder>();
                updSvr.DefineFileBuilder(fb);
                return updSvr;
            });          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
