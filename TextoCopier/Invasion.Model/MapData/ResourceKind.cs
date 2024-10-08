﻿namespace Lyt.Invasion.Model.MapData;

public enum ResourceKind
{
    // Fossil, never renew
    Stone,
    Metal,
    Oil,
    RareEarths,

    // Bio, renew slowly, limited by region size
    Fish, 
    Game, 
    Fruit,
    Timber,

    // Natural, infinite
    Wind, 
    Sun,
}
