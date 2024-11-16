# GOCAP SOCIAL MEDIA

Welcome to GOCAP Social Media, where we connect and grow our community through engaging and valuable content. We are committed to bringing you the latest news, trending topics, and opportunities for meaningful interactions.

## Technologies
- Backend: ASP.NET CORE API 8, C#, Entity Framework Core, LINQ, DI, AutoMapper, Fluent Validation.
- Database: SQL SERVER
- Frontend: Reactjs, Hook, HTML5, CSS, JS, Bootstrap.
- Tool: GITHUB

## IDE
Visual Studio 2022, Sql Server 2019, Visual Studio Code.

## How does this source work
- GOCAP.Api -> GOCAP.Common, GOCAP.Database, GOCAP.Repository, GOCAP.Repository.Intention, GOCAP.Service, GOCAP.Service.Intetion
- GOCAP.Database -> GOCAP.Common
- GOCAP.Repository -> GOCAP.Repository.Intention, GOCAP.Database, GOCAP.Common
- GOCAP.Repository.Intention -> GOCAP.Database, GOCAP.Common
- GOCAP.Service -> GOCAP.Repository, GOCAP.Repository.Intention, GOCAP.Service.Intention,  GOCAP.Database, GOCAP.Common
- GOCAP.Service.Intention -> GOCAP.Database, GOCAP.Common

## How to run source
- Clone src from github
- Go to appsettings in webapp and modify information to be suitable for your database.
- In GOCAP.Database Layer, run some code lines to map code to db (Entity Framework).
  ```sh
   dotnet ef migrations  add "Initial"
   dotnet ef database update
- Run src

## How to use Github

To install this project, follow these steps:

1. Clone the repository:
   ```sh
   git clone https://github.com/TranNgocChi/GOCAP.git

2. When clone done, create a branch base on master:
   ```sh
   git checkout -b NameBranch
   
2. Implement Coding, Push to created branch (Just push necessary files, not push unnecessary files):
   ```sh
   git add .
   git commit -m"Push Code"
   git push origin NameBranch

2. Create merge request on github:
   - Go to github
   - Choose "Pull Request"
   - Choose "New Pull Request"
   - Choose based on branch is "master" and compare branch is "branch to merge"

