# Guía de Arquitectura - Microservicios Workstation

## 🏗️ Arquitectura Actual

### Componentes

#### 1. **Nginx Load Balancer**
- **Puerto**: 80
- **Función**: Distribuir peticiones entre instancias del Gateway
- **Algoritmo**: `least_conn` (menos conexiones activas)

#### 2. **API Gateway** (2+ instancias)
- **Puertos**: 80 (interno)
- **Responsabilidades**:
  - Autenticación JWT
  - Usuarios (CRUD)
  - Oficinas (CRUD)
  - Ratings
  - Proxy hacia ContractService
- **Base de Datos**: `workstation_db`
- **Contextos**: UserContext, OfficesContext

#### 3. **Contract Service** (Microservicio)
- **Puerto**: 80 (interno)
- **Responsabilidades**:
  - Gestión de Contratos
  - Cláusulas
  - Compensaciones
  - Recibos
- **Base de Datos**: `contracts_db`
- **Contextos**: ContractsContext

#### 4. **MySQL Database**
- **Puerto**: 3306
- **Bases de Datos**:
  - `workstation_db`: Users + Offices
  - `contracts_db`: Contracts

---

## 🔄 Flujo de Peticiones

### Petición de Usuarios
```
Cliente
  │
  GET /api/users
  │
  v
Nginx (Load Balancer)
  │
  └─> Gateway1 o Gateway2 (Round-robin)
       │
       └─> WorkstationContext
            └─> MySQL (workstation_db)
```

### Petición de Contratos
```
Cliente
  │
  GET /api/contracts
  │
  v
Nginx (Load Balancer)
  │
  └─> ContractService
       │
       └─> ContractContext
            └─> MySQL (contracts_db)
```

---

## 🔐 Autenticación

### JWT Token

Todos los servicios validan el mismo JWT:

```csharp
// El token se genera en el Gateway
POST /api/users/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}

// Response
{
  "token": "eyJhbGc...",
  "expiresIn": 3600
}

// Usar el token en cualquier petición protegida
GET /api/contracts
Authorization: Bearer eyJhbGc...
```

### Validación

Cada servicio valida:
- Firma del token (con la clave compartida)
- Expiración
- Claims (si es necesario)

---

## 💾 Bases de Datos

### Estrategia: Database Per Service (con matiz)

```
MySQL Server
├── workstation_db
│   ├── users
│   ├── offices
│   ├── ratings
│   └── (tablas de soporte)
│
└── contracts_db
    ├── contracts
    ├── clauses
    ├── compensations
    ├── payment_receipts
    └── (tablas de soporte)
```

### Ventajas
- ✅ Escalabilidad independiente
- ✅ Migración sin bloqueos
- ✅ No hay dependencias de BD
- ✅ Fácil backup individual

---

## 🔌 Comunicación Entre Servicios

### Opción 1: HTTP Síncrono (Actual)

El Gateway puede llamar al ContractService vía HTTP:

```csharp
// En Gateway/Program.cs
builder.Services.AddHttpClient("ContractService", client =>
{
    client.BaseAddress = new Uri("http://contract-service:80");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Uso en un controller
[ApiController]
public class ProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProxyController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("create-with-user")]
    public async Task<IActionResult> CreateContractWithUser(CreateContractDto dto)
    {
        // 1. Validar usuario en este servicio
        var user = await _userService.GetUserAsync(dto.UserId);
        
        // 2. Llamar al ContractService
        var client = _httpClientFactory.CreateClient("ContractService");
        var response = await client.PostAsJsonAsync("/api/contracts", dto);
        
        return Ok(response);
    }
}
```

### Opción 2: Message Queue Asíncrono (Futuro)

Con RabbitMQ:

```csharp
// Cuando se crea un usuario
await _eventBus.PublishAsync(new UserCreatedEvent 
{ 
    UserId = user.Id,
    Email = user.Email
});

// El ContractService escucha
public class UserCreatedEventHandler : IConsumer<UserCreatedEvent>
{
    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        // Hacer algo cuando un usuario es creado
        _logger.LogInformation("Nuevo usuario: {userId}", context.Message.UserId);
    }
}
```

---

## 📊 Escalado

### Escalar Gateway

```bash
# Aumentar a 3 instancias
docker-compose up -d --scale gateway=3

# Nginx distribuye automáticamente
```

### Escalar ContractService

```bash
# Crear 2 instancias de ContractService
docker-compose up -d --scale contract-service=2
```

### Problema: State Management

⚠️ **Importante**: Si hay estado en memoria, cada instancia verá diferente.

Soluciones:
1. **Redis**: Session store compartida
2. **Stateless**: Diseñar servicios sin estado
3. **Kafka/RabbitMQ**: Sincronizar entre instancias

---

## 🚀 Mejoras Futuras

### Fase 1: Comunicación Asíncrona
```
Gateway ──┐
          ├─> RabbitMQ ──> ContractService
          │
          └─> NotificationService (futuro)
```

### Fase 2: Más Microservicios
```
Nginx
├─> UserService
├─> OfficeService
├─> ContractService
└─> NotificationService
```

### Fase 3: API Gateway Completo (Ocelot)
```
Ocelot (API Gateway)
├─> UserService
├─> OfficeService
├─> ContractService
└─> Reescritura de URLs
```

### Fase 4: Kubernetes
```
K8s Cluster
├─> StatefulSet: MySQL
├─> Deployment: UserService (3 replicas)
├─> Deployment: OfficeService (2 replicas)
├─> Deployment: ContractService (2 replicas)
└─> Ingress: Nginx Controller
```

---

## 🧪 Testing

### Unit Tests
```bash
cd Services/ContractService
dotnet test
```

### Integration Tests (Local)
```bash
# Asegúrate que docker-compose está corriendo
docker-compose up -d

# Ejecutar tests que llaman a los servicios
dotnet test --filter "Integration"
```

### Load Testing con Apache Bench
```bash
# Instalar: choco install apache-bench

# Test Gateway
ab -n 1000 -c 10 http://localhost/api/users

# Test ContractService
ab -n 1000 -c 10 http://localhost/api/contracts
```

---

## 📋 Checklist de Deployment

- [ ] Cambiar BD a instancia en producción
- [ ] Cambiar JWT key a valor seguro
- [ ] Configurar variables de entorno en K8s/Docker
- [ ] Backups diarios de ambas BDs
- [ ] Monitoring con Prometheus + Grafana
- [ ] Logging centralizado con ELK
- [ ] Health checks configurados
- [ ] Timeout valores reales
- [ ] Rate limiting en Nginx
- [ ] SSL/TLS en Nginx

---

**Última actualización**: 28 Nov 2025
