FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["./RinhaBackend.Api.csproj", "./"]
RUN dotnet restore "RinhaBackend.Api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "RinhaBackend.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RinhaBackend.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RinhaBackend.Api.dll"]