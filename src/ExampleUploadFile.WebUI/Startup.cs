using ExampleUploadFile.WebUI.Data;
using ExampleUploadFile.WebUI.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace ExampleUploadFile.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            #region snippet_AddRazorPages
            services.AddRazorPages()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions
                        .AddPageApplicationModelConvention("/StreamedSingleFileUploadDb",
                            model =>
                            {
                                model.Filters.Add(
                                    new GenerateAntiforgeryTokenCookieAttribute());
                                model.Filters.Add(
                                    new DisableFormValueModelBindingAttribute());
                            });
                    options.Conventions
                        .AddPageApplicationModelConvention("/StreamedSingleFileUploadPhysical",
                            model =>
                            {
                                model.Filters.Add(
                                    new GenerateAntiforgeryTokenCookieAttribute());
                                model.Filters.Add(
                                    new DisableFormValueModelBindingAttribute());
                            });
                });
            #endregion
            
            // To list physical files from a path provided by configuration:
            var physicalProvider = new PhysicalFileProvider(Configuration.GetValue<string>("StoredFilesPath"));

            // To list physical files in the temporary files folder, use:
            //var physicalProvider = new PhysicalFileProvider(Path.GetTempPath());

            services.AddSingleton<IFileProvider>(physicalProvider);

            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMemoryDb"));
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}