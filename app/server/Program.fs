module Program


open Giraffe
open Shared
open Server
open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Serilog
open Serilog.Events
open Microsoft.AspNetCore.Hosting

open System.ComponentModel.DataAnnotations
open Microsoft.EntityFrameworkCore
open EntityFrameworkCore.FSharp.Extensions
open Model
open Microsoft.EntityFrameworkCore.Design
open System
open Microsoft.Extensions.Options

let webApi =
    Remoting.createApi()
    |> Remoting.fromContext (fun (ctx: HttpContext) -> ctx.GetService<ServerApi>().Build())
    |> Remoting.withRouteBuilder routerPaths
    |> Remoting.buildHttpHandler

let webApp = choose [ webApi; GET >=> text "Welcome to full stack F#" ]

let configureServices (ctx : WebHostBuilderContext) (services : IServiceCollection) =
    let config = ctx.Configuration

    let getDbConfig (sp: IServiceProvider) =
        let appSettings = sp.GetRequiredService<IOptions<Configuration.AppSettings>>()
        let dbConfig = appSettings.Value.DbApplicationData
        {
            DbContext.DbSettings.Host = dbConfig.Host
            DbContext.DbSettings.Port = dbConfig.Port
            DbContext.DbSettings.Database = dbConfig.Database
            DbContext.DbSettings.Username = dbConfig.Username
            DbContext.DbSettings.Password = dbConfig.Password
        }

    services
        //.Configure<Configuration.DbApplicationData>(config.GetSection("DbApplicationData"))
        .Configure<Configuration.AppSettings>(config)
        .AddSingleton<ServerApi>()
        .AddDbContext<DbContext.AppDbContext>(fun sp options -> getDbConfig sp |> DbContext.configureDbContext options |> ignore)
        .AddResponseCompression()
    |> Database.Startup.AddDatabaseServices getDbConfig
    |> ignore
        

let builder = WebApplication.CreateBuilder()
builder.Host.UseSerilog() |> ignore
builder.WebHost
    .ConfigureServices(configureServices)
    |> ignore


let app = builder.Build()


app.UseGiraffe webApp
app
    .UseStaticFiles("/wwwroot")
    .UseResponseCompression() 
    |> ignore

do 

Log.Logger <- LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
try
    try 
        Log.Information("Applying database migrations")
        use scope = app.Services.CreateScope()
        let db = scope.ServiceProvider.GetRequiredService<DbContext.AppDbContext>()
        db.Database.Migrate()

        Log.Information("Starting web host")
        app.Run("http://0.0.0.0:5000")
    with 
        | ex -> 
            Log.Fatal(ex, "Error starting web host")
finally
    Log.CloseAndFlush()