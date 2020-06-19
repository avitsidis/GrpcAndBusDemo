using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.AspNetCore.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleInjector;
using TodoService.Services;

namespace TodoService.Hosting
{
    //https://github.com/simpleinjector/SimpleInjector/issues/785
    public class GrpcSimpleInjectorActivator<T> : IGrpcServiceActivator<T>
    where T : class
    {
        private readonly Container container;

        public GrpcSimpleInjectorActivator(Container container) => this.container = container;

        public GrpcActivatorHandle<T> Create(IServiceProvider serviceProvider) =>
            new GrpcActivatorHandle<T>(container.GetInstance<T>(), false, null);

        public ValueTask ReleaseAsync(GrpcActivatorHandle<T> service) => default;
    }

    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddHealthChecks();
            services.AddSingleton(typeof(IGrpcServiceActivator<>),
            typeof(GrpcSimpleInjectorActivator<>));


            services.AddSimpleInjector(CompositionRoot.Container, options =>
            {
                // AddAspNetCore() wraps web requests in a Simple Injector scope and
                // allows request-scoped framework services to be resolved.
                options.AddAspNetCore();
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSimpleInjector(CompositionRoot.Container);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");

                endpoints.MapGrpcService<TodoGrpcService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
