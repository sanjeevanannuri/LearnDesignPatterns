namespace DesignPatterns.Creational.AbstractFactory
{
    // Abstract Products
    public interface IButton { string Paint(); }
    public interface ICheckbox { string Paint(); }

    // Concrete Products (Windows)
    public class WindowsButton : IButton { public string Paint() => "Render a button in Windows style."; }
    public class WindowsCheckbox : ICheckbox { public string Paint() => "Render a checkbox in Windows style."; }

    // Concrete Products (Mac)
    public class MacButton : IButton { public string Paint() => "Render a button in Mac style."; }
    public class MacCheckbox : ICheckbox { public string Paint() => "Render a checkbox in Mac style."; }

    // Abstract Factory
    public interface IGUIFactory
    {
        IButton CreateButton();
        ICheckbox CreateCheckbox();
    }

    // Concrete Factories
    public class WindowsFactory : IGUIFactory
    {
        public IButton CreateButton() => new WindowsButton();
        public ICheckbox CreateCheckbox() => new WindowsCheckbox();
    }

    public class MacFactory : IGUIFactory
    {
        public IButton CreateButton() => new MacButton();
        public ICheckbox CreateCheckbox() => new MacCheckbox();
    }
}
