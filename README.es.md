# CitizensAPI

< [EN](README.md) | [ES](README.es.md) >

Accede al contenido bilingue seleccionando tu idioma preferido arriba antes de continuar.

API de registro ciudadano construida con ASP.NET Core, persistencia en CSV, integracion con API externa e ideas practicas de Twelve-Factor App.

| Campo | Descripcion |
| --- | --- |
| Estilo de API | ASP.NET Core Web API basada en controllers |
| Persistencia | `CitizenDataStore.csv` |
| Servicio externo | `https://api.restful-api.dev/objects` |
| Estado | Proyecto academico, funcional para el curso, no listo para produccion. |

## Tabla de Contenidos

- [👤 Contexto de la Autora y Desarrollo del Proyecto](#author-background)
- [📌 Descripcion General](#project-overview)
- [🧭 Como Funciona](#how-it-works)
- [🧱 Arquitectura](#architecture)
- [📚 Explicacion de Twelve-Factor App](#12-factor-app-explanation)
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
- [🔌 Endpoints de la API](#api-endpoints)
- [🧰 Stack Tecnologico](#tech-stack)
- [📋 Prerrequisitos](#prerequisites)
- [🧩 Instalacion y Configuracion](#installation--setup)
- [▶️ Ejecucion de la Aplicacion](#running-the-application)
- [🗂️ Estructura del Proyecto](#project-structure)
- [💻 Desarrollo](#development)
- [🛠️ Solucion de Problemas](#troubleshooting)
- [🔒 Notas Criticas de Seguridad](#critical-security-notes)
- [🛡️ Mejoras de Seguridad](#security-improvements)
- [📝 Conclusion](#conclusion)
- [📚 Referencias](#references)

<a id="author-background"></a>
## 👤 Contexto de la Autora y Desarrollo del Proyecto

`CitizensAPI` es un proyecto academico de ASP.NET Core Web API desarrollado por `Susan Laime Lucero` para la materia `Certificacion I` en la Universidad Privada Boliviana (UPB). El proyecto se mantiene en el repositorio `https://github.com/SusanLaime/CitizensAPI` e incluye ASP.NET Core, Serilog, Swagger y persistencia basada en CSV. Tambien esta documentado en ingles y en espanol mediante `README.md` y `README.es.md`. Si tienes preguntas sobre el proyecto o su implementacion, puedes contactarme en `susanlaimel1@upb.edu`. Este proyecto se comparte bajo la `MIT License` y actualmente se presenta como un proyecto academico en estado activo de trabajo. <br>

Ultima actualizacion: `March 19, 2026`.

<a id="project-overview"></a>
## 📌 Descripcion General

CitizensAPI es una aplicacion Web API en .NET creada para gestionar ciudadanos dentro de un sistema de registro. Soporta operaciones CRUD, persistencia basada en CSV, integracion con una API externa, logging estructurado con Serilog y configuracion mediante archivos `appsettings` y variables de entorno.

Cada registro de ciudadano incluye `FirstName`, `LastName`, `CI`, `BloodGroup` y `PersonalAsset`. La API tambien se integra con `https://api.restful-api.dev/objects` para asignar un activo personal aleatorio cuando se crea un nuevo ciudadano.

### Caracteristicas Principales

| Caracteristica | Descripcion |
| --- | --- |
| Operaciones CRUD | Crear, consultar, actualizar y eliminar ciudadanos |
| Persistencia en CSV | Almacenar registros de ciudadanos en `CitizenDataStore.csv` |
| Integracion con API externa | Obtener objetos aleatorios para asignar activos personales |
| Asignacion aleatoria de grupo sanguineo | Asignar automaticamente un grupo sanguineo valido al crear un ciudadano |
| Arquitectura basada en controllers | Organizar endpoints usando controllers de ASP.NET Core |
| Configuracion por entorno | Cargar ajustes desde `appsettings.json`, `appsettings.Development.json` y variables de entorno |
| Logging con Serilog | Registrar eventos de la aplicacion en consola y archivos |
| Soporte con Swagger | Habilitar pruebas interactivas de la API en el entorno de desarrollo |
| Alineacion con Twelve-Factor | Documentar como el proyecto aplica los principios Twelve-Factor en la practica |

<a id="how-it-works"></a>
## 🧭 Como Funciona

| La logica principal de este proyecto incluye validar ciudadanos duplicados, asignar un grupo sanguineo aleatorio, obtener un activo personal desde la API externa y persistir el registro final en el archivo CSV. |
| --- |

### Estructura Basada en Controllers

Este proyecto usa una arquitectura basada en controllers en lugar del estilo de minimal API. Para esta practica, eso hace que la API sea mas facil de leer, mantener y extender.

La aplicacion inicia en `Program.cs`, donde carga la configuracion, configura Serilog, registra servicios y habilita Swagger en el entorno de desarrollo.

Las solicitudes del cliente son manejadas por `CitizenController`, que expone los endpoints CRUD para ciudadanos. El controller lee y actualiza los datos almacenados en `CitizenDataStore.csv`, mientras que `CitizenBGService` se conecta a la API externa para obtener objetos aleatorios usados como activos personales.

La logica de soporte esta separada en partes pequenas:

- `Models` define los objetos de solicitud y de dominio
- `Services` maneja la integracion externa
- `Utils` contiene logica auxiliar para lectura y escritura de CSV

La seccion [Explicacion de Twelve-Factor App](#12-factor-app-explanation) conecta estas decisiones tecnicas con los factores que mas se notan en este proyecto, como configuracion, logs y backing services.

<a id="architecture"></a>
## 🧱 Arquitectura: Controller-Based ASP.NET Core Web API

![Diagrama de Arquitectura](assets/architecture-diagram.png)<br>
Fuente: Elaboración Propia. <br>

El proyecto sigue una arquitectura de ASP.NET Core Web API basada en controllers y organizada por capas. En la parte superior, la capa de entrada incluye `Program.cs` y los controllers, que configuran la aplicacion y reciben las solicitudes HTTP mediante los endpoints de la API. La capa de aplicacion contiene `Models`, `Services` y `Utils`, donde se organiza la estructura de datos, la logica del negocio, la comunicacion con la API externa y el manejo del archivo CSV. La capa de infraestructura incluye la API externa y el archivo `CitizenDataStore.csv`, que actuan como la fuente de datos externa y el mecanismo de persistencia usado por el sistema.

Desde un punto de vista practico, el flujo comienza cuando el `Client - FrontEnd`, como Swagger UI, envia una solicitud HTTP a uno de los endpoints del controller. El controller recibe esa solicitud y se convierte en el punto que conecta el resto del sistema: usa los models para representar datos, los services para manejar logica y comunicacion externa, y los utils para gestionar operaciones de archivos. Si la operacion lo requiere, la aplicacion lee o escribe en el archivo CSV y tambien puede consultar la API externa. Una vez que todo se procesa, el controller envia el resultado de vuelta al cliente como respuesta JSON. De esta manera, el diagrama muestra no solo como se organiza internamente el proyecto, sino tambien como cada solicitud se mueve desde el cliente hacia el backend y regresa otra vez.

<a id="12-factor-app-explanation"></a>
## 📚 Explicacion de Twelve-Factor App

<a id="factor-1-codebase"></a>
### 1. Codebase

> One codebase tracked in revision control, many deploys. <br>
> Un solo codebase rastreado en control de versiones, muchos despliegues.

El proyecto se administra en un unico repositorio Git y esta publicado en GitHub. La mayor parte del trabajo de desarrollo se realizo mediante commits en la rama de practica `P2-001`.

Mantener todo en un solo codebase compartido y con historial visible favorece la trazabilidad y se ajusta bien a este factor.

<a id="factor-2-dependencies"></a>
### 2. Dependencies

> Explicitly declare and isolate dependencies. <br>
> Declarar y aislar dependencias de forma explicita.

Las dependencias estan declaradas explicitamente en el archivo `CitizensAPI.csproj`.

Las referencias actuales de paquetes incluyen:

| Paquete | Version |
| --- | --- |
| `Microsoft.AspNetCore.OpenApi` | `10.0.3` |
| `Newtonsoft.Json` | `13.0.4` |
| `Swashbuckle.AspNetCore` | `10.1.5` |
| `Swashbuckle.AspNetCore.Swagger` | `10.1.5` |
| `Swashbuckle.AspNetCore.SwaggerGen` | `10.1.5` |
| `Swashbuckle.AspNetCore.SwaggerUI` | `10.1.5` |
| `Serilog` | `4.3.1` |
| `Serilog.AspNetCore` | `10.0.0` |
| `Serilog.Settings.Configuration` | `10.0.0` |
| `Serilog.Sinks.Console` | `6.1.1` |
| `Serilog.Sinks.File` | `7.0.0` |

<a id="factor-3-config"></a>
### 3. Config

> Store config in the environment. <br>
> Almacenar la configuracion en el entorno.

Este proyecto mantiene la configuracion operativa fuera de la logica de negocio y tambien fuera de los controllers o services. En la practica, usa `appsettings.json`, `appsettings.Development.json` y variables de entorno, siendo estas ultimas lo mas cercano a la recomendacion de Twelve-Factor.

Esta configuracion se carga en `Program.cs`, de modo que los valores de ejecucion pueden ajustarse sin cambiar la logica principal de la aplicacion.

Valores de ejemplo del proyecto actual:

```json
{
  "Data": {
    "Location": "D:\\UPB D\\UPB 5th Semester\\Certification I\\CitizensAPI\\CitizensAPI\\CitizenDataStore.csv"
  },
  "External Services": {
    "ObjectsApi": {
      "BaseUrl": "https://api.restful-api.dev/"
    }
  }
}
```

Nota: en la configuracion actual de desarrollo, `Data:Location` usa una ruta local absoluta de Windows. Esto funciona en la maquina actual para el entorno academico, pero es menos portable y menos alineado con Twelve-Factor a menos que se sobrescriba mediante variables de entorno.

Ejemplos de override con variables de entorno:

```powershell
$env:Data__Location="D:\\path\\to\\CitizenDataStore.csv"
$env:ASPNETCORE_ENVIRONMENT="Development"
```

<a id="factor-4-backing-services"></a>
### 4. Backing Services

> Treat backing services as attached resources. <br>
> Tratar los servicios de soporte como recursos adjuntos.

El proyecto utiliza el servicio externo:

- `https://api.restful-api.dev/objects`

En este proyecto, el servicio externo se trata como un recurso adjunto que puede reemplazarse o reconfigurarse sin cambiar la logica principal del negocio.

<a id="factor-5-build-release-run"></a>
### 5. Build, Release, Run

> Strictly separate build and run stages. <br>
> Separar estrictamente las etapas de build y run.

En este proyecto, estas etapas pueden entenderse de una manera simple:

- **Build**: `dotnet build` compila el codigo y verifica que el proyecto sea valido.
- **Release**: la aplicacion puede prepararse para ejecucion o despliegue usando el codigo y la configuracion actuales, sin modificar el codigo fuente. Esto se muestra de forma mas practica en la seccion [Development](#development), especialmente en **Build for Production**.
- **Run**: `dotnet run` inicia la aplicacion.

Como puede verse, build y run se tratan como etapas separadas y repetibles.

<a id="factor-6-processes"></a>
### 6. Processes

> Execute the app as one or more stateless processes. <br>
> Ejecutar la aplicacion como uno o mas procesos sin estado.

La API esta pensada para comportarse como un proceso web sin estado.

Los datos de ciudadanos no se almacenan permanentemente dentro de la aplicacion en ejecucion. En su lugar, los datos persistentes se guardan en el archivo CSV:

- `CitizenDataStore.csv`

Cada operacion lee el estado actual desde el archivo, aplica el cambio solicitado y escribe el estado actualizado de vuelta en el CSV. En este proyecto, `async/await` se usa para la llamada a la API externa. Esto significa que la aplicacion puede solicitar datos al servicio externo y esperar la respuesta de una manera organizada antes de continuar. Conceptualmente, ayuda a que el codigo maneje operaciones que toman tiempo, como las solicitudes HTTP, de una forma mas limpia y apropiada para una Web API.


<a id="factor-7-port-binding"></a>
### 7. Port Binding

> Export services via port binding. <br>
> Exponer servicios mediante enlace a puertos.

La aplicacion se expone a traves del servidor web de ASP.NET Core y puede accederse por el puerto local configurado al ejecutarse con `dotnet run` o con el perfil de lanzamiento.

En el repositorio actual, el endpoint local configurado es:

- `https://localhost:9070`

Esto se define en:

- `Properties/launchSettings.json`

Configuracion de lanzamiento actual:

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

> Scale out via the process model. <br>
> Escalar mediante el modelo de procesos.

| La concurrencia esta implementada solo parcialmente. La API es mayormente stateless, pero el uso de persistencia en CSV limita un escalado horizontal seguro. |
| --- |

En este proyecto, la aplicacion esta disenada de manera mayormente stateless a nivel de API, pero su capa de persistencia es un unico archivo CSV. El proyecto incluye bloqueo en proceso para acceso al archivo, lo cual ayuda a evitar corrupcion dentro de una sola instancia en ejecucion. Sin embargo, como la aplicacion reescribe el archivo CSV durante operaciones de creacion, actualizacion y eliminacion, todavia no ofrece coordinacion completa para multiples procesos o multiples instancias desplegadas.

Por esa razon, el proyecto no esta preparado para un escalado concurrente real entre multiples procesos o instancias. El uso de `async/await` mejora el manejo de llamadas a la API externa al evitar bloqueos innecesarios, pero por si solo no garantiza concurrencia segura ni escalabilidad horizontal. En esta practica, la concurrencia se aborda solo de manera parcial y conceptual.

En el futuro, este factor podria aplicarse con mayor fuerza mediante:

- reemplazar la persistencia en CSV por una base de datos con transacciones y escrituras concurrentes
- agregar una estrategia mas segura de coordinacion de escritura
- ejecutar multiples instancias de la API detras de un balanceador
- separar solicitudes web de procesos worker en segundo plano si el sistema crece

<a id="factor-9-disposability"></a>
### 9. Disposability

> Maximize robustness with fast startup and graceful shutdown. <br>
> Maximizar la robustez con inicio rapido y apagado ordenado.

La aplicacion inicia rapidamente con `dotnet run` y puede detenerse sin requerir pasos complejos de cierre, solo presionando `Ctrl + C`. Como los datos se almacenan en el archivo CSV, reiniciar el proceso no elimina los registros de ciudadanos ya guardados.

<a id="factor-10-dev-prod-parity"></a>
### 10. Dev / Prod Parity

> Keep development, staging, and production as similar as possible. <br>
> Mantener desarrollo, staging y produccion tan similares como sea posible.

Desarrollo y produccion deben mantenerse lo mas similares posible mediante:

- usar el mismo codebase
- usar las mismas definiciones de dependencias
- usar archivos de configuracion y variables de entorno para valores especificos por entorno

Esto reduce diferencias innecesarias entre entornos.

Tambien es importante **evitar** detalles especificos de una maquina en el repositorio, como **rutas locales absolutas, archivos generados o finales de linea inconsistentes.** En este proyecto, por ejemplo, el valor actual de `Data:Location` usa una ruta absoluta de Windows, lo cual funciona para el entorno academico actual, pero no es ideal para la portabilidad. Esta es una de las razones por las que los valores especificos del entorno deben manejarse mediante configuracion y variables de entorno, en lugar de depender de una sola maquina. La portabilidad tambien mejora al ignorar archivos generados y al usar `.gitattributes` para definir explicitamente finales de linea **Line Feed (LF)**.


<a id="factor-11-logs"></a>
### 11. Logs

> Treat logs as event streams. <br>
> Tratar los logs como flujos de eventos.

| El logging es una de las partes operativas mas fuertes porque ayuda a rastrear solicitudes, detectar errores y entender lo que ocurre durante la ejecucion. |
| --- |


Los logs se manejan con Serilog y se usan principalmente para registrar eventos importantes de la aplicacion, como operaciones CRUD, comunicacion con la API externa y casos de falla. Esto mejora la observabilidad y apoya la depuracion, especialmente durante el desarrollo.

La implementacion usa niveles de log como `Debug`, `Information`, `Warning` y `Error`, lo cual ayuda a separar eventos normales de advertencias y fallos. De esta manera, el proyecto sigue la idea de Twelve-Factor de tratar los logs como un flujo de eventos util durante la ejecucion.

Es importante notar que el logging se aplica en partes relevantes del proyecto, como el flujo de los controllers y el servicio de la API externa, pero no en todas las partes del repositorio. Esto ayuda a mantener los logs enfocados y a evitar ruido innecesario.

Desde una perspectiva de seguridad, esto tambien requiere cuidado. Algunos logs actuales incluyen valores como la ruta del archivo CSV, numeros de CI de ciudadanos e informacion de activos personales. Aunque esto puede ser util para depuracion en un entorno academico, en un sistema mas seguro o mas orientado a produccion podria exponer datos sensibles o informacion interna innecesaria. Por eso, el contenido de los logs debe revisarse cuidadosamente y reducirse cuando pueda revelar informacion confidencial o personal.

<a id="factor-12-admin-processes"></a>
### 12. Admin Processes

> Run admin/management tasks as one-off processes. <br>
> Ejecutar tareas administrativas como procesos puntuales e independientes.

| El mantenimiento esta soportado indirectamente por los logs, pero todavia no existe un comando o script administrativo dedicado. Por eso, este factor no esta implementado completamente como un proceso administrativo independiente de ejecucion unica en el proyecto actual. |
| --- |

En este repositorio no se incluye un script o comando dedicado para tareas administrativas como:

- limpiar el archivo CSV
- reiniciar datos almacenados de ciudadanos
- sembrar registros iniciales
- migrar la estructura del archivo

Si la aplicacion se extiende en el futuro y se orienta a ser mas escalable o mas cercana a un sistema real, estas tareas administrativas deberian implementarse. Aun asi, el proyecto ya incluye elementos de soporte que pueden ayudar a futuras tareas de mantenimiento y administracion.

Por ejemplo, la implementacion actual de logging ayuda con:

- revisar eventos de creacion, actualizacion y eliminacion de ciudadanos
- detectar fallos de la API externa
- identificar problemas de lectura o escritura de archivos
- apoyar solucion de problemas, mantenimiento y analisis operativo

Por ello, este factor se aborda solo de manera parcial y conceptual en esta practica.

Los logs brindan soporte util para mantenimiento y administracion, pero no reemplazan un verdadero proceso administrativo.

<a id="api-endpoints"></a>
## 🔌 Endpoints de la API

| Operacion | Endpoint | Resultado principal |
| --- | --- | --- |
| Crear ciudadano | `POST /api/Citizen` | Crea un ciudadano y asigna `BloodGroup` y `PersonalAsset` |
| Obtener todos los ciudadanos | `GET /api/Citizen` | Devuelve la lista actual de ciudadanos |
| Obtener ciudadano por CI | `GET /api/Citizen/{id}` | Devuelve un ciudadano si existe |
| Actualizar ciudadano | `PUT /api/Citizen/{ci}` | Actualiza solo `FirstName` y `LastName` |
| Eliminar ciudadano | `DELETE /api/Citizen/{ci}` | Elimina un ciudadano por CI |

### Crear Ciudadano

- `POST /api/Citizen`

Comportamiento:

- el cuerpo de la solicitud solo incluye `FirstName`, `LastName` y `CI`
- valida CI duplicado
- asigna un grupo sanguineo aleatorio
- llama a la API externa
- asigna un activo personal aleatorio
- guarda el ciudadano en el CSV

Ejemplo de solicitud:

```json
{
  "firstName": "Hans",
  "lastName": "Tanaka",
  "ci": 80751770
}
```

Forma del ciudadano creado:

```json
{
  "firstName": "Hans",
  "lastName": "Tanaka",
  "ci": 80751770,
  "bloodGroup": "O+",
  "personalAsset": "Apple Watch"
}
```

Comportamiento actual de respuesta:

- en exito devuelve `200 OK`
- el controller actualmente devuelve la lista completa de ciudadanos despues de crear
- un CI duplicado devuelve `409 Conflict`
- una falla del servicio externo de activos puede devolver `503 Service Unavailable`

### Obtener Todos los Ciudadanos

- `GET /api/Citizen`

Comportamiento actual de respuesta:

- devuelve `200 OK` con la lista completa de ciudadanos

Ejemplo de respuesta:

```json
[
  {
    "firstName": "Hans",
    "lastName": "Tanaka",
    "ci": 80751770,
    "bloodGroup": "O+",
    "personalAsset": "Apple Watch"
  },
  {
    "firstName": "Gundham",
    "lastName": "Gamboa",
    "ci": 654321,
    "bloodGroup": "A-",
    "personalAsset": "Keyboard"
  }
]
```

### Obtener Ciudadano por CI

- `GET /api/Citizen/{id}`

Comportamiento actual de respuesta:

- devuelve `200 OK` con el ciudadano cuando existe
- devuelve `200 OK` con un mensaje de texto cuando el ciudadano no existe

Ejemplo de respuesta:

```json
{
  "firstName": "Hans",
  "lastName": "Tanaka",
  "ci": 80751770,
  "bloodGroup": "O+",
  "personalAsset": "Apple Watch"
}
```

### Actualizar Ciudadano

- `PUT /api/Citizen/{ci}`

Regla de negocio:

- el cuerpo de la solicitud solo incluye `FirstName` y `LastName`
- solo se actualizan `FirstName` y `LastName`

Ejemplo de solicitud:

```json
{
  "firstName": "Hanigo Hans",
  "lastName": "Tanaka"
}
```

Comportamiento actual de respuesta:

- devuelve `200 OK` con la lista completa de ciudadanos despues de actualizar
- si el CI no existe, el controller actualmente devuelve `200 OK` con un mensaje de texto

### Eliminar Ciudadano

- `DELETE /api/Citizen/{ci}`

Comportamiento actual de respuesta:

- devuelve `200 OK` con el ciudadano eliminado cuando existe
- si el CI no existe, el controller actualmente devuelve `200 OK` con un mensaje de texto

### Ejemplo de Formato CSV

Archivo: `CitizenDataStore.csv`

```csv
Susan,Laime,123456,O-,Laptop
Hanigo Hans,Tanaka,80751770,O+,Apple Watch
```

<a id="operational-notes"></a>
#### Notas Operativas

- Swagger esta habilitado en modo desarrollo.
- El proyecto usa finales de linea LF mediante `.gitattributes`.
- El archivo CSV es la capa de persistencia de esta practica.
- El perfil de lanzamiento actual usa `https://localhost:9070`.
- El repositorio sigue un estilo orientado a controllers en lugar del estilo minimal scripting.

En conjunto, estos elementos ayudan a explicar como la aplicacion esta configurada, ejecutada y mantenida.

<a id="tech-stack"></a>
## 🧰 Stack Tecnologico

- **Framework:** ASP.NET Core Web API (`net10.0`)
- **Lenguaje:** C#
- **Documentacion de API:** OpenAPI + Swagger / Swashbuckle
- **Logging:** Serilog con sinks de consola y archivo
- **Integracion HTTP externa:** `HttpClient`
- **Manejo de JSON:** Newtonsoft.Json
- **Configuracion:** `appsettings.json`, `appsettings.Development.json` y variables de entorno
- **Persistencia:** almacenamiento en CSV mediante `CitizenDataStore.csv`
- **Control de versiones:** Git y GitHub

<a id="prerequisites"></a>
## 📋 Prerrequisitos

Antes de ejecutar el proyecto, asegurate de tener disponibles las siguientes herramientas:

- **.NET SDK 10.0**
- **Git**, si deseas clonar el repositorio y administrar versiones localmente
- **Visual Studio** o **Visual Studio Code** con soporte para C#
- **Conexion a internet**, porque la aplicacion consume `https://api.restful-api.dev/objects`

<a id="installation--setup"></a>
## 🧩 Instalacion y Configuracion

1. Clona el repositorio, restaura dependencias, compila y ejecuta la API.
2. Abre el proyecto en tu editor o IDE si deseas revisarlo o modificarlo.
3. Revisa los archivos de configuracion si necesitas cambiar rutas, logging o valores de entorno.

### Paso 1: Clonar, Restaurar, Compilar y Ejecutar

```bash
# SSH
git clone git@github.com:SusanLaime/CitizensAPI.git
cd CitizensAPI
dotnet restore
dotnet build
dotnet run
```

o

```bash
# HTTPS
git clone https://github.com/SusanLaime/CitizensAPI.git
cd CitizensAPI
dotnet restore
dotnet build
dotnet run
```

Esta secuencia descarga las dependencias del proyecto, verifica que el codigo compile e inicia la API localmente.

### Paso 2: Abrir el Proyecto

Abre el archivo de solucion en Visual Studio o la carpeta del proyecto en Visual Studio Code.

### Paso 3: Revisar la Configuracion

La aplicacion lee configuracion desde:

- `appsettings.json`
- `appsettings.Development.json`
- `Properties/launchSettings.json`

Estos archivos definen ajustes importantes como:

- ubicacion del almacenamiento CSV
- URL base de la API externa
- configuracion de Serilog
- seleccion del entorno mediante `ASPNETCORE_ENVIRONMENT`

Este paso de configuracion esta directamente relacionado con el factor [Config](#factor-3-config) explicado arriba, donde el enfoque de configuracion del proyecto se describe en terminos arquitectonicos y practicos.


<a id="running-the-application"></a>
## ▶️ Ejecucion de la Aplicacion

### Iniciar la API

Usa el siguiente comando para iniciar la API localmente:

```bash
dotnet run
```

### Funciones de Desarrollo

Cuando el entorno esta establecido en `Development`, la aplicacion tambien habilita:

- Swagger UI para probar endpoints en `https://localhost:9070/swagger/index.html`
- configuracion especifica de desarrollo desde `appsettings.Development.json`
- mapeo OpenAPI mediante `app.MapOpenApi()`

### Consideraciones

- Asegurate de que la API externa `https://api.restful-api.dev/objects` sea alcanzable
- Asegurate de que la ruta del archivo CSV configurada en app settings sea valida
- Los archivos de log se generan automaticamente mediante Serilog durante la ejecucion

<a id="project-structure"></a>
## 🗂️ Estructura del Proyecto

El repositorio esta organizado de la siguiente manera:

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

Los artefactos generados en ejecucion, como `Logs/`, `bin/` y `obj/`, no forman parte de la estructura central del codigo fuente y por eso se omiten en esta vista general. Ademas, la carpeta `assets/` fue agregada al proyecto para guardar la imagen del diagrama de arquitectura.

| Area / Archivo | Responsabilidad |
| --- | --- |
| `Controllers` | Expone los endpoints de la API y coordina las operaciones CRUD |
| `Models` | Contiene las entidades de dominio y modelos de solicitud |
| `Properties` | Contiene ajustes locales de ejecucion como la URL de la aplicacion y el perfil de entorno |
| `Services` | Maneja la comunicacion con la API externa mediante `CitizenBGService` |
| `Utils` | Contiene logica auxiliar para persistencia en CSV |
| `CitizensAPI.sln` | Organiza la solucion para abrir y administrar el proyecto en Visual Studio |
| `Program.cs` | Configura el pipeline de la aplicacion, servicios, logging y Swagger |


<a id="development"></a>
## 💻 Desarrollo

### Flujo de Desarrollo

| Paso | Accion |
| --- | --- |
| 1 | Abrir el proyecto en Visual Studio o Visual Studio Code |
| 2 | Modificar controllers, services, models, archivos de configuracion o utilidades |
| 3 | Ejecutar `dotnet build` para verificar que el proyecto compile |
| 4 | Ejecutar `dotnet run` para validar el comportamiento localmente |
| 5 | Probar endpoints con Swagger y revisar logs junto con la persistencia CSV |

### Estilo de Codigo

- **Lenguaje:** C#
- **Estilo de arquitectura:** ASP.NET Core Web API basada en controllers
- **Formato:** Formato consistente de C# mediante las herramientas del IDE o editor
- **Convenciones de nombres:**
  - **Clases y archivos:** PascalCase, por ejemplo `CitizenController.cs`
  - **Metodos y propiedades:** PascalCase, por ejemplo `GetCitizenBGs`
  - **Campos privados:** `_camelCase`, por ejemplo `_httpClient`
  - **Variables locales y parametros:** camelCase, por ejemplo `citizenRequest`

### Patrones Clave de Desarrollo

| Elemento | Proposito |
| --- | --- |
| Controllers | Manejan solicitudes HTTP y coordinan el flujo de la aplicacion |
| Services | Encapsulan la comunicacion con APIs externas y la logica de soporte |
| Models | Representan cuerpos de solicitud y entidades de dominio |
| Utilities | Centralizan logica reutilizable como operaciones CSV |
| Configuration | Mantienen ajustes de ejecucion fuera de la logica del negocio mediante app settings y variables de entorno |
| Logging | Usa Serilog para registrar eventos de desarrollo y ejecucion con niveles adecuados |

### Validacion Durante el Desarrollo

| Validacion | Proposito |
| --- | --- |
| `dotnet build` | Verificar compilacion correcta |
| `dotnet run` | Ejecutar la API localmente |
| Swagger | Probar endpoints de forma interactiva |
| Salidas de Serilog | Revisar comportamiento en ejecucion y errores |

### Build for Production

Una version lista para publicarse o desplegarse de la aplicacion puede generarse con:

```bash
dotnet publish -c Release
```

Este comando crea una salida optimizada para despliegue. La opcion `-c Release` indica a .NET que use la configuracion **Release** en lugar de la configuracion orientada a desarrollo.

Ejecutar `dotnet publish -c Release` no cambia el flujo normal de desarrollo. Despues de publicar, el proyecto puede seguir compilando y ejecutandose localmente con `dotnet build` y `dotnet run` como de costumbre.

<a id="troubleshooting"></a>
## 🛠️ Solucion de Problemas

| Problema | Verificacion |
| --- | --- |
| Problemas de compilacion | Ejecuta `dotnet restore` y `dotnet build`, verifica el SDK de .NET 10, los paquetes NuGet restaurados y la validez de `CitizensAPI.csproj` |
| Problemas de ejecucion | Ejecuta `dotnet run`, confirma que el proyecto compile primero, que el puerto este libre y que `launchSettings.json` tenga valores validos |
| Problemas de configuracion | Revisa `appsettings.json`, `appsettings.Development.json`, `Properties/launchSettings.json`, `Data:Location`, `External Services:ObjectsApi:BaseUrl` y `ASPNETCORE_ENVIRONMENT` |
| Problemas con la API externa | Verifica que `https://api.restful-api.dev/objects` sea alcanzable, que la URL base sea correcta y que los logs no muestren fallas de la API |
| Problemas de persistencia CSV | Confirma que la ruta del CSV exista, que la aplicacion tenga permisos de archivo y que el contenido sea valido |
| Problemas de logging | Revisa los ajustes de Serilog, `MinimumLevel`, rutas de salida y el entorno seleccionado |

### Comandos Comunes de Validacion

```bash
dotnet restore
dotnet build
dotnet run
dotnet publish -c Release
```

<a id="critical-security-notes"></a>
## 🔒 Notas Criticas de Seguridad
| La API actual no implementa autenticacion ni autorizacion, por lo que debe tratarse como un proyecto academico y no como un servicio seguro listo para produccion. |
| --- |

Desde una perspectiva de ciberseguridad, la limitacion mas critica del proyecto actual es la ausencia de autenticacion y autorizacion. Cualquier cliente que pueda alcanzar la API puede crear, leer, actualizar o eliminar registros de ciudadanos, lo que significa que el acceso a informacion personal no esta restringido adecuadamente.

Tambien existen otros riesgos de seguridad relevantes:

- `CitizenDataStore.csv` es aceptable para una practica academica, pero no es un mecanismo de persistencia fuerte para confidencialidad, integridad, auditoria o acceso concurrente controlado
- la API externa usada para activos personales debe tratarse como entrada no confiable y sus respuestas deben validarse cuidadosamente
- los logs son utiles para observabilidad, pero en un despliegue real deben evitar exponer datos personales innecesarios o detalles internos sensibles

Aun asi, el proyecto ya tiene algunas bases positivas de seguridad, incluyendo **HTTPS local**, configuracion fuera del codebase, Swagger limitado a desarrollo y logging estructurado mediante Serilog. Aun asi, el estado actual debe entenderse como una implementacion academica y no como un sistema seguro listo para produccion.

<a id="security-improvements"></a>
## 🛡️ Mejoras de Seguridad

Si el proyecto evoluciona mas alla del ambito academico, las mejoras mas importantes serian:

- agregar autenticacion y autorizacion para controlar quien puede acceder o modificar registros de ciudadanos
- reemplazar la persistencia en CSV por una base de datos con mejor control de acceso, integridad y capacidad de auditoria
- fortalecer la validacion de entradas y la validacion de respuestas de la API externa
- reducir logging sensible en produccion y usar niveles de log mas estrictos fuera de desarrollo
- mantener las acciones administrativas controladas y auditables
- mantener ramas protegidas y merges controlados como parte de una practica de desarrollo seguro

<a id="conclusion"></a>
## 📝 Conclusion

Este proyecto aplica los principios de Twelve-Factor App a un nivel practico al combinar gestion de configuracion, integracion con servicios externos, logging estructurado y una arquitectura simple orientada al mantenimiento. Aunque algunos factores, como concurrencia y procesos administrativos, no estan implementados o solo lo estan parcialmente, el repositorio documenta tanto lo que ya funciona como lo que podria mejorarse en una version futura.

<a id="references"></a>
## 📚 Referencias

1. Wiggins, A. (2017). *The Twelve-Factor App*. https://12factor.net/
2. GitHub. (n.d.). *Managing rulesets for a repository*. GitHub Docs. https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets
3. GitHub. (n.d.). *Managing protected branches*. GitHub Docs. https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches
4. Microsoft. (n.d.). *ASP.NET Core configuration*. Microsoft Learn. https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/
5. Microsoft. (2022, April 13). *Storing application secrets safely during development*. Microsoft Learn. https://learn.microsoft.com/en-us/dotnet/architecture/microservices/secure-net-microservices-web-applications/developer-app-secrets-storage
6. OWASP Foundation. (2023). *OWASP API Security Top 10*. https://owasp.org/API-Security/
7. serilog. (n.d.). *Serilog.AspNetCore*. GitHub. https://github.com/serilog/serilog-aspnetcore