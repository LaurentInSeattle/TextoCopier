namespace Lyt.Invasion.Model.MapData
{
    /// <summary> Resources for a region </summary>
    public sealed class Resources
    {
        public sealed class ResourcesLimit
        {
            public float Initial { get; set; }
            public float Max { get; set; }
            public float RenewRate { get; set; }
        }

        public static Dictionary<ResourceKind, ResourcesLimit> Limits = new(8)
        {
            // Fossil, never renew, min and max == 0.0f, useless
            { ResourceKind.Stone, new ResourcesLimit { Initial = 10_000.0f , RenewRate = 0.0f } },
            { ResourceKind.Metal, new ResourcesLimit { Initial = 5_000.0f  , RenewRate = 0.0f } },
            { ResourceKind.Oil, new ResourcesLimit { Initial = 2_000.0f    , RenewRate = 0.0f } },
            { ResourceKind.Lithium, new ResourcesLimit { Initial = 1_000.0f, RenewRate = 0.0f } },

            // Natural, infinite, never renew 
            { ResourceKind.Wind, new ResourcesLimit { Initial = 2_000.0f , RenewRate = float.PositiveInfinity } },
            { ResourceKind.Sun, new ResourcesLimit { Initial = 2_000.0f, RenewRate = float.PositiveInfinity } },

            // Biological resources, renew slowly, limited by region size (use max), can go extinct.
            { ResourceKind.Fish, new ResourcesLimit { Initial = 2_000.0f   , Max = 4_000.0f, RenewRate = 0.15f } },
            { ResourceKind.Game, new ResourcesLimit { Initial = 100.0f    , Max = 500.0f, RenewRate = 0.07f } },
            { ResourceKind.Fruit, new ResourcesLimit { Initial = 500.0f   , Max = 2_000.0f, RenewRate = 0.10f } },
            { ResourceKind.Timber, new ResourcesLimit { Initial = 20_000.0f, Max = 50_000.0f, RenewRate = 0.05f} },
        };

        public const float VeryHighMultiplier = 1.3f;
        public const float HighMultiplier = 1.15f;
        public const float NormalMultiplier = 1.0f;
        public const float LowMultiplier = 0.85f;
        public const float VeryLowMultiplier = 0.7f;

        public Dictionary<ResourceKind, float> resources { get; private set; }

        public Resources(Region region)
        {
            this.resources = new Dictionary<ResourceKind, float>();
            this.AllocateInitialResources(region);
        }

        public void Renew(Region region)
        {
            switch (region.Ecosystem)
            {
                default:
                case Ecosystem.Unknown:
                    throw new Exception("Unknown ecosystem");

                case Ecosystem.Mountain:
                case Ecosystem.Ocean:
                case Ecosystem.Desert:
                    // Nothing renewable 
                    break;

                case Ecosystem.Forest:
                    break;
                case Ecosystem.Grassland:
                    break;
                case Ecosystem.Hills:
                    break;
                case Ecosystem.Wetland:
                    break;
                case Ecosystem.Coast:
                    break;
            }
        }

        public void AllocateInitialResources(Region region)
        {
            void AddInitialResource(ResourceKind kind, float multiplier = Resources.NormalMultiplier)
            {
                // Varies from 0.95f to 1.05f => + or - 5% 
                float randomize = 1.0f + ((region.Game.Random.NextSingle() - 0.5f) / 10.0f);
                var limits = Resources.Limits[kind];
                this.resources.Add(kind, limits.Initial * multiplier * randomize);
            }

            this.resources.Clear();
            switch (region.Ecosystem)
            {
                default:
                case Ecosystem.Unknown:
                    throw new Exception("Unknown ecosystem");

                case Ecosystem.Mountain:
                case Ecosystem.Ocean:
                    // Absolutely Nothing there, since these regions cannot be owned
                    break;

                case Ecosystem.Forest:
                    // Game, Fruit,  Timber:  High ,
                    AddInitialResource(ResourceKind.Game, Resources.VeryHighMultiplier);
                    AddInitialResource(ResourceKind.Fruit, Resources.VeryHighMultiplier);
                    AddInitialResource(ResourceKind.Timber, Resources.VeryHighMultiplier);
                    break;

                case Ecosystem.Grassland:
                    // Game,     Fruit Low   Wind High , Sun Low 
                    AddInitialResource(ResourceKind.Game);
                    AddInitialResource(ResourceKind.Fruit, Resources.LowMultiplier);
                    AddInitialResource(ResourceKind.Timber, Resources.VeryLowMultiplier);
                    AddInitialResource(ResourceKind.Wind, Resources.HighMultiplier);
                    AddInitialResource(ResourceKind.Sun, Resources.LowMultiplier);
                    AddInitialResource(ResourceKind.Metal, Resources.LowMultiplier);
                    break;

                case Ecosystem.Desert:
                    // Oil High  Lithium  Sun High
                    AddInitialResource(ResourceKind.Oil, Resources.VeryHighMultiplier);
                    AddInitialResource(ResourceKind.Sun, Resources.VeryHighMultiplier);
                    AddInitialResource(ResourceKind.Lithium, Resources.NormalMultiplier);
                    AddInitialResource(ResourceKind.Stone, Resources.LowMultiplier);
                    AddInitialResource(ResourceKind.Metal, Resources.LowMultiplier);
                    break;

                case Ecosystem.Hills:
                    // Timber Low , Stone , Metal , Fruit Low , Sun 
                    AddInitialResource(ResourceKind.Timber, Resources.LowMultiplier);
                    AddInitialResource(ResourceKind.Stone, Resources.HighMultiplier);
                    AddInitialResource(ResourceKind.Metal, Resources.HighMultiplier);
                    AddInitialResource(ResourceKind.Fruit, Resources.LowMultiplier);
                    AddInitialResource(ResourceKind.Game, Resources.LowMultiplier);
                    break;

                case Ecosystem.Wetland:
                    // Fish , Game , Oil Low  , Wind Low , Sun Low 
                    AddInitialResource(ResourceKind.Fish, Resources.VeryHighMultiplier);
                    AddInitialResource(ResourceKind.Game, Resources.LowMultiplier);
                    AddInitialResource(ResourceKind.Timber, Resources.VeryLowMultiplier);
                    AddInitialResource(ResourceKind.Oil, Resources.LowMultiplier);
                    AddInitialResource(ResourceKind.Sun, Resources.LowMultiplier);
                    AddInitialResource(ResourceKind.Wind, Resources.HighMultiplier);
                    break;

                case Ecosystem.Coast:
                    // Stone , Fish  , Timber low , Wind , Sun 
                    AddInitialResource(ResourceKind.Fish, Resources.VeryHighMultiplier);
                    AddInitialResource(ResourceKind.Game, Resources.LowMultiplier);
                    AddInitialResource(ResourceKind.Timber, Resources.VeryLowMultiplier);
                    AddInitialResource(ResourceKind.Sun, Resources.NormalMultiplier);
                    AddInitialResource(ResourceKind.Wind, Resources.HighMultiplier);
                    AddInitialResource(ResourceKind.Stone, Resources.LowMultiplier);
                    break;
            }
        }
    }
}
