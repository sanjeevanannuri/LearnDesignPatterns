namespace DesignPatterns.Creational.FactoryMethod
{
    // Product interface
    public interface IProduct
    {
        string Operation();
    }

    // Concrete Products
    public class ConcreteProductA : IProduct
    {
        public string Operation() => "Result of ConcreteProductA";
    }

    public class ConcreteProductB : IProduct
    {
        public string Operation() => "Result of ConcreteProductB";
    }

    // Creator abstract class
    public abstract class Creator
    {
        public abstract IProduct FactoryMethod();
    }

    // Concrete Creators
    public class ConcreteCreatorA : Creator
    {
        public override IProduct FactoryMethod() => new ConcreteProductA();
    }

    public class ConcreteCreatorB : Creator
    {
        public override IProduct FactoryMethod() => new ConcreteProductB();
    }
}
