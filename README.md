# .NET-Core-Network

Twitter like social media site using .NET Core 3.0 & Razor Pages


# Development

## Create Migrations
Navigate to ./Network.Data/ folder run ```dotnet ef migrations add add-likes -s ..\Network\``` -s flag mark the startup path

## Apply Database Migrations
Navigate to ./Network.Data/ folder run ```dotnet ef database update -s ..\Network\```