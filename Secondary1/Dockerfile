FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Secondary1/Secondary1.csproj", "Secondary1/"]
RUN dotnet restore "Secondary1/Secondary1.csproj"
COPY . .
WORKDIR "/src/Secondary1"
RUN dotnet build "Secondary1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Secondary1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app    
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Secondary1a.dll"]