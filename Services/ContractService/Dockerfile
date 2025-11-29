FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar el csproj primero (para aprovechar el caché de Docker)
COPY ["Services/ContractService/contract-service.csproj", "Services/ContractService/"]

# Restaurar dependencias ANTES de copiar el código
RUN dotnet restore "Services/ContractService/contract-service.csproj"

# Ahora copiar TODO el código (incluyendo Shared)
COPY ["Services/ContractService/", "Services/ContractService/"]

# Compilar y publicar
WORKDIR "/src/Services/ContractService"
RUN dotnet publish "contract-service.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- Runtime image ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copiar solo lo publicado
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "contract-service.dll"]