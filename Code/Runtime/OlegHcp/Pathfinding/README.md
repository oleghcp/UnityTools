## Pathfinding

### Code

```csharp
public class PathFinder
{
    public void Find(IPathNode origin, IPathNode target, List<IPathNode> result);
}
```

```csharp
public interface IPathNode
{
    IReadOnlyList<PathTransition> GetTransitions();
}
```

```csharp
public struct PathTransition
{
    public IPathNode Neighbor { get; }
    public float Cost { get; }

    public PathTransition(IPathNode neighbor, float cost);
    public void Deconstruct(out IPathNode neighbor, out float cost);
}
```

### Example with rect grid

Implementing abstract `IPathNode` with a custom position struct.

```csharp
public class ExamplePathNode : IPathNode
{
    private PathTransition[] _transitions;

    public NodePosition Position { get; }

    public ExamplePathNode(NodePosition pos)
    {
        Position = pos;
    }

    public override IReadOnlyList<PathTransition> GetTransitions()
    {
        return _transitions;
    }

    public void SetTransitions(PathTransition[] transitions)
    {
        _transitions = transitions;
    }
}
```

```csharp
public struct NodePosition : IEquatable<NodePosition>
{
    public readonly int I;
    public readonly int J;

    private int _hashCode;

    public NodePosition(int i, int j)
    {
        I = i;
        J = j;
        _hashCode = HashCode.Combine(I, J);
    }

    public void Deconstruct(out int i, out int j)
    {
        i = I;
        j = J;
    }

    public override int GetHashCode() => _hashCode;
    public override bool Equals(object other) => other is NodePosition pos && Equals(pos);
    public bool Equals(NodePosition other) => other.I == I && other.J == J;
    public static bool operator ==(NodePosition a, NodePosition b) => a.Equals(b);
    public static bool operator !=(NodePosition a, NodePosition b) => !a.Equals(b);
}
```

Generating grid of nodes.

```csharp
public class Example : MonoBehaviour
{
    private void Start()
    {
        PathFinder pathFinder = new PathFinder();

        // @ is origin cell
        // $ is target cell
        string[] field = new string[]
        {
            "████████████████",
            "█              █",
            "█  █████  ██   █",
            "█  █$      █   █",
            "█  █████████   █",
            "█         █    █",
            "█         ████ █",
            "█   ███   █    █",
            "█         █    █",
            "█        @     █",
            "█              █",
            "████████████████",
        };

        IPathNode[,] grid = GenerateGrid(field, out IPathNode origin, out IPathNode target);

        // Set transitions in the grid
        foreach (ExamplePathNode node in grid)
        {
            // Obstacle nodes are null
            node?.SetTransitions(GetNeighbors(grid, node));
        }

        List<IPathNode> result = new List<IPathNode>();
        pathFinder.Find(origin, target, result);

        if (result.Count == 0)
        {
            Debug.Log("Path doesn't exist.");
            return;
        }

        OverrideFieldWithPath(field, result);

        Debug.Log(field.ConcatToString('\n'));

        // Result:
        // ████████████████
        // █        +++   █
        // █  █████+ ██+  █
        // █  █X+++   █ + █
        // █  █████████  +█
        // █         █   +█
        // █         ████+█
        // █   ███   █  + █
        // █         █ +  █
        // █        @++   █
        // █              █
        // ████████████████
    }

    private static IPathNode[,] GenerateGrid(string[] filed, out IPathNode origin, out IPathNode target)
    {
        int iDimension = filed.Length;
        int jDimension = filed[0].Length;

        IPathNode[,] grid = new IPathNode[iDimension, jDimension];

        origin = default;
        target = default;

        for (int i = 0; i < iDimension; i++)
        {
            for (int j = 0; j < jDimension; j++)
            {
                char c = filed[i][j];
                NodePosition pos = new NodePosition(i, j);

                // Create a node if the current cell is not an obstacle
                // otherwise keep cell as null
                if (c != '█')
                    grid[i, j] = new ExamplePathNode(pos);

                if (c == '@')
                    origin = grid[i, j]; // Start cell
                else if (c == '$')
                    target = grid[i, j]; // Target cell
            }
        }

        return grid;
    }

    private static PathTransition[] GetNeighbors(IPathNode[,] nodeGrid, ExamplePathNode sourceNode)
    {
        List<PathTransition> neighbors = new List<PathTransition>();

        int iDimension = nodeGrid.GetLength(0);
        int jDimension = nodeGrid.GetLength(1);

        var (i, j) = sourceNode.Position;

        float diagonalCost = MathF.Sqrt(2f);

        bool iLess = i - 1 >= 0;
        bool iMore = i + 1 < iDimension;
        bool jLess = j - 1 >= 0;
        bool jMore = j + 1 < jDimension;

        if (iLess)
            tryAddNeighbor(nodeGrid[i - 1, j], 1f);

        if (iMore)
            tryAddNeighbor(nodeGrid[i + 1, j], 1f);

        if (jLess)
            tryAddNeighbor(nodeGrid[i, j - 1], 1f);

        if (jMore)
            tryAddNeighbor(nodeGrid[i, j + 1], 1f);

        if (iLess && jLess)
            tryAddNeighbor(nodeGrid[i - 1, j - 1], diagonalCost);

        if (iMore && jMore)
            tryAddNeighbor(nodeGrid[i + 1, j + 1], diagonalCost);

        if (iLess && jMore)
            tryAddNeighbor(nodeGrid[i - 1, j + 1], diagonalCost);

        if (iMore && jLess)
            tryAddNeighbor(nodeGrid[i + 1, j - 1], diagonalCost);

        return neighbors.ToArray();

        void tryAddNeighbor(IPathNode neighbor, float cost)
        {
            // Null is unavailable cell
            if (neighbor == null)
                return;

            neighbors.Add(new PathTransition(neighbor, cost));
        }
    }

    private static void OverrideFieldWithPath(string[] field, List<IPathNode> result)
    {
        char[][] chars = new char[field.Length][];

        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = field[i].ToCharArray();
        }

        for (int c = 0; c < result.Count; c++)
        {
            ExamplePathNode node = (ExamplePathNode)result[c];
            var (i, j) = node.Position;
            chars[i][j] = c == result.Count - 1 ? 'X' : '+';
        }

        for (int i = 0; i < chars.Length; i++)
        {
            field[i] = new string(chars[i]);
        }
    }
}
```
