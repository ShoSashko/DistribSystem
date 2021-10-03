FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["MasterNode/MasterNode.csproj", "MasterNode/"]
RUN dotnet restore "MasterNode/MasterNode.csproj"
COPY . .
WORKDIR "/src/MasterNode"
RUN dotnet build "MasterNode.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MasterNode.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app    
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MasterNode1.dll"]