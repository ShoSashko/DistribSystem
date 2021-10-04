FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Secondary2/Secondary2.csproj", "Secondary2/"]
RUN dotnet restore "Secondary2/Secondary2.csproj"
COPY . .
WORKDIR "/src/Secondary2"
RUN dotnet build "Secondary2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Secondary2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app    
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Secondary2a.dll"]