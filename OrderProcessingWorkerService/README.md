# Order Processing Worker Service

Este proyecto implementa un servicio en segundo plano para procesar órdenes de pago o cobranzas y sus ítems de manera automática y eficiente. 

## Historia de Usuario

### Título

**Creación de Worker Service para Procesamiento de Órdenes en Segundo Plano**

### Descripción

**Como** administrador del sistema, **quiero** implementar un Worker Service, desarrollado en C# .Net 8, que opere en segundo plano para procesar órdenes de manera automática, **con el fin de** ejecutar un procesamiento eficiente y escalable de las órdenes recibidas en el sistema.

## Criterios de Aceptación

### 1. Implementación del Worker Service
- **Cumplimiento**: El `Worker Service` se implementa como una clase que hereda de `BackgroundService`, lo que permite su ejecución en segundo plano y el manejo de tareas de procesamiento de órdenes y sus ítems.

### 2. Procesamiento Automático de Órdenes
- **Identificación de órdenes pendientes**: 
  - El método `GetPendingOrders` en `OrderRepository` identifica y retorna las órdenes con estado `Pending`.

- **Consulta basada en cron**: 
  - Se utiliza `CronExpression` para definir la periodicidad de ejecución del Worker. Esto se maneja en la clase `Worker` con `_cronExpression`.

- **Procesamiento automático y eficiente**: 
  - El método `ProcessOrderAsync` en `OrderProcessor` maneja el procesamiento de cada orden y sus ítems.

### 3. Aplicación de Principios SOLID
- **Single Responsibility Principle**: Cada clase tiene una única responsabilidad, por ejemplo, `OrderProcessor` se encarga del procesamiento de órdenes, `OrderRepository` maneja las operaciones de datos, y `ReportGenerator` genera reportes.
- **Open/Closed Principle**: Las clases están abiertas para extensión pero cerradas para modificación. Por ejemplo, se puede extender `OrderProcessor` sin modificar su implementación.
- **Liskov Substitution Principle**: Las interfaces como `IOrderProcessor` aseguran que cualquier implementación puede ser sustituida sin alterar el comportamiento esperado.
- **Interface Segregation Principle**: Se han creado interfaces específicas como `IOrderProcessor`, `IOrderRepository`, `IErrorLogger` y `IReportGenerator`.
- **Dependency Inversion Principle**: La inyección de dependencias se maneja a través de `IServiceCollection` y `IServiceScopeFactory`.

### 4. Escalabilidad y Gestión de Carga
- **Escalabilidad**: La configuración permite ajustar `MaxConcurrentOrders` y `MaxConcurrentItems` para manejar la carga.
- **Gestión de carga**: Los parámetros `MaxConcurrentOrders` y `MaxConcurrentItems` en `WorkerOptions` controlan la concurrencia.

### 5. Inicio y Detención Bajo Demanda
- **Inicio y detención**: La clase `Worker` hereda de `BackgroundService`, lo que permite su control a través del ciclo de vida del servicio hospedado en .NET.

### 6. Reintento y Manejo de Errores
- **Manejo de errores**: El método `ProcessOrderAsync` captura excepciones y actualiza el estado de la orden a `Failed` en caso de error.
- **Reintento**: El reintento se puede implementar a través de políticas adicionales en `OrderProcessor`.

### 7. Registro y Monitoreo
- **Registro de actividades**: Se usa `ILogger` para registrar actividades y eventos en el `Worker` y en otras clases de servicio.
- **Monitoreo**: Los registros realizados con `ILogger` pueden ser revisados para monitorear el estado y rendimiento del servicio.

## Pruebas de Aceptación

Las siguientes pruebas aseguran que los criterios de aceptación se cumplan:

### Pruebas Unitarias

1. **OrderProcessorTests**
   - Verifica que el `OrderProcessor` maneja errores correctamente y procesa ítems de manera exitosa.

2. **OrderRepositoryTests**
   - Verifica que el `OrderRepository` puede obtener órdenes pendientes y actualizar el estado de las órdenes correctamente.

3. **WorkerTests**
   - Verifica que el `Worker` procesa las órdenes pendientes correctamente y genera reportes.

### Pruebas de Integración

1. **IntegrationTests**
   - Verifica que el `Worker` puede procesar las órdenes de manera completa y actualiza el estado de las órdenes, generando los reportes necesarios.

## Configuración del Proyecto

### appsettings.json

```json
{
  "WorkerOptions": {
    "CronExpression": "*/5 * * * * *",
    "MaxConcurrentOrders": 2,
    "MaxConcurrentItems": 2
  }
}
