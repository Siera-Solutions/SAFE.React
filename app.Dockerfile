FROM mcr.microsoft.com/dotnet/sdk:6.0 as debug 

RUN curl -sL https://deb.nodesource.com/setup_14.x | bash
RUN apt-get update && apt-get install -y nodejs unzip
RUN curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

WORKDIR /workspace
COPY ./lib ./lib
COPY ./app ./app
RUN dotnet tool restore --tool-manifest ./app/.config/dotnet-tools.json
RUN dotnet dev-certs https --trust
RUN cd ./app/client && npm install
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 8080
ENTRYPOINT [ "dotnet","run","--project","./app/build/Build.fsproj","--","RunDefaultOr", "Run"]

FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

# Install nodejs
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash
RUN apt-get update && apt-get install -y nodejs

WORKDIR /workspace
COPY ./lib ./lib
COPY ./app ./app
RUN dotnet tool restore --tool-manifest ./app/.config/dotnet-tools.json
RUN dotnet run --project ./app/build/Build.fsproj -- RunDefaultOr PackNoTests

FROM mcr.microsoft.com/dotnet/aspnet:6.0

COPY --from=build /workspace/app/dist /app
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT [ "dotnet", "Server.dll" ]