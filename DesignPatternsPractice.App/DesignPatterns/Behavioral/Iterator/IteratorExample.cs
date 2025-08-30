namespace DesignPatterns.Behavioral.Iterator
{
    // Iterator interface
    public interface IIterator<T>
    {
        bool HasNext();
        T Next();
        void Reset();
    }

    // Aggregate interface
    public interface IIterable<T>
    {
        IIterator<T> CreateIterator();
    }

    // Concrete collection
    public class BookCollection : IIterable<Book>
    {
        private readonly List<Book> _books = new();

        public void AddBook(Book book)
        {
            _books.Add(book);
            Console.WriteLine($"Added book: {book.Title} by {book.Author}");
        }

        public void RemoveBook(Book book)
        {
            _books.Remove(book);
            Console.WriteLine($"Removed book: {book.Title}");
        }

        public int Count => _books.Count;

        public IIterator<Book> CreateIterator()
        {
            return new BookIterator(_books);
        }

        public IIterator<Book> CreateReverseIterator()
        {
            return new ReverseBookIterator(_books);
        }

        public IIterator<Book> CreateGenreIterator(string genre)
        {
            return new GenreBookIterator(_books, genre);
        }
    }

    public class Book
    {
        public string Title { get; }
        public string Author { get; }
        public string Genre { get; }
        public int Year { get; }

        public Book(string title, string author, string genre, int year)
        {
            Title = title;
            Author = author;
            Genre = genre;
            Year = year;
        }

        public override string ToString()
        {
            return $"{Title} by {Author} ({Genre}, {Year})";
        }
    }

    // Concrete iterators
    public class BookIterator : IIterator<Book>
    {
        private readonly List<Book> _books;
        private int _position = 0;

        public BookIterator(List<Book> books)
        {
            _books = books;
        }

        public bool HasNext()
        {
            return _position < _books.Count;
        }

        public Book Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("No more books to iterate");
            }
            return _books[_position++];
        }

        public void Reset()
        {
            _position = 0;
        }
    }

    public class ReverseBookIterator : IIterator<Book>
    {
        private readonly List<Book> _books;
        private int _position;

        public ReverseBookIterator(List<Book> books)
        {
            _books = books;
            _position = books.Count - 1;
        }

        public bool HasNext()
        {
            return _position >= 0;
        }

        public Book Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("No more books to iterate");
            }
            return _books[_position--];
        }

        public void Reset()
        {
            _position = _books.Count - 1;
        }
    }

    public class GenreBookIterator : IIterator<Book>
    {
        private readonly List<Book> _books;
        private readonly string _genre;
        private int _position = 0;

        public GenreBookIterator(List<Book> books, string genre)
        {
            _books = books;
            _genre = genre;
        }

        public bool HasNext()
        {
            while (_position < _books.Count)
            {
                if (_books[_position].Genre.Equals(_genre, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                _position++;
            }
            return false;
        }

        public Book Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException($"No more {_genre} books to iterate");
            }
            return _books[_position++];
        }

        public void Reset()
        {
            _position = 0;
        }
    }

    // Tree traversal example
    public class TreeNode<T>
    {
        public T Data { get; set; }
        public List<TreeNode<T>> Children { get; } = new();

        public TreeNode(T data)
        {
            Data = data;
        }

        public void AddChild(TreeNode<T> child)
        {
            Children.Add(child);
        }
    }

    public class Tree<T> : IIterable<T>
    {
        private TreeNode<T>? _root;

        public TreeNode<T>? Root
        {
            get => _root;
            set => _root = value;
        }

        public IIterator<T> CreateIterator()
        {
            return new DepthFirstIterator<T>(_root);
        }

        public IIterator<T> CreateBreadthFirstIterator()
        {
            return new BreadthFirstIterator<T>(_root);
        }
    }

    public class DepthFirstIterator<T> : IIterator<T>
    {
        private readonly Stack<TreeNode<T>> _stack = new();
        private readonly TreeNode<T>? _root;

        public DepthFirstIterator(TreeNode<T>? root)
        {
            _root = root;
            Reset();
        }

        public bool HasNext()
        {
            return _stack.Count > 0;
        }

        public T Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("No more nodes to iterate");
            }

            var node = _stack.Pop();
            
            // Add children in reverse order for left-to-right traversal
            for (int i = node.Children.Count - 1; i >= 0; i--)
            {
                _stack.Push(node.Children[i]);
            }

            return node.Data;
        }

        public void Reset()
        {
            _stack.Clear();
            if (_root != null)
            {
                _stack.Push(_root);
            }
        }
    }

    public class BreadthFirstIterator<T> : IIterator<T>
    {
        private readonly Queue<TreeNode<T>> _queue = new();
        private readonly TreeNode<T>? _root;

        public BreadthFirstIterator(TreeNode<T>? root)
        {
            _root = root;
            Reset();
        }

        public bool HasNext()
        {
            return _queue.Count > 0;
        }

        public T Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("No more nodes to iterate");
            }

            var node = _queue.Dequeue();
            
            // Add all children to queue
            foreach (var child in node.Children)
            {
                _queue.Enqueue(child);
            }

            return node.Data;
        }

        public void Reset()
        {
            _queue.Clear();
            if (_root != null)
            {
                _queue.Enqueue(_root);
            }
        }
    }

    // Social network example
    public class Profile
    {
        public string Name { get; }
        public string Email { get; }
        public List<Profile> Friends { get; } = new();

        public Profile(string name, string email)
        {
            Name = name;
            Email = email;
        }

        public void AddFriend(Profile friend)
        {
            if (!Friends.Contains(friend))
            {
                Friends.Add(friend);
                friend.Friends.Add(this); // Bi-directional friendship
            }
        }

        public override string ToString()
        {
            return $"{Name} ({Email})";
        }

        public override bool Equals(object? obj)
        {
            return obj is Profile profile && Email == profile.Email;
        }

        public override int GetHashCode()
        {
            return Email.GetHashCode();
        }
    }

    public class SocialNetwork : IIterable<Profile>
    {
        private readonly List<Profile> _profiles = new();

        public void AddProfile(Profile profile)
        {
            _profiles.Add(profile);
            Console.WriteLine($"Added profile: {profile}");
        }

        public IIterator<Profile> CreateIterator()
        {
            return new ProfileIterator(_profiles);
        }

        public IIterator<Profile> CreateFriendsIterator(Profile profile)
        {
            return new FriendsIterator(profile);
        }

        public IIterator<Profile> CreateFriendsOfFriendsIterator(Profile profile)
        {
            return new FriendsOfFriendsIterator(profile);
        }
    }

    public class ProfileIterator : IIterator<Profile>
    {
        private readonly List<Profile> _profiles;
        private int _position = 0;

        public ProfileIterator(List<Profile> profiles)
        {
            _profiles = profiles;
        }

        public bool HasNext()
        {
            return _position < _profiles.Count;
        }

        public Profile Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("No more profiles to iterate");
            }
            return _profiles[_position++];
        }

        public void Reset()
        {
            _position = 0;
        }
    }

    public class FriendsIterator : IIterator<Profile>
    {
        private readonly List<Profile> _friends;
        private int _position = 0;

        public FriendsIterator(Profile profile)
        {
            _friends = new List<Profile>(profile.Friends);
        }

        public bool HasNext()
        {
            return _position < _friends.Count;
        }

        public Profile Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("No more friends to iterate");
            }
            return _friends[_position++];
        }

        public void Reset()
        {
            _position = 0;
        }
    }

    public class FriendsOfFriendsIterator : IIterator<Profile>
    {
        private readonly HashSet<Profile> _visited = new();
        private readonly Queue<Profile> _queue = new();
        private readonly Profile _startProfile;

        public FriendsOfFriendsIterator(Profile startProfile)
        {
            _startProfile = startProfile;
            Reset();
        }

        public bool HasNext()
        {
            return _queue.Count > 0;
        }

        public Profile Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("No more friends of friends to iterate");
            }

            var profile = _queue.Dequeue();
            
            // Add friends of current profile to queue
            foreach (var friend in profile.Friends)
            {
                if (!_visited.Contains(friend))
                {
                    _visited.Add(friend);
                    _queue.Enqueue(friend);
                }
            }

            return profile;
        }

        public void Reset()
        {
            _visited.Clear();
            _queue.Clear();
            _visited.Add(_startProfile);

            // Start with direct friends
            foreach (var friend in _startProfile.Friends)
            {
                _visited.Add(friend);
                _queue.Enqueue(friend);
            }
        }
    }

    // Custom enumerable implementation using C# IEnumerable
    public class NumberSequence : IEnumerable<int>
    {
        private readonly int _start;
        private readonly int _end;
        private readonly int _step;

        public NumberSequence(int start, int end, int step = 1)
        {
            _start = start;
            _end = end;
            _step = step;
        }

        public IEnumerator<int> GetEnumerator()
        {
            for (int i = _start; i <= _end; i += _step)
            {
                yield return i;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class FibonacciSequence : IEnumerable<long>
    {
        private readonly int _count;

        public FibonacciSequence(int count)
        {
            _count = count;
        }

        public IEnumerator<long> GetEnumerator()
        {
            long a = 0, b = 1;
            for (int i = 0; i < _count; i++)
            {
                yield return a;
                (a, b) = (b, a + b);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
