FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

# Install nodejs
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash
RUN apt-get update && apt-get install -y nodejs

# install tools
RUN dotnet tool install --tool-path /tools dotnet-trace
RUN dotnet tool install --tool-path /tools dotnet-counters
RUN dotnet tool install --tool-path /tools dotnet-dump

WORKDIR /workspace
COPY ./lib ./lib
COPY ./app ./app
RUN dotnet tool restore --tool-manifest ./app/.config/dotnet-tools.json
RUN dotnet run --project ./app/build/Build.fsproj -- RunDefaultOr PackNoTests

FROM mcr.microsoft.com/dotnet/sdk:6.0 as debug 

RUN curl -sL https://deb.nodesource.com/setup_14.x | bash
RUN apt-get update && apt-get install -y nodejs

WORKDIR /workspace
COPY ./lib ./lib
COPY ./app ./app
RUN dotnet tool restore --tool-manifest ./app/.config/dotnet-tools.json
RUN dotnet dev-certs https --trust
RUN cd ./app/client && npm install
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 8080
ENTRYPOINT [ "dotnet","run","--project","./app/build/Build.fsproj","--","RunDefaultOr", "Run"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0

COPY --from=build /tools /app
COPY --from=build /workspace/app/dist /app
# add gcompat so dotnet-dump can be run.
# RUN apk add gcompat 
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5000;http://+:8085
EXPOSE 5000
EXPOSE 8085
ENTRYPOINT [ "dotnet", "Server.dll" ]