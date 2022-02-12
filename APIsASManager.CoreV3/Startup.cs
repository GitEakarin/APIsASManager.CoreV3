using APIsASManager.CoreV3.MyClass;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.AspNetCore;
using Newtonsoft.Json.Serialization;
using System.Text.Json;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Routing;

namespace APIsASManager.CoreV3
{
    public class Startup
    {
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        static string XmlCommentsFileName
        {
            get
            {
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return fileName;
            }
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            // Add Hangfire services.
            //services.AddHangfire(configuration => configuration
            // .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            // .UseSimpleAssemblyNameTypeSerializer()
            // .UseRecommendedSerializerSettings()
            // .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
            // {
            //     CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            //     SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            //     QueuePollInterval = TimeSpan.Zero,
            //     UseRecommendedIsolationLevel = true,
            //     DisableGlobalLocks = true
            // }));

            //services.AddHangfireServer();

            services.AddDbContext<Models.EFModels.TALogContext>(options => options.UseSqlServer(Configuration.GetConnectionString("TALogConnection"))
            );
            services.AddDbContext<Models.EFModels.TAConfigContext>(options => options.UseSqlServer(Configuration.GetConnectionString("TAConfigConnection"))
            );

            services.AddControllers();

            services.AddApiVersioning(options =>
            {
                // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                options.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(
            options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";
                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(options =>
            {
                // add a custom operation filter which sets default values
                options.OperationFilter<SwaggerDefaultValues>();
                // integrate xml comments
                options.IncludeXmlComments(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), XmlCommentsFileName));
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "attn",
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme{
                    Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                    }
                });
                options.ResolveConflictingActions(x => { return x.FirstOrDefault(); });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("1.0", new OpenApiInfo { Title = "My API V1.0", Version = "1.0", });
            //    c.SwaggerDoc("2.0", new OpenApiInfo { Title = "My API V2.0", Version = "2.0", });
            //    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //    c.IncludeXmlComments(xmlPath);
            //});

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, MyClass.BasicAuthenticationHandler>("BasicAuthentication", null);

            services.AddSingleton<MyClass.IAuthorizationManager>(new MyClass.AuthorizationManager(Configuration));
            services.AddSingleton<MyClass.IASManager>(new MyClass.ASManager(Configuration));
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            #region MQTT Configuration
            
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            applicationLifetime.ApplicationStopping.Register(OnShutdown);

            app.UseSwagger(c => {
                c.RouteTemplate = ("docs/{documentname}/swagger.json");

            });
            //https://medium.com/@hendisuhardja/creating-net-core-api-and-swagger-ui-with-versioning-e21979a54d2c
            app.UseSwaggerUI(
            options =>
            {
                // build a swagger endpoint for each discovered API version  
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    /*options.SwaggerEndpoint($"/docs/{description.GroupName}/swagger.json", */
                    options.SwaggerEndpoint($"/docs/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    options.RoutePrefix = "docs";
                };
            });

            //var varJobOptions = new BackgroundJobServerOptions();
            //varJobOptions.ServerName = "job.asmanager";
            //varJobOptions.ServerTimeout = TimeSpan.FromMinutes(10);
            //varJobOptions.WorkerCount = 10;
            ////Remove Duplicte HangFire Server
            //RemoveHanfireServer(varJobOptions.ServerName);
            //app.UseHangfireServer(varJobOptions);
            //app.UseHangfireDashboard("/hangfire", options: new DashboardOptions { DashboardTitle = "Backgroup Job ASManager" })
            //   .UseCors(builder => builder.AllowAnyHeader());

            //pASManager.ClearBackgroundJobClient();
            //pASManager.BackgroundJobClient = backgroundJobs;
            //pASManager.StartBackgroundJobClient();

            app.UseDeveloperExceptionPage();
            
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete });
            //HangfireJobScheduler.ScheduleRecurringJobs();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                    );
                //endpoints.MapHangfireDashboard();
            });

            //app.UseMvc();
            #region MQTT

            #endregion
        }
        void RemoveHanfireServer(string pName)
        {
            var varMonitoringApi = JobStorage.Current.GetMonitoringApi();
            var varServerList = varMonitoringApi.Servers().Where(r => r.Name.Contains(pName));
            foreach (var varServerItem in varServerList)
            {
                JobStorage.Current.GetConnection().RemoveServer(varServerItem.Name);
            }
        }
        private void OnShutdown()
        {
            //Wait while the data is flushed
            System.Threading.Thread.Sleep(1000);
        }
    }
}
