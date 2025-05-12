# Usar la imagen base de .NET SDK para compilar el proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar los archivos del proyecto y restaurar las dependencias
COPY *.sln .
COPY Vemo.Gestion.Recursos.Data/*.csproj ./Vemo.Gestion.Recursos.Data/
COPY Vemo.Gestion.Recursos/*.csproj ./Vemo.Gestion.Recursos/
COPY Vemo.Gestion.Recursos.Tests/*.csproj ./Vemo.Gestion.Recursos.Tests/
RUN dotnet restore

# Copiar el resto de los archivos y compilar el proyecto
COPY . .
RUN dotnet publish -c Release -o out

# Usar la imagen base de .NET Runtime para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Exponer los puertos
EXPOSE 5000
EXPOSE 5001

# Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "Vemo.Gestion.Recursos.dll"]