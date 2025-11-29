# Cambios Necesarios en el Código Original

## 📝 Resumen de cambios

Esta guía lista los cambios mínimos necesarios en el código del Gateway para que funcione correctamente con la nueva arquitectura.

---

## 1. Program.cs - Remover ContractsContext

### ❌ Eliminar estas líneas:

```csharp
// Eliminar estos imports
using workstation_backend.ContractsContext.Domain;
using workstation_backend.ContractsContext.Infrastructure;
using workstation_backend.ContractsContext.Domain.Services;
using workstation_backend.ContractsContext.Application.QueriesServices;
using workstation_backend.ContractsContext.Application.CommandServices;
using workstation_backend.ContractsContext.Domain.Models.Validators;
using workstation_backend.ContractsContext.Application.EventServices;

// Eliminar registro de servicios
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractCommandService, ContractCommandService>();
builder.Services.AddScoped<IContractQueryService, ContractQueryService>();
builder.Services.AddScoped<IContractEventService, ContractEventService>();

// Eliminar validadores de contratos
builder.Services.AddValidatorsFromAssemblyContaining<AddClauseCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddCompensationCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateContractCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<FinishContractCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SignContractCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateReceiptCommandValidator>();
```

### ✅ Agregar HttpClient para ContractService:

```csharp
// Agregar después de las otras configuraciones
builder.Services.AddHttpClient("ContractService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:ContractService"] ?? "http://contract-service:80");
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

---

## 2. appsettings.json - Agregar referencia a ContractService

### ✅ Agregar sección Services:

```json
{
  "Services": {
    "ContractService": "http://contract-service:80"
  }
}
```

**Versión completa**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=mysql;database=workstation_db;user=root;password=root;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Key": "A1s2D3f4G5h6J7k8L9z0Q1w2E3r4T5y6",
    "Issuer": "WorkstationIssuer",
    "Audience": "WorkstationAudience",
    "ExpirationHours": "1"
  },
  "Services": {
    "ContractService": "http://contract-service:80"
  }
}
```

---

## 3. Controllers - Remover o mover ContractControllers

### ❌ Opción 1: Eliminar Controllers de Contratos

Si tienes `OfficesContext/Interface/OfficeController.cs` y `UserContext/Interfaces/REST/UserController.cs`, puedes dejar solo esos.

Los archivos a eliminar del Gateway:
- `ContractsContext/Interface/ContractsController.cs`

### ✅ Opción 2: Crear Proxy Controller (Recomendado)

Si quieres que las rutas sigan igual, crea un proxy:

**Gateway/Interface/ProxyController.cs**:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Threading.Tasks;

namespace workstation_gateway.Interface
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProxyController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProxyController> _logger;

        public ProxyController(IHttpClientFactory httpClientFactory, ILogger<ProxyController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // Forward GET /api/contracts to ContractService
        [HttpGet("contracts")]
        public async Task<IActionResult> GetContracts()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ContractService");
                var response = await client.GetAsync("/api/contracts");
                
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode);

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling ContractService");
                return StatusCode(500, "Error contacting contract service");
            }
        }

        // Forward POST /api/contracts to ContractService
        [HttpPost("contracts")]
        public async Task<IActionResult> CreateContract([FromBody] object request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ContractService");
                var json = System.Text.Json.JsonSerializer.Serialize(request);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/contracts", content);
                
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode);

                var responseContent = await response.Content.ReadAsStringAsync();
                return Content(responseContent, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling ContractService");
                return StatusCode(500, "Error contacting contract service");
            }
        }
    }
}
```

---

## 4. gateway.csproj - Remover referencia a ContractsContext

### ❌ Eliminar si existe:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\ContractsContext\..." />
</ItemGroup>
```

### ✅ El csproj debe ser simple:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
    <PropertyGroup>
        <TargetFramework>net9.0</TargetFramework>
        ...
    </PropertyGroup>

    <ItemGroup>
        <!-- Solo dependencias NuGet -->
        <PackageReference Include="..." />
    </ItemGroup>
</Project>
```

---

## 5. .csproj original - Remover ContractsContext

Si tienes archivo `.csproj` viejo en la raíz que referencia ContractsContext:

### ✅ Renombrar o eliminar:

```powershell
# Renombrar para no afectar builds
mv workstation-backend.csproj workstation-backend.csproj.bak

# O eliminar si no lo usas
rm workstation-backend.csproj
```

---

## 6. Estructura final del Gateway

```
Gateway/
├── UserContext/
│   ├── Application/
│   ├── Domain/
│   ├── Infrastructure/
│   └── Interfaces/
│       └── REST/
│           └── UserController.cs
├── OfficesContext/
│   ├── Application/
│   ├── Domain/
│   ├── Infrastructure/
│   └── Interface/
│       └── OfficeController.cs
├── Shared/
├── Properties/
├── Program.cs               ← Sin ContractsContext
├── appsettings.json         ← Con Services:ContractService
├── gateway.csproj
└── Dockerfile
```

---

## 7. Verificación Post-Cambios

Después de hacer estos cambios, verifica:

```powershell
# 1. Build del Gateway
cd Gateway
dotnet build

# 2. Build del ContractService
cd ..\Services\ContractService
dotnet build

# 3. Docker build
cd ..\..
docker-compose up --build
```

### Endpoints esperados:

```
Gateway:
✓ GET  /api/users
✓ POST /api/users/login
✓ GET  /api/offices
✓ GET  /api/ratings

ContractService:
✓ GET    /api/contracts
✓ POST   /api/contracts
✓ GET    /api/contracts/{id}
✓ PUT    /api/contracts/{id}/sign
✓ DELETE /api/contracts/{id}
```

---

## 📋 Checklist

- [ ] Remover imports de ContractsContext en Program.cs
- [ ] Remover registro de servicios ContractsContext
- [ ] Remover validadores de contratos
- [ ] Agregar HttpClient para ContractService
- [ ] Agregar sección Services en appsettings.json
- [ ] Crear o remover ContractController
- [ ] Actualizar gateway.csproj
- [ ] Build local exitoso
- [ ] Docker-compose up exitoso
- [ ] Swagger Gateway cargando
- [ ] Swagger ContractService cargando

---

## 🔍 Debugging

Si hay errores:

```powershell
# Ver logs detallados
docker-compose logs -f gateway1
docker-compose logs -f contract-service

# Verificar conectividad entre servicios
docker exec workstation-gateway-1 curl http://contract-service:80/health
```

---

**Próximo paso**: Ejecuta `docker-compose up --build` y verifica que todo funcione.
