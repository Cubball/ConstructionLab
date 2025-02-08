using System.Drawing.Drawing2D;
using UI.Models;
using UI.State;

namespace UI.Components;

internal class ConditionalBlock : Panel
{
    private readonly ArrowOrigin _trueArrow;
    private readonly ArrowOrigin _falseArrow;

    public ConditionalBlock(Point location)
    {
        Location = location;
        Size = new(300, 200);
        BackColor = Color.White;

        _trueArrow = new(new(Location.X + 150, Location.Y + 200));
        _falseArrow = new(new(Location.X + 300, Location.Y + 100));
        ArrowsManager.AddOrigin(_trueArrow);
        ArrowsManager.AddOrigin(_falseArrow);

        var addTrueLabel = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "+",
            Font = new(Font.FontFamily, 14),
            Size = new(30, 30),
            Location = new(135, 155),
        };
        Controls.Add(addTrueLabel);
        addTrueLabel.Click += (_, _) => ArrowsManager.SelectedOrigin = _trueArrow;

        var addFalseLabel = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "+",
            Font = new(Font.FontFamily, 14),
            Size = new(30, 30),
            Location = new(245, 85),
        };
        Controls.Add(addFalseLabel);
        addFalseLabel.Click += (_, _) => ArrowsManager.SelectedOrigin = _falseArrow;
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
        Region = new Region(path);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.Black, 3);
        e.Graphics.DrawPath(pen, path);
    }
}