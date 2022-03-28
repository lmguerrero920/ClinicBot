// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EmptyBot v4.15.2

using BotClinic.Dialogs;
using BotClinic.Infrastructure.Data;
using BotClinic.Infrastructure.LUIS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure.Blobs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BotClinic
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

            var storage = new BlobsStorage(
            Configuration.GetSection("StorageConnectionString").Value,
            Configuration.GetSection("StorageContainer").Value
              );

            var userState = new UserState(storage);
            services.AddSingleton(userState);
            var conversationState = new ConversationState(storage);

            services.AddSingleton(conversationState);


            services.AddHttpClient().AddControllers().AddNewtonsoftJson();

            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

            // Create the Bot Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            services.AddSingleton<ILuisService, LuisService>();
            services.AddDbContext<DataBaseServie>(options =>
            {
                options.UseCosmos(
                    Configuration["CosmosEndPoint"],
                    Configuration["Cosmoskey"],
                    Configuration["CosmosDatabase"]
                    );
            }
            );

            services.AddScoped<IDataBaseService, DataBaseServie>();

            services.AddTransient<RootDialog>();
            services.AddTransient<IBot, ClinicBot<RootDialog>>();
            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
           // services.AddTransient<IBot, ClinicBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
