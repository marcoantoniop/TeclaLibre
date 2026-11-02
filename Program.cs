using TeclaLibre.Forms;
using TeclaLibre.Native;
using TeclaLibre.Services;

namespace TeclaLibre;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        try
        {
            NativeLayoutValidator.Validate();

            (string mutexName, string pipeName) =
                SingleInstanceService.GetNames();

            using Mutex mutex = new(
                initiallyOwned: true,
                name: mutexName,
                createdNew: out bool isFirstInstance);

            if (!isFirstInstance)
            {
                bool isAutomaticStartup = args.Any(argument =>
                    string.Equals(
                        argument,
                        "--startup",
                        StringComparison.OrdinalIgnoreCase));

                // Si Windows inició otra copia automáticamente,
                // la cerramos sin mostrar la ventana existente.
                if (isAutomaticStartup)
                {
                    return;
                }

                bool requested =
                    SingleInstanceService.RequestExistingWindow(
                        pipeName);

                if (!requested)
                {
                    MessageBox.Show(
                        "TeclaLibre ya parece estar ejecutándose, " +
                        "pero no fue posible mostrar su ventana.\r\n\r\n" +
                        "Revisa el icono de la bandeja de Windows.",
                        "TeclaLibre",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                return;
            }

            try
            {
                Application.Run(new MainForm());
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
        catch (Exception ex) when (
            ex is InvalidOperationException or
            UnauthorizedAccessException or
            IOException)
        {
            MessageBox.Show(
                ex.Message,
                "TeclaLibre - Error de inicio",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}