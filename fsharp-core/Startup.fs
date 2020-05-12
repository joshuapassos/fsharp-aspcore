namespace fsharp_core

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open fsharp_core.Data.CoreContext
open Microsoft.EntityFrameworkCore

type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        // Add framework services.
        services.AddControllers() |> ignore
        services.AddHealthChecks() |> ignore
        services.AddOpenApiDocument(fun doc ->
            doc.Title <- "Core API"
        ) |> ignore
        
        services.AddDbContext<CoreContext>(fun optionsBuilder ->
            let connectionString = this.Configuration.GetConnectionString("Default")
            optionsBuilder
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention() |> ignore
        ) |> ignore 


    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        
        app.UseHttpsRedirection() |> ignore
        app.UseRouting() |> ignore

        app.UseAuthorization() |> ignore
        
        app.UseEndpoints(fun endpoints ->
            endpoints.MapHealthChecks("/health") |> ignore
            endpoints.MapControllers() |> ignore
        ) |> ignore
        
        app.UseOpenApi(fun config ->
            config.PostProcess <- fun document request -> () 
        ) |> ignore
        
        app.UseSwaggerUi3() |> ignore


    member val Configuration : IConfiguration = null with get, set
