# GOCAP SOCIAL MEDIA

Welcome to GOCAP Social Media, where we connect and grow our community through engaging and valuable content. We are committed to bringing you the latest news, trending topics, and opportunities for meaningful interactions.

## Technologies
- Backend: ASP.NET CORE API 8, C#, Entity Framework Core, LINQ, DI, AutoMapper, Fluent Validation.
- Database: SQL SERVER, MongoDB.
- Cache: Redis.
- Message Queue: Kafka.
- Frontend: Angular, Typescript, HTML5, CSS, JS, Bootstrap.
- Tool: Git, Github, Postman.

## IDE
Visual Studio 2022, Sql Server 2019, Visual Studio Code, MongoDBCompass.

## How does this source work
- GOCAP.Api -> GOCAP.Services -> GOCAP.Repository -> GOCAP.Database.

## How to run source
- Clone src from github
- Go to appsettings in webapp and modify information to be suitable for your database.
- In GOCAP.Migrations Layer, run some code lines to map code to db (Entity Framework).
  ```sh
   dotnet ef database update
- Run src

## How to use Github

To install this project, follow these steps:

1. Clone the repository:
   ```sh
   git clone https://github.com/VirtualMeetCapstone/Virtual-Meet-BE.git

2. When clone done, create a branch base on master:
   ```sh
   git checkout -b dev-gau
   
2. Implement Coding, Push to created branch (Just push necessary files, not push unnecessary files):
   ```sh
   git add .
   git commit -m"Push Code"
   git push origin dev-gau

2. Create merge request on github:
   - Go to github
   - Choose "Pull Request"
   - Choose "New Pull Request"
   - Choose based on branch is "master" and compare branch is "branch to merge"

