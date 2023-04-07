# Graduation_Project

Getting Started
To use this project, please follow the steps below:

1- Change the connection string in the appsettings.json file located in the root of the project.
    Replace the value of the DefaultConnection key with the connection string for your database.
    
3-Update the Cloudinary settings in the same appsettings.json file.
    Replace the CloudName, ApiKey, and ApiSecret values with your own Cloudinary API credentials.
    
"
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "CloudinarySettings": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}"

3- Open the Package Manager Console in Visual Studio by going to Tools > NuGet Package Manager > Package Manager Console.

4- Run the command update-database in the Package Manager Console to create the database schema.
