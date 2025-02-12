using System.Globalization;
using Core.Testing;

namespace UI.Components;

internal class TestingResultForm : Form
{
    public TestingResultForm(TestingResult result)
    {
        Text = "Testing Result";
        Size = new(300, 200);

        var button = new Button
        {
            Text = "Submit",
            Dock = DockStyle.Bottom,
            Size = new(200, 50),
        };
        Controls.Add(button);

        var input = new TextBox
        {
            Dock = DockStyle.Fill,
        };
        Controls.Add(input);

        var label = new Label
        {
            Text = "Enter K - total number of steps executed",
            Dock = DockStyle.Top,
            Size = new(200, 50),
        };
        Controls.Add(label);

        button.Click += (sender, args) =>
        {
            var k = int.Parse(input.Text, CultureInfo.InvariantCulture);
            if (!result.SuccessRates.TryGetValue(k, out var successRate))
            {
                MessageBox.Show("No data for this K", "Error");
                return;
            }

            var message = $"Success rate for K = {k} is {successRate}%";
            MessageBox.Show(message, "Success Rate");
        };
    }
}