using System.Drawing.Drawing2D;
using UI.Models;
using UI.State;

namespace UI.Components;

internal class StartBlock : Panel
{
    public StartBlock(Point location)
    {
        Exists = true;
        Location = location;
        Size = new(300, 200);
        BackColor = Color.White;

        NextArrow = new(new(Location.X + 150, Location.Y + 200));
        ArrowsManager.CurrentInstance.AddOrigin(NextArrow);
        var nextLabel = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "+",
            Font = new(Font.FontFamily, 14),
            Size = new(30, 30),
            Location = new(135, 155),
        };
        Controls.Add(nextLabel);
        nextLabel.Click += (_, _) => ArrowsManager.CurrentInstance.SelectedOrigin = NextArrow;

        var label = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "Start",
            Font = new(Font.FontFamily, 14),
            Size = new(280, 30),
            Location = new(10, 85),
        };
        Controls.Add(label);
    }

    public static bool Exists { get; private set; }

    public ArrowOrigin NextArrow { get; }

    public void RemoveOrigins()
    {
        ArrowsManager.CurrentInstance.RemoveOrigin(NextArrow);
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