namespace Lyt.Invasion.Model.GameControl;

public enum WinCondition
{
    None = 0,

    Conquest,       // X % of regions conquered
    LastStanding,   // All other players have lost or resigned 
    Landmarks,      // Build 3 special buildings, keep them all for three turns 
    Prestige,       // Aura and Knowledge High Score
    Singularity,    // Reached the final age for a complete turn 
}

public enum LostCondition
{
    None = 0,

    Resigned, 

    LostCapital,         // The region of the player's capital has been invaded 
    Underdevelopment,    // Three ages behind any other player
    Bankruptcy,          // Player cannot pay building and units upkeep costs
    Poisoning,           // Health too low / Pollution too high 
}
