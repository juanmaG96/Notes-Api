# Etapa 1: Build y publicacion
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar definicion del proyecto y restaurar paquetes NuGet
COPY ["Notes.Api.csproj", "./"]
RUN dotnet restore "Notes.Api.csproj"

# Copiar el resto del codigo fuente
COPY . .

# Compilar y publicar artefactos optimizados
RUN dotnet publish "Notes.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Runtime ligero
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Configuracion de puerto para Northflank y proveedores cloud
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Copiar binarios compilados
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Notes.Api.dll"]
