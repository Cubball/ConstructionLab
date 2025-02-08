using UI.Models;

namespace UI.Components;

public class AddBlockDialogForm : Form
{
    private static readonly string[] AvailableBlockTypes = [BlockTypes.Start, BlockTypes.End, BlockTypes.Simple, BlockTypes.Conditional];

    public AddBlockDialogForm()
    {
        Text = "Select Block Type";
        Size = new Size(300, 180);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;

        var text = new Label
        {
            Text = "Choose block type:",
            Location = new Point(10, 20),
            Size = new Size(200, 25)
        };
        Controls.Add(text);

        var select = new ComboBox
        {
            Location = new Point(10, 45),
            Size = new Size(265, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        select.Items.AddRange(AvailableBlockTypes);
        select.SelectedIndex = 0;
        Controls.Add(select);

        var ok = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(110, 80),
            Size = new Size(75, 35)
        };
        Controls.Add(ok);
        ok.Click += (sender, e) => BlockTypeSelected?.Invoke(this, select.SelectedItem?.ToString() ?? "");

        var cancel = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(200, 80),
            Size = new Size(75, 35)
        };
        Controls.Add(cancel);

        AcceptButton = ok;
        CancelButton = cancel;
    }

    public event EventHandler<string>? BlockTypeSelected;
}