namespace UI.Drawing;

internal static class LineDrawer
{
    private const int StepInPixels = 50;
    private const int PaddingInPixels = 10;

    public static void Draw(Point from, Point to, Control canvas, Graphics graphics)
    {
        var queue = new Queue<Point>();
        var previous = new Dictionary<Point, Point?>();
        queue.Enqueue(from);
        previous[from] = null;
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == to)
            {
                break;
            }

            var bottom = GetNextPoint(current, 0, StepInPixels, to);
            var right = GetNextPoint(current, StepInPixels, 0, to);
            var top = GetNextPoint(current, 0, -StepInPixels, to);
            var left = GetNextPoint(current, -StepInPixels, 0, to);
            if (WithinControl(bottom, canvas) && !previous.ContainsKey(bottom) && !IntersectsChild(bottom, canvas, to))
            {
                queue.Enqueue(bottom);
                previous[bottom] = current;
            }

            if (WithinControl(right, canvas) && !previous.ContainsKey(right) && !IntersectsChild(right, canvas, to))
            {
                queue.Enqueue(right);
                previous[right] = current;
            }

            if (WithinControl(top, canvas) && !previous.ContainsKey(top) && !IntersectsChild(top, canvas, to))
            {
                queue.Enqueue(top);
                previous[top] = current;
            }

            if (WithinControl(left, canvas) && !previous.ContainsKey(left) && !IntersectsChild(left, canvas, to))
            {
                queue.Enqueue(left);
                previous[left] = current;
            }
        }

        var lineStart = to;
        while (true)
        {
            if (!previous.TryGetValue(lineStart, out var lineEnd) || lineEnd is null)
            {
                break;
            }

            graphics.DrawLine(Pens.Black, lineStart, lineEnd.Value);
            lineStart = lineEnd.Value;
        }
    }

    private static bool WithinControl(Point point, Control canvas)
    {
        return point.X >= 0 && point.Y >= 0
            && point.X < canvas.Size.Width && point.Y < canvas.Size.Height;
    }

    private static bool IntersectsChild(Point point, Control canvas, Point to)
    {
        for (var i = 0; i < canvas.Controls.Count; i++)
        {
            var control = canvas.Controls[i];
            var bounds = new Rectangle(
                control.Bounds.Left - PaddingInPixels,
                control.Bounds.Top - PaddingInPixels,
                control.Bounds.Width + (2 * PaddingInPixels),
                control.Bounds.Height + (2 * PaddingInPixels));
            if (bounds.Contains(point))
            {
                return !bounds.Contains(to);
            }
        }

        return false;
    }

    private static Point GetNextPoint(Point current, int dx, int dy, Point to)
    {
        var newX = current.X + dx;
        var newY = current.Y + dy;
        if (Math.Abs(newX - to.X) < StepInPixels)
        {
            newX = to.X;
        }

        if (Math.Abs(newY - to.Y) < StepInPixels)
        {
            newY = to.Y;
        }

        return new(newX, newY);
    }
}