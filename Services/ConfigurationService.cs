using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TeclaLibre.Models;

namespace TeclaLibre.Services;

internal sealed class ConfigurationService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,

        Converters =
        {
            new JsonStringEnumConverter(
                namingPolicy: null,
                allowIntegerValues: false)
        }
    };

    public string DirectoryPath { get; }

    public string DefaultFilePath { get; }

    public string BackupFilePath { get; }

    public ConfigurationService()
    {
        string localAppData = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);

        DirectoryPath = Path.Combine(
            localAppData,
            "TeclaLibre");

        DefaultFilePath = Path.Combine(
            DirectoryPath,
            "config.json");

        BackupFilePath = Path.Combine(
            DirectoryPath,
            "config.backup.json");
    }

    public bool DefaultFileExists()
    {
        return File.Exists(DefaultFilePath);
    }

    public AppConfiguration LoadDefault()
    {
        return LoadFromFile(DefaultFilePath);
    }

    public AppConfiguration LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path, Encoding.UTF8);

        AppConfiguration? configuration =
            JsonSerializer.Deserialize<AppConfiguration>(
                json,
                _jsonOptions);

        if (configuration is null)
        {
            throw new InvalidDataException(
                "El archivo no contiene una configuración válida.");
        }

        Validate(configuration);

        return configuration;
    }

    public void SaveDefault(AppConfiguration configuration)
    {
        Validate(configuration);

        Directory.CreateDirectory(DirectoryPath);

        // Conservamos el archivo anterior como respaldo solo
        // si se puede leer y validar.
        if (File.Exists(DefaultFilePath))
        {
            bool previousFileIsValid;

            try
            {
                _ = LoadFromFile(DefaultFilePath);
                previousFileIsValid = true;
            }
            catch (JsonException)
            {
                previousFileIsValid = false;
            }
            catch (InvalidDataException)
            {
                previousFileIsValid = false;
            }

            if (previousFileIsValid)
            {
                File.Copy(
                    DefaultFilePath,
                    BackupFilePath,
                    overwrite: true);
            }
        }

        WriteFile(DefaultFilePath, configuration);
    }

    public void ExportToFile(
        string path,
        AppConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        Validate(configuration);

        WriteFile(path, configuration);
    }

    private void WriteFile(
        string path,
        AppConfiguration configuration)
    {
        string fullPath = Path.GetFullPath(path);

        string? directory = Path.GetDirectoryName(fullPath);

        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new InvalidDataException(
                "No se pudo determinar la carpeta de destino.");
        }

        Directory.CreateDirectory(directory);

        string temporaryPath = Path.Combine(
            directory,
            $".TeclaLibre-{Guid.NewGuid():N}.tmp");

        try
        {
            string json = JsonSerializer.Serialize(
                configuration,
                _jsonOptions);

            File.WriteAllText(
                temporaryPath,
                json,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            File.Move(
                temporaryPath,
                fullPath,
                overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private static void Validate(
        AppConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (configuration.SchemaVersion != 1)
        {
            throw new InvalidDataException(
                $"Versión de configuración no compatible: " +
                $"{configuration.SchemaVersion}.");
        }

        if (configuration.Settings is null)
        {
            throw new InvalidDataException(
                "Falta la sección de preferencias.");
        }

        if (configuration.Rules is null)
        {
            throw new InvalidDataException(
                "Falta la lista de reglas.");
        }

        if (configuration.Settings.PauseShortcut is not null)
        {
            ValidateTrigger(
                configuration.Settings.PauseShortcut,
                "Atajo de pausa");
        }

        HashSet<Guid> ids = new();

        foreach (MappingRule? rule in configuration.Rules)
        {
            if (rule is null)
            {
                throw new InvalidDataException(
                    "La lista contiene una regla vacía.");
            }

            if (rule.Id == Guid.Empty || !ids.Add(rule.Id))
            {
                throw new InvalidDataException(
                    "Hay reglas con identificadores vacíos o duplicados.");
            }

            if (string.IsNullOrWhiteSpace(rule.Name))
            {
                throw new InvalidDataException(
                    "Hay una regla sin nombre.");
            }

            if (rule.Trigger is null || rule.Output is null)
            {
                throw new InvalidDataException(
                    $"La regla «{rule.Name}» está incompleta.");
            }

            if (!Enum.IsDefined(rule.RepeatMode))
            {
                throw new InvalidDataException(
                    $"La repetición de «{rule.Name}» no es válida.");
            }

            ValidateTrigger(
                rule.Trigger,
                $"Entrada de «{rule.Name}»");

            ValidateOutput(
                rule.Output,
                rule.Name);
        }
    }

    private static void ValidateTrigger(
        KeyTrigger trigger,
        string description)
    {
        ValidateActionKey(trigger.VirtualKey, description);
        ValidateModifiers(trigger.Modifiers, description);
    }

    private static void ValidateOutput(
        MappingOutput output,
        string ruleName)
    {
        string description = $"Salida de «{ruleName}»";

        if (!Enum.IsDefined(output.Kind))
        {
            throw new InvalidDataException(
                $"{description}: tipo de salida desconocido.");
        }

        switch (output.Kind)
        {
            case OutputKind.Text:
                if (string.IsNullOrEmpty(output.Text))
                {
                    throw new InvalidDataException(
                        $"{description}: el texto está vacío.");
                }

                break;

            case OutputKind.Key:
                ValidateActionKey(output.VirtualKey, description);

                if (output.Modifiers != KeyModifiers.None)
                {
                    throw new InvalidDataException(
                        $"{description}: una tecla individual " +
                        "no debe tener modificadores.");
                }

                break;

            case OutputKind.Shortcut:
                ValidateActionKey(output.VirtualKey, description);
                ValidateModifiers(output.Modifiers, description);

                if (output.Modifiers == KeyModifiers.None)
                {
                    throw new InvalidDataException(
                        $"{description}: falta un modificador.");
                }

                bool hasAltGr =
                    (output.Modifiers & KeyModifiers.AltGr) != 0;

                bool hasControl =
                    (output.Modifiers & KeyModifiers.Control) != 0;

                if (hasAltGr && hasControl)
                {
                    throw new InvalidDataException(
                        $"{description}: AltGr con Control adicional " +
                        "todavía no está admitido.");
                }

                break;
        }
    }

    private static void ValidateActionKey(
        int virtualKey,
        string description)
    {
        if (virtualKey < 1 || virtualKey > 254)
        {
            throw new InvalidDataException(
                $"{description}: código de tecla no válido.");
        }

        if (IsModifierKey(virtualKey))
        {
            throw new InvalidDataException(
                $"{description}: la tecla principal no puede " +
                "ser un modificador.");
        }
    }

    private static bool IsModifierKey(int virtualKey)
    {
        return virtualKey is
            0x10 or 0x11 or 0x12 or
            0x5B or 0x5C or
            0xA0 or 0xA1 or
            0xA2 or 0xA3 or
            0xA4 or 0xA5;
    }

    private static void ValidateModifiers(
        KeyModifiers modifiers,
        string description)
    {
        const KeyModifiers allowed =
            KeyModifiers.Control |
            KeyModifiers.Shift |
            KeyModifiers.Alt |
            KeyModifiers.Windows |
            KeyModifiers.AltGr;

        if ((modifiers & ~allowed) != 0)
        {
            throw new InvalidDataException(
                $"{description}: hay modificadores desconocidos.");
        }
    }
}