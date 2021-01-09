# .NET-Core-Network

Twitter like social media site using .NET Core 3.1, Razor Pages and Web Api Controllers.

This web app was developed very quickly (less than 7 days) and the core functionaity is in place, however as always some tasks are outstanding. See here for remaining task - https://github.com/mh453Uol/Network/projects/1

Here is a video demoing the different features - https://vimeo.com/498706108

# Development

## Set up
1. Go to ./Network/Network folder and run `dotnet restore` (Install dependencies)
2. Go to ./Network/Network.Data/ folder and run ```dotnet ef database update -s ..\Network\``` (We use Sqlite as a database)
3. Go to ./Network/Network folder and run dotnet watch run (Run web app in watch mode)

## Create Migrations
Navigate to ./Network.Data/ folder run ```dotnet ef migrations add add-likes -s ..\Network\``` -s flag mark the startup path

## Apply Database Migrations
Navigate to ./Network.Data/ folder run ```dotnet ef database update -s ..\Network\```
