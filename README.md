# Sensor Monitoring IoT API (.NET 8 Clean Architecture)

[![.NET 8](https://img.shields.io/badge/.NET-8.0%20LTS-purple.svg)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20%2F%20Onion-darkgreen.svg)](#arquitectura)
[![Tests](https://img.shields.io/badge/Tests-10%2F10%20Passed-brightgreen.svg)](#pruebas-automatizadas)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

API RESTful de alto rendimiento desarrollada en **.NET 8 LTS** y **C# 12** siguiendo los principios de **Clean Architecture (Onion)** y **Domain-Driven Design (DDD)** para la ingesta, validación y detección de anomalías en tiempo real de telemetría proveniente de dispositivos y sensores industriales IoT (turbinas, generadores, motores eléctricos).

---

## 📌 Contexto y Problema de Negocio

En entornos de monitorización industrial (SCADA, generación de energía y flotas IoT), los sensores transmiten lecturas continuas de variables físicas críticas (temperatura, vibración mecánica, voltaje de baterías de respaldo).

### Desafíos resueltos:
1. **Reglas de negocio desacopladas:** Separación total de las entidades del dominio de la base de datos y de los controladores HTTP.
2. **Evaluación de anomalías en caliente:** Reglas de umbrales automáticas que clasifican lecturas críticas (`Warning` / `Anomalía`) y actualizan el estado de salud de la maquinaria en tiempo real.
3. **Mantenibilidad enterprise:** Arquitectura modular por capas con Inyección de Dependencias nativa, Entity Framework Core y especificación OpenAPI / Swagger UI.

---

## 🏛️ Arquitectura del Sistema (Clean Architecture)

El proyecto está organizado en 5 módulos desacoplados bajo una solución unificada (`SensorMonitoring.sln`):

```
portafolio/08-csharp-iot-monitoring-api/
├── src/
│   ├── SensorMonitoring.Domain/            # Capa Núcleo: Entidades ricas, Enums, Excepciones e Interfaces de Repositorio
│   │   ├── Entities/                       # SensorDevice, TelemetryReading
│   │   ├── Enums/                          # SensorType, DeviceStatus
│   │   ├── Exceptions/                     # DeviceNotFoundException
│   │   └── Repositories/                   # IDeviceRepository, ITelemetryRepository
│   │
│   ├── SensorMonitoring.Application/       # Capa de Aplicación: Casos de uso, DTOs y Servicios de Negocio
│   │   ├── DTOs/                           # RegisterDeviceDto, RecordTelemetryDto, TelemetryResponseDto, DeviceStatsDto
│   │   └── Services/                       # IDeviceService, ITelemetryService, DeviceService, TelemetryService
│   │
│   ├── SensorMonitoring.Infrastructure/    # Capa de Infraestructura: Persistencia EF Core InMemory y Repositorios
│   │   ├── Persistence/                    # SensorDbContext
│   │   ├── Repositories/                   # DeviceRepository, TelemetryRepository
│   │   └── DependencyInjection.cs          # Registro de IoC container
│   │
│   └── SensorMonitoring.WebApi/            # Capa de Presentación: Controladores RESTful y Swagger UI
│       ├── Controllers/                    # DevicesController, TelemetryController
│       ├── Program.cs                      # Pipeline HTTP, CORS, Swagger y Data Seeding
│       └── appsettings.json
│
└── tests/
    └── SensorMonitoring.Tests/             # Capa de Calidad: Pruebas unitarias con xUnit, Moq y FluentAssertions
        ├── Domain/                         # DeviceTests (Validaciones e invariantes de dominio)
        └── Services/                       # TelemetryServiceTests (Mocking de repositorios y reglas de negocio)
```

---

## 🚀 Endpoints de la API RESTful

La API cuenta con documentación interactiva **Swagger UI** servida automáticamente en la ruta raíz (`/`):

### 1. Gestión de Dispositivos (`/api/v1/devices`)
| Método | Endpoint | Descripción | Status Codes |
|---|---|---|---|
| `POST` | `/api/v1/devices` | Registra un nuevo dispositivo sensor | `201 Created`, `400 Bad Request` |
| `GET` | `/api/v1/devices/{deviceCode}` | Obtiene el detalle y estado de un sensor | `200 OK`, `404 Not Found` |
| `GET` | `/api/v1/devices` | Lista todos los sensores registrados | `200 OK` |

### 2. Telemetría y Detección de Anomalías (`/api/v1/telemetry`)
| Método | Endpoint | Descripción | Status Codes |
|---|---|---|---|
| `POST` | `/api/v1/telemetry` | Ingesta lectura, evalúa umbrales y actualiza estado | `201 Created`, `404 Not Found` |
| `GET` | `/api/v1/telemetry/readings/{deviceCode}` | Histórico de lecturas de un dispositivo | `200 OK`, `404 Not Found` |
| `GET` | `/api/v1/telemetry/anomalies` | Lista lecturas anómalas recientes que superan umbrales | `200 OK` |
| `GET` | `/api/v1/telemetry/stats/{deviceCode}` | Métricas agregadas (promedio última hora, anomalías) | `200 OK`, `404 Not Found` |

---

## 🧪 Pruebas Automatizadas (Unit Tests)

Se implementaron pruebas unitarias exhaustivas con **xUnit**, **Moq** y **FluentAssertions**, validando:
- Invariantes de dominio (código no nulo, normalización a mayúsculas, actualización de heartbeat).
- Detección de anomalías en telemetría por tipo de sensor (Temperatura $> 75^\circ\text{C}$, Vibración $> 8.5\,\text{mm/s}$, Batería $< 10.5\,\text{V}$).
- Aislamiento de capas mediante Mocks de `IDeviceRepository` y `ITelemetryRepository`.
- Control de excepciones de negocio (`DeviceNotFoundException`).

### Ejecución de Pruebas:
```bash
dotnet test
```

### Resultado de la Ejecución:
```text
Serie de pruebas para SensorMonitoring.Tests.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (x64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error: 0, Superado: 10, Omitido: 0, Total: 10, Duración: 170 ms
```

---

## 💻 Instrucciones de Instalación y Ejecución

### Prerrequisitos:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado.

### 1. Clonar el repositorio
```bash
git clone https://github.com/Gus7Ghost/csharp-iot-monitoring-api.git
cd csharp-iot-monitoring-api
```

### 2. Compilar la solución
```bash
dotnet build
```

### 3. Iniciar la API
```bash
dotnet run --project src/SensorMonitoring.WebApi
```

### 4. Probar en Swagger UI
Abra su navegador web en:
```
http://localhost:5000/
o
https://localhost:7001/
```
*La base de datos en memoria viene precargada automáticamente con dos sensores de prueba (`SENS-TEMP-101` y `SENS-VIB-202`) y sus respectivas lecturas para pruebas inmediatas.*

---

## 👤 Autor

**Gustavo Bryan López Cosme**  
- **Rol:** Estudiante de 9no ciclo de Ingeniería de Sistemas — Universidad Nacional de Ingeniería (UNI, Lima, Perú)
- **Investigador:** LabIAR (Laboratorio de Inteligencia Artificial y Robótica - UNI)
- **Email:** [glopezc@uni.pe](mailto:glopezc@uni.pe)
- **GitHub:** [@Gus7Ghost](https://github.com/Gus7Ghost)
- **LinkedIn:** [Perfil en LinkedIn](https://www.linkedin.com)
