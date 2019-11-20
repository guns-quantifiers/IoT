FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.1-buster-slim-arm64v8 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src
COPY ["src/BlackjackAPI/BlackjackAPI.csproj", "BlackjackAPI/"]
RUN dotnet restore "BlackjackAPI/BlackjackAPI.csproj"
COPY . .
WORKDIR "/src/BlackjackAPI"
RUN dotnet build "BlackjackAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BlackjackAPI.csproj" -c Release -o /app

FROM base AS final 
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BlackjackAPI.dll"]