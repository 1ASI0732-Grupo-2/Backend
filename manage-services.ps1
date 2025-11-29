#!/usr/bin/env pwsh

# Script para gestionar la arquitectura de microservicios
# Uso: .\manage-services.ps1 -Action up

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("up", "down", "rebuild", "logs", "status", "scale", "clean", "help")]
    [string]$Action = "help",
    
    [Parameter(Mandatory=$false)]
    [string]$Service = "all",
    
    [Parameter(Mandatory=$false)]
    [int]$Replicas = 2
)

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = $scriptPath

function Show-Help {
    Write-Host @"
╔════════════════════════════════════════════════════════════════╗
║     Workstation Microservicios - Gestor de Servicios          ║
╚════════════════════════════════════════════════════════════════╝

USO: .\manage-services.ps1 -Action <acción> [-Service <servicio>] [-Replicas <número>]

ACCIONES:
  up        → Iniciar todos los servicios
  down      → Detener todos los servicios
  rebuild   → Reconstruir imágenes Docker
  logs      → Ver logs de un servicio
  status    → Ver estado de los servicios
  scale     → Escalar un servicio a N replicas
  clean     → Limpiar volúmenes y contenedores
  help      → Mostrar esta ayuda

EJEMPLOS:
  .\manage-services.ps1 -Action up
  .\manage-services.ps1 -Action logs -Service gateway1
  .\manage-services.ps1 -Action scale -Service gateway -Replicas 3
  .\manage-services.ps1 -Action down
  .\manage-services.ps1 -Action rebuild -Service contract-service

SERVICIOS DISPONIBLES:
  - nginx
  - gateway1, gateway2
  - contract-service
  - mysql
  - all (predeterminado)

"@
}

function Start-Services {
    Write-Host "🚀 Iniciando servicios..." -ForegroundColor Green
    Set-Location $projectRoot
    docker-compose up -d --build
    Write-Host "✓ Servicios iniciados" -ForegroundColor Green
    Show-Status
}

function Stop-Services {
    Write-Host "🛑 Deteniendo servicios..." -ForegroundColor Yellow
    docker-compose down
    Write-Host "✓ Servicios detenidos" -ForegroundColor Green
}

function Rebuild-Services {
    Write-Host "🔨 Reconstruyendo servicios..." -ForegroundColor Cyan
    
    if ($Service -eq "all") {
        docker-compose up -d --build
    } else {
        docker-compose up -d --build $Service
    }
    
    Write-Host "✓ Servicios reconstruidos" -ForegroundColor Green
    Show-Status
}

function Show-Logs {
    if ($Service -eq "all") {
        Write-Host "📋 Mostrando logs de todos los servicios..." -ForegroundColor Blue
        docker-compose logs -f
    } else {
        Write-Host "📋 Mostrando logs de $Service..." -ForegroundColor Blue
        docker-compose logs -f $Service
    }
}

function Show-Status {
    Write-Host "`n📊 Estado de los servicios:" -ForegroundColor Blue
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
    
    docker-compose ps
    
    Write-Host "`n🌐 Endpoints:" -ForegroundColor Blue
    Write-Host "  Gateway (Balanceado)  : http://localhost" -ForegroundColor Green
    Write-Host "  Swagger Gateway       : http://localhost/swagger" -ForegroundColor Green
    Write-Host "  Swagger Contracts     : http://localhost/contracts-api/swagger" -ForegroundColor Green
    Write-Host "  MySQL                 : localhost:3306" -ForegroundColor Green
    Write-Host ""
}

function Scale-Services {
    Write-Host "📈 Escalando $Service a $Replicas replicas..." -ForegroundColor Cyan
    docker-compose up -d --scale $Service=$Replicas
    Write-Host "✓ Escalado completado" -ForegroundColor Green
    Show-Status
}

function Clean-Services {
    Write-Host "🧹 Limpiando volúmenes y contenedores..." -ForegroundColor Red
    $confirm = Read-Host "¿Estás seguro? (s/n)"
    
    if ($confirm -eq "s") {
        docker-compose down -v
        Write-Host "✓ Limpieza completada" -ForegroundColor Green
    } else {
        Write-Host "✗ Operación cancelada" -ForegroundColor Yellow
    }
}

# Ejecutar acción
switch ($Action) {
    "up" {
        Start-Services
    }
    "down" {
        Stop-Services
    }
    "rebuild" {
        Rebuild-Services
    }
    "logs" {
        Show-Logs
    }
    "status" {
        Show-Status
    }
    "scale" {
        Scale-Services
    }
    "clean" {
        Clean-Services
    }
    "help" {
        Show-Help
    }
    default {
        Show-Help
    }
}
