# TeclaLibre

TeclaLibre es una aplicación para Windows que permite crear reglas para reasignar teclas y combinaciones, y convertirlas en texto u otras combinaciones. La idea es facilitar la personalización del teclado y construir la herramienta junto con otras personas interesadas.

> **Estado:** proyecto en desarrollo. Sus funciones principales se han probado de forma manual; aún necesita más pruebas y colaboración.

## Funciones

- Definir reglas con una tecla o combinación de entrada y una salida de texto o de teclas.
- Usar combinaciones que incluyan modificadores como Ctrl, Shift, Alt y AltGr.
- Agregar, editar, duplicar, activar, pausar y eliminar reglas.
- Activar o detener el motor de reasignación y trabajar desde la bandeja del sistema.
- Usar el panel **Pruebas** para observar eventos de teclado y probar el comportamiento de la aplicación.
- Guardar, importar y exportar la configuración.
- Configurar el inicio con Windows, el inicio minimizado, la activación al iniciar y el comportamiento al cerrar la ventana.
- Habilitar opcionalmente `Ctrl + Shift + F9` para activar o detener el motor.

Por ejemplo, se puede configurar **AltGr + Z** para escribir `>`.

## Requisitos

- Windows y el SDK de .NET 10 para compilar desde el código fuente.
- Una versión de Visual Studio con soporte para .NET 10 y la carga de trabajo **Desarrollo de escritorio de .NET**, si se utiliza Visual Studio.

La aplicación utiliza Windows Forms y funciones de Windows; no está preparada para ejecutarse en macOS o Linux. La compatibilidad depende también de los permisos y del entorno de la aplicación de destino.

## Compilar y ejecutar

1. Descarga o clona el repositorio.
2. Abre `TeclaLibre.slnx` en Visual Studio.
3. Restaura los paquetes si Visual Studio lo solicita.
4. Compila la solución y ejecuta el proyecto.

También puedes hacerlo desde una terminal en la carpeta que contiene `TeclaLibre.csproj`:

```powershell
dotnet restore TeclaLibre.csproj
dotnet build TeclaLibre.csproj
dotnet run --project TeclaLibre.csproj
```

## Primeros pasos

1. Abre la pestaña **Reasignaciones** y selecciona **Nueva**.
2. Captura la tecla o combinación de entrada y define la salida deseada.
3. Guarda la regla y activa el motor desde la ventana principal.
4. Pruébala en un editor de texto, por ejemplo el Bloc de notas.
5. Para cambiar las preferencias o exportar una copia de la configuración, abre **Configuración**.

El panel **Pruebas** muestra información de eventos de teclado mientras la prueba está activa. Evita introducir contraseñas u otros datos sensibles durante las pruebas y no compartas capturas que los revelen.

## Configuración y bandeja

La configuración se guarda en `%LOCALAPPDATA%\TeclaLibre\config.json`. Puedes exportar una copia desde la pestaña **Configuración** antes de cambiarla o importarla.

Si activas la opción de mantener TeclaLibre en la bandeja al cerrar la ventana, utiliza el menú del icono de la bandeja para volver a abrirla o salir. Mientras el motor esté activo, las reglas pueden afectar la escritura en otras aplicaciones.

## Alcance y limitaciones

- Las teclas especiales como **Fn** pueden ser gestionadas por el hardware y no estar disponibles para la aplicación.
- Algunas aplicaciones, ventanas con privilegios elevados o pantallas protegidas pueden no recibir o permitir las reasignaciones de la misma manera.
- El comportamiento puede variar según la distribución del teclado y las aplicaciones. Se agradecen pruebas y reportes con pasos reproducibles.
- La combinación opcional `Ctrl + Shift + F9` puede coincidir con atajos de otras aplicaciones.

## Colaborar

Las ideas, pruebas, reportes y mejoras son bienvenidas. Consulta [CONTRIBUTING.md](CONTRIBUTING.md) para comenzar y [SECURITY.md](SECURITY.md) para informar problemas de seguridad. Los cambios previstos o realizados se registran en [CHANGELOG.md](CHANGELOG.md).

## Licencia

TeclaLibre se distribuye bajo la licencia MIT. Consulta el archivo [LICENSE](LICENSE) para conocer sus términos.
