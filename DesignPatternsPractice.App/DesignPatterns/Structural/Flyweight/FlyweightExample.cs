namespace DesignPatterns.Structural.Flyweight
{
    // Flyweight interface
    public interface ICharacterFlyweight
    {
        void Display(int size, string color, string font);
    }

    // Concrete Flyweight - stores intrinsic state (shared)
    public class Character : ICharacterFlyweight
    {
        private readonly char _symbol; // Intrinsic state (shared)

        public Character(char symbol)
        {
            _symbol = symbol;
        }

        public void Display(int size, string color, string font)
        {
            // Extrinsic state (size, color, font) is passed as parameters
            Console.WriteLine($"Character '{_symbol}' - Size: {size}, Color: {color}, Font: {font}");
        }
    }

    // Flyweight Factory
    public class CharacterFactory
    {
        private readonly Dictionary<char, ICharacterFlyweight> _characters = new();

        public ICharacterFlyweight GetCharacter(char key)
        {
            if (!_characters.ContainsKey(key))
            {
                _characters[key] = new Character(key);
                Console.WriteLine($"Creating new flyweight for character '{key}'");
            }
            return _characters[key];
        }

        public int GetCreatedFlyweightsCount() => _characters.Count;
    }

    // Context - stores extrinsic state
    public class Document
    {
        private readonly List<DocumentCharacter> _characters = new();
        private readonly CharacterFactory _factory = new();

        public void AddCharacter(char symbol, int size, string color, string font)
        {
            var flyweight = _factory.GetCharacter(symbol);
            _characters.Add(new DocumentCharacter(flyweight, size, color, font));
        }

        public void Display()
        {
            Console.WriteLine("\nDocument content:");
            foreach (var character in _characters)
            {
                character.Display();
            }
            Console.WriteLine($"\nTotal characters in document: {_characters.Count}");
            Console.WriteLine($"Flyweight objects created: {_factory.GetCreatedFlyweightsCount()}");
        }
    }

    // Context helper class
    public class DocumentCharacter
    {
        private readonly ICharacterFlyweight _flyweight;
        private readonly int _size;
        private readonly string _color;
        private readonly string _font;

        public DocumentCharacter(ICharacterFlyweight flyweight, int size, string color, string font)
        {
            _flyweight = flyweight;
            _size = size;
            _color = color;
            _font = font;
        }

        public void Display()
        {
            _flyweight.Display(_size, _color, _font);
        }
    }

    // Real-world example: Tree Forest
    public interface ITreeType
    {
        void Render(int x, int y, string season);
    }

    public class TreeType : ITreeType
    {
        private readonly string _name;
        private readonly string _sprite;

        public TreeType(string name, string sprite)
        {
            _name = name;
            _sprite = sprite;
        }

        public void Render(int x, int y, string season)
        {
            Console.WriteLine($"Rendering {_name} tree at ({x}, {y}) in {season} with sprite: {_sprite}");
        }
    }

    public class TreeTypeFactory
    {
        private static readonly Dictionary<string, ITreeType> _treeTypes = new();

        public static ITreeType GetTreeType(string name, string sprite)
        {
            var key = $"{name}_{sprite}";
            if (!_treeTypes.ContainsKey(key))
            {
                _treeTypes[key] = new TreeType(name, sprite);
                Console.WriteLine($"Creating new tree type: {name}");
            }
            return _treeTypes[key];
        }

        public static int GetCreatedTreeTypesCount() => _treeTypes.Count;
    }

    public class Tree
    {
        private readonly int _x, _y;
        private readonly ITreeType _type;

        public Tree(int x, int y, ITreeType type)
        {
            _x = x;
            _y = y;
            _type = type;
        }

        public void Render(string season)
        {
            _type.Render(_x, _y, season);
        }
    }

    public class Forest
    {
        private readonly List<Tree> _trees = new();

        public void PlantTree(int x, int y, string name, string sprite)
        {
            var type = TreeTypeFactory.GetTreeType(name, sprite);
            _trees.Add(new Tree(x, y, type));
        }

        public void Render(string season)
        {
            Console.WriteLine($"\nRendering forest in {season}:");
            foreach (var tree in _trees)
            {
                tree.Render(season);
            }
            Console.WriteLine($"Total trees: {_trees.Count}");
            Console.WriteLine($"Tree types created: {TreeTypeFactory.GetCreatedTreeTypesCount()}");
        }
    }
}
