namespace Database

open DbContext

type Repository(context : AppDbContext) =
    member _.getBlog(id) =
        printfn "Hello %s" id // TODO