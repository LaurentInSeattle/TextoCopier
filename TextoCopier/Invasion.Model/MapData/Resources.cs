namespace Lyt.Invasion.Model.MapData
{
    /// <summary> Resources for a region </summary>
    public sealed class Resources
    {
        public Dictionary<ResourceKind, int> resources { get; private set; }

        public Resources()
        {
            this.resources = new Dictionary<ResourceKind, int>();
        }

        public void Renew (Region region)
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

        private void AllocateInitialResources(Region region) 
        {
            /*
    Forest,     // Farms    Game,     Fruit,  Timber High ,

    Grassland,  // Farms    Game,     Fruit Low   Wind High , Sun Low 

    Desert,     // Oil High  Lithium  Sun High

    Mountain,   // ---

    Hills,      // Timber Low , Stone , Metal , Fruit Low , Sun 

    Ocean,      // ---
                
    Wetland,    // Fish , Game , Oil Low  , Wind Low , Sun Low 

    Coast,      // Fish  , Timber low , Wind , Sun 
            */
            this.resources.Clear();
            switch (region.Ecosystem)
            {
                default:
                case Ecosystem.Unknown:
                    throw new Exception("Unknown ecosystem");

                case Ecosystem.Forest:
                    break;
                case Ecosystem.Grassland:
                    break;
                case Ecosystem.Desert:
                    break;
                case Ecosystem.Mountain:
                    break;
                case Ecosystem.Hills:
                    break;
                case Ecosystem.Ocean:
                    break;
                case Ecosystem.Wetland:
                    break;
                case Ecosystem.Coast:
                    break;
            }
        }
    }
}
