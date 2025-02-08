using UI.Models;

namespace UI.State;

internal static class ArrowsManager
{
    private static readonly List<ArrowOrigin> Origins = [];
    private static readonly Dictionary<ArrowDestination, Control> Destinations = [];

    public static ArrowOrigin? SelectedOrigin { get; set; }

    public static event EventHandler? ArrowsChanged;

    public static void AddOrigin(ArrowOrigin origin)
    {
        Origins.Add(origin);
        origin.DestinationChanged += HandleDestinationChanged;
        ArrowsChanged?.Invoke(null, EventArgs.Empty);
    }

    public static void RemoveOrigin(ArrowOrigin origin)
    {
        Origins.Remove(origin);
        origin.DestinationChanged -= HandleDestinationChanged;
        ArrowsChanged?.Invoke(null, EventArgs.Empty);
    }

    public static IReadOnlyList<ArrowOrigin> GetOrigins()
    {
        return Origins;
    }

    public static void AddDestination(ArrowDestination destination, Control control)
    {
        Destinations[destination] = control;
    }

    public static Control? GetDestination(ArrowDestination destination)
    {
        return Destinations.GetValueOrDefault(destination);
    }

    public static void RemoveDestination(ArrowDestination destination)
    {
        Destinations.Remove(destination);
        foreach (var origin in Origins)
        {
            if (origin.Destination == destination)
            {
                origin.Destination = null;
            }
        }
    }

    private static void HandleDestinationChanged(object? sender, EventArgs e)
    {
        ArrowsChanged?.Invoke(null, EventArgs.Empty);
    }
}