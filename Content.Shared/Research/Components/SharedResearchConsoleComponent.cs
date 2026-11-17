using Content.Shared._Polonium.Research; // Polonium: random research tree
using Robust.Shared.Serialization;

namespace Content.Shared.Research.Components
{
    [NetSerializable, Serializable]
    public enum ResearchConsoleUiKey : byte
    {
        Key,
    }

    [Serializable, NetSerializable]
    public sealed class ConsoleUnlockTechnologyMessage : BoundUserInterfaceMessage
    {
        public string Id;

        public ConsoleUnlockTechnologyMessage(string id)
        {
            Id = id;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ConsoleServerSelectionMessage : BoundUserInterfaceMessage
    {

    }

    [Serializable, NetSerializable]
    public sealed class ResearchConsoleBoundInterfaceState : BoundUserInterfaceState
    {
        public int Points;

        // Polonium: random research tree
        /// <summary>
        /// Availability of every technology in the tree.
        /// </summary>
        public Dictionary<string, ResearchAvailability> Researches;

        /// <summary>
        /// Position and prerequisites of every technology in the tree.
        /// </summary>
        public Dictionary<string, TechLayoutEntry> Layout;

        public ResearchConsoleBoundInterfaceState(
            int points,
            Dictionary<string, ResearchAvailability>? researches = null,
            Dictionary<string, TechLayoutEntry>? layout = null)
        {
            Points = points;
            Researches = researches ?? new();
            Layout = layout ?? new();
        }
        // Polonium end
    }
}
