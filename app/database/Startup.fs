namespace Database

module Startup =

    open System
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.Extensions.DependencyInjection.Extensions
    open Database
    open DbContext

    let AddDatabaseServices (provideDbSettings: IServiceProvider -> DbSettings) (services : IServiceCollection) =
        services
            .AddDbContext<AppDbContext>(fun sp options -> provideDbSettings sp |> DbContext.configureDbContext options |> ignore)
            .TryAddScoped<Repository>()
            |> ignore

        services