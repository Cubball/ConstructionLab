using UI.Controls;

namespace UI;

internal class MainForm : Form
{
    public MainForm()
    {
        Size = new(300, 300);
        var draggablePanel = new DraggablePanel(new(3_000, 3_000))
        {
            Location = new(-500, -500),
        };
        Controls.Add(draggablePanel);
    }
}