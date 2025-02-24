**How to run application locally:**
- Prepare a new and empty SQLExpress DB and set up "ConnectionString" in the appsettings.json file.
- Set "FilePath" for logs (note: use the full path, not a relative path).
- Build project (+ restore NuGet packages for the project)
- Run the "update-database" command from the Package Manager Console to generate DB tables.
- Seeding will happen automatically on application startup if DB tables are empty. If they are not empty, seeding will be skipped. Seeding parameters can be adjusted in the appsettings.json file.
- Build and run the application.
- I used Scalar UI for the API Client. Access it at: [https://localhost:7110/scalar/v1](https://localhost:7110/scalar/v1)

**Testing API:**
- Initially, there are no users in the DB (and no seeders for it either):

1. Register a new user with a username and password.
2. Use the login endpoint and registered user's credentials to obtain a JWT token.
3. Use the Authorization header with the acquired JWT token to access and test all other endpoints.

**Improvements and discussion points**
- Using secrets manager for secrets
- DI for weather forcast service
- relative path for the logs
- cascading delete (or soft delete?) for PowerPlants/Readings
- code refactoring
