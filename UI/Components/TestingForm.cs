using Core.Testing;
using UI.Controls;
using StartBlockModel = Core.Models.StartBlock;

namespace UI.Components;

internal class TestingForm : Form
{
    private static readonly string[] Separators = ["\r\n", "\r", "\n"];

    private readonly CancellationTokenSource _cts = new();
    private readonly List<StartBlockModel> _startBlocks;

    private readonly TextBox _stdinInput;
    private readonly TextBox _stdoutInput;
    private readonly CheckBox _runAllCheckBox;
    private readonly Button _runButton;
    private readonly Button _cancelButton;

    public TestingForm(List<StartBlockModel> startBlocks)
    {
        _startBlocks = startBlocks;

        Text = "Run Tests";
        Size = new Size(1000, 660);

        _cancelButton = new Button
        {
            Text = "Cancel",
            Dock = DockStyle.Top,
            Padding = new Padding(10),
            Size = new Size(200, 50),
            Visible = false,
        };
        _cancelButton.Click += CancelButtonClick;
        Controls.Add(_cancelButton);

        _runButton = new Button
        {
            Text = "Run",
            Dock = DockStyle.Top,
            Padding = new Padding(10),
            Size = new Size(200, 50),
        };
        _runButton.Click += RunButtonClick;
        Controls.Add(_runButton);

        _runAllCheckBox = new CheckBox
        {
            Text = "Run All Combinations",
            Dock = DockStyle.Top,
            Padding = new Padding(10),
            Size = new Size(200, 50),
            Checked = true,
        };
        Controls.Add(_runAllCheckBox);

        _stdoutInput = new TextBox
        {
            Multiline = true,
            Dock = DockStyle.Top,
            Height = 200,
            Padding = new Padding(10),
        };
        Controls.Add(_stdoutInput);

        var stdoutLabel = new Label
        {
            Text = "Expected Standard Output",
            Dock = DockStyle.Top,
            Padding = new Padding(10),
            Size = new Size(200, 50),
        };
        Controls.Add(stdoutLabel);

        _stdinInput = new TextBox
        {
            Multiline = true,
            Dock = DockStyle.Top,
            Height = 200,
            Padding = new Padding(10),
        };
        Controls.Add(_stdinInput);

        var stdinLabel = new Label
        {
            Text = "Standard Input",
            Dock = DockStyle.Top,
            Padding = new Padding(10),
            Size = new Size(200, 50),
        };
        Controls.Add(stdinLabel);

        _runAllCheckBox.CheckedChanged += (_, _) =>
        {
            stdoutLabel.Visible = _runAllCheckBox.Checked;
            _stdoutInput.Enabled = _runAllCheckBox.Checked;
        };
    }

    private void RunButtonClick(object? sender, EventArgs e)
    {
        _cancelButton.Visible = true;
        _runButton.Visible = false;
        var runAll = _runAllCheckBox.Checked;
        var stdin = _stdinInput.Text.Split(Separators, StringSplitOptions.None).ToList();
        var stdout = _stdoutInput.Text.Split(Separators, StringSplitOptions.None).ToList();
        new Thread(() =>
        {
            if (runAll)
            {
            }
            else
            {
                var form = new TextInputForm(
                    Tester.TestSingle(_startBlocks, stdin, _cts.Token)
                );
                form.ShowDialog();
                Invoke(new Action(() =>
                {
                    _cancelButton.Visible = false;
                    _runButton.Visible = true;
                }));
            }
        }).Start();
    }

    private void CancelButtonClick(object? sender, EventArgs e)
    {
        _cancelButton.Visible = false;
        _runButton.Visible = true;
    }
}