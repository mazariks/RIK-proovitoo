# Event system


// new migration

dotnet ef migrations add InitialCreate --project DAL --startup-project WebApp

// update database

dotnet ef database update --project DAL --startup-project WebApp

// drop database

dotnet ef database drop --project DAL --startup-project WebApp

// CRUD controller

cd App

dotnet aspnet-codegenerator razorpage -m Event -dc ApplicationDbContext -udl -outDir Pages\Events --referenceScriptLibraries
---
dotnet aspnet-codegenerator razorpage -m Participant -dc ApplicationDbContext -udl -outDir Pages\Participants --referenceScriptLibraries
---