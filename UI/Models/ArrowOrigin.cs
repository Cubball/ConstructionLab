namespace UI.Models;

internal record ArrowOrigin(Point Location)
{
    private ArrowDestination? _destination;

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
        }
    }

    public event EventHandler? DestinationChanged;
}