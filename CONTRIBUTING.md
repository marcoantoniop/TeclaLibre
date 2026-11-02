# Cómo colaborar con TeclaLibre

¡Gracias por tu interés! Puedes ayudar probando la aplicación, proponiendo mejoras, revisando la documentación o aportando código.

## Antes de comenzar

TeclaLibre se desarrolla en C# con Windows Forms y .NET 10. Para compilarlo necesitas Windows, el SDK de .NET 10 y, si usas Visual Studio, soporte para el desarrollo de escritorio de .NET. Consulta el [README](README.md) para compilar y ejecutar el proyecto.

Si la propuesta requiere cambios grandes, abre primero una discusión o un issue para explicar el problema y conversar sobre la solución. Así será más fácil coordinar el trabajo.

## Informar un problema

Al crear un issue, incluye cuando sea posible:

1. La versión de Windows y la versión o revisión de TeclaLibre.
2. La distribución del teclado y los modificadores utilizados, si corresponden.
3. Los pasos para reproducir el problema.
4. El resultado esperado y el resultado obtenido.
5. La aplicación donde ocurrió y si se ejecutaba con privilegios elevados.

Comparte capturas o configuración solo después de revisar que no contengan contraseñas, texto privado ni datos personales. Para fallos de seguridad consulta [SECURITY.md](SECURITY.md).

## Proponer código o documentación

1. Haz un fork del repositorio y crea una rama para tu cambio.
2. Realiza un cambio acotado y explica el motivo.
3. Compila el proyecto y prueba los escenarios afectados en Windows.
4. Si modificas las reglas, comprueba su funcionamiento dentro de TeclaLibre y en otra aplicación, por ejemplo el Bloc de notas. Incluye combinaciones con AltGr o modificadores cuando sean relevantes.
5. Envía un pull request que describa el problema, la solución y las pruebas realizadas. Menciona el issue relacionado si existe.

No incluyas carpetas generadas como `bin`, `obj` o `.vs`, ni configuraciones personales, contraseñas o registros de pulsaciones en el pull request. Conserva los archivos de código, los recursos y los iconos que el proyecto necesita para compilar.

Al participar, mantén un trato respetuoso y constructivo. Las propuestas se revisarán según el tiempo disponible de las personas que mantienen el proyecto.
