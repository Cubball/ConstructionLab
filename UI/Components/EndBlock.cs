using System.Drawing.Drawing2D;
using UI.Models;
using UI.State;

namespace UI.Components;

internal class EndBlock : Panel
{
    public EndBlock(Point location)
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

        var label = new Label
        {
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "End",
            Font = new(Font.FontFamily, 14),
            Size = new(280, 30),
            Location = new(10, 85),
        };
        Controls.Add(label);
    }

    public ArrowDestination Destination { get; }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        using var dialog = new BlockDialogForm(true);
        dialog.BlockDeleted += (_, _) =>
        {
            Parent?.Controls.Remove(this);
            ArrowsManager.CurrentInstance.RemoveDestination(Destination);
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