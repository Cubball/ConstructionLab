namespace UI.Controls;

internal class BlockDialogForm : Form
{
    public BlockDialogForm(bool onlyDelete = false, string initialText = "")
    {
        Text = "Update Block";
        StartPosition = FormStartPosition.CenterScreen;
        if (onlyDelete)
        {
            InitDeleteControls();
        }
        else
        {
            InitFullControls(initialText);
        }
    }

    private void InitDeleteControls()
    {
        Size = new Size(410, 180);
        var deleteLabel = new Label
        {
            Text = "Delete this block?",
            Location = new Point(20, 20),
            Size = new Size(370, 25),
            TextAlign = ContentAlignment.MiddleCenter
        };
        Controls.Add(deleteLabel);
        var deleteButton = new Button
        {
            Text = "Delete",
            Location = new Point(100, 80),
            Size = new Size(100, 35),
            DialogResult = DialogResult.Abort
        };
        deleteButton.Click += (sender, e) =>
        {
            BlockDeleted?.Invoke(this, EventArgs.Empty);
            Close();
        };
        Controls.Add(deleteButton);
        var cancelButton = new Button
        {
            Text = "Cancel",
            Location = new Point(210, 80),
            Size = new Size(100, 35),
            DialogResult = DialogResult.Cancel
        };
        cancelButton.Click += (sender, e) => Close();
        Controls.Add(cancelButton);
    }

    private void InitFullControls(string initialText)
    {
        Size = new Size(600, 250);
        var inputLabel = new Label
        {
            Text = "Enter the operation:",
            Location = new Point(20, 40),
            AutoSize = true
        };
        Controls.Add(inputLabel);
        var inputTextBox = new TextBox
        {
            Location = new Point(20, 70),
            Size = new Size(540, 25),
            Text = initialText
        };
        Controls.Add(inputTextBox);

        var okButton = new Button
        {
            Text = "OK",
            Location = new Point(140, 120),
            Size = new Size(100, 35),
            DialogResult = DialogResult.OK
        };
        okButton.Click += (sender, e) =>
        {
            OperationEntered?.Invoke(this, inputTextBox.Text);
            Close();
        };
        Controls.Add(okButton);

        var cancelButton = new Button
        {
            Text = "Cancel",
            Location = new Point(250, 120),
            Size = new Size(100, 35),
            DialogResult = DialogResult.Cancel
        };
        cancelButton.Click += (sender, e) => Close();
        Controls.Add(cancelButton);

        var deleteButton = new Button
        {
            Text = "Delete",
            Location = new Point(360, 120),
            Size = new Size(100, 35),
            DialogResult = DialogResult.Abort
        };
        deleteButton.Click += (sender, e) =>
        {
            BlockDeleted?.Invoke(this, EventArgs.Empty);
            Close();
        };
        Controls.Add(deleteButton);
    }

    public event EventHandler<string>? OperationEntered;

    public event EventHandler? BlockDeleted;
}