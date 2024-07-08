# API Gateway

## Descripción

Este proyecto implementa un API Gateway desarrollado en C# utilizando .NET 8. Proporciona una puerta de enlace centralizada para las solicitudes HTTP, permitiendo la autenticación y autorización mediante JWT. También incluye logging y manejo de excepciones.

## Características

- **Autenticación y Autorización**: Utiliza JWT para autenticar y autorizar solicitudes.
- **Redirección de Solicitudes**: Redirige las solicitudes a los endpoints de destino configurados.
- **Logging**: Registra todas las solicitudes y respuestas.
- **Manejo de Excepciones**: Maneja las excepciones globalmente y devuelve respuestas de error adecuadas.
- **Health Check**: Proporciona un endpoint para verificar el estado del servicio.

## Requisitos

- .NET 8 SDK
- Visual Studio 2022 (recomendado)
- Postman para probar las solicitudes

## Configuración

1. **Clonar el repositorio:**

    ```bash
    git clone https://github.com/tu-usuario/api-gateway.git
    cd api-gateway
    ```

2. **Restaurar los paquetes NuGet:**

    ```bash
    dotnet restore
    ```

3. **Configurar la clave secreta para JWT:**

    En `Program.cs` y `AuthController.cs`, asegúrate de que la clave secreta utilizada sea de al menos 256 bits (32 caracteres).

    ```csharp
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKeyThatIsLongEnoughForHS256"));
    ```

4. **Compilar la solución:**

    ```bash
    dotnet build
    ```

## Ejecución

1. **Ejecutar la aplicación:**

    ```bash
    dotnet run
    ```

2. **Probar con Postman:**

    - **Obtener el Token JWT:**

        ```http
        POST https://localhost:5001/api/authenticate
        Content-Type: application/json

        {
          "username": "admin",
          "password": "password"
        }
        ```

    - **Usar el Token JWT para Redirigir una Solicitud:**

        ```http
        POST https://localhost:5001/api/gateway/forward
        Content-Type: application/json
        Authorization: Bearer YOUR_JWT_TOKEN

        {
          "method": "GET",
          "url": "https://jsonplaceholder.typicode.com/posts/1",
          "body": "",
          "headers": {}
        }
        ```

## Cumplimiento de la Historia de Usuario y Criterios de Aceptación

### Descripción de la Historia de Usuario

- **Como**: Un usuario del sistema autorizado para interactuar con los servicios del sistema.
- **Quiero**: Implementar un API Gateway, desarrollado en C# .Net 8, que centralice y proteja los endpoints relacionados con la consulta de órdenes e ítems.
- **Con el fin de**: Asegurar un acceso seguro y unificado a estas funcionalidades.

### Criterios de Aceptación

1. **API Gateway**:
    - **Debe permitir parametrizar los endpoint del resto de las Apis**: El API Gateway permite redirigir solicitudes a diversos endpoints configurados en las solicitudes de tipo `POST /api/gateway/forward`.
    - **Debe redirigir y canalizar las solicitudes entrantes a los servicios correspondientes de manera eficiente y segura**: Utilizando `HttpClient` y configuraciones de seguridad mediante JWT.
    
2. **Seguridad y Autorización**:
    - **Debe aplicar medidas de seguridad adecuadas, como HTTPS, para proteger la transmisión de datos**: La aplicación está configurada para usar HTTPS y autenticar solicitudes con JWT.
    
3. **Monitoreo y Registro**:
    - **Se debe implementar un registro de actividad para registrar las solicitudes y respuestas procesadas por el API Gateway**: Utilizando `LoggingMiddleware` y `ActivityLogService` para registrar todas las solicitudes y respuestas.
    - **Debe implementarse un endpoint de Health Check donde se integren todos los endpoint de health de cada una de las Apis para poder confirmar su disponibilidad**: El controlador `HealthController` proporciona un endpoint de health check.

## Estructura del Proyecto

- **Controllers**: Contiene los controladores para manejar las solicitudes HTTP.
- **Services**: Contiene los servicios que implementan la lógica de negocio.
- **Middlewares**: Contiene los middlewares para logging y manejo de excepciones.
- **Models**: Contiene los modelos utilizados en el proyecto.
- **Program.cs**: Punto de entrada de la aplicación donde se configuran los servicios y middlewares.

## Pruebas Unitarias

Para ejecutar las pruebas unitarias, utiliza el siguiente comando:

```bash
dotnet test
