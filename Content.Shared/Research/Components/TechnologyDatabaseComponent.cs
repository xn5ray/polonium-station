using Content.Shared._Polonium.Research; // Polonium: random research tree
using Content.Shared.Lathe;
using Content.Shared.Research.Prototypes;
using Content.Shared.Research.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Research.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedResearchSystem), typeof(SharedLatheSystem)), AutoGenerateComponentState]
public sealed partial class TechnologyDatabaseComponent : Component
{
    /// <summary>
    /// A main discipline that locks out other discipline technology past a certain tier.
    /// </summary>
    [DataField, AutoNetworkedField]
    public ProtoId<TechDisciplinePrototype>? MainDiscipline;

    [DataField, AutoNetworkedField]
    public List<string> CurrentTechnologyCards = new();

    /// <summary>
    /// Which research disciplines are able to be unlocked
    /// </summary>
    [AutoNetworkedField]
    [DataField]
    public List<ProtoId<TechDisciplinePrototype>> SupportedDisciplines = new();

    /// <summary>
    /// The ids of all the technologies which have been unlocked.
    /// </summary>
    [AutoNetworkedField]
    [DataField]
    public List<ProtoId<TechnologyPrototype>> UnlockedTechnologies = new();

    /// <summary>
    /// The ids of all the lathe recipes which have been unlocked.
    /// This is maintained alongside the TechnologyIds
    /// </summary>
    /// todo: if you unlock all the recipes in a tech, it doesn't count as unlocking the tech. sadge
    [AutoNetworkedField]
    [DataField]
    public List<ProtoId<LatheRecipePrototype>> UnlockedRecipes = new();

    // Polonium: random research tree
    /// <summary>
    /// Positions and prerequisites of the technologies in the random tree generated for the research server.
    /// Consoles get a copy when they are synchronized with the server, the clients get it in the console UI state,
    /// so it is not networked with the component. When it is empty, the tiers and prerequisites from the
    /// technology prototypes are used.
    /// </summary>
    public Dictionary<string, TechLayoutEntry> Layout = new();
    // Polonium end
}

/// <summary>
/// Event raised on the database whenever its
/// technologies or recipes are modified.
/// </summary>
/// <remarks>
/// This event is forwarded from the
/// server to all of it's clients.
/// </remarks>
[ByRefEvent]
public readonly record struct TechnologyDatabaseModifiedEvent(List<string>? NewlyUnlockedRecipes);

/// <summary>
/// Event raised on a database after being synchronized
/// with the values from another database.
/// </summary>
[ByRefEvent]
public readonly record struct TechnologyDatabaseSynchronizedEvent;
