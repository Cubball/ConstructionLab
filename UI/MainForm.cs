using UI.Components;

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
    }
}