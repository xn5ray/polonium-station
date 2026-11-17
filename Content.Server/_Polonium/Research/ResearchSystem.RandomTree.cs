using System.Linq;
using Content.Shared._Polonium.Research;
using Content.Shared.CCVar;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Robust.Shared.Configuration;
using Robust.Shared.Random;

namespace Content.Server.Research.Systems;

/// <summary>
/// Gives every research server its own random technology tree, generated when the server is created.
/// </summary>
public sealed partial class ResearchSystem
{
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private IRobustRandom _random = default!;

    private void InitializeRandomTree()
    {
        SubscribeLocalEvent<ResearchServerComponent, MapInitEvent>(OnRandomTreeServerMapInit);
    }

    private void OnRandomTreeServerMapInit(Entity<ResearchServerComponent> ent, ref MapInitEvent args)
    {
        if (!TryComp<TechnologyDatabaseComponent>(ent, out var database))
            return;

        GenerateTechnologyLayout(ent, database);
    }

    /// <summary>
    /// Generates a new random tree for the technologies of the disciplines supported by the database.
    /// Hidden technologies are not part of the tree.
    /// </summary>
    public void GenerateTechnologyLayout(EntityUid uid, TechnologyDatabaseComponent database)
    {
        var technologies = ProtoMan.EnumeratePrototypes<TechnologyPrototype>()
            .Where(t => !t.Hidden && database.SupportedDisciplines.Contains(t.Discipline));

        database.Layout = ResearchTreeGenerator.Generate(
            technologies,
            _random,
            _cfg.GetCVar(CCVars.ResearchTreeSecondPrerequisiteChance));
    }
}
