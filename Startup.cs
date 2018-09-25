using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using smartAnalytics.Context;
using smartAnalytics.Interfaces;
using smartAnalytics.Services;

namespace smartAnalytics
{
    public class Startup
    {

        private IHostingEnvironment _hostingEnvironment;
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            _hostingEnvironment = env;
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);;
            var physicalProvider = _hostingEnvironment.ContentRootFileProvider;
            services.AddSingleton<IFileProvider>(physicalProvider);
            services.AddScoped<IRepositoryService, RepositoryService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddDbContext<RepositoryContext>(options => 
                options.UseLazyLoadingProxies().UseSqlite(Configuration.GetConnectionString("SqliteConnection"))
            );
            
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, RepositoryContext rp)
        { 
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                   HotModuleReplacement = true
                   //ReactHotModuleReplacement = true
                });
            }
        
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc();   
        }
    }
}
