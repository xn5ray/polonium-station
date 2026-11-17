using System.Linq;
using Robust.Shared.Serialization;

namespace Content.Shared._Polonium.Research;

/// <summary>
/// Position and prerequisites of a single technology in the tree generated for a research server.
/// They replace the static <c>technologyPrerequisites</c> of the technology prototype.
/// </summary>
[Serializable, NetSerializable]
public sealed class TechLayoutEntry : IEquatable<TechLayoutEntry>
{
    /// <summary>
    /// Position on the console grid. Tier 1 fills the middle and every next tier is a ring around the previous one.
    /// </summary>
    public readonly Vector2i Position;

    /// <summary>
    /// Ids of the technologies that have to be researched first.
    /// </summary>
    public readonly List<string> Prerequisites;

    public TechLayoutEntry(Vector2i position, List<string> prerequisites)
    {
        Position = position;
        Prerequisites = prerequisites;
    }

    public bool Equals(TechLayoutEntry? other)
    {
        return other != null && Position == other.Position && Prerequisites.SequenceEqual(other.Prerequisites);
    }

    public override bool Equals(object? obj)
    {
        return obj is TechLayoutEntry other && Equals(other);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Position);
        foreach (var prerequisite in Prerequisites)
        {
            hash.Add(prerequisite);
        }

        return hash.ToHashCode();
    }
}

/// <summary>
/// Defines the visual style of the lines between a technology and its prerequisites in the research console.
/// </summary>
public enum PrerequisiteLineType : byte
{
    /// <summary>
    /// Clean L-shaped connections.
    /// </summary>
    LShape = 0,

    /// <summary>
    /// Direct diagonal lines.
    /// </summary>
    Diagonal = 1,

    /// <summary>
    /// Tree-like branching connections.
    /// </summary>
    Tree = 2,

    /// <summary>
    /// Spread connections that avoid overlaps by using offset routing paths.
    /// </summary>
    Spread = 3,
}
