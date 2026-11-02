using Microsoft.Win32;

namespace TeclaLibre.Services;

internal sealed class StartupService
{
    private const string RunKeyPath =
        @"Software\Microsoft\Windows\CurrentVersion\Run";

    private const string ValueName = "TeclaLibre";

    public string? ReadEntry()
    {
        using RegistryKey? key =
            Registry.CurrentUser.OpenSubKey(
                RunKeyPath,
                writable: false);

        return key?.GetValue(ValueName) as string;
    }

    public bool IsCurrentExecutableRegistered()
    {
        string? existing = ReadEntry();

        if (string.IsNullOrWhiteSpace(existing))
        {
            return false;
        }

        string? executable = Environment.ProcessPath;

        if (string.IsNullOrWhiteSpace(executable))
        {
            return false;
        }

        string expected = $"\"{executable}\" --startup";

        return string.Equals(
            existing,
            expected,
            StringComparison.OrdinalIgnoreCase);
    }

    public void SetEnabled(bool enabled)
    {
        if (!enabled)
        {
            WriteEntry(null);
            return;
        }

        string? executable = Environment.ProcessPath;

        if (string.IsNullOrWhiteSpace(executable) ||
            !File.Exists(executable))
        {
            throw new InvalidOperationException(
                "No se pudo localizar el ejecutable de TeclaLibre.");
        }

        string fileName = Path.GetFileName(executable);

        if (string.Equals(
                fileName,
                "dotnet.exe",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "TeclaLibre se está ejecutando mediante dotnet.exe. " +
                "Ejecuta TeclaLibre.exe directamente para habilitar " +
                "el inicio automático.");
        }

        WriteEntry($"\"{executable}\" --startup");
    }

    public void WriteEntry(string? command)
    {
        using RegistryKey key =
            Registry.CurrentUser.CreateSubKey(
                RunKeyPath,
                writable: true)
            ?? throw new InvalidOperationException(
                "No se pudo abrir la configuración de inicio de Windows.");

        if (command is null)
        {
            key.DeleteValue(
                ValueName,
                throwOnMissingValue: false);
        }
        else
        {
            key.SetValue(
                ValueName,
                command,
                RegistryValueKind.String);
        }
    }
}