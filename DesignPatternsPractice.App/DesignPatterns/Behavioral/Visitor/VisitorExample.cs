namespace DesignPatterns.Behavioral.Visitor
{
    // Visitor interface
    public interface IShapeVisitor
    {
        void Visit(Circle circle);
        void Visit(Rectangle rectangle);
        void Visit(Triangle triangle);
    }

    // Element interface
    public interface IShape
    {
        void Accept(IShapeVisitor visitor);
    }

    // Concrete Elements
    public class Circle : IShape
    {
        public double Radius { get; }

        public Circle(double radius)
        {
            Radius = radius;
        }

        public void Accept(IShapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class Rectangle : IShape
    {
        public double Width { get; }
        public double Height { get; }

        public Rectangle(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public void Accept(IShapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class Triangle : IShape
    {
        public double Base { get; }
        public double Height { get; }

        public Triangle(double baseLength, double height)
        {
            Base = baseLength;
            Height = height;
        }

        public void Accept(IShapeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // Concrete Visitors
    public class AreaCalculator : IShapeVisitor
    {
        public double TotalArea { get; private set; }

        public void Visit(Circle circle)
        {
            double area = Math.PI * circle.Radius * circle.Radius;
            Console.WriteLine($"Circle area: {area:F2}");
            TotalArea += area;
        }

        public void Visit(Rectangle rectangle)
        {
            double area = rectangle.Width * rectangle.Height;
            Console.WriteLine($"Rectangle area: {area:F2}");
            TotalArea += area;
        }

        public void Visit(Triangle triangle)
        {
            double area = 0.5 * triangle.Base * triangle.Height;
            Console.WriteLine($"Triangle area: {area:F2}");
            TotalArea += area;
        }
    }

    public class PerimeterCalculator : IShapeVisitor
    {
        public double TotalPerimeter { get; private set; }

        public void Visit(Circle circle)
        {
            double perimeter = 2 * Math.PI * circle.Radius;
            Console.WriteLine($"Circle perimeter: {perimeter:F2}");
            TotalPerimeter += perimeter;
        }

        public void Visit(Rectangle rectangle)
        {
            double perimeter = 2 * (rectangle.Width + rectangle.Height);
            Console.WriteLine($"Rectangle perimeter: {perimeter:F2}");
            TotalPerimeter += perimeter;
        }

        public void Visit(Triangle triangle)
        {
            // Assuming equilateral triangle for simplicity
            double side = triangle.Base;
            double perimeter = 3 * side;
            Console.WriteLine($"Triangle perimeter: {perimeter:F2}");
            TotalPerimeter += perimeter;
        }
    }

    public class ShapeRenderer : IShapeVisitor
    {
        public void Visit(Circle circle)
        {
            Console.WriteLine($"Rendering Circle with radius {circle.Radius} using SVG");
        }

        public void Visit(Rectangle rectangle)
        {
            Console.WriteLine($"Rendering Rectangle {rectangle.Width}x{rectangle.Height} using Canvas API");
        }

        public void Visit(Triangle triangle)
        {
            Console.WriteLine($"Rendering Triangle with base {triangle.Base} and height {triangle.Height} using WebGL");
        }
    }

    // File system example
    public interface IFileSystemVisitor
    {
        void Visit(File file);
        void Visit(Directory directory);
    }

    public interface IFileSystemElement
    {
        void Accept(IFileSystemVisitor visitor);
        string Name { get; }
    }

    public class File : IFileSystemElement
    {
        public string Name { get; }
        public long Size { get; }
        public string Extension { get; }

        public File(string name, long size)
        {
            Name = name;
            Size = size;
            Extension = Path.GetExtension(name);
        }

        public void Accept(IFileSystemVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class Directory : IFileSystemElement
    {
        public string Name { get; }
        public List<IFileSystemElement> Children { get; }

        public Directory(string name)
        {
            Name = name;
            Children = new List<IFileSystemElement>();
        }

        public void Add(IFileSystemElement element)
        {
            Children.Add(element);
        }

        public void Accept(IFileSystemVisitor visitor)
        {
            visitor.Visit(this);
            foreach (var child in Children)
            {
                child.Accept(visitor);
            }
        }
    }

    public class SizeCalculator : IFileSystemVisitor
    {
        public long TotalSize { get; private set; }

        public void Visit(File file)
        {
            Console.WriteLine($"📄 {file.Name} ({file.Size} bytes)");
            TotalSize += file.Size;
        }

        public void Visit(Directory directory)
        {
            Console.WriteLine($"📁 {directory.Name}/");
        }
    }

    public class FileCounter : IFileSystemVisitor
    {
        public int FileCount { get; private set; }
        public int DirectoryCount { get; private set; }
        public Dictionary<string, int> ExtensionCount { get; private set; } = new();

        public void Visit(File file)
        {
            FileCount++;
            if (!string.IsNullOrEmpty(file.Extension))
            {
                ExtensionCount[file.Extension] = ExtensionCount.GetValueOrDefault(file.Extension, 0) + 1;
            }
        }

        public void Visit(Directory directory)
        {
            DirectoryCount++;
        }

        public void PrintStatistics()
        {
            Console.WriteLine($"Total files: {FileCount}");
            Console.WriteLine($"Total directories: {DirectoryCount}");
            Console.WriteLine("File types:");
            foreach (var kvp in ExtensionCount)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value} files");
            }
        }
    }

    public class SearchVisitor : IFileSystemVisitor
    {
        private readonly string _searchTerm;
        public List<string> FoundItems { get; private set; } = new();

        public SearchVisitor(string searchTerm)
        {
            _searchTerm = searchTerm.ToLower();
        }

        public void Visit(File file)
        {
            if (file.Name.ToLower().Contains(_searchTerm))
            {
                FoundItems.Add($"File: {file.Name}");
                Console.WriteLine($"Found file: {file.Name}");
            }
        }

        public void Visit(Directory directory)
        {
            if (directory.Name.ToLower().Contains(_searchTerm))
            {
                FoundItems.Add($"Directory: {directory.Name}");
                Console.WriteLine($"Found directory: {directory.Name}");
            }
        }
    }

    // Expression tree example
    public interface IExpressionVisitor
    {
        double Visit(Number number);
        double Visit(Addition addition);
        double Visit(Subtraction subtraction);
        double Visit(Multiplication multiplication);
        double Visit(Division division);
    }

    public interface IExpression
    {
        double Accept(IExpressionVisitor visitor);
    }

    public class Number : IExpression
    {
        public double Value { get; }

        public Number(double value)
        {
            Value = value;
        }

        public double Accept(IExpressionVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class Addition : IExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }

        public Addition(IExpression left, IExpression right)
        {
            Left = left;
            Right = right;
        }

        public double Accept(IExpressionVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class Subtraction : IExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }

        public Subtraction(IExpression left, IExpression right)
        {
            Left = left;
            Right = right;
        }

        public double Accept(IExpressionVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class Multiplication : IExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }

        public Multiplication(IExpression left, IExpression right)
        {
            Left = left;
            Right = right;
        }

        public double Accept(IExpressionVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class Division : IExpression
    {
        public IExpression Left { get; }
        public IExpression Right { get; }

        public Division(IExpression left, IExpression right)
        {
            Left = left;
            Right = right;
        }

        public double Accept(IExpressionVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class EvaluationVisitor : IExpressionVisitor
    {
        public double Visit(Number number)
        {
            return number.Value;
        }

        public double Visit(Addition addition)
        {
            return addition.Left.Accept(this) + addition.Right.Accept(this);
        }

        public double Visit(Subtraction subtraction)
        {
            return subtraction.Left.Accept(this) - subtraction.Right.Accept(this);
        }

        public double Visit(Multiplication multiplication)
        {
            return multiplication.Left.Accept(this) * multiplication.Right.Accept(this);
        }

        public double Visit(Division division)
        {
            return division.Left.Accept(this) / division.Right.Accept(this);
        }
    }

    public class PrintVisitor : IExpressionVisitor
    {
        public double Visit(Number number)
        {
            Console.Write(number.Value);
            return number.Value;
        }

        public double Visit(Addition addition)
        {
            Console.Write("(");
            addition.Left.Accept(this);
            Console.Write(" + ");
            addition.Right.Accept(this);
            Console.Write(")");
            return 0; // Not used for printing
        }

        public double Visit(Subtraction subtraction)
        {
            Console.Write("(");
            subtraction.Left.Accept(this);
            Console.Write(" - ");
            subtraction.Right.Accept(this);
            Console.Write(")");
            return 0;
        }

        public double Visit(Multiplication multiplication)
        {
            Console.Write("(");
            multiplication.Left.Accept(this);
            Console.Write(" * ");
            multiplication.Right.Accept(this);
            Console.Write(")");
            return 0;
        }

        public double Visit(Division division)
        {
            Console.Write("(");
            division.Left.Accept(this);
            Console.Write(" / ");
            division.Right.Accept(this);
            Console.Write(")");
            return 0;
        }
    }
}
