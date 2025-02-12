namespace UI.Controls;

internal class TextInputForm : Form
{
    public TextInputForm(string text)
    {
        Text = "Result";
        Size = new(1000, 500);
        var textBox = new TextBox
        {
            Multiline = true,
            Dock = DockStyle.Fill,
            Text = text,
            ReadOnly = true,
            Font = new("Consolas", 14),
        };
        Controls.Add(textBox);
    }
}