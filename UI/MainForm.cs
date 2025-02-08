using UI.Controls;

namespace UI;

internal class MainForm : Form
{
    public MainForm()
    {
        Size = new(1000, 1000);
        var draggablePanel = new DraggablePanel
        {
            Size = new(10_000, 10_000),
            Location = new(-500, -500),
        };
        Controls.Add(draggablePanel);
    }
}