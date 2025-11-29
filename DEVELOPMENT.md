# Guía de Desarrollo Local

## 🔧 Configuración para Desarrollo

### Prerequisitos
- Docker Desktop instalado
- .NET 9.0 SDK
- Visual Studio Code o Visual Studio

### Primeros Pasos

#### 1. Navegar a la rama correcta
```powershell
cd f:\AplicacionesMoviles\Backend
git branch -v  # Verificar que estés en la rama de microservicios
```

#### 2. Iniciar Docker Compose
```powershell
docker-compose up --build
```

**Esperado**:
```
workstation-nginx          | ✓ Ready
workstation-gateway-1      | ✓ Running on http://+:80
workstation-gateway-2      | ✓ Running on http://+:80
workstation-contract-service  | ✓ Running on http://+:80
workstation-mysql          | ✓ healthy
```

#### 3. Verificar servicios
```powershell
docker-compose ps

# Output esperado:
# NAME                          STATUS
# workstation-nginx             Up 2 minutes
# workstation-gateway-1         Up 2 minutes
# workstation-gateway-2         Up 2 minutes
# workstation-contract-service  Up 2 minutes
# workstation-mysql             Up 2 minutes (healthy)
```

---

## 🧪 Pruebas Manuales

### 1. Health Check
```bash
curl http://localhost/health
# Response: OK
```

### 2. Login (obtener JWT token)
```bash
curl -X POST http://localhost/api/users/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "password123"
  }'
```

### 3. Usar el token
```bash
# Copiar el token del response anterior
curl -X GET http://localhost/api/users \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### 4. Ver documentación Swagger

**Gateway**: http://localhost/swagger/index.html
**Contract Service**: http://localhost/contracts-api/swagger/index.html

---

## 🔍 Debugging

### Ver logs de un servicio
```powershell
# Gateway 1
docker-compose logs -f gateway1

# Gateway 2
docker-compose logs -f gateway2

# Contract Service
docker-compose logs -f contract-service

# MySQL
docker-compose logs -f mysql

# Nginx
docker-compose logs -f nginx
```

### Acceso a base de datos MySQL
```powershell
# Conectar con MySQL Workbench o línea de comandos
mysql -h localhost -u root -p
# Password: root

# Ver bases de datos
SHOW DATABASES;

# Ver tablas del Gateway
USE workstation_db;
SHOW TABLES;

# Ver tablas de Contratos
USE contracts_db;
SHOW TABLES;
```

### Ejecutar comando dentro de un contenedor
```powershell
# Ejecutar bash en gateway1
docker exec -it workstation-gateway-1 /bin/bash

# Ver logs dentro del contenedor
cat /app/logs/app.log  # si existen
```

---

## 🛠️ Cambios y Reconstrucción

### Después de cambiar código en Gateway
```powershell
# Opción 1: Reconstruir solo gateway
docker-compose up -d --build gateway1 gateway2

# Opción 2: Reconstruir todo
docker-compose down
docker-compose up --build
```

### Después de cambiar código en ContractService
```powershell
docker-compose up -d --build contract-service
```

### Después de cambiar nginx.conf
```powershell
docker-compose up -d --build nginx
```

---

## 📝 Modificación de Configuraciones

### Cambiar puertos
En `docker-compose.yml`:
```yaml
nginx:
  ports:
    - "8080:80"  # Usar puerto 8080 en lugar de 80
```

### Cambiar claves JWT
En `docker-compose.yml`:
```yaml
environment:
  - Jwt__Key=TU_NUEVA_CLAVE_SUPER_SEGURA_AQUI
```

### Cambiar credenciales MySQL
En `docker-compose.yml`:
```yaml
mysql:
  environment:
    MYSQL_ROOT_PASSWORD: mi_nueva_password
```

Luego actualizar en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=mysql;database=workstation_db;user=root;password=mi_nueva_password;"
  }
}
```

---

## 🔄 Migraciones de BD

### Ver estado de migraciones
```powershell
# En Gateway
cd Gateway
dotnet ef migrations list

# En ContractService
cd Services/ContractService
dotnet ef migrations list
```

### Crear nueva migración
```powershell
# Para Gateway
dotnet ef migrations add NombreMigracion -p Gateway/gateway.csproj

# Para ContractService
dotnet ef migrations add NombreMigracion -p Services/ContractService/contract-service.csproj
```

### Revertir migraciones
```powershell
# Listar
dotnet ef migrations list

# Revertir a una específica
dotnet ef database update MigracionAnterior
```

---

## 📊 Monitoreo de Performance

### Top commands
```powershell
# CPU y memoria de containers
docker stats

# Ver procesos dentro de un container
docker top workstation-gateway-1
```

### Analizar requests lento
1. Habilitar logging detallado en `appsettings.json`
2. Ver logs: `docker-compose logs -f gateway1`
3. Buscar líneas con duración > 1000ms

---

## 🐛 Problemas Comunes

### "Port 80 already in use"
```powershell
# Cambiar en docker-compose.yml o:
docker-compose up -p my-project
```

### "Connection refused" a MySQL
```powershell
# Verificar que MySQL esté healthy
docker-compose ps
# Status debe ser "Up X minutes (healthy)"

# Si no está saludable:
docker-compose logs mysql
```

### "Could not resolve host contract-service"
Significa que los contenedores no están en la misma red.
```powershell
# Reconstruir
docker-compose down
docker-compose up --build
```

### Migraciones fallan
```powershell
# Ver qué error es
docker-compose logs gateway1

# Si es de BD, reintentar:
docker-compose down -v  # Elimina volúmenes
docker-compose up
```

---

## 🎯 Workflow Típico

### Para agregar feature en Gateway
```powershell
# 1. Crear rama
git checkout -b feature/nueva-funcionalidad

# 2. Hacer cambios en Gateway/
# 3. Reconstruir
docker-compose up -d --build gateway1 gateway2

# 4. Probar en Swagger
# http://localhost/swagger

# 5. Commit y push
git add .
git commit -m "feat: nueva funcionalidad"
git push origin feature/nueva-funcionalidad
```

### Para agregar feature en ContractService
```powershell
# 1. Hacer cambios en Services/ContractService/
# 2. Reconstruir
docker-compose up -d --build contract-service

# 3. Probar
# http://localhost/contracts-api/swagger

# 4. Commit
git add .
git commit -m "feat: nueva funcionalidad contratos"
git push origin feature/microservicios-architecture
```

---

## 📚 Recursos útiles

- [Docker Compose CLI](https://docs.docker.com/compose/reference/)
- [Entity Framework CLI](https://learn.microsoft.com/en-us/ef/core/cli/)
- [MySQL Reference](https://dev.mysql.com/doc/)
- [ASP.NET Configuration](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)

---

**¿Necesitas ayuda?** Revisa los logs: `docker-compose logs -f [servicio]`
