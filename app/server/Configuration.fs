module Configuration

[<CLIMutable>]
type DbApplicationData = {
    Host: string
    Port: int
    Username: string
    Password: string
    Database: string
}

[<CLIMutable>]
type AppSettings = {
    DbApplicationData : DbApplicationData
}