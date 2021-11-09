# Microservices .NET 5

## From Course
- Les Jackson https://www.youtube.com/watch?v=DgVjEo3OGBI&t=1246s

### .Net 5 commands
- dotnet --version
- dotnet new webapi -n {name}
- dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
- dotnet add package Microsoft.EntityFrameworkCore --version 5.0.12
- dotnet add package Microsoft.EntityFrameworkCore.Design -v 5.0.12
- dotnet add package Microsoft.EntityFrameworkCore.InMemory -v 5.0.12
- dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 5.0.12
- dotnet build
- dotnet run

### Docker help
- Dockerize an ASP.NET Core application https://docs.docker.com/samples/dotnetcore/
- docker --version
- Build an image: docker build -t {your docker hub id}/{image name} . --> julianecheverri23/platformservice (dont forget the period!)
- For running an image as a container: docker run -p 8080:80 -d {your docker hub id}/{image name} --> julianecheverri23/platformservice
- For seeing containers running: docker ps
- For stop container: docker stop {CONTAINER ID}
- For start a container: docker start {CONTAINER ID}
- For publish container: docker push {your docker hub id}/{image name}
- Running service url http://localhost:8080/api/platforms/