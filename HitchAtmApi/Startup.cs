using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HitchAtmApi.Lib;
using System.Reflection;
using System;
using Swashbuckle.AspNetCore.Filters;
using Hangfire;
using Hangfire.PostgreSql;

namespace HitchAtmApi
{
    public class Startup
    {        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Utils.ATTACHMENTS_FOLDER = Configuration.GetValue<string>("AttachmentFolder");
            Utils.SalesforceApiVersion = Configuration.GetValue<string>("SalesforceApiVersion");
            Utils.SalesforceInstanceUrl = Configuration.GetValue<string>("SalesforceInstanceUrl");
            Utils.Credentials = new Credentials
            {
                ClientId = Configuration.GetValue<string>("SalesforceAuth:ClientId"),
                ClientSecret = Configuration.GetValue<string>("SalesforceAuth:ClientSecret"),
                GrantType = Configuration.GetValue<string>("SalesforceAuth:GrantType"),
                Password = Configuration.GetValue<string>("SalesforceAuth:Password"),
                Token = Configuration.GetValue<string>("SalesforceAuth:Token"),
                Username = Configuration.GetValue<string>("SalesforceAuth:Username")
            };
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Habilitar cors y permitir cualquier host (para pruebas)
            services.AddCors(options => options.AddPolicy("AllowAllOrigins", corsBuilder => corsBuilder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()));

            // Definir tamaño de request al maximo permitido
            services.AddOptions();
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.ValueCountLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

            ConnectionParameters connectionParameters = new ConnectionParameters
            {
                Server = Configuration.GetValue<string>("Database:Server"),
                Database = Configuration.GetValue<string>("Database:Name"),
                User = Configuration.GetValue<string>("Database:User"),
                Password = Configuration.GetValue<string>("Database:Password")
            };

            string sqlServerConnectionString = Configuration.GetValue<string>("SqlServerConnection");
            string urlApi = Configuration.GetValue<string>("UrlApi");

            // Inyectar servicios y configuracion como dependencias
            services.AddSingleton(Configuration);
            
            services.AddSingleton(new Hs2Service(sqlServerConnectionString, urlApi));
            services.AddSingleton(new LogService(connectionParameters));
            services.AddSingleton(new NotificationsService(connectionParameters));
            services.AddSingleton(new SalesOrdersService(connectionParameters));
            services.AddSingleton(new PurchasesOrdersService(connectionParameters));
            services.AddSingleton(new TransfersRequestsService(connectionParameters));
            services.AddSingleton<SapService>();

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Hitch ATM API", Version = "v1" });

                x.ExampleFilters();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                x.IncludeXmlComments(xmlPath);
            });

            services.AddSwaggerExamplesFromAssemblyOf<Startup>();

            string postgresConnectionString = $"Host={connectionParameters.Server};Port=5432;Database={connectionParameters.Database};User Id={connectionParameters.User};Password={connectionParameters.Password};";

            services.AddHangfire(hangfire => {
                hangfire.UsePostgreSqlStorage(postgresConnectionString);
            });

            services.AddHangfireServer();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireDashboardAuthorization() } // Configura la autorización si es necesario
            });

            app.UseHsts();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hitch ATM Api API V1");
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMiddleware<ApiLoggerMiddleware>();
            app.UseMvc();
        }
    }
}
