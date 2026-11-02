using TeclaLibre.Models;
using System.ComponentModel;




namespace TeclaLibre.Controls;

public partial class RulesPanel : UserControl
{
    private readonly List<MappingRule> _rules = new();

    /*
    public Func<MappingRule?, MappingRule?>? EditRuleRequested
    {
        get;
        set;
    }
    */

    public event EventHandler? RulesChanged;

    //public IReadOnlyList<MappingRule> Rules => _rules;

    /*
    public bool HasEnabledRules =>
        _rules.Any(rule => rule.Enabled);
    */

    [Browsable(false)]
    [DesignerSerializationVisibility(
    DesignerSerializationVisibility.Hidden)]
    public Func<MappingRule?, MappingRule?>? EditRuleRequested
    {
        get;
        set;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(
        DesignerSerializationVisibility.Hidden)]
    public IReadOnlyList<MappingRule> Rules => _rules;

    [Browsable(false)]
    [DesignerSerializationVisibility(
        DesignerSerializationVisibility.Hidden)]
    public bool HasEnabledRules =>
        _rules.Any(rule => rule.Enabled);

    public RulesPanel()
    {
        InitializeComponent();

        ConfigureGrid();

        btnAdd.Click += BtnAdd_Click;
        btnEdit.Click += BtnEdit_Click;
        btnDuplicate.Click += BtnDuplicate_Click;
        btnToggle.Click += BtnToggle_Click;
        btnDelete.Click += BtnDelete_Click;

        gridRules.CellDoubleClick += GridRules_CellDoubleClick;
    }

    private void ConfigureGrid()
    {
        gridRules.Columns.Clear();

        gridRules.Columns.Add("Name", "Nombre");
        gridRules.Columns.Add("Trigger", "Entrada");
        gridRules.Columns.Add("Output", "Salida");
        gridRules.Columns.Add("Repeat", "Repetición");
        gridRules.Columns.Add("Enabled", "Estado");
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (EditRuleRequested is null)
        {
            return;
        }

        MappingRule? result = EditRuleRequested(null);

        if (result is null)
        {
            return;
        }

        AddRule(result);
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        EditSelectedRule();
    }

    private void GridRules_CellDoubleClick(
        object? sender,
        DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            EditSelectedRule();
        }
    }

    private void EditSelectedRule()
    {
        MappingRule? selected = GetSelectedRule();

        if (selected is null || EditRuleRequested is null)
        {
            return;
        }

        // El editor recibe una copia para que Cancelar
        // no modifique la regla original.
        MappingRule? result =
            EditRuleRequested(CopyRule(selected));

        if (result is null)
        {
            return;
        }

        if (HasConflict(result, selected.Id))
        {
            ShowConflict();
            return;
        }

        int index = _rules.FindIndex(
            rule => rule.Id == selected.Id);

        if (index < 0)
        {
            return;
        }

        _rules[index] = result;

        RefreshGrid(result.Id);
        RulesChanged?.Invoke(this, EventArgs.Empty);
    }

    private void BtnDuplicate_Click(object? sender, EventArgs e)
    {
        MappingRule? selected = GetSelectedRule();

        if (selected is null || EditRuleRequested is null)
        {
            return;
        }

        MappingRule copy = CopyRule(selected);

        copy.Id = Guid.NewGuid();
        copy.Name = $"{selected.Name} - copia";

        MappingRule? result = EditRuleRequested(copy);

        if (result is null)
        {
            return;
        }

        AddRule(result);
    }

    private void AddRule(MappingRule rule)
    {
        if (HasConflict(rule, excludedId: null))
        {
            ShowConflict();
            return;
        }

        _rules.Add(rule);

        RefreshGrid(rule.Id);
        RulesChanged?.Invoke(this, EventArgs.Empty);
    }

    private void BtnToggle_Click(object? sender, EventArgs e)
    {
        MappingRule? selected = GetSelectedRule();

        if (selected is null)
        {
            return;
        }

        MappingRule updated = CopyRule(selected);
        updated.Enabled = !updated.Enabled;

        if (HasConflict(updated, selected.Id))
        {
            ShowConflict();
            return;
        }

        int index = _rules.FindIndex(
            rule => rule.Id == selected.Id);

        if (index < 0)
        {
            return;
        }

        _rules[index] = updated;

        RefreshGrid(updated.Id);
        RulesChanged?.Invoke(this, EventArgs.Empty);
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        MappingRule? selected = GetSelectedRule();

        if (selected is null)
        {
            return;
        }

        DialogResult answer = MessageBox.Show(
            this,
            $"¿Eliminar la regla «{selected.Name}»?",
            "TeclaLibre",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (answer != DialogResult.Yes)
        {
            return;
        }

        _rules.RemoveAll(rule => rule.Id == selected.Id);

        RefreshGrid();
        RulesChanged?.Invoke(this, EventArgs.Empty);
    }

    private MappingRule? GetSelectedRule()
    {
        if (gridRules.SelectedRows.Count == 0)
        {
            MessageBox.Show(
                this,
                "Selecciona una regla de la lista.",
                "TeclaLibre",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return null;
        }

        if (gridRules.SelectedRows[0].Tag is not Guid id)
        {
            return null;
        }

        return _rules.FirstOrDefault(rule => rule.Id == id);
    }

    private void RefreshGrid(Guid? selectedId = null)
    {
        gridRules.Rows.Clear();

        foreach (MappingRule rule in _rules)
        {
            int rowIndex = gridRules.Rows.Add(
                rule.Name,
                FormatTrigger(rule.Trigger),
                FormatOutput(rule.Output),
                rule.RepeatMode == RepeatMode.RepeatWhileHeld
                    ? "Repetir"
                    : "Una vez",
                rule.Enabled ? "Habilitada" : "Pausada");

            DataGridViewRow row = gridRules.Rows[rowIndex];
            row.Tag = rule.Id;

            if (selectedId.HasValue && selectedId.Value == rule.Id)
            {
                row.Selected = true;
                gridRules.CurrentCell = row.Cells[0];
            }
        }
    }

    private bool HasConflict(
        MappingRule candidate,
        Guid? excludedId)
    {
        if (!candidate.Enabled)
        {
            return false;
        }

        return _rules.Any(existing =>
            existing.Enabled &&
            existing.Id != excludedId &&
            TriggersOverlap(
                candidate.Trigger,
                existing.Trigger));
    }

    private static bool TriggersOverlap(
        KeyTrigger a,
        KeyTrigger b)
    {
        if (a.VirtualKey != b.VirtualKey)
        {
            return false;
        }

        if (a.ExactMatch && b.ExactMatch)
        {
            return a.Modifiers == b.Modifiers;
        }

        if (a.ExactMatch)
        {
            return (a.Modifiers & b.Modifiers) == b.Modifiers;
        }

        if (b.ExactMatch)
        {
            return (b.Modifiers & a.Modifiers) == a.Modifiers;
        }

        // Dos reglas no exactas siempre pueden coincidir
        // cuando se presiona la unión de sus modificadores.
        return true;
    }

    private void ShowConflict()
    {
        MessageBox.Show(
            this,
            "La entrada se superpone con otra regla habilitada.\r\n\r\n" +
            "Cambia la combinación o pausa la otra regla.",
            "TeclaLibre - Conflicto de reglas",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }

    private static MappingRule CopyRule(MappingRule source)
    {
        return new MappingRule
        {
            Id = source.Id,
            Name = source.Name,
            Enabled = source.Enabled,
            RepeatMode = source.RepeatMode,

            Trigger = new KeyTrigger
            {
                VirtualKey = source.Trigger.VirtualKey,
                Modifiers = source.Trigger.Modifiers,
                ExactMatch = source.Trigger.ExactMatch
            },

            Output = new MappingOutput
            {
                Kind = source.Output.Kind,
                Text = source.Output.Text,
                VirtualKey = source.Output.VirtualKey,
                Modifiers = source.Output.Modifiers
            }
        };
    }

    private static string FormatTrigger(KeyTrigger trigger)
    {
        string key = ((Keys)trigger.VirtualKey).ToString();

        if (trigger.Modifiers == KeyModifiers.None)
        {
            return key;
        }

        return $"{trigger.Modifiers} + {key}";
    }

    private static string FormatOutput(MappingOutput output)
    {
        return output.Kind switch
        {
            OutputKind.Text =>
                $"Texto: {output.Text}",

            OutputKind.Key =>
                $"Tecla: {(Keys)output.VirtualKey}",

            OutputKind.Shortcut =>
                $"{output.Modifiers} + {(Keys)output.VirtualKey}",

            _ => "Salida desconocida"
        };
    }
    public void ReplaceRules(IEnumerable<MappingRule> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);

        // Creamos una lista separada antes de cambiar la actual.
        List<MappingRule> incoming = rules
            .Select(CopyRule)
            .ToList();

        for (int i = 0; i < incoming.Count; i++)
        {
            if (!incoming[i].Enabled)
            {
                continue;
            }

            for (int j = i + 1; j < incoming.Count; j++)
            {
                if (!incoming[j].Enabled)
                {
                    continue;
                }

                if (TriggersOverlap(
                        incoming[i].Trigger,
                        incoming[j].Trigger))
                {
                    throw new InvalidDataException(
                        $"Las reglas «{incoming[i].Name}» y " +
                        $"«{incoming[j].Name}» tienen entradas " +
                        "habilitadas que se superponen.");
                }
            }
        }

        _rules.Clear();
        _rules.AddRange(incoming);

        RefreshGrid();

        RulesChanged?.Invoke(this, EventArgs.Empty);
    }
}