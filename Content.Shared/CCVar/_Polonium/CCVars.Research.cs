using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    /// Chance (0 to 1) that a technology in the random research tree requires two technologies
    /// of the previous tier instead of one.
    /// </summary>
    public static readonly CVarDef<float> ResearchTreeSecondPrerequisiteChance =
        CVarDef.Create("research.tree_second_prerequisite_chance", 0.4f, CVar.SERVERONLY);
}
