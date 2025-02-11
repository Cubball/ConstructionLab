using Core.Models;
using UI.Controls;
using UI.Drawing;
using UI.Models;
using UI.State;

namespace UI.Components;

internal class Grid : DraggablePanel
{
    private const int GridSize = 10_000;
    private const int CellSize = 500;

    private readonly Control?[,] _controls;

    public Grid()
    {
        var n = GridSize / CellSize;
        _controls = new Control[n, n];
        Size = new(GridSize, GridSize);
        ArrowsManager.CurrentInstance.ArrowsChanged += (_, _) => Invalidate();
    }

    public void SetBlock(int x, int y)
    {
    }

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        var x = e.X / CellSize;
        var y = e.Y / CellSize;
        RemoveBlockIfNotInControls(x, y);
        if (_controls[x, y] is not null)
        {
            var control = _controls[x, y];
            return;
        }

        using var dialog = new AddBlockDialogForm();
        dialog.BlockTypeSelected += (sender, blockType) =>
        {
            var blockX = (x * CellSize) + 100;
            var blockY = (y * CellSize) + 150;
            var location = new Point(blockX, blockY);
            Control block = blockType switch
            {
                BlockTypes.Start => new StartBlock(location),
                BlockTypes.End => new EndBlock(location),
                BlockTypes.Simple => new SimpleBlock(location),
                BlockTypes.Conditional => new ConditionalBlock(location),
                _ => throw new ArgumentOutOfRangeException(nameof(blockType), blockType, "Invalid block type.")
            };
            Controls.Add(block);
            block.Location = location;
            _controls[x, y] = block;
        };
        dialog.ShowDialog();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        PaintGrid(e.Graphics);
        foreach (var origin in ArrowsManager.CurrentInstance.GetOrigins())
        {
            if (origin.Destination is not null)
            {
                LineDrawer.Draw(origin.Location, origin.Destination.Location, this, e.Graphics);
            }
        }
    }

    private static void PaintGrid(Graphics graphics)
    {
        for (var x = 0; x < GridSize; x += CellSize)
        {
            graphics.DrawLine(Pens.LightGray, new(x, 0), new(x, GridSize - 1));
        }

        for (var y = 0; y < GridSize; y += CellSize)
        {
            graphics.DrawLine(Pens.LightGray, new(0, y), new(GridSize - 1, y));
        }
    }

    private void RemoveBlockIfNotInControls(int x, int y)
    {
        var control = _controls[x, y];
        var found = false;
        for (var i = 0; i < Controls.Count; i++)
        {
            if (Controls[i] == control)
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            _controls[x, y] = null;
        }
    }
}