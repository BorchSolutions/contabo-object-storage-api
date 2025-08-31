# Contabo Object Storage API

Una API REST desarrollada en .NET 8 para interactuar con Contabo Object Storage usando el protocolo S3 compatible. Este proyecto proporciona una interfaz completa para gestionar buckets y objetos en el servicio de almacenamiento de objetos de Contabo.

## 🚀 Características

- ✅ **Gestión de Buckets**: Crear, listar y eliminar buckets
- ✅ **Gestión de Objetos**: Subir, descargar, listar y eliminar archivos
- ✅ **URLs Pre-firmadas**: Generar enlaces temporales para acceso directo
- ✅ **CORS Habilitado**: Configurado para aplicaciones Angular
- ✅ **Docker Ready**: Contenedor optimizado para producción
- ✅ **Swagger/OpenAPI**: Documentación interactiva incluida
- ✅ **Logging Integrado**: Sistema completo de logs para monitoreo

## 📋 Prerrequisitos

- .NET 8 SDK
- Cuenta en Contabo Object Storage
- Visual Studio 2022 o VS Code (opcional)
- Docker (opcional para contenedores)

## 🔧 Instalación y Configuración

### 1. Clonar el Repositorio

```bash
git clone <repository-url>
cd ContaboObjectStorageAPI
```

### 2. Configurar Credenciales de Contabo

Edita el archivo `appsettings.json` y `appsettings.Development.json` con tus credenciales:

```json
{
  "ContaboS3": {
    "ServiceUrl": "https://usc1.contabostorage.com",
    "AccessKey": "your-access-key-here",
    "SecretKey": "your-secret-key-here"
  }
}
```

> ⚠️ **Importante**: Nunca subas las credenciales reales al repositorio. Usa User Secrets para desarrollo local.

### 3. Configurar User Secrets (Recomendado)

```bash
dotnet user-secrets init
dotnet user-secrets set "ContaboS3:AccessKey" "your-access-key"
dotnet user-secrets set "ContaboS3:SecretKey" "your-secret-key"
```

### 4. Restaurar Dependencias

```bash
dotnet restore
```

### 5. Ejecutar la Aplicación

```bash
dotnet run
```

La API estará disponible en:
- HTTP: `http://localhost:5154`
- HTTPS: `https://localhost:7154`
- Swagger UI: `http://localhost:5154/swagger`

## 🐳 Docker

### Construir la Imagen

```bash
docker build -t contabo-storage-api .
```

### Ejecutar el Contenedor

```bash
docker run -d \
  --name contabo-api \
  -p 8080:8080 \
  -e ContaboS3__AccessKey="your-access-key" \
  -e ContaboS3__SecretKey="your-secret-key" \
  contabo-storage-api
```

## 📚 Endpoints de la API

### Buckets

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/contabo/buckets` | Listar todos los buckets |
| POST | `/api/contabo/buckets/{bucketName}` | Crear un nuevo bucket |
| DELETE | `/api/contabo/buckets/{bucketName}` | Eliminar un bucket |

### Objetos

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/contabo/objects/{bucketName}` | Listar objetos en un bucket |
| POST | `/api/contabo/upload` | Subir un archivo |
| GET | `/api/contabo/download/{bucketName}/{key}` | Descargar un archivo |
| DELETE | `/api/contabo/delete/{bucketName}/{key}` | Eliminar un archivo |
| GET | `/api/contabo/presigned-url/{bucketName}/{key}` | Generar URL pre-firmada |

## 💡 Ejemplos de Uso

### Listar Buckets

```http
GET /api/contabo/buckets
Accept: application/json
```

**Respuesta:**
```json
[
  {
    "name": "my-bucket",
    "creationDate": "2024-01-15T10:30:00Z"
  }
]
```

### Crear Bucket

```http
POST /api/contabo/buckets/my-new-bucket
Content-Type: application/json
```

**Respuesta:**
```json
{
  "message": "Bucket created successfully"
}
```

### Subir Archivo

```http
POST /api/contabo/upload
Content-Type: multipart/form-data

{
  "bucketName": "my-bucket",
  "key": "documents/file.pdf",
  "file": [archivo]
}
```

**Respuesta:**
```json
{
  "success": true,
  "message": "File uploaded successfully",
  "objectUrl": "s3://my-bucket/documents/file.pdf",
  "eTag": "d41d8cd98f00b204e9800998ecf8427e"
}
```

### Listar Objetos

```http
GET /api/contabo/objects/my-bucket?prefix=documents/
Accept: application/json
```

**Respuesta:**
```json
[
  {
    "key": "documents/file.pdf",
    "bucketName": "my-bucket",
    "size": 1024,
    "lastModified": "2024-01-15T14:20:00Z",
    "eTag": "d41d8cd98f00b204e9800998ecf8427e",
    "contentType": "application/pdf"
  }
]
```

### Generar URL Pre-firmada

```http
GET /api/contabo/presigned-url/my-bucket/documents/file.pdf?expireMinutes=120
Accept: application/json
```

**Respuesta:**
```json
{
  "url": "https://usc1.contabostorage.com/my-bucket/documents/file.pdf?X-Amz-Algorithm=...",
  "expiresIn": 120
}
```

## 🔧 Integración con Angular

### Servicio Angular Example

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ContaboService {
  private apiUrl = 'http://localhost:5154/api/contabo';

  constructor(private http: HttpClient) { }

  getBuckets(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/buckets`);
  }

  uploadFile(bucketName: string, key: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('bucketName', bucketName);
    formData.append('key', key);
    formData.append('file', file);

    return this.http.post(`${this.apiUrl}/upload`, formData);
  }

  getObjects(bucketName: string, prefix?: string): Observable<any[]> {
    const params = prefix ? `?prefix=${prefix}` : '';
    return this.http.get<any[]>(`${this.apiUrl}/objects/${bucketName}${params}`);
  }
}
```

### Component Example

```typescript
import { Component } from '@angular/core';
import { ContaboService } from './contabo.service';

@Component({
  selector: 'app-file-manager',
  template: `
    <div class="file-manager">
      <input type="file" (change)="onFileSelected($event)" />
      <button (click)="uploadFile()" [disabled]="!selectedFile">
        Upload File
      </button>
      
      <div *ngFor="let object of objects" class="file-item">
        {{ object.key }} ({{ object.size | number }} bytes)
      </div>
    </div>
  `
})
export class FileManagerComponent {
  selectedFile: File | null = null;
  objects: any[] = [];

  constructor(private contaboService: ContaboService) {
    this.loadObjects();
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  uploadFile() {
    if (this.selectedFile) {
      this.contaboService.uploadFile('my-bucket', this.selectedFile.name, this.selectedFile)
        .subscribe(response => {
          console.log('Upload successful', response);
          this.loadObjects();
        });
    }
  }

  loadObjects() {
    this.contaboService.getObjects('my-bucket')
      .subscribe(objects => {
        this.objects = objects;
      });
  }
}
```

## 🏗️ Arquitectura del Proyecto

```
ContaboObjectStorageAPI/
├── Controllers/
│   └── ContaboController.cs      # Controlador principal de la API
├── Services/
│   ├── IS3Service.cs             # Interfaz del servicio S3
│   └── S3Service.cs              # Implementación del servicio S3
├── Models/
│   └── S3ObjectInfo.cs           # Modelos de datos
├── Program.cs                    # Configuración de la aplicación
├── appsettings.json              # Configuración general
├── appsettings.Development.json  # Configuración de desarrollo
├── Dockerfile                    # Configuración Docker
└── ContaboObjectStorageAPI.csproj # Archivo del proyecto
```

## 📦 Dependencias Principales

- **AWSSDK.S3** (3.7.308.1) - Cliente S3 para interactuar con Contabo
- **Microsoft.AspNetCore.OpenApi** (8.0.4) - Soporte OpenAPI/Swagger
- **Swashbuckle.AspNetCore** (6.6.2) - Generación de documentación Swagger
- **System.Text.Json** (8.0.5) - Serialización JSON optimizada

## ⚙️ Configuración Avanzada

### Variables de Entorno

| Variable | Descripción | Ejemplo |
|----------|-------------|---------|
| `ContaboS3__ServiceUrl` | URL del servicio Contabo | `https://usc1.contabostorage.com` |
| `ContaboS3__AccessKey` | Clave de acceso | `your-access-key` |
| `ContaboS3__SecretKey` | Clave secreta | `your-secret-key` |
| `ASPNETCORE_ENVIRONMENT` | Entorno de ejecución | `Development`, `Production` |

### CORS Configuration

Para modificar la configuración CORS, edita `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        builder.WithOrigins("http://localhost:4200", "https://yourdomain.com")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});
```

## 🔍 Logging y Monitoreo

El proyecto incluye logging estructurado usando `ILogger`. Los logs incluyen:

- Operaciones exitosas y errores
- Información de buckets y objetos
- Detalles de uploads y downloads
- Errores de autenticación y permisos

### Configurar Niveles de Log

En `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "ContaboObjectStorageAPI.Services": "Debug"
    }
  }
}
```

## 🚨 Manejo de Errores

La API maneja los siguientes tipos de errores:

- **400 Bad Request**: Parámetros inválidos o archivo faltante
- **404 Not Found**: Bucket u objeto no encontrado  
- **403 Forbidden**: Credenciales inválidas o permisos insuficientes
- **500 Internal Server Error**: Errores del servidor o conexión

### Ejemplo de Respuesta de Error

```json
{
  "error": "The specified bucket does not exist"
}
```

## 🔐 Seguridad

### Mejores Prácticas

1. **Credenciales**: Usa User Secrets en desarrollo y variables de entorno en producción
2. **HTTPS**: Siempre usa HTTPS en producción
3. **CORS**: Configura CORS solo para dominios confiables
4. **Validación**: Valida todos los inputs del usuario
5. **Logging**: No logees información sensible

### Configuración de Producción

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "yourdomain.com"
}
```

## 🧪 Testing

### Ejecutar con .NET CLI

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar con cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Probar con curl

```bash
# Listar buckets
curl -X GET "http://localhost:5154/api/contabo/buckets"

# Subir archivo
curl -X POST "http://localhost:5154/api/contabo/upload" \
  -F "bucketName=test-bucket" \
  -F "key=test-file.txt" \
  -F "file=@./local-file.txt"
```

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está licenciado bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.


## 🙏 Agradecimientos

- [Contabo](https://contabo.com/) por su servicio de Object Storage
- [AWS SDK para .NET](https://aws.amazon.com/sdk-for-net/) por la compatibilidad S3
- La comunidad de .NET por las mejores prácticas