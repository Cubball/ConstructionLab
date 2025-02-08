using Core.CodeGeneration;
using UI.Components;
using UI.State;

namespace UI;

internal class MainForm : Form
{
    public MainForm()
    {
        Size = new(1000, 1000);
        var draggablePanel = new Grid
        {
            Location = new(-500, -500),
        };
        Controls.Add(draggablePanel);
        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.P)
            {
                var controls = new List<Control>();
                for (var i = 0; i < draggablePanel.Controls.Count; i++)
                {
                    controls.Add(draggablePanel.Controls[i]);
                }

                var startBlock = Converter.Convert(controls);
                var code = CodeGenerator.Generate([startBlock]);
                MessageBox.Show(code);
            }
        };
    }
}