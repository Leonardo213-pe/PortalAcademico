FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY PortalAcademicoWeb/*.csproj ./PortalAcademicoWeb/
RUN dotnet restore ./PortalAcademicoWeb/PortalAcademicoWeb.csproj

COPY PortalAcademicoWeb/. ./PortalAcademicoWeb/
RUN dotnet publish ./PortalAcademicoWeb/PortalAcademicoWeb.csproj \
    -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "PortalAcademicoWeb.dll"]