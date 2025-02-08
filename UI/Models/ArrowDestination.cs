namespace UI.Models;

internal class ArrowDestination(Point location)
{
    public Point Location { get; set; } = location;

    public ArrowOrigin? Origin { get; set; }
}