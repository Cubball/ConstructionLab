using System.Drawing.Drawing2D;
using UI.Models;
using UI.State;

namespace UI.Components;

internal class StartBlock : Panel
{
    private readonly ArrowOrigin _nextArrow;

    public StartBlock(Point location)
    {
        Exists = true;
        Location = location;
        Size = new(300, 200);
        BackColor = Color.White;

        _nextArrow = new(new(Location.X + 150, Location.Y + 200));
        ArrowsManager.AddOrigin(_nextArrow);
        var nextLabel = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "+",
            Font = new(Font.FontFamily, 14),
            Size = new(30, 30),
            Location = new(135, 155),
        };
        Controls.Add(nextLabel);
        nextLabel.Click += (_, _) => ArrowsManager.SelectedOrigin = _nextArrow;
    }

    public static bool Exists { get; private set; }

    public void RemoveOrigins()
    {
        ArrowsManager.RemoveOrigin(_nextArrow);
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        using var dialog = new BlockDialogForm(true);
        dialog.BlockDeleted += (_, _) =>
        {
            Parent?.Controls.Remove(this);
            Exists = false;
            RemoveOrigins();
        };
        dialog.ShowDialog();
    }


    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.Black, 3);
        using var path = GetRoundedPath(ClientRectangle);
        e.Graphics.DrawPath(pen, path);
        Region = new(path);
    }

    private static GraphicsPath GetRoundedPath(Rectangle bounds)
    {
        const int diameter = 100;
        var path = new GraphicsPath();
        path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
        path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
        path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}