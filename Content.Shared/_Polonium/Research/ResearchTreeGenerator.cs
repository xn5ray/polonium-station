using System.Linq;
using Content.Shared.Research.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._Polonium.Research;

/// <summary>
/// Builds a random technology tree. Tier 1 fills a square in the middle and every next tier fills a square ring
/// around the previous one. The technologies of a tier take random cells of their ring, and each technology of a
/// higher tier requires one or two technologies of the previous tier, preferably the closest ones, so the
/// connections run outwards from the middle.
/// </summary>
public static class ResearchTreeGenerator
{
    /// <summary>
    /// Generates positions and prerequisites for the given technologies.
    /// </summary>
    /// <param name="technologies">Technologies that should appear in the tree.</param>
    /// <param name="random">Source of randomness.</param>
    /// <param name="secondPrerequisiteChance">
    /// Chance (0 to 1) that a technology requires two technologies of the previous tier instead of one.
    /// </param>
    /// <returns>The layout of every given technology, by technology id.</returns>
    public static Dictionary<string, TechLayoutEntry> Generate(
        IEnumerable<TechnologyPrototype> technologies,
        IRobustRandom random,
        float secondPrerequisiteChance)
    {
        var layout = new Dictionary<string, TechLayoutEntry>();
        var previousSide = 0;
        List<(TechnologyPrototype Tech, Vector2i Position)>? previousTier = null;

        foreach (var tierGroup in technologies.GroupBy(t => t.Tier).OrderBy(g => g.Key))
        {
            var tier = tierGroup.ToList();
            var side = GetSideLength(previousSide, tier.Count);
            var cells = GetRingCells(previousSide, side);
            random.Shuffle(cells);

            var placed = new List<(TechnologyPrototype Tech, Vector2i Position)>(tier.Count);
            for (var i = 0; i < tier.Count; i++)
            {
                var tech = tier[i];
                var position = cells[i];
                var prerequisites = previousTier == null
                    ? new List<string>()
                    : PickPrerequisites(position, previousTier, random, secondPrerequisiteChance);

                layout[tech.ID] = new TechLayoutEntry(position, prerequisites);
                placed.Add((tech, position));
            }

            previousTier = placed;
            previousSide = side;
        }

        return layout;
    }

    /// <summary>
    /// Gets the side of the smallest square that has enough room for the given number of technologies
    /// outside of the previous square. The first tier fills the whole square.
    /// </summary>
    private static int GetSideLength(int previousSide, int count)
    {
        if (previousSide == 0)
        {
            var first = 1;
            while (first * first < count)
            {
                first++;
            }

            return first;
        }

        // Rings grow by one cell on each side, so the squares stay nested and centered on the same point.
        var side = previousSide + 2;
        while (side * side - previousSide * previousSide < count)
        {
            side += 2;
        }

        return side;
    }

    /// <summary>
    /// Gets all cells that are inside the square with the given side, but outside of the inner one.
    /// </summary>
    private static List<Vector2i> GetRingCells(int innerSide, int side)
    {
        var low = -(side / 2);
        var high = low + side - 1;
        var innerLow = -(innerSide / 2);
        var innerHigh = innerLow + innerSide - 1;

        var cells = new List<Vector2i>();
        for (var x = low; x <= high; x++)
        {
            for (var y = low; y <= high; y++)
            {
                if (x >= innerLow && x <= innerHigh && y >= innerLow && y <= innerHigh)
                    continue;

                cells.Add(new Vector2i(x, y));
            }
        }

        return cells;
    }

    /// <summary>
    /// Picks one or two technologies of the previous tier, the closest ones with a bit of randomness.
    /// </summary>
    private static List<string> PickPrerequisites(
        Vector2i position,
        List<(TechnologyPrototype Tech, Vector2i Position)> candidates,
        IRobustRandom random,
        float secondPrerequisiteChance)
    {
        var count = candidates.Count > 1 && random.Prob(secondPrerequisiteChance) ? 2 : 1;

        return candidates
            .OrderBy(c => DistanceSquared(position, c.Position) * (0.75f + random.NextFloat() * 0.5f))
            .Take(count)
            .Select(c => c.Tech.ID)
            .ToList();
    }

    private static float DistanceSquared(Vector2i a, Vector2i b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        return dx * dx + dy * dy;
    }
}
