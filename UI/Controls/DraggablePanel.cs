using UI.Drawing;

namespace UI.Controls;

internal class DraggablePanel : Panel
{
    private Point _startPoint;

    public DraggablePanel()
    {
        DoubleBuffered = true;
        BackColor = Color.Fuchsia;
        // FIXME:
        var panel1 = new MovablePanel
        {
            Location = new(500, 500),
            Size = new(50, 50),
            BackColor = Color.White,
        };
        Controls.Add(panel1);
        var panel2 = new MovablePanel
        {
            Location = new(600, 600),
            Size = new(50, 50),
            BackColor = Color.White,
        };
        Controls.Add(panel2);
        panel1.Move += (_, _) => Invalidate();
        panel2.Move += (_, _) => Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        _startPoint = e.Location;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (e.Button != MouseButtons.Left)
        {
            return;
        }

        var parentSize = Parent!.Size;
        var dx = e.X - _startPoint.X;
        var dy = e.Y - _startPoint.Y;
        var newX = Math.Min(0, Location.X + dx);
        var newY = Math.Min(0, Location.Y + dy);
        newX = Math.Max(newX, parentSize.Width - Size.Width);
        newY = Math.Max(newY, parentSize.Height - Size.Height);
        Location = new(newX, newY);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        LineDrawer.Draw(new(20, 20), new(700, 300), this, e.Graphics);
    }
}