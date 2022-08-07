module DbContext

open System.ComponentModel.DataAnnotations
open Microsoft.EntityFrameworkCore
open EntityFrameworkCore.FSharp.Extensions
open Model
open Microsoft.EntityFrameworkCore.Design
open Microsoft.Extensions.DependencyInjection

//type AppDbContext(host: string, port: int, username: string, password: string, database: string) =
type AppDbContext(options: DbContextOptions<AppDbContext>) =
    inherit DbContext(options)

    [<DefaultValue>] val mutable blogs : DbSet<Blog>
    member this.Blogs
        with get() = this.blogs
        and set v = this.blogs <- v

    //member val Blogs : DbSet<Blog> = null with get, set

    override _.OnModelCreating builder =
        builder.RegisterOptionTypes() // enables option values for all entities

    //override __.OnConfiguring(options: DbContextOptionsBuilder) : unit =
        //options.UseNpgsql("Data Source=blogging.db").UseFSharpTypes() |> ignore        
        //options.UseNpgsql($"Server={host};Port={port};Database={database};User Id={username};Password={password}").UseFSharpTypes() |> ignore


// This class only exists so that the ef tools can create an instance of the DbContext at design time (e.g. when creating a migration)
type AppDbContextFactory() =
    interface IDesignTimeDbContextFactory<AppDbContext> with
        member this.CreateDbContext(args : string[]) =
            let options = new DbContextOptionsBuilder<AppDbContext>()
            options.UseNpgsql($"Server=;Port={5000};Database=;User Id=;Password=").UseFSharpTypes() |> ignore
            //new AppDbContext("", 5000, "", "", "")
            new AppDbContext(options.Options)

let configureDbContext (options : DbContextOptionsBuilder) (host: string, port: int, database: string, username: string, password: string) =
    let assemblyName = typeof<AppDbContext>.Assembly.FullName
    options.UseNpgsql($"Server={host};Port={port};Database={database};User Id={username};Password={password}", fun x -> x.MigrationsAssembly assemblyName |> ignore).UseFSharpTypes()
