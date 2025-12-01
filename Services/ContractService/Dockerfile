FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Services/ContractService/contract-service.csproj", "Services/ContractService/"]

RUN dotnet restore "Services/ContractService/contract-service.csproj"

COPY ["Services/ContractService/", "Services/ContractService/"]

WORKDIR "/src/Services/ContractService"
RUN dotnet publish "contract-service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "contract-service.dll"]