namespace DesignPatterns.Creational.Prototype
{
    // Prototype interface
    public interface IPrototype
    {
        IPrototype Clone();
    }

    // Concrete Prototype
    public class Document : IPrototype
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Tags { get; set; }

        public Document(string title, string content)
        {
            Title = title;
            Content = content;
            Tags = new List<string>();
        }

        // Shallow clone
        public IPrototype Clone()
        {
            return (Document)this.MemberwiseClone();
        }

        // Deep clone
        public Document DeepClone()
        {
            var clone = (Document)this.MemberwiseClone();
            clone.Tags = new List<string>(this.Tags);
            return clone;
        }

        public override string ToString()
        {
            return $"Document: {Title}, Content: {Content}, Tags: [{string.Join(", ", Tags)}]";
        }
    }

    // Prototype Manager (Registry)
    public class DocumentManager
    {
        private Dictionary<string, Document> _prototypes = new Dictionary<string, Document>();

        public void RegisterPrototype(string key, Document prototype)
        {
            _prototypes[key] = prototype;
        }

        public Document CreateDocument(string key)
        {
            return _prototypes.ContainsKey(key) ? _prototypes[key].DeepClone() : null;
        }
    }
}