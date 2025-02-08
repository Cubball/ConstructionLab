using System.Drawing.Drawing2D;
using UI.Controls;
using UI.Models;
using UI.State;

namespace UI.Components;

internal class EndBlock : Panel
{
    private readonly ArrowDestination _destination;

    public EndBlock(Point location)
    {
        Location = location;
        Size = new(300, 200);
        BackColor = Color.White;

        _destination = new(new(Location.X + 150, Location.Y));
        ArrowsManager.AddDestination(_destination, this);
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
                ArrowsManager.SelectedOrigin.Destination = _destination;
            }
        };
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        using var dialog = new BlockDialogForm(true);
        dialog.BlockDeleted += (_, _) => Parent?.Controls.Remove(this);
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