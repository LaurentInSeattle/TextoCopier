namespace Lyt.TextoCopier.Model
{
    public sealed class Template
    {
        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public bool IsLink { get; set; } 

        public bool ShouldHide { get; set; }
    }
}
