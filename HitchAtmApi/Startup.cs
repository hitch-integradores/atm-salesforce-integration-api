using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HitchAtmApi.Lib;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Hosting;

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

            SapConnectionParameters sapConnectionParameters = new SapConnectionParameters
            {
                DatabaseType = Configuration.GetValue<string>("Sap:DatabaseType"),
                License = Configuration.GetValue<string>("Sap:License"),
                Password = Configuration.GetValue<string>("Sap:Password"),
                Server = Configuration.GetValue<string>("Sap:Server"),
                ServerDatabase = Configuration.GetValue<string>("Sap:ServerDatabase"),
                ServerPassword = Configuration.GetValue<string>("Sap:ServerPassword"),
                ServerUser = Configuration.GetValue<string>("Sap:ServerUser"),
                Username = Configuration.GetValue<string>("Sap:Username")
            };

            // Inyectar servicios y configuracion como dependencias
            services.AddSingleton(Configuration);
            services.AddSingleton(new LogService(connectionParameters));
            services.AddSingleton(new NotificationsService(connectionParameters));
            services.AddSingleton(new SalesOrdersService(connectionParameters));
            services.AddSingleton(new PurchasesOrdersService(connectionParameters));
            services.AddSingleton(new TransfersRequestsService(connectionParameters));
            services.AddSingleton(new SapService(sapConnectionParameters.ToConnectionParameters()));

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
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHsts();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hitch ATM Api API V1");
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            // app.UseMiddleware<ApiLoggerMiddleware>();
            app.UseMvc();
        }
    }
}
