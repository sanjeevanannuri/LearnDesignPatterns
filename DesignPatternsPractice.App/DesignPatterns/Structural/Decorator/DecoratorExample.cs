namespace DesignPatterns.Structural.Decorator
{
    // Component interface
    public interface ICoffee
    {
        string GetDescription();
        double GetCost();
    }

    // Concrete Component
    public class SimpleCoffee : ICoffee
    {
        public string GetDescription() => "Simple coffee";
        public double GetCost() => 2.0;
    }

    // Base Decorator
    public abstract class CoffeeDecorator : ICoffee
    {
        protected ICoffee coffee;

        public CoffeeDecorator(ICoffee coffee)
        {
            this.coffee = coffee;
        }

        public virtual string GetDescription() => coffee.GetDescription();
        public virtual double GetCost() => coffee.GetCost();
    }

    // Concrete Decorators
    public class MilkDecorator : CoffeeDecorator
    {
        public MilkDecorator(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() => coffee.GetDescription() + ", Milk";
        public override double GetCost() => coffee.GetCost() + 0.5;
    }

    public class SugarDecorator : CoffeeDecorator
    {
        public SugarDecorator(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() => coffee.GetDescription() + ", Sugar";
        public override double GetCost() => coffee.GetCost() + 0.2;
    }

    public class WhipDecorator : CoffeeDecorator
    {
        public WhipDecorator(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() => coffee.GetDescription() + ", Whip";
        public override double GetCost() => coffee.GetCost() + 0.7;
    }

    public class VanillaDecorator : CoffeeDecorator
    {
        public VanillaDecorator(ICoffee coffee) : base(coffee) { }

        public override string GetDescription() => coffee.GetDescription() + ", Vanilla";
        public override double GetCost() => coffee.GetCost() + 0.6;
    }
}
