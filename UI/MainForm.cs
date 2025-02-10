using Core.CodeGeneration;
using UI.Components;
using UI.State;

namespace UI;

internal class MainForm : Form
{
    private readonly Panel _sidebarPanel;
    private readonly ComboBox _diagramSelector;
    private readonly List<Grid> _diagrams;
    private Grid? _currentGrid;

    public MainForm()
    {
        Size = new(1200, 1000);
        _diagrams = [];

        _sidebarPanel = new Panel
        {
            Dock = DockStyle.Right,
            Width = 200,
            BackColor = Color.WhiteSmoke,
            Padding = new Padding(10)
        };
        Controls.Add(_sidebarPanel);

        var deleteDiagramButton = new Button
        {
            Text = "Delete Current Diagram",
            Dock = DockStyle.Top,
            Margin = new Padding(0, 5, 0, 5),
            Size = new(200, 35),
        };
        deleteDiagramButton.Click += DeleteDiagramButtonClick;
        _sidebarPanel.Controls.Add(deleteDiagramButton);

        var newDiagramButton = new Button
        {
            Text = "New Diagram",
            Dock = DockStyle.Top,
            Margin = new Padding(0, 5, 0, 5),
            Size = new(200, 35),
        };
        newDiagramButton.Click += NewDiagramButtonClick;
        _sidebarPanel.Controls.Add(newDiagramButton);

        _diagramSelector = new ComboBox
        {
            Dock = DockStyle.Top,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin = new Padding(0, 0, 0, 10),
            Size = new(200, 35),
        };
        _diagramSelector.SelectedIndexChanged += DiagramSelectorSelectedIndexChanged;
        _sidebarPanel.Controls.Add(_diagramSelector);

        CreateNewDiagram();
        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.P && _currentGrid != null)
            {
                var controls = new List<Control>();
                for (var i = 0; i < _currentGrid.Controls.Count; i++)
                {
                    controls.Add(_currentGrid.Controls[i]);
                }

                var startBlock = Converter.Convert(controls);
                var code = CodeGenerator.Generate([startBlock]);
                MessageBox.Show(code);
            }
        };
    }

    private void CreateNewDiagram()
    {
        ArrowsManager.AddInstance();
        var newGrid = new Grid
        {
            Location = new(-5_000, -5_000),
        };
        _diagrams.Add(newGrid);
        _diagramSelector.Items.Add($"Diagram {_diagrams.Count}");
        _diagramSelector.SelectedIndex = _diagrams.Count - 1;
    }

    private void DiagramSelectorSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_currentGrid != null)
        {
            Controls.Remove(_currentGrid);
        }

        ArrowsManager.SetCurrentInstance(_diagramSelector.SelectedIndex);
        _currentGrid = _diagrams[_diagramSelector.SelectedIndex];
        Controls.Add(_currentGrid);
        _currentGrid.BringToFront();
        _sidebarPanel.BringToFront();
    }

    private void NewDiagramButtonClick(object? sender, EventArgs e)
    {
        CreateNewDiagram();
    }

    private void DeleteDiagramButtonClick(object? sender, EventArgs e)
    {
        if (_diagrams.Count <= 1)
        {
            MessageBox.Show("Cannot delete the last diagram.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        ArrowsManager.RemoveInstance(_diagramSelector.SelectedIndex);
        var currentIndex = _diagramSelector.SelectedIndex;
        Controls.Remove(_currentGrid);
        _diagrams.RemoveAt(currentIndex);
        _diagramSelector.Items.RemoveAt(currentIndex);
        _diagramSelector.Items.Clear();
        for (int i = 0; i < _diagrams.Count; i++)
        {
            _diagramSelector.Items.Add($"Diagram {i + 1}");
        }

        _diagramSelector.SelectedIndex = Math.Min(currentIndex, _diagrams.Count - 1);
        ArrowsManager.SetCurrentInstance(_diagramSelector.SelectedIndex);
    }
}