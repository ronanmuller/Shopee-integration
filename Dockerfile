 
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/1.Presentation/GE.Integration.Shopee.Api/GE.Integration.Shopee.Api.csproj", "src/1.Presentation/GE.Integration.Shopee.Api/"]
COPY ["src/2.Application/GE.Integration.Shopee.Application/GE.Integration.Shopee.Application.csproj", "src/2.Application/GE.Integration.Shopee.Application/"]
COPY ["src/3.Domain/GE.Integration.Shopee.Domain/GE.Integration.Shopee.Domain.csproj", "src/3.Domain/GE.Integration.Shopee.Domain/"]
COPY ["src/4.Infra/GE.Integration.Shopee.Infra.External/GE.Integration.Shopee.Infra.External.csproj", "src/4.Infra/GE.Integration.Shopee.Infra.External/"]
COPY ["src/Nuget/GE.Contracts/GE.Contracts.csproj", "src/Nuget/GE.Contracts/"]
RUN dotnet restore "src/1.Presentation/GE.Integration.Shopee.Api/GE.Integration.Shopee.Api.csproj"
COPY . .
WORKDIR "/src/src/1.Presentation/GE.Integration.Shopee.Api"
RUN dotnet build "GE.Integration.Shopee.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GE.Integration.Shopee.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GE.Integration.Shopee.Api.dll"]