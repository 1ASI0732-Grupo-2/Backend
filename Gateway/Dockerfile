FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos el csproj del gateway
COPY ["Gateway/gateway.csproj", "Gateway/"]


# Restauramos dependencias
RUN dotnet restore "Gateway/gateway.csproj"

# Copiamos TODO el código del Gateway (incluye OfficesContext, UserContext, Shared interno)
COPY ["Gateway/", "Gateway/"]

# Compilamos
RUN dotnet build "Gateway/gateway.csproj" -c Release -o /app/build


# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/build .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "gateway.dll"]
