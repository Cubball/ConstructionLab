using UI.Components;
using UI.Drawing;
using UI.State;

namespace UI.Controls;

internal class DraggablePanel : Panel
{
    private Point _startPoint;

    public DraggablePanel()
    {
        DoubleBuffered = true;
        BackColor = Color.Fuchsia;
        ArrowsManager.ArrowsChanged += (_, _) => Invalidate();

        // FIXME:
        var cond = new ConditionalBlock(new(100, 100));
        Controls.Add(cond);
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);
        if (ArrowsManager.SelectedOrigin is not null)
        {
            ArrowsManager.SelectedOrigin.Destination = new(e.Location);
        }
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
        foreach (var origin in ArrowsManager.GetOrigins())
        {
            if (origin.Destination is not null)
            {
                LineDrawer.Draw(origin.Location, origin.Destination.Location, this, e.Graphics);
            }
        }
    }
}