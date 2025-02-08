namespace UI.Controls;

internal class DraggablePanel : Panel
{
    private Point _startPoint;

    public DraggablePanel()
    {
        DoubleBuffered = true;
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
}