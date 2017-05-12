## CoreSample
.NET Core sample app with docker preview. WebApi running in a linux docker container using a reverse proxy to hit the endpoint via subdomain.

nginx hosted at localhost:80 -> docker listening on 80
.net app localhost:5000 -> docker listening on 80
Reverse proxy listening at demo1.localhost -> routes to localhost:5000

### Environment
- VS 2017 RC Windows 10
- Docker for Windows
- Docker .NET Core preview install for VS

This was helpful https://docs.docker.com/docker-for-windows/

### WebApi Setup
- Code and run in browser locally without docker
- Add docker project support **This will likely reference the wrong version of .NET Core you are running.**
- Add entry for build path in .dockerignore so that docker can find it.
- Fix publish path Dockerfile
- Run command in succession: `dotnet  clean, restore, build, publish`
- Maybe try it locally to see it work `dotnet run`

### Docker
Install and run a nginx reverse proxy
```
docker run -d -p 80:80 -v /var/run/docker.sock:/tmp/docker.sock:ro jwilder/nginx-proxy:latest
```
Notes: http://jasonwilder.com/blog/2014/03/25/automated-nginx-reverse-proxy-for-docker/

Create image for .NET Core app
```
docker build -t netcore --no-cache .
```
Create and run container for .NET Core app
```
docker run -d -p 5000:80 -e VIRTUAL_HOST=demo1.localhost --name netcoreserver netcore
```
This will run a container that listens on port 80 that can be accessed via locahost on port 5000 `http://localhost:5000` the image will be named `netcoreserver`

Go try it out. This is the location when using the nginx proxy esposed url `http://demo1.localhost/`

Other helpful commands
- Where did things go wrong while deploying a container: `docker logs netcoreserver`
- Remove container: `docker rm netcoreserver`
- Remove image: `docker rmi netcore`
- Stop container: `docker stop netcoreserver`
- Start container: `docker start cocky_borg`
- Start container: `docker start netcoreserver`

Add RavenDB as container
https://github.com/ravendb/ravendb/blob/d9ff0a195c4cdb23c97439505c7715e1dbda5fbb/docker/run-ubuntu1604.ps1


