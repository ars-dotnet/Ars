namespace MyApiWithIdentityServer4
{
    public interface IConfig
    {
        int Age { get; }
        AnimalConfig AnimalConfig { get; }
    }

    public class Config : IConfig
    {
        public int Age { get; set; }

        public AnimalConfig AnimalConfig { get; set; }
    }

    public interface IAnimalConfig
    {
        string Name { get; }
    }

    public class AnimalConfig : IAnimalConfig
    {
        public string Name { get; set; }
    }
}
