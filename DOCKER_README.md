# Workstation Backend - Arquitectura Microservicios con Docker

## 📋 Descripción

Backend de WorkStation con arquitectura híbrida:
- **Gateway API**: Monolito con Users + Offices (2 instancias con balanceo)
- **Contract Service**: Microservicio independiente
- **Nginx**: Load Balancer entre instancias del Gateway
- **MySQL**: Base de datos compartida

## 🏗️ Estructura

```
Backend/
├── Gateway/                    ← API Gateway (Users + Offices)
│   ├── UserContext/
│   ├── OfficesContext/
│   ├── Shared/
│   ├── gateway.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   └── Dockerfile
├── Services/
│   └── ContractService/        ← Microservicio Contratos
│       ├── ContractsContext/
│       ├── contract-service.csproj
│       ├── Program.cs
│       ├── appsettings.json
│       └── Dockerfile
├── Shared/                     ← Código compartido
├── docker-compose.yml
├── nginx.conf
└── README.md
```

## 🚀 Comandos

### Build y ejecución

```powershell
# Construir imágenes y ejecutar
docker-compose up --build

# Ejecutar en background
docker-compose up -d --build

# Ver logs
docker-compose logs -f

# Parar servicios
docker-compose down

# Parar y eliminar volúmenes
docker-compose down -v
```

### Escalado (opcional)

```powershell
# Escalar a 3 gateways
docker-compose up -d --scale gateway=3

# Ver servicios corriendo
docker-compose ps
```

## 🌐 Endpoints

Una vez que todo esté corriendo en `localhost`:

### Gateway (Balanceado)
- **Users API**: `http://localhost/api/users`
- **Offices API**: `http://localhost/api/offices`
- **Ratings API**: `http://localhost/api/ratings`
- **Swagger Gateway**: `http://localhost/swagger`

### Contract Service (Directo)
- **Contracts API**: `http://localhost/api/contracts`
- **Swagger Contratos**: `http://localhost/contracts-api/swagger`

## 📊 Arquitectura

```
Cliente
   │
   └─ http://localhost (Nginx Load Balancer)
        │
        ├─────── /api/users        ─┐
        ├─────── /api/offices       ├─> Gateway1 o Gateway2 (Round-robin)
        └─────── /api/ratings       ┘
        │
        └─────── /api/contracts     ─> ContractService

MySQL Database
   ├─ workstation_db (Gateway)
   └─ contracts_db (ContractService)
```

## 🔐 Configuración JWT

Todos los servicios usan la misma clave JWT:
```
Key: A1s2D3f4G5h6J7k8L9z0Q1w2E3r4T5y6
```

Para cambiar, edita `docker-compose.yml` en las variables de entorno.

## 📦 Dependencias

- .NET 9.0
- MySQL 8.0
- Nginx (Alpine)
- Docker & Docker Compose

## ⚙️ Configuración de Base de Datos

### Gateway (workstation_db)
- Usuarios, Oficinas, Ratings

### Contract Service (contracts_db)
- Contratos, Cláusulas, Compensaciones, Recibos

Ambas se crean automáticamente al ejecutar.

## 🔧 Variables de Entorno

Puedes sobrescribir configuraciones en `docker-compose.yml`:

```yaml
environment:
  - ConnectionStrings__DefaultConnection=server=mysql;database=workstation_db;user=root;password=root;
  - Jwt__Key=TU_CLAVE_AQUI
  - Services__ContractService=http://contract-service:80
```

## 📝 Migraciones

Las migraciones se aplican automáticamente al iniciar cada servicio.

Si necesitas crear una nueva:

```powershell
# En el Gateway
dotnet ef migrations add NombreMigracion -p Gateway/gateway.csproj

# En Contract Service
dotnet ef migrations add NombreMigracion -p Services/ContractService/contract-service.csproj
```

## 🐛 Troubleshooting

### Los containers no inician
```powershell
# Ver logs detallados
docker-compose logs -f [servicio]

# Ejemplo
docker-compose logs -f gateway1
docker-compose logs -f contract-service
```

### Errores de conexión a BD
Asegúrate que MySQL está saludable:
```powershell
docker-compose ps
# Status debe ser "healthy" para mysql
```

### Puerto 80 ya en uso
Cambia en `docker-compose.yml`:
```yaml
ports:
  - "8080:80"  # Usar puerto 8080
```

## 🔮 Próximos Pasos

1. **Message Queue**: Integrar RabbitMQ para comunicación asíncrona
2. **Service Discovery**: Implementar Consul o similar
3. **Logging Centralizado**: ELK Stack o Serilog
4. **Separar más servicios**: UserService y OfficeService independientes
5. **Kubernetes**: Migrar de Docker Compose a K8s

## 📚 Referencias

- [Docker Compose](https://docs.docker.com/compose/)
- [Nginx Load Balancing](https://nginx.org/en/docs/http/load_balancing.html)
- [.NET Entity Framework](https://learn.microsoft.com/en-us/ef/core/)
- [MySQL in Docker](https://hub.docker.com/_/mysql)

---

**Última actualización**: 28 Nov 2025
