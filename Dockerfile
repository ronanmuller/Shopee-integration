#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/1.Presentation/GE.Integration.MELI.Api/GE.Integration.MELI.Api.csproj", "src/1.Presentation/GE.Integration.MELI.Api/"]
COPY ["src/2.Application/GE.Integration.MELI.Application.Configuration/GE.Integration.MELI.Application.Configuration.csproj", "src/2.Application/GE.Integration.MELI.Application.Configuration/"]
COPY ["src/2.Application/GE.Integration.MELI.Application/GE.Integration.MELI.Application.csproj", "src/2.Application/GE.Integration.MELI.Application/"]
COPY ["src/4.Infra/GE.Integration.MELI.Infra.External/GE.Integration.MELI.Infra.External.csproj", "src/4.Infra/GE.Integration.MELI.Infra.External/"]
COPY ["src/3.Domain/GE.Integration.MELI.Domain/GE.Integration.MELI.Domain.csproj", "src/3.Domain/GE.Integration.MELI.Domain/"]
COPY ["src/Nuget/GE.Contracts/GE.Contracts.csproj", "src/Nuget/GE.Contracts/"]
RUN dotnet restore "src/1.Presentation/GE.Integration.MELI.Api/GE.Integration.MELI.Api.csproj"
COPY . .
WORKDIR "/src/src/1.Presentation/GE.Integration.MELI.Api"
RUN dotnet build "GE.Integration.MELI.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GE.Integration.MELI.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GE.Integration.MELI.Api.dll"]