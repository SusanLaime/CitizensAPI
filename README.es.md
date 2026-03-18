# CitizensAPI

< [EN](README.md) | [ES](README.es.md) >

Accede al contenido bilingüe seleccionando el idioma de tu preferencia arriba antes de continuar.

Un proyecto de API Web en .NET que administra ciudadanos mediante operaciones CRUD, persistencia en CSV, integración con una API externa y prácticas de Twelve-Factor App.

## Tabla de Contenidos

- [Descripción General del Proyecto](#descripción-general-del-proyecto)
- [Cómo Funciona](#cómo-funciona)
- [Arquitectura](#arquitectura)
- [Explicación de Twelve-Factor App](#explicación-de-twelve-factor-app)
  - [1. Codebase](#factor-1-codebase)
  - [2. Dependencies](#factor-2-dependencies)
  - [3. Config](#factor-3-config)
  - [4. Backing Services](#factor-4-backing-services)
  - [5. Build, Release, Run](#factor-5-build-release-run)
  - [6. Processes](#factor-6-processes)
  - [7. Port Binding](#factor-7-port-binding)
  - [8. Concurrency](#factor-8-concurrency)
  - [9. Disposability](#factor-9-disposability)
  - [10. Dev / Prod Parity](#factor-10-dev-prod-parity)
  - [11. Logs](#factor-11-logs)
  - [12. Admin Processes](#factor-12-admin-processes)
- [Tecnologías](#tecnologías)
- [Prerrequisitos](#prerrequisitos)
- [Instalación y Configuración](#instalación-y-configuración)
- [Configuración](#configuración)
- [Ejecución de la Aplicación](#ejecución-de-la-aplicación)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Desarrollo](#desarrollo)
- [Solución de Problemas](#solución-de-problemas)
- [Conclusión](#conclusión)

<a id="descripción-general-del-proyecto"></a>
## 📌 Descripción General del Proyecto

CitizensAPI es una API Web en .NET diseñada para administrar ciudadanos dentro de un sistema de registro. Soporta operaciones CRUD, persistencia basada en CSV, integración con una API externa, registro estructurado con Serilog y configuración mediante archivos `appsettings` y variables de entorno.

Cada registro de ciudadano incluye:

- `FirstName`
- `LastName`
- `CI`
- `BloodGroup`
- `PersonalAsset`

La aplicación también se integra con `https://api.restful-api.dev/objects` para asignar un activo personal aleatorio cuando se crea un nuevo ciudadano.

### Características Principales

- **Operaciones CRUD:** Crear, consultar, actualizar y eliminar ciudadanos
- **Persistencia en CSV:** Almacenar registros de ciudadanos en `CitizenDataStore.csv`
- **Integración con API Externa:** Obtener objetos aleatorios para asignar activos personales
- **Asignación Aleatoria de Grupo Sanguíneo:** Asignar automáticamente un grupo sanguíneo válido al crear un ciudadano
- **Arquitectura Basada en Controllers:** Organizar endpoints usando controllers de ASP.NET Core
- **Configuración por Entorno:** Cargar ajustes desde `appsettings.json`, `appsettings.Development.json` y variables de entorno
- **Logging con Serilog:** Registrar eventos de la aplicación en consola y archivos
- **Soporte con Swagger:** Habilitar pruebas interactivas de la API en desarrollo
- **Alineación con Twelve-Factor:** Documentar cómo el proyecto aplica los principios Twelve-Factor en la práctica

<a id="cómo-funciona"></a>
## 🧭 Cómo Funciona

### Estructura Basada en Controllers

El proyecto utiliza una arquitectura basada en controllers en lugar del estilo minimal API. Este enfoque hace que la API sea más clara, más fácil de mantener y más fácil de extender a medida que el proyecto crece.

La aplicación inicia en `Program.cs`, donde carga la configuración, configura Serilog, registra servicios y habilita Swagger en el entorno de desarrollo.

Las solicitudes de cliente son manejadas por `CitizenController`, que expone los endpoints CRUD para ciudadanos. El controller lee y actualiza los datos de ciudadanos almacenados en `CitizenDataStore.csv`, mientras que `CitizenBGService` se conecta a la API externa para obtener objetos aleatorios usados como activos personales.

La lógica de soporte está separada en partes pequeñas:

- `Models` define los objetos de dominio y de solicitud
- `Services` maneja la integración externa
- `Utils` contiene lógica auxiliar para lectura y escritura de CSV

Las decisiones de arquitectura, el flujo de configuración, el modelo de ejecución y el comportamiento operativo de la aplicación se explican con mayor detalle en la sección **Explicación de Twelve-Factor App**, donde cada aspecto relevante del proyecto se conecta con su factor correspondiente.

<a id="arquitectura"></a>
## 🧱 Arquitectura

```text
Program.cs
   |
   v
Configuration + Serilog + Dependencies Registration
   |
   v
CitizenController
   |
   v
Request Handling + CRUD Flow
   |
   +--> CSVHelper --> CitizenDataStore.csv
   |
   +--> CitizenBGService --> External API
```

La aplicación inicia en `Program.cs`, donde se inicializan configuración, logging, registro de dependencias y middleware. Las solicitudes HTTP entrantes son manejadas por `CitizenController`, que coordina el manejo de solicitudes, operaciones CRUD, persistencia en CSV mediante `CSVHelper` y obtención de objetos externos a través de `CitizenBGService`.

<a id="explicación-de-twelve-factor-app"></a>
## 📚 Explicación de Twelve-Factor App

<a id="factor-1-codebase"></a>
### 1. Codebase

> One codebase tracked in revision control, many deploys.<br>
> Un solo codebase rastreado en control de versiones, muchos despliegues.

El proyecto se administra en un único repositorio Git y está publicado en GitHub. El desarrollo se realizó mediante commits en la rama de práctica `P2-001`.

Mantener el proyecto en un solo codebase compartido con historial visible favorece la trazabilidad y se alinea con este factor.

<a id="factor-2-dependencies"></a>
### 2. Dependencies

> Explicitly declare and isolate dependencies.<br>
> Declarar y aislar dependencias de forma explícita.

Las dependencias están declaradas explícitamente en el archivo de proyecto `CitizensAPI.csproj`.

Dependencias principales:

- `Microsoft.AspNetCore.OpenApi`
- `Swashbuckle.AspNetCore`
- `Newtonsoft.Json`
- `Serilog`

<a id="factor-3-config"></a>
### 3. Config

> Store config in the environment.<br>
> Almacenar la configuración en el entorno.

La configuración práctica de la aplicación se describe en la sección [Configuración](#configuración).

Desde la perspectiva Twelve-Factor, este proyecto externaliza la configuración mediante `appsettings.json`, `appsettings.Development.json` y variables de entorno en lugar de hardcodear valores operativos en controllers o servicios.

<a id="factor-4-backing-services"></a>
### 4. Backing Services

> Treat backing services as attached resources.<br>
> Tratar los servicios de apoyo como recursos adjuntos.

El proyecto utiliza el servicio externo:

- `https://api.restful-api.dev/objects`

Este servicio se trata como un recurso adjunto que puede reemplazarse o reconfigurarse sin cambiar la lógica de negocio principal.

<a id="factor-5-build-release-run"></a>
### 5. Build, Release, Run

> Strictly separate build and run stages.<br>
> Separar estrictamente las etapas de build y run.

La aplicación separa las siguientes etapas:

- build: `dotnet build`
- run: `dotnet run`

La release puede generarse a partir del estado del repositorio y su configuración sin modificar el código fuente.

En este proyecto, build y run se tratan como etapas separadas y repetibles.

<a id="factor-6-processes"></a>
### 6. Processes

> Execute the app as one or more stateless processes.<br>
> Ejecutar la aplicación como uno o más procesos sin estado.

La API está diseñada para comportarse como un proceso web stateless.

Los datos de ciudadanos no se almacenan permanentemente dentro de la aplicación en ejecución. En su lugar, los datos persistentes se guardan en el archivo CSV:

- `CitizenDataStore.csv`

Cada operación lee el estado actual desde el archivo, aplica el cambio solicitado y escribe el estado actualizado nuevamente en el CSV. El proyecto también utiliza `async/await` para llamadas a la API externa, evitando bloqueos innecesarios mientras espera respuestas.

<a id="factor-7-port-binding"></a>
### 7. Port Binding

> Export services via port binding.<br>
> Exponer servicios mediante port binding.

La aplicación se expone a través del servidor web de ASP.NET Core y puede accederse mediante el puerto local configurado al ejecutarse con `dotnet run` o con el perfil de launch settings.

En el repositorio actual, el endpoint local configurado es:

- `https://localhost:9070`

Esto está definido en:

- `Properties/launchSettings.json`

Configuración actual:

```json
{
  "profiles": {
    "https": {
      "applicationUrl": "https://localhost:9070"
    }
  }
}
```

<a id="factor-8-concurrency"></a>
### 8. Concurrency

> Scale out via the process model.<br>
> Escalar horizontalmente mediante el modelo de procesos.

Este factor no está implementado completamente en el proyecto actual.

La aplicación está diseñada de forma mayormente stateless a nivel de API, pero su capa de persistencia es un único archivo CSV. El proyecto incluye locking en proceso para acceso al archivo, lo que ayuda a evitar corrupción dentro de una sola instancia en ejecución. Sin embargo, como la aplicación reescribe el CSV durante operaciones de creación, actualización y eliminación, no ofrece coordinación completa para múltiples procesos o múltiples instancias desplegadas.

Por esa razón, el proyecto no está preparado para un escalado concurrente real entre múltiples procesos o instancias. El uso de `async/await` mejora el manejo de llamadas a la API externa al evitar bloqueos innecesarios, pero por sí solo no garantiza concurrencia segura ni escalabilidad horizontal. En este proyecto, la concurrencia se aborda solo de manera conceptual.

En el futuro, este factor podría aplicarse con más fuerza mediante:

- reemplazar la persistencia CSV con una base de datos que soporte transacciones y escrituras concurrentes
- agregar una estrategia más segura de coordinación de escritura
- ejecutar múltiples instancias de la API detrás de un balanceador de carga
- separar solicitudes web de procesos worker si el sistema crece

<a id="factor-9-disposability"></a>
### 9. Disposability

> Maximize robustness with fast startup and graceful shutdown.<br>
> Maximizar la robustez con inicio rápido y apagado controlado.

La aplicación inicia rápidamente con `dotnet run` y puede detenerse sin pasos complejos de apagado, por ejemplo presionando `Ctrl + C`. Como los datos persistentes se almacenan en el CSV, un reinicio del proceso no provoca pérdida de datos de ciudadanos.

<a id="factor-10-dev-prod-parity"></a>
### 10. Dev / Prod Parity

> Keep development, staging, and production as similar as possible.<br>
> Mantener desarrollo, staging y producción lo más similares posible.

Desarrollo y producción deben permanecer lo más similares posible mediante:

- usar el mismo codebase
- usar las mismas definiciones de dependencias
- usar archivos de configuración y variables de entorno para valores específicos de cada entorno

Esto reduce diferencias innecesarias entre entornos.

Es importante evitar ruido específico de una máquina dentro del repositorio y mantener el proyecto portable mediante configuración y exclusión de archivos generados.

<a id="factor-11-logs"></a>
### 11. Logs

> Treat logs as event streams.<br>
> Tratar los logs como flujos de eventos.

Los logs se tratan como flujos de eventos escritos por la aplicación.

La implementación actual registra:

- creación de ciudadanos
- actualización de ciudadanos
- eliminación de ciudadanos
- llamadas a la API externa
- fallos de lectura de archivos
- fallos de operaciones de la API

En este proyecto, Serilog utiliza niveles de log para controlar qué eventos se registran. El orden de severidad comienza con `Debug`, seguido de `Information`, `Warning`, `Error` y `Fatal`.

Cuando `MinimumLevel` está configurado en `Debug`, la aplicación no registra solo mensajes debug. Registra todos los mensajes desde `Debug` hacia arriba, incluyendo `Information`, `Warning`, `Error` y `Fatal`. En otras palabras, `Debug` actúa como el umbral más bajo, por lo que todos los niveles superiores también quedan incluidos.

Esto es útil en desarrollo porque permite un rastreo detallado del comportamiento de la aplicación.

En la implementación actual, `Debug` se usa para trazas internas de flujo, como cargar el CSV, buscar ciudadanos, preparar escrituras de archivo y enviar solicitudes a la API externa. `Information` se usa para eventos de negocio exitosos, como cargar ciudadanos, crear un ciudadano, actualizarlo, eliminarlo o recibir datos válidos de la API externa.

`Warning` se usa para situaciones importantes que no rompen completamente la aplicación. Por ejemplo, cuando un ciudadano no existe, cuando se detecta un CI duplicado, cuando la API externa no devuelve activos disponibles o cuando se encuentran filas mal formadas en el CSV. `Error` se usa en bloques `try-catch` y escenarios de fallo donde una operación no pudo completarse correctamente.

Esto complementa la idea Twelve-Factor de tratar los logs como event streams, porque la aplicación no solo escribe logs, sino que también los clasifica por severidad y propósito.

<a id="factor-12-admin-processes"></a>
### 12. Admin Processes

> Run admin/management tasks as one-off processes.<br>
> Ejecutar tareas administrativas o de gestión como procesos de una sola ejecución.

Este factor no está implementado completamente como un proceso administrativo separado en el proyecto actual.

En este proyecto, el repositorio no incluye un script o comando dedicado para tareas administrativas como:

- limpiar el archivo CSV
- reiniciar los datos almacenados
- sembrar registros iniciales
- migrar la estructura del archivo

Sin embargo, el proyecto ya incluye elementos de soporte que pueden ayudar a futuras tareas de mantenimiento y administración.

Por ejemplo, la implementación actual de logging ayuda a:

- revisar eventos de creación, actualización y eliminación de ciudadanos
- detectar fallos de la API externa
- identificar problemas de lectura o escritura de archivos
- apoyar tareas de troubleshooting, mantenimiento y análisis operativo

Por ello, este factor se aborda solo de forma parcial y conceptual en esta práctica.

Los logs proporcionan soporte útil para mantenimiento y administración, pero no reemplazan un admin process real. En este proyecto, una tarea administrativa real sería un script o comando de una sola ejecución para actividades como limpiar `CitizenDataStore.csv`, precargar ciudadanos de ejemplo, reparar filas mal formadas o migrar la estructura del CSV si el modelo cambia.

En el futuro, este factor podría aplicarse de forma más completa agregando un script o comando de mantenimiento dedicado para limpiar, reiniciar, sembrar o migrar los datos del CSV.

<a id="endpoints-de-la-api"></a>
#### Endpoints de la API

### Crear Ciudadano

- `POST /api/Citizen`

Comportamiento:

- el body de la solicitud incluye solo `FirstName`, `LastName` y `CI`
- valida CI duplicado
- asigna un grupo sanguíneo aleatorio
- llama a la API externa
- asigna un activo personal aleatorio
- almacena el ciudadano en el CSV

### Obtener Todos los Ciudadanos

- `GET /api/Citizen`

### Obtener Ciudadano por CI

- `GET /api/Citizen/{id}`

### Actualizar Ciudadano

- `PUT /api/Citizen/{ci}`

Regla de negocio:

- el body de la solicitud incluye solo `FirstName` y `LastName`
- solo se actualizan `FirstName` y `LastName`

### Eliminar Ciudadano

- `DELETE /api/Citizen/{ci}`

<a id="notas-operativas"></a>
#### Notas Operativas

- Swagger está habilitado en modo desarrollo.
- El proyecto usa finales de línea LF mediante `.gitattributes`.
- El archivo CSV es la capa de persistencia de esta práctica.
- El perfil actual de lanzamiento usa `https://localhost:9070`.
- El repositorio sigue el estilo orientado a controllers en lugar del estilo minimal scripting.

Todos estos elementos pueden ayudar a un administrador a entender cómo la aplicación se configura, ejecuta y mantiene.

<a id="tecnologías"></a>
## 🧰 Tecnologías

- **Framework:** ASP.NET Core Web API (.NET 10)
- **Lenguaje:** C#
- **Documentación de API:** Swagger / Swashbuckle
- **Logging:** Serilog
- **Integración HTTP Externa:** `HttpClient`
- **Manejo de JSON:** Newtonsoft.Json
- **Configuración:** `appsettings.json`, `appsettings.Development.json` y variables de entorno
- **Persistencia:** Almacenamiento en CSV mediante `CitizenDataStore.csv`
- **Control de Versiones:** Git y GitHub

<a id="prerrequisitos"></a>
## 📋 Prerrequisitos

Antes de ejecutar el proyecto, asegúrate de contar con:

- **.NET SDK 10.0**
- **Git**, si deseas clonar el repositorio y manejar versiones localmente
- **Visual Studio** o **Visual Studio Code** con soporte para C#
- **Conexión a internet**, porque la aplicación consume `https://api.restful-api.dev/objects`

<a id="instalación-y-configuración"></a>
## 🧩 Instalación y Configuración

1. Clona el repositorio.
2. Abre el proyecto en tu editor o IDE.
3. Restaura dependencias y compila el proyecto.
4. Revisa los archivos de configuración.

### Paso 1: Clonar el Repositorio

```bash
# SSH
git clone git@github.com:SusanLaime/CitizensAPI.git
cd CitizensAPI
```

or 
```bash
#HTTPS
git clone https://github.com/SusanLaime/CitizensAPI.git
cd CitizensAPI
```

### Paso 2: Abrir el Proyecto

Abre la carpeta de la solución en tu editor o IDE preferido, por ejemplo:

- Visual Studio
- Visual Studio Code

### Paso 3: Restaurar y Compilar el Proyecto

Restaura dependencias usando:

```bash
dotnet restore
```

Compila la aplicación usando:

```bash
dotnet build
```

### Paso 4: Revisar la Configuración

La aplicación lee configuración desde:

- `appsettings.json`
- `appsettings.Development.json`
- `Properties/launchSettings.json`

Estos archivos definen ajustes importantes como:

- ubicación del almacenamiento CSV
- URL base de la API externa
- configuración de Serilog
- valores del entorno local de desarrollo

<a id="configuración"></a>
## 🗝️ Configuración

La aplicación utiliza archivos de configuración externos y variables de entorno para definir su comportamiento en tiempo de ejecución.

Las fuentes principales de configuración son:

- `appsettings.json`
- `appsettings.Development.json`
- `Properties/launchSettings.json`
- variables de entorno

Estas fuentes controlan valores como:

- ubicación del almacenamiento CSV
- URL base de la API externa
- niveles y salidas de Serilog
- selección del entorno mediante `ASPNETCORE_ENVIRONMENT`

Esta configuración se carga en `Program.cs`, permitiendo mantener valores operativos fuera de la lógica de negocio.

<a id="ejecución-de-la-aplicación"></a>
## 🚦 Ejecución de la Aplicación

### Iniciar la API

Usa el siguiente comando para ejecutar la API localmente:

```bash
dotnet run
```

### Características de Desarrollo

Cuando el entorno está configurado como `Development`, la aplicación también habilita:

- Swagger UI para probar endpoints
- configuración específica de desarrollo desde `appsettings.Development.json`

### Notas

- Asegúrate de que la API externa `https://api.restful-api.dev/objects` sea alcanzable
- Asegúrate de que la ruta del CSV configurada en los app settings sea válida
- Los archivos de log se generan automáticamente mediante Serilog durante la ejecución

<a id="estructura-del-proyecto"></a>
## 🗃️ Estructura del Proyecto

El repositorio está organizado de la siguiente manera:

```text
CitizensAPI/
├── Controllers/
│   └── CitizenController.cs
├── Models/
│   ├── BloodGroups.cs
│   ├── Citizen.cs
│   ├── CitizenBG.cs
│   ├── CreateCitizenRequest.cs
│   ├── Response.cs
│   └── UpdateCitizenRequest.cs
├── Properties/
│   └── launchSettings.json
├── Services/
│   └── CitizenService.cs
├── Utils/
│   └── CSVHelper.cs
├── appsettings.Development.json
├── appsettings.json
├── CitizenDataStore.csv
├── CitizensAPI.csproj
├── CitizensAPI.sln
├── Program.cs
└── README.md
```

Artefactos generados en tiempo de ejecución como `Logs/`, `bin/` y `obj/` no forman parte de la estructura principal del código y por eso se omiten de esta vista.

Responsabilidades principales:

- `Controllers` expone los endpoints de la API y coordina operaciones CRUD
- `Models` contiene entidades de dominio y modelos de solicitud
- `Services` maneja la comunicación con la API externa. En la implementación actual, la clase del servicio es `CitizenBGService` y se encuentra en `CitizenService.cs`
- `Utils` contiene lógica auxiliar para persistencia en CSV
- `Program.cs` configura pipeline, servicios, logging y Swagger

<a id="desarrollo"></a>
## 🧪 Desarrollo

### Flujo de Desarrollo

1. **Abrir el Proyecto**
   - Abre la solución en Visual Studio o la carpeta del proyecto en Visual Studio Code

2. **Hacer Cambios**
   - Modifica controllers, services, models, archivos de configuración o utilidades según la funcionalidad a implementar

3. **Compilar el Proyecto**
   - Ejecuta `dotnet build` para verificar que el código compile correctamente

4. **Ejecutar la Aplicación**
   - Ejecuta `dotnet run` para validar el comportamiento localmente

5. **Verificar los Cambios**
   - Prueba los endpoints con Swagger
   - Revisa los logs generados por Serilog
   - Confirma que la persistencia en CSV funcione correctamente

### Estilo de Código

- **Lenguaje:** C#
- **Estilo de Arquitectura:** ASP.NET Core Web API basado en controllers
- **Formato:** Formato consistente de C# mediante el IDE o herramientas del editor
- **Convenciones de Nombres:**
  - **Clases y archivos:** PascalCase, por ejemplo `CitizenController.cs`
  - **Métodos y propiedades:** PascalCase, por ejemplo `GetCitizenBGs`
  - **Campos privados:** `_camelCase`, por ejemplo `_httpClient`
  - **Variables locales y parámetros:** camelCase, por ejemplo `citizenRequest`

### Patrones de Desarrollo Clave

1. **Controllers**
   - Manejan solicitudes HTTP y coordinan el flujo principal de la aplicación

2. **Services**
   - Encapsulan comunicación con la API externa y lógica de apoyo

3. **Models**
   - Representan cuerpos de solicitud y entidades de dominio

4. **Utilities**
   - Centralizan lógica reutilizable, como operaciones sobre archivos CSV

5. **Configuration**
   - Mantienen los valores de ejecución fuera de la lógica de negocio mediante archivos de configuración y variables de entorno

6. **Logging**
   - Usan Serilog para registrar eventos de desarrollo y ejecución con niveles apropiados

### Validación Durante el Desarrollo

Actualmente, el proyecto se valida mediante:

- `dotnet build` para verificar compilación correcta
- `dotnet run` para ejecutar la API localmente
- Swagger para probar endpoints de forma interactiva
- salidas de Serilog para revisar comportamiento en ejecución y errores

### Build para Producción

Una versión lista para producción o una salida publicable puede generarse con:

```bash
dotnet publish -c Release
```

Este comando crea una salida optimizada para despliegue. La opción `-c Release` le indica a .NET que use la configuración **Release** en lugar de la configuración orientada a desarrollo.

Ejecutar `dotnet publish -c Release` no cambia el flujo normal de desarrollo. Después de publicar, el proyecto puede seguir compilándose y ejecutándose localmente con `dotnet build` y `dotnet run` como siempre.

<a id="solución-de-problemas"></a>
## 🆘 Solución de Problemas

### Problemas de Compilación

Si el proyecto no compila correctamente, ejecuta:

```bash
dotnet restore
dotnet build
```

Verifica que:

- el .NET SDK 10.0 esté instalado correctamente
- todas las dependencias de NuGet se hayan restaurado
- el archivo `CitizensAPI.csproj` sea válido
- no existan errores de sintaxis en archivos modificados recientemente

### Problemas de Ejecución

Si la aplicación no inicia correctamente, ejecuta:

```bash
dotnet run
```

Verifica que:

- el proyecto compile correctamente primero
- el puerto configurado no esté siendo utilizado por otra aplicación
- los valores en `launchSettings.json` sean válidos
- la configuración del entorno sea correcta

### Problemas de Configuración

Si la aplicación no encuentra archivos o servicios, verifica las siguientes fuentes de configuración:

- `appsettings.json`
- `appsettings.Development.json`
- `Properties/launchSettings.json`

Presta especial atención a:

- `Data:Location`
- `External Services:ObjectsApi:BaseUrl`
- `ASPNETCORE_ENVIRONMENT`

Valores incorrectos pueden impedir que la aplicación lea el archivo CSV o se conecte a la API externa.

### Problemas con la API Externa

Si los activos personales no se asignan correctamente:

- verifica que `https://api.restful-api.dev/objects` sea accesible
- verifica que la URL base configurada sea correcta
- revisa logs de warning y error generados por Serilog
- confirma que la API externa esté devolviendo datos válidos

### Problemas de Persistencia en CSV

Si los datos de ciudadanos no se almacenan o actualizan correctamente:

- verifica que la ruta del CSV configurada exista
- verifica que la aplicación tenga permisos de lectura y escritura sobre el archivo
- confirma que el formato del CSV sea válido
- revisa logs relacionados con errores de lectura o escritura

### Problemas de Logging

Si los logs no aparecen como se espera, verifica:

- configuración de Serilog en `appsettings.json`
- configuración de Serilog en `appsettings.Development.json`
- el `MinimumLevel` configurado
- la existencia de la ruta de salida de logs

También confirma que la aplicación esté ejecutándose en el entorno esperado para que se cargue el archivo de configuración correcto.

### Comandos Comunes de Validación

```bash
dotnet restore
dotnet build
dotnet run
dotnet publish -c Release
```

<a id="conclusión"></a>
## 📝 Conclusión

Este proyecto aplica los principios de Twelve-Factor App a un nivel práctico mediante gestión de configuración, integración con servicios externos, logging estructurado y una arquitectura simple orientada al mantenimiento. Aunque algunos factores, como concurrencia y admin processes, están implementados solo parcialmente, el repositorio documenta con claridad tanto lo que ya funciona como lo que podría mejorarse en una iteración futura.
