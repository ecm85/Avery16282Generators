namespace Avery16282Generator.Brewcrafters
{
    public class Beer
    {
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public int Points { get; set; }
        public bool Barrel { get; set; }
        public bool Hops { get; set; }
        public string GoldLabelImageName { get; set; }

        public string FullName
        {
            get
            {
                var name = Name1;
                if (!string.IsNullOrWhiteSpace(Name2))
                    name += " " + Name2;
                if (!string.IsNullOrWhiteSpace(Name3))
                    name += " " + Name3;
                return name;
            }
        }
    }
}
