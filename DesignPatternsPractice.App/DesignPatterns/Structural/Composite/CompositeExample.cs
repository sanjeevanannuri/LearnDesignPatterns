namespace DesignPatterns.Structural.Composite
{
    // Component interface
    public abstract class FileSystemComponent
    {
        protected string name;

        public FileSystemComponent(string name)
        {
            this.name = name;
        }

        public abstract void Display(int depth = 0);
        public abstract int GetSize();

        // Default implementations for composite operations
        public virtual void Add(FileSystemComponent component)
        {
            throw new NotSupportedException();
        }

        public virtual void Remove(FileSystemComponent component)
        {
            throw new NotSupportedException();
        }
    }

    // Leaf - represents files
    public class File : FileSystemComponent
    {
        private int size;

        public File(string name, int size) : base(name)
        {
            this.size = size;
        }

        public override void Display(int depth = 0)
        {
            Console.WriteLine(new string(' ', depth * 2) + $"📄 {name} ({size}KB)");
        }

        public override int GetSize()
        {
            return size;
        }
    }

    // Composite - represents directories
    public class Directory : FileSystemComponent
    {
        private List<FileSystemComponent> children = new List<FileSystemComponent>();

        public Directory(string name) : base(name) { }

        public override void Add(FileSystemComponent component)
        {
            children.Add(component);
        }

        public override void Remove(FileSystemComponent component)
        {
            children.Remove(component);
        }

        public override void Display(int depth = 0)
        {
            Console.WriteLine(new string(' ', depth * 2) + $"📁 {name}/");
            foreach (var child in children)
            {
                child.Display(depth + 1);
            }
        }

        public override int GetSize()
        {
            int totalSize = 0;
            foreach (var child in children)
            {
                totalSize += child.GetSize();
            }
            return totalSize;
        }
    }
}