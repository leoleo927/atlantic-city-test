# Order Management System

Sistema completo de gestión de pedidos con backend en .NET 8 y frontend en React con TypeScript.

## Características

### Backend (.NET 8)
- Clean Architecture con carpetas organizadas
- Autenticación JWT con BCrypt
- CRUD completo de Pedidos con validaciones
- Repository Pattern
- Entity Framework Core + SQL Server
- **Patrones de Resiliencia con Polly:**
  - Circuit Breaker (50% fallos, 30s break duration)
  - Retry Policies (3 intentos, exponential backoff)
- Rate Limiting (protección contra ataques de fuerza bruta)
- Middleware global de manejo de excepciones
- CORS configurado
- Swagger/OpenAPI

### Frontend (React + TypeScript)
- React 19 con TypeScript
- Vite como build tool
- React Router para navegación
- Tailwind CSS para estilos
- Axios para peticiones HTTP
- Autenticación JWT con protección de rutas
- Gestión completa de pedidos (Crear, Editar, Listar, Eliminar)

## Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (v20.19.0 o superior recomendado)
- [pnpm](https://pnpm.io/) - Instalación: `npm install -g pnpm`
- [SQL Server](https://www.microsoft.com/sql-server/) (local o remoto)
- SQL Server Management Studio (opcional, para ejecutar scripts SQL)

## Instalación y Configuración

### 1. Configurar Base de Datos

1. Abrir SQL Server Management Studio
2. Conectarse a tu instancia de SQL Server (por defecto: `localhost`)
3. Abrir el archivo `backend/PedidosDB.sql`
4. Ejecutar el script completo (esto creará la base de datos, tablas y datos de prueba)

**Credenciales de prueba:**
- Email: `admin@test.com`
- Contraseña: `Admin123!`

### 2. Configurar Backend

```bash
# Navegar a la carpeta del backend
cd backend/OrderManagement.API

# Restaurar dependencias
dotnet restore

# Compilar el proyecto
dotnet build

# Ejecutar el backend
dotnet run
```

El backend estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000/swagger`

### 3. Configurar Frontend

```bash
# Navegar a la carpeta del frontend
cd frontend

# Instalar dependencias
pnpm install

# Ejecutar en modo desarrollo
pnpm dev
```

El frontend estará disponible en:
- URL: `http://localhost:5173`

## Estructura del Proyecto

```
ATLANTIC CITY/
├── backend/
│   ├── OrderManagement.API/
│   │   ├── Application/          # Lógica de aplicación
│   │   │   ├── DTOs/             # Data Transfer Objects
│   │   │   └── Services/         # Servicios de negocio
│   │   ├── Domain/               # Entidades del dominio
│   │   │   ├── Entities/
│   │   │   └── Interfaces/
│   │   ├── Infrastructure/       # Acceso a datos
│   │   │   ├── Data/             # DbContext
│   │   │   └── Repositories/
│   │   ├── Controllers/          # API Controllers
│   │   ├── Middleware/           # Middlewares personalizados
│   │   └── Program.cs            # Configuración de la app
│   └── PedidosDB.sql            # Script de base de datos
├── frontend/
│   ├── src/
│   │   ├── components/          # Componentes reutilizables
│   │   ├── pages/               # Páginas (Login, Pedidos)
│   │   ├── services/            # Llamadas API
│   │   ├── types/               # Tipos TypeScript
│   │   ├── utils/               # Utilidades (auth)
│   │   ├── App.tsx              # Router principal
│   │   └── main.tsx             # Punto de entrada
│   └── package.json
└── README.md
```

## Uso de la Aplicación

### Login
1. Abrir `http://localhost:5173`
2. Usar las credenciales de prueba:
   - Email: `admin@test.com`
   - Contraseña: `Admin123!`
3. Click en "Iniciar Sesión"

### Gestión de Pedidos
Una vez autenticado, podrás:
- **Ver listado** de todos los pedidos
- **Crear** un nuevo pedido con el botón "Nuevo Pedido"
- **Editar** un pedido existente
- **Eliminar** un pedido (eliminación lógica)
- **Cerrar sesión** con el botón en el navbar

### Estados de Pedidos
- **Registrado** (azul)
- **En Proceso** (amarillo)
- **Completado** (verde)

## API Endpoints

### Autenticación
- `POST /auth/login` - Iniciar sesión

### Pedidos (requiere autenticación)
- `GET /pedidos` - Listar todos los pedidos
- `GET /pedidos/{id}` - Obtener un pedido por ID
- `POST /pedidos` - Crear un nuevo pedido
- `PUT /pedidos/{id}` - Actualizar un pedido
- `DELETE /pedidos/{id}` - Eliminar un pedido (lógico)

## Seguridad y Resiliencia

### Rate Limiting
- **General**: 100 peticiones por minuto por IP
- **Login**: 5 intentos por minuto por IP (protección contra fuerza bruta)

### Patrones de Resiliencia (Polly)

#### Retry Policy (Política de Reintentos)
- **Reintentos máximos**: 3 intentos
- **Delay inicial**: 1 segundo
- **Estrategia**: Exponential Backoff
- **Uso**: Manejo de fallos transitorios en operaciones

#### Circuit Breaker (Interruptor de Circuito)
- **Ratio de fallo**: 50% (abre el circuito si la mitad de las peticiones fallan)
- **Ventana de muestreo**: 10 segundos
- **Throughput mínimo**: 5 peticiones antes de evaluar
- **Duración de apertura**: 30 segundos
- **Uso**: Previene llamadas repetidas a servicios que están fallando

### JWT
- Expiración: 6 horas
- Claims incluidos: `sub`, `email`, `name`, `role`
- Almacenamiento: localStorage en el navegador

## Validaciones

### Backend
- Total del pedido debe ser mayor que 0
- Número de pedido debe ser único
- Email y contraseña requeridos para login

### Frontend
- Campos requeridos en formularios
- Validación de formato de email
- Confirmación antes de eliminar

## Scripts Disponibles

### Backend
```bash
dotnet run              # Ejecutar en modo desarrollo
dotnet build            # Compilar el proyecto
dotnet test             # Ejecutar tests (si existen)
```

### Frontend
```bash
pnpm dev                # Modo desarrollo
pnpm build              # Compilar para producción
pnpm preview            # Preview del build de producción
pnpm lint               # Ejecutar linter
```

## Tecnologías Utilizadas

### Backend
- .NET 8
- Entity Framework Core 8
- SQL Server
- JWT Bearer Authentication
- BCrypt.Net (Password Hashing)
- Polly 8.2.1 (Circuit Breaker & Retry Policies)
- AspNetCoreRateLimit (Rate Limiting)
- Swagger/OpenAPI

### Frontend
- React 19
- TypeScript 5
- Vite 7
- React Router 7
- Axios
- Tailwind CSS 4
- pnpm

## Solución de Problemas

### Error: No se puede conectar a la base de datos
- Verificar que SQL Server esté corriendo
- Verificar la cadena de conexión en `appsettings.json`
- Ejecutar el script `PedidosDB.sql`

### Error: Rate Limit excedido
- Esperar 1 minuto antes de volver a intentar
- Para desarrollo, puedes ajustar los límites en `appsettings.json`

### Error: CORS
- Verificar que el backend esté corriendo en el puerto 5000
- Verificar que el frontend esté corriendo en el puerto 5173 o 3000

### Error: Token expirado
- Volver a iniciar sesión
- El token expira después de 6 horas

## Desarrollo

### Agregar nuevas migraciones (si es necesario)
```bash
cd backend/OrderManagement.API
dotnet ef migrations add NombreMigracion
dotnet ef database update
```

### Modificar configuración de Resiliencia
Los patrones de resiliencia se configuran en `backend/OrderManagement.API/Program.cs`:
- **Retry Policy**: Ajustar `MaxRetryAttempts`, `Delay` y `BackoffType`
- **Circuit Breaker**: Ajustar `FailureRatio`, `SamplingDuration`, `MinimumThroughput` y `BreakDuration`

### Modificar puerto del backend
Editar `backend/OrderManagement.API/Properties/launchSettings.json`

### Modificar puerto del frontend
Editar `frontend/vite.config.ts`
