using UI.Models;

namespace UI.State;

internal class ArrowsManager
{
    private static readonly List<ArrowsManager> Instances = [];

    private readonly List<ArrowOrigin> _origins = [];
    private readonly Dictionary<ArrowDestination, Control> _destinations = [];

    public static ArrowsManager CurrentInstance { get; private set; } = default!;

    public static void AddInstance()
    {
        var instance = new ArrowsManager();
        Instances.Add(instance);
        CurrentInstance = instance;
    }

    public static void RemoveInstance(int idx)
    {
        var instance = Instances[idx];
        Instances.RemoveAt(idx);
        if (CurrentInstance == instance)
        {
            CurrentInstance = Instances.LastOrDefault() ?? throw new InvalidOperationException();
        }
    }

    public static void SetCurrentInstance(int idx)
    {
        CurrentInstance = Instances[idx];
    }

    public ArrowOrigin? SelectedOrigin { get; set; }

    public event EventHandler? ArrowsChanged;

    public void AddOrigin(ArrowOrigin origin)
    {
        _origins.Add(origin);
        origin.DestinationChanged += HandleDestinationChanged;
        ArrowsChanged?.Invoke(null, EventArgs.Empty);
    }

    public void RemoveOrigin(ArrowOrigin origin)
    {
        _origins.Remove(origin);
        origin.DestinationChanged -= HandleDestinationChanged;
        ArrowsChanged?.Invoke(null, EventArgs.Empty);
    }

    public IReadOnlyList<ArrowOrigin> GetOrigins()
    {
        return _origins;
    }

    public void AddDestination(ArrowDestination destination, Control control)
    {
        _destinations[destination] = control;
    }

    public Control? GetDestination(ArrowDestination destination)
    {
        return _destinations.GetValueOrDefault(destination);
    }

    public void RemoveDestination(ArrowDestination destination)
    {
        _destinations.Remove(destination);
        foreach (var origin in _origins)
        {
            if (origin.Destination == destination)
            {
                origin.Destination = null;
            }
        }
    }

    private void HandleDestinationChanged(object? sender, EventArgs e)
    {
        ArrowsChanged?.Invoke(null, EventArgs.Empty);
    }
}