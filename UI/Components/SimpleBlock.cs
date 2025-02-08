using System.Drawing.Drawing2D;
using UI.Models;
using UI.State;

namespace UI.Components;

internal class SimpleBlock : Panel
{
    private readonly Label _operationLabel;

    public SimpleBlock(Point location)
    {
        Location = location;
        Size = new(300, 200);
        BackColor = Color.White;

        Destination = new(new(Location.X + 150, Location.Y));
        ArrowsManager.AddDestination(Destination, this);
        var destinationLabel = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "×",
            Font = new(Font.FontFamily, 14),
            Size = new(30, 30),
            Location = new(137, 20),
        };
        Controls.Add(destinationLabel);
        destinationLabel.Click += (_, _) =>
        {
            if (ArrowsManager.SelectedOrigin is not null)
            {
                ArrowsManager.SelectedOrigin.Destination = Destination;
            }
        };

        _operationLabel = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter,
            Text = Operation,
            Font = new(Font.FontFamily, 14),
            Size = new(200, 30),
            Location = new(50, 80),
        };
        Controls.Add(_operationLabel);

        NextArrow = new(new(Location.X + 150, Location.Y + 200));
        ArrowsManager.AddOrigin(NextArrow);
        var nextLabel = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "+",
            Font = new(Font.FontFamily, 14),
            Size = new(30, 30),
            Location = new(135, 155),
        };
        Controls.Add(nextLabel);
        nextLabel.Click += (_, _) => ArrowsManager.SelectedOrigin = NextArrow;
    }

    public string Operation { get; set; } = string.Empty;

    public ArrowOrigin NextArrow { get; }

    public ArrowDestination Destination { get; }

    public void RemoveOrigins()
    {
        ArrowsManager.RemoveOrigin(NextArrow);
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        using var dialog = new BlockDialogForm(initialText: Operation);
        dialog.OperationEntered += (_, operation) => SetOperation(operation);
        dialog.BlockDeleted += (_, _) =>
        {
            Parent?.Controls.Remove(this);
            RemoveOrigins();
        };
        dialog.ShowDialog();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.Black, 3);
        e.Graphics.DrawRectangle(pen, new(new(0, 0), Size));
    }

    private void SetOperation(string operation)
    {
        Operation = operation;
        _operationLabel.Text = operation;
    }
}