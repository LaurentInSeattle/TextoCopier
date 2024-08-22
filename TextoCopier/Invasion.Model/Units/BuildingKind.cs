namespace Lyt.Invasion.Model.Units;

public enum Trait
{
    Economy,
    Military,
    Clergy,
    Education,
    Civics,
    Energy,
}

public enum BuildingKind
{
    None,

    // Cities 
    Campfire,
    Settlement,
    Hamlet,
    Village,
    Town,
    City,
    Capital,
    Conurbation,

    // Economy 
    Farm,
    Port,
    LumberCamp,
    Workshop,
    Blacksmith,
    Factory,
    Quarry,
    Mine,
    CoalField,
    OilRig,
    TradingPost,
    Market,

    // Military
    Camp,
    Fort,
    Castle,
    Barracks,
    Archery,
    AirBase,
    SpacePort,
    MissileSilo,

    // Clergy
    StoneCircle,
    Temple,
    Church,
    Cathedral,
    Mosque,
    ShintoShrine,
    ChinSweeTower,
    Mausoleum,
    Scientology,

    // Education
    Preschool,
    Library,
    School,
    College,
    University,
    Museum,
    TechnologyCampus, 
    ResearchCenter,
    ScienceAcademy,

    // Civics
    Daycare,
    OldCastle,
    Arcology,
    CityHall,
    Clinic,
    Hospital,
    Stadium,
    SportComplex,
    BusinessPark,
    Landfill,
    WaterTreatment,
    RecyclingCenter,
    ShoppingMall,

    // Energy 
    HydropowerDam,
    CoalPlant,
    Refinery,
    WindFarm,
    SolarField,
    NuclearFissionPlant,
    NuclearFusionPlant,
    GeothermalPlant,
    QuantumFluxGenerator,
}
