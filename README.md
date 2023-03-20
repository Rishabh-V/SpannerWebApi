# Spanner WebApi

A simple CRUD Web API for Singers in a Spanner Database.

## Getting started

- Clone this repo. (Needs .NET 6 to run)
- Update Spanner ConnectionString in `appsettings.json` and run the app locally. 
- A background service runs runs and ensures that the database, table and test data is created in the database specified in the ConnectionString. 
- The app should run fine and display the Swagger UI in the app startup by which CRUD operations can be done on Singers table.
- Go to `Program.cs` and follow the instructions towards the end of `Main` method to deploy the app to CloudRun using the steps [here](https://cloud.google.com/run/docs/quickstarts/build-and-deploy/deploy-dotnet-service)
