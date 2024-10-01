# Etapa base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar os arquivos de projeto
COPY ["src/Sales.API/Sales.API.csproj", "src/Sales.API/"]
COPY ["src/Sales.Application/Sales.Application.csproj", "src/Sales.Application/"]
COPY ["src/Sales.Infra/Sales.Infra.csproj", "src/Sales.Infra/"]
COPY ["src/Sales.Domain/Sales.Domain.csproj", "src/Sales.Domain/"]

# Restaurar dependências
RUN dotnet restore "./src/Sales.API/Sales.API.csproj"

# Copiar todo o código fonte
COPY . .

# Definir diretório de trabalho
WORKDIR "/src/src/Sales.API"

# Compilar a aplicação
RUN dotnet build "./Sales.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar a aplicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Sales.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final
FROM base AS final
WORKDIR /app

# Copiar os arquivos publicados
COPY --from=publish /app/publish .

# Criar o diretório de certificados
RUN mkdir -p /app/certificates

# Copiar o certificado para o container
COPY ./src/Sales.API/certificates/aspnetapp.pfx /app/certificates/

# Definir o ponto de entrada
ENTRYPOINT ["dotnet", "Sales.API.dll"]
