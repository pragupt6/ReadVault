param(
    [Parameter(Position = 0)]
    [ValidateSet("restore", "build", "run", "run-no-build", "clean", "ef-migrations-list", "ef-migrations-add", "ef-migrations-remove", "ef-database-update")]
    [string]$Action = "run"
,
    [Parameter(Position = 1)]
    [string]$MigrationName = ""
)

$project = ".\VaultConnection\VaultConnection.csproj"

switch ($Action) {
    "restore" { dotnet restore $project }
    "build" { dotnet build $project }
    "run" { dotnet run --project $project }
    "run-no-build" { dotnet run --no-build --project $project }
    "clean" { dotnet clean $project }
    "ef-migrations-list" { dotnet ef migrations list --project $project --startup-project $project }
    "ef-migrations-add" {
        if ([string]::IsNullOrWhiteSpace($MigrationName)) {
            throw "Please provide a migration name. Example: .\scripts.ps1 ef-migrations-add AddSiteSchema"
        }
        dotnet ef migrations add $MigrationName --project $project --startup-project $project --output-dir Migrations
    }
    "ef-migrations-remove" { dotnet ef migrations remove --project $project --startup-project $project }
    "ef-database-update" { dotnet ef database update --project $project --startup-project $project }
}
