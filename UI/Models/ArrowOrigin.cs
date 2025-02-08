namespace UI.Models;

internal class ArrowOrigin(Point location)
{
    private ArrowDestination? _destination;

    public Point Location { get; set; } = location;

    public ArrowDestination? Destination
    {
        get => _destination;
        set
        {
            if (value == _destination)
            {
                return;
            }

            _destination = value;
            DestinationChanged?.Invoke(this, EventArgs.Empty);
            if (value is not null)
            {
                value.Origin = this;
            }
        }
    }

    public event EventHandler? DestinationChanged;
}