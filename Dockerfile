FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app
COPY ./src/*.csproj ./
RUN dotnet restore
COPY ./src ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:5.0
RUN apk add --no-cache python3 py3-pip
WORKDIR /app
COPY --from=build-env /app/out .
VOLUME /app/config

ENTRYPOINT ["dotnet", "GearstashBot.dll"]
