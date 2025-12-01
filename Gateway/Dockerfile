FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Gateway/gateway.csproj", "Gateway/"]

RUN dotnet restore "Gateway/gateway.csproj"

COPY ["Gateway/", "Gateway/"]

RUN dotnet build "Gateway/gateway.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/build .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "gateway.dll"]
