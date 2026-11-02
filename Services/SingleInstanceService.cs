using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;

namespace TeclaLibre.Services;

internal static class SingleInstanceService
{
    public static (string MutexName, string PipeName) GetNames()
    {
        using WindowsIdentity identity =
            WindowsIdentity.GetCurrent();

        string userSid = identity.User?.Value
            ?? throw new InvalidOperationException(
                "No se pudo identificar al usuario actual de Windows.");

        int sessionId = Process.GetCurrentProcess().SessionId;

        string mutexName =
            $@"Local\TeclaLibre.{userSid}";

        string pipeName =
            $"TeclaLibre.{userSid}.Session{sessionId}";

        return (mutexName, pipeName);
    }

    public static bool RequestExistingWindow(
        string pipeName,
        int timeoutMilliseconds = 3000)
    {
        try
        {
            using NamedPipeClientStream client = new(
                serverName: ".",
                pipeName: pipeName,
                direction: PipeDirection.Out,
                options: PipeOptions.CurrentUserOnly);

            client.Connect(timeoutMilliseconds);

            // Para esta versión, conectarse es la solicitud
            // para mostrar la ventana.
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }
}