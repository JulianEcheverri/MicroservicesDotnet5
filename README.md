# Building .NET Microservices using the REST API pattern

## From Course
- Les Jackson https://www.youtube.com/watch?v=DgVjEo3OGBI&t=1246s

### Overview
- .NET 5
- Entity Framework Core
- Docker
- Kubernetes: Deploying our services to Kubernetes cluster
- HTTP & gRPC: Building Synchronous messaging between services
- RabbitMQ: Building Asynchronous messaging between services using an Event Bus 
- Working with dedicated persistence layers for both services
- NGINX Ingress Controller: Employing the API Gateway pattern to route to our services. https://github.com/kubernetes/ingress-nginx https://kubernetes.github.io/ingress-nginx/deploy/

#### .Net 5 commands
- dotnet --version
- dotnet new webapi -n {name}
- dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
- dotnet add package Microsoft.EntityFrameworkCore --version 5.0.12
- dotnet add package Microsoft.EntityFrameworkCore.Design -v 5.0.12
- dotnet add package Microsoft.EntityFrameworkCore.InMemory -v 5.0.12
- dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 5.0.12
- dotnet build
- dotnet run

#### Docker help
- Dockerize an ASP.NET Core application https://docs.docker.com/samples/dotnetcore/
- docker --version
- Build an image: docker build -t {your docker hub id}/{image name} . --> julianecheverri23/platformservice (dont forget the period!)
- For running an image as a container: docker run -p 8080:80 -d {your docker hub id}/{image name} --> julianecheverri23/platformservice
- For seeing containers running: docker ps
- For stop container: docker stop {CONTAINER ID}
- For start a container: docker start {CONTAINER ID}
- For publish container: docker push {your docker hub id}/{image name}
- Running service url http://localhost:8080/api/platforms/

#### Kubernetes
- kubectl version
- kubectl apply -f {.yml name file} --> kubectl apply -f platforms-depl.yaml
- kubectl get deployments
- kubectl get pods
- kubectl delete deployment {name}
- kubectl get services
- To refresh image after change it on docker hub: kubectl rollout restart deployment {deploy name}
- kubectl get namespace

#### Kubernetes NGINX Ingress Controller
- kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.0.4/deploy/static/provider/cloud/deploy.yaml
- kubectl get pods --namespace=ingress-nginx
- kubectl get services --namespace=ingress-nginx

#### Visual studio code
- code -r .