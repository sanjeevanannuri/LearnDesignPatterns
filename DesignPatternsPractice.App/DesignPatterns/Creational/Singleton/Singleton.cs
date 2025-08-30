namespace DesignPatterns.Creational.Singleton
{
    // Simple thread-safe Singleton implementation
    public sealed class Singleton
    {
        private static readonly Singleton _instance = new Singleton();
        public string Message { get; set; } = "Hello from Singleton!";

        // Private constructor ensures no external instantiation
        private Singleton() { }

        public static Singleton Instance => _instance;
    }
}
