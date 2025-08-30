namespace DesignPatterns.Creational.Builder
{
    // Product
    public class House
    {
        public string Walls { get; set; }
        public string Roof { get; set; }
        public string Door { get; set; }
        public override string ToString() => $"House with {Walls}, {Roof}, {Door}";
    }

    // Builder interface
    public interface IHouseBuilder
    {
        void BuildWalls();
        void BuildRoof();
        void BuildDoor();
        House GetResult();
    }

    // Concrete Builder
    public class WoodenHouseBuilder : IHouseBuilder
    {
        private House _house = new House();
        public void BuildWalls() => _house.Walls = "wooden walls";
        public void BuildRoof() => _house.Roof = "wooden roof";
        public void BuildDoor() => _house.Door = "wooden door";
        public House GetResult() => _house;
    }

    // Director
    public class Director
    {
        public void Construct(IHouseBuilder builder)
        {
            builder.BuildWalls();
            builder.BuildRoof();
            builder.BuildDoor();
        }
    }
}
