namespace Database

open EntityFrameworkCore.FSharp.DbContextHelpers

open DbContext
open Model

type Repository(context : AppDbContext) =
    member _.getBlog(id : int) =
        tryFindEntityAsync<Blog> context id
