using System.Drawing.Drawing2D;
using UI.Models;
using UI.State;

namespace UI.Components;

internal class ConditionalBlock : Panel
{
    private readonly Label _operationLabel;

    public ConditionalBlock(Point location)
    {
        Location = location;
        Size = new(300, 200);
        BackColor = Color.White;

        Destination = new(new(Location.X + 150, Location.Y));
        ArrowsManager.CurrentInstance.AddDestination(Destination, this);
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
            if (ArrowsManager.CurrentInstance.SelectedOrigin is not null)
            {
                ArrowsManager.CurrentInstance.SelectedOrigin.Destination = Destination;
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

        TrueArrow = new(new(Location.X + 150, Location.Y + 200));
        ArrowsManager.CurrentInstance.AddOrigin(TrueArrow);
        var addTrueLabel = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "+",
            Font = new(Font.FontFamily, 14),
            Size = new(30, 30),
            Location = new(135, 155),
        };
        Controls.Add(addTrueLabel);
        addTrueLabel.Click += (_, _) => ArrowsManager.CurrentInstance.SelectedOrigin = TrueArrow;

        FalseArrow = new(new(Location.X + 300, Location.Y + 100));
        ArrowsManager.CurrentInstance.AddOrigin(FalseArrow);
        var addFalseLabel = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "+",
            Font = new(Font.FontFamily, 14),
            Size = new(30, 30),
            Location = new(245, 85),
        };
        Controls.Add(addFalseLabel);
        addFalseLabel.Click += (_, _) => ArrowsManager.CurrentInstance.SelectedOrigin = FalseArrow;
    }

    public string Operation { get; set; } = string.Empty;

    public ArrowOrigin TrueArrow { get; }

    public ArrowOrigin FalseArrow { get; }

    public ArrowDestination Destination { get; }

    public void RemoveOrigins()
    {
        ArrowsManager.CurrentInstance.RemoveOrigin(TrueArrow);
        ArrowsManager.CurrentInstance.RemoveOrigin(FalseArrow);
    }

    public void SetOperation(string operation)
    {
        Operation = operation;
        _operationLabel.Text = operation;
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
        Point[] points = [
            new(Width / 2, 0),
            new(Width, Height / 2),
            new(Width / 2, Height),
            new(0, Height / 2)
        ];
        using var path = new GraphicsPath();
        path.AddPolygon(points);
        Region = new(path);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.Black, 3);
        e.Graphics.DrawPath(pen, path);
    }
}