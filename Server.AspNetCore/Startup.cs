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

using Microsoft.AspNetCore.Http.Internal;

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

        const string queuesDirName = "FileUploadRestfulQueues-A85FF241";

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
                var queuesDir = Path.Combine(tempDir, queuesDirName);
                Directory.CreateDirectory(queuesDir);
                var updSvr =  new FileUploadRestful.UploadServer.UploadServerV1(queuesDir);
                var fb = sp.GetRequiredService<FileUploadRestful.FileBuilder.IFileBuilder>();
                updSvr.DefineFileBuilder(fb);
                return updSvr;
            });          
        }

        private static void CheckRequest(IApplicationBuilder app)
        {
            app.Run(async context => {
                var buffer = new byte[0x10000];
                int count = 0;
                while (count < 0x10000)
                {
                    count += await context.Request.Body.ReadAsync(buffer, count, 0x10000 - count);
                }

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.Use(next => context => {
                context.Request.EnableRewind();

                return next(context);
            });

            if (env.IsDevelopment())
            {
                //app.Map("/Upload/UploadChunk", CheckRequest);
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
