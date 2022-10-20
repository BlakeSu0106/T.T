#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/applications/Telligent.Tag.WebApi/Telligent.Tag.WebApi.csproj", "src/applications/Telligent.Tag.WebApi/"]
COPY ["src/modules/Telligent.Tag.Application/Telligent.Tag.Application.csproj", "src/modules/Telligent.Tag.Application/"]
COPY ["src/modules/Telligent.Tag.Database/Telligent.Tag.Database.csproj", "src/modules/Telligent.Tag.Database/"]
COPY ["src/modules/Telligent.Tag.Domain/Telligent.Tag.Domain.csproj", "src/modules/Telligent.Tag.Domain/"]

RUN dotnet nuget add source http://teola.3rdchannel.com.tw/Nuget/nuget/ -n Telexpress
RUN dotnet restore "src/applications/Telligent.Tag.WebApi/Telligent.Tag.WebApi.csproj"
COPY . .
WORKDIR "/src/src/applications/Telligent.Tag.WebApi"
RUN dotnet build "Telligent.Tag.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Telligent.Tag.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Telligent.Tag.WebApi.dll"]
