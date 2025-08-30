# Iterator Pattern

## Overview
The Iterator pattern provides a way to access elements of a collection sequentially without exposing the underlying representation. Think of it like a **TV remote control** - you can navigate through channels one by one without knowing how the TV stores the channel list internally, or like a **book bookmark** that helps you read page by page without needing to understand how the book is bound.

## Problem It Solves
Imagine you have different types of collections (arrays, lists, trees, graphs) and you want to traverse them:
- Each collection has a different internal structure
- You want to provide a uniform way to access all elements
- You need different traversal algorithms (forward, backward, filtered)
- You want to support multiple simultaneous traversals of the same collection

Without Iterator pattern, you'd expose internal structure:
```csharp
// BAD: Exposing internal representation
public class BookCollection
{
    private List<Book> _books; // Internal structure exposed

    public List<Book> GetBooks() => _books; // Breaks encapsulation
}

// Client code becomes dependent on internal structure
foreach (var book in collection.GetBooks()) // Coupled to List
{
    // Process book
}
```

This creates tight coupling and makes it hard to change the internal representation.

## Real-World Analogy
Think of a **library browsing system**:
1. **Books on Shelves** (Collection): Books are stored in a specific order on shelves
2. **Library Card Catalog** (Iterator): You use the catalog to find books without walking through every shelf
3. **Librarian** (Iterator Interface): Provides a standard way to search through different sections
4. **Search Methods**: You can browse alphabetically, by genre, by author, etc.

Or consider a **music playlist**:
- **Playlist** (Collection): Contains songs in some internal structure
- **Media Player** (Iterator): Navigates through songs (next, previous, shuffle, repeat)
- **Different Modes**: Sequential, random, filtered by genre
- **Multiple Listeners**: Multiple people can have different positions in the same playlist

## Implementation Details

### Basic Structure
```csharp
// Iterator interface
public interface IIterator<T>
{
    bool HasNext();
    T Next();
    void Reset();
}

// Aggregate interface
public interface IAggregate<T>
{
    IIterator<T> CreateIterator();
}

// Concrete Iterator
public class ConcreteIterator<T> : IIterator<T>
{
    private List<T> _collection;
    private int _position = 0;

    public ConcreteIterator(List<T> collection)
    {
        _collection = collection;
    }

    public bool HasNext()
    {
        return _position < _collection.Count;
    }

    public T Next()
    {
        if (!HasNext())
            throw new InvalidOperationException("No more elements");
        
        return _collection[_position++];
    }

    public void Reset()
    {
        _position = 0;
    }
}

// Concrete Aggregate
public class ConcreteAggregate<T> : IAggregate<T>
{
    private List<T> _items;

    public ConcreteAggregate()
    {
        _items = new List<T>();
    }

    public void Add(T item)
    {
        _items.Add(item);
    }

    public IIterator<T> CreateIterator()
    {
        return new ConcreteIterator<T>(_items);
    }
}
```

### Key Components
1. **Iterator**: Interface for accessing and traversing elements
2. **ConcreteIterator**: Implements the Iterator interface
3. **Aggregate**: Interface for creating iterators
4. **ConcreteAggregate**: Implements the Aggregate interface

## Example from Our Code
```csharp
// Iterator interface with additional functionality
public interface IIterator<T>
{
    bool HasNext();
    T Next();
    void Reset();
    T Current { get; }
    bool HasPrevious();
    T Previous();
}

// Book entity
public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public int Pages { get; set; }
    public DateTime PublishedDate { get; set; }

    public Book(string title, string author, string genre, int pages, DateTime publishedDate)
    {
        Title = title;
        Author = author;
        Genre = genre;
        Pages = pages;
        PublishedDate = publishedDate;
    }

    public override string ToString()
    {
        return $"'{Title}' by {Author} ({Genre}, {Pages} pages, {PublishedDate.Year})";
    }
}

// Custom collection interface
public interface IBookCollection
{
    void AddBook(Book book);
    void RemoveBook(Book book);
    int Count { get; }
    IIterator<Book> CreateIterator();
    IIterator<Book> CreateReverseIterator();
    IIterator<Book> CreateFilteredIterator(Func<Book, bool> predicate);
}

// Forward iterator implementation
public class BookIterator : IIterator<Book>
{
    private readonly List<Book> _books;
    private int _position;

    public BookIterator(List<Book> books)
    {
        _books = books;
        _position = -1;
    }

    public Book Current
    {
        get
        {
            if (_position < 0 || _position >= _books.Count)
                throw new InvalidOperationException("Iterator is not positioned on a valid element");
            return _books[_position];
        }
    }

    public bool HasNext()
    {
        return _position + 1 < _books.Count;
    }

    public Book Next()
    {
        if (!HasNext())
            throw new InvalidOperationException("No more elements");

        _position++;
        Console.WriteLine($"📖 Moving to next: {Current}");
        return Current;
    }

    public bool HasPrevious()
    {
        return _position > 0;
    }

    public Book Previous()
    {
        if (!HasPrevious())
            throw new InvalidOperationException("No previous elements");

        _position--;
        Console.WriteLine($"📖 Moving to previous: {Current}");
        return Current;
    }

    public void Reset()
    {
        _position = -1;
        Console.WriteLine("🔄 Iterator reset to beginning");
    }
}

// Reverse iterator implementation
public class ReverseBookIterator : IIterator<Book>
{
    private readonly List<Book> _books;
    private int _position;

    public ReverseBookIterator(List<Book> books)
    {
        _books = books;
        _position = books.Count;
    }

    public Book Current
    {
        get
        {
            if (_position < 0 || _position >= _books.Count)
                throw new InvalidOperationException("Iterator is not positioned on a valid element");
            return _books[_position];
        }
    }

    public bool HasNext()
    {
        return _position - 1 >= 0;
    }

    public Book Next()
    {
        if (!HasNext())
            throw new InvalidOperationException("No more elements");

        _position--;
        Console.WriteLine($"📖 Moving to next (reverse): {Current}");
        return Current;
    }

    public bool HasPrevious()
    {
        return _position + 1 < _books.Count;
    }

    public Book Previous()
    {
        if (!HasPrevious())
            throw new InvalidOperationException("No previous elements");

        _position++;
        Console.WriteLine($"📖 Moving to previous (reverse): {Current}");
        return Current;
    }

    public void Reset()
    {
        _position = _books.Count;
        Console.WriteLine("🔄 Reverse iterator reset to end");
    }
}

// Filtered iterator implementation
public class FilteredBookIterator : IIterator<Book>
{
    private readonly List<Book> _filteredBooks;
    private int _position;

    public FilteredBookIterator(List<Book> allBooks, Func<Book, bool> predicate)
    {
        _filteredBooks = allBooks.Where(predicate).ToList();
        _position = -1;
        Console.WriteLine($"🔍 Created filtered iterator with {_filteredBooks.Count} books matching criteria");
    }

    public Book Current
    {
        get
        {
            if (_position < 0 || _position >= _filteredBooks.Count)
                throw new InvalidOperationException("Iterator is not positioned on a valid element");
            return _filteredBooks[_position];
        }
    }

    public bool HasNext()
    {
        return _position + 1 < _filteredBooks.Count;
    }

    public Book Next()
    {
        if (!HasNext())
            throw new InvalidOperationException("No more elements");

        _position++;
        Console.WriteLine($"📖 Moving to next (filtered): {Current}");
        return Current;
    }

    public bool HasPrevious()
    {
        return _position > 0;
    }

    public Book Previous()
    {
        if (!HasPrevious())
            throw new InvalidOperationException("No previous elements");

        _position--;
        Console.WriteLine($"📖 Moving to previous (filtered): {Current}");
        return Current;
    }

    public void Reset()
    {
        _position = -1;
        Console.WriteLine("🔄 Filtered iterator reset to beginning");
    }
}

// Concrete collection implementation
public class BookCollection : IBookCollection
{
    private readonly List<Book> _books;

    public BookCollection()
    {
        _books = new List<Book>();
    }

    public int Count => _books.Count;

    public void AddBook(Book book)
    {
        _books.Add(book);
        Console.WriteLine($"📚 Added book: {book}");
    }

    public void RemoveBook(Book book)
    {
        if (_books.Remove(book))
        {
            Console.WriteLine($"🗑️ Removed book: {book}");
        }
        else
        {
            Console.WriteLine($"❌ Book not found: {book}");
        }
    }

    public IIterator<Book> CreateIterator()
    {
        Console.WriteLine("➡️ Creating forward iterator");
        return new BookIterator(_books);
    }

    public IIterator<Book> CreateReverseIterator()
    {
        Console.WriteLine("⬅️ Creating reverse iterator");
        return new ReverseBookIterator(_books);
    }

    public IIterator<Book> CreateFilteredIterator(Func<Book, bool> predicate)
    {
        Console.WriteLine("🔍 Creating filtered iterator");
        return new FilteredBookIterator(_books, predicate);
    }

    // Additional convenience methods
    public IIterator<Book> CreateGenreIterator(string genre)
    {
        return CreateFilteredIterator(book => book.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));
    }

    public IIterator<Book> CreateAuthorIterator(string author)
    {
        return CreateFilteredIterator(book => book.Author.Contains(author, StringComparison.OrdinalIgnoreCase));
    }

    public IIterator<Book> CreateYearIterator(int year)
    {
        return CreateFilteredIterator(book => book.PublishedDate.Year == year);
    }

    public void PrintCollectionInfo()
    {
        Console.WriteLine($"\n📚 Book Collection Info:");
        Console.WriteLine($"   Total books: {Count}");
        
        var genres = _books.GroupBy(b => b.Genre).OrderBy(g => g.Key);
        Console.WriteLine($"   Genres: {string.Join(", ", genres.Select(g => $"{g.Key} ({g.Count()})"))}");
        
        var authors = _books.GroupBy(b => b.Author).OrderBy(g => g.Key);
        Console.WriteLine($"   Authors: {string.Join(", ", authors.Select(a => $"{a.Key} ({a.Count()})"))}");
    }
}

// Iterator utilities
public static class IteratorExtensions
{
    public static List<T> ToList<T>(this IIterator<T> iterator)
    {
        var list = new List<T>();
        iterator.Reset();
        
        while (iterator.HasNext())
        {
            list.Add(iterator.Next());
        }
        
        return list;
    }

    public static void ForEach<T>(this IIterator<T> iterator, Action<T> action)
    {
        iterator.Reset();
        
        while (iterator.HasNext())
        {
            action(iterator.Next());
        }
    }

    public static int Count<T>(this IIterator<T> iterator)
    {
        int count = 0;
        iterator.Reset();
        
        while (iterator.HasNext())
        {
            iterator.Next();
            count++;
        }
        
        return count;
    }

    public static T FirstOrDefault<T>(this IIterator<T> iterator, Func<T, bool> predicate = null)
    {
        iterator.Reset();
        
        while (iterator.HasNext())
        {
            var item = iterator.Next();
            if (predicate?.Invoke(item) != false)
            {
                return item;
            }
        }
        
        return default(T);
    }
}

// Usage - demonstrating iterator pattern
var library = new BookCollection();

Console.WriteLine("=== Library Management System with Iterator Pattern ===");

// Add books to the collection
library.AddBook(new Book("The Catcher in the Rye", "J.D. Salinger", "Fiction", 277, new DateTime(1951, 7, 16)));
library.AddBook(new Book("To Kill a Mockingbird", "Harper Lee", "Fiction", 324, new DateTime(1960, 7, 11)));
library.AddBook(new Book("1984", "George Orwell", "Dystopian", 328, new DateTime(1949, 6, 8)));
library.AddBook(new Book("Pride and Prejudice", "Jane Austen", "Romance", 432, new DateTime(1813, 1, 28)));
library.AddBook(new Book("The Great Gatsby", "F. Scott Fitzgerald", "Fiction", 180, new DateTime(1925, 4, 10)));
library.AddBook(new Book("Dune", "Frank Herbert", "Science Fiction", 688, new DateTime(1965, 8, 1)));
library.AddBook(new Book("Foundation", "Isaac Asimov", "Science Fiction", 244, new DateTime(1951, 5, 1)));

library.PrintCollectionInfo();

// Test forward iteration
Console.WriteLine("\n=== Forward Iteration ===");
var forwardIterator = library.CreateIterator();
while (forwardIterator.HasNext())
{
    forwardIterator.Next();
}

// Test reverse iteration
Console.WriteLine("\n=== Reverse Iteration ===");
var reverseIterator = library.CreateReverseIterator();
while (reverseIterator.HasNext())
{
    reverseIterator.Next();
}

// Test filtered iteration by genre
Console.WriteLine("\n=== Science Fiction Books ===");
var sciFiIterator = library.CreateGenreIterator("Science Fiction");
while (sciFiIterator.HasNext())
{
    sciFiIterator.Next();
}

// Test filtered iteration by year
Console.WriteLine("\n=== Books from 1951 ===");
var year1951Iterator = library.CreateYearIterator(1951);
while (year1951Iterator.HasNext())
{
    year1951Iterator.Next();
}

// Test navigation methods
Console.WriteLine("\n=== Navigation Test ===");
var navIterator = library.CreateIterator();
if (navIterator.HasNext())
{
    navIterator.Next(); // First book
    
    if (navIterator.HasNext())
    {
        navIterator.Next(); // Second book
        
        if (navIterator.HasPrevious())
        {
            navIterator.Previous(); // Back to first book
        }
    }
}

// Test extension methods
Console.WriteLine("\n=== Extension Methods Test ===");
var fictionIterator = library.CreateGenreIterator("Fiction");
Console.WriteLine($"Fiction books count: {fictionIterator.Count()}");

var firstFiction = fictionIterator.FirstOrDefault();
Console.WriteLine($"First fiction book: {firstFiction}");

// Convert to list using extension method
var fictionList = fictionIterator.ToList();
Console.WriteLine($"Fiction books as list: {fictionList.Count} items");

// Multiple simultaneous iterations
Console.WriteLine("\n=== Multiple Simultaneous Iterations ===");
var iter1 = library.CreateIterator();
var iter2 = library.CreateIterator();

Console.WriteLine("Iterator 1 - First 3 books:");
for (int i = 0; i < 3 && iter1.HasNext(); i++)
{
    iter1.Next();
}

Console.WriteLine("Iterator 2 - All books:");
while (iter2.HasNext())
{
    iter2.Next();
}

Console.WriteLine("Iterator 1 - Continue from where it left off:");
while (iter1.HasNext())
{
    iter1.Next();
}
```

## Real-World Examples

### 1. **File System Directory Iterator**
```csharp
// File system entry
public class FileSystemEntry
{
    public string Name { get; set; }
    public string FullPath { get; set; }
    public bool IsDirectory { get; set; }
    public long Size { get; set; }
    public DateTime LastModified { get; set; }

    public FileSystemEntry(string fullPath)
    {
        FullPath = fullPath;
        Name = Path.GetFileName(fullPath);
        IsDirectory = Directory.Exists(fullPath);
        
        if (IsDirectory)
        {
            Size = 0;
        }
        else if (File.Exists(fullPath))
        {
            var fileInfo = new FileInfo(fullPath);
            Size = fileInfo.Length;
            LastModified = fileInfo.LastWriteTime;
        }
    }

    public override string ToString()
    {
        var type = IsDirectory ? "📁" : "📄";
        var sizeStr = IsDirectory ? "" : $" ({FormatFileSize(Size)})";
        return $"{type} {Name}{sizeStr}";
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}

// Directory iterator interface
public interface IDirectoryIterator : IIterator<FileSystemEntry>
{
    string CurrentDirectory { get; }
    void ChangeDirectory(string path);
    bool IncludeSubdirectories { get; set; }
    string FilePattern { get; set; }
}

// Concrete directory iterator
public class DirectoryIterator : IDirectoryIterator
{
    private string _currentDirectory;
    private List<FileSystemEntry> _entries;
    private int _position;

    public DirectoryIterator(string directoryPath)
    {
        _currentDirectory = directoryPath;
        IncludeSubdirectories = false;
        FilePattern = "*";
        _position = -1;
        LoadEntries();
    }

    public string CurrentDirectory => _currentDirectory;
    public bool IncludeSubdirectories { get; set; }
    public string FilePattern { get; set; }

    public FileSystemEntry Current
    {
        get
        {
            if (_position < 0 || _position >= _entries.Count)
                throw new InvalidOperationException("Iterator not positioned on valid element");
            return _entries[_position];
        }
    }

    public void ChangeDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            _currentDirectory = path;
            Reset();
            LoadEntries();
            Console.WriteLine($"📁 Changed directory to: {path}");
        }
        else
        {
            throw new DirectoryNotFoundException($"Directory not found: {path}");
        }
    }

    public bool HasNext()
    {
        return _position + 1 < _entries.Count;
    }

    public FileSystemEntry Next()
    {
        if (!HasNext())
            throw new InvalidOperationException("No more elements");

        _position++;
        return Current;
    }

    public bool HasPrevious()
    {
        return _position > 0;
    }

    public FileSystemEntry Previous()
    {
        if (!HasPrevious())
            throw new InvalidOperationException("No previous elements");

        _position--;
        return Current;
    }

    public void Reset()
    {
        _position = -1;
    }

    private void LoadEntries()
    {
        _entries = new List<FileSystemEntry>();

        try
        {
            // Add directories
            var directories = Directory.GetDirectories(_currentDirectory, FilePattern);
            foreach (var dir in directories)
            {
                _entries.Add(new FileSystemEntry(dir));
            }

            // Add files
            var files = Directory.GetFiles(_currentDirectory, FilePattern);
            foreach (var file in files)
            {
                _entries.Add(new FileSystemEntry(file));
            }

            // If including subdirectories, recursively add their contents
            if (IncludeSubdirectories)
            {
                foreach (var dir in directories)
                {
                    var subIterator = new DirectoryIterator(dir)
                    {
                        IncludeSubdirectories = true,
                        FilePattern = FilePattern
                    };

                    while (subIterator.HasNext())
                    {
                        _entries.Add(subIterator.Next());
                    }
                }
            }

            Console.WriteLine($"📊 Loaded {_entries.Count} entries from {_currentDirectory}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"❌ Access denied: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading directory: {ex.Message}");
        }
    }
}

// File system browser
public class FileSystemBrowser
{
    private IDirectoryIterator _iterator;

    public FileSystemBrowser(string startPath)
    {
        _iterator = new DirectoryIterator(startPath);
    }

    public void Browse()
    {
        Console.WriteLine($"\n🗂️ Browsing: {_iterator.CurrentDirectory}");
        _iterator.Reset();

        int count = 0;
        while (_iterator.HasNext() && count < 20) // Limit output for demo
        {
            var entry = _iterator.Next();
            Console.WriteLine($"  {entry}");
            count++;
        }

        if (_iterator.HasNext())
        {
            Console.WriteLine($"  ... and {_iterator.ToList().Count - count} more items");
        }
    }

    public void SearchFiles(string pattern)
    {
        _iterator.FilePattern = pattern;
        _iterator.Reset();
        
        Console.WriteLine($"\n🔍 Searching for: {pattern}");
        Browse();
    }

    public void NavigateToParent()
    {
        var parentPath = Directory.GetParent(_iterator.CurrentDirectory)?.FullName;
        if (parentPath != null)
        {
            _iterator.ChangeDirectory(parentPath);
        }
        else
        {
            Console.WriteLine("Already at root directory");
        }
    }

    public void NavigateToSubdirectory(string subdirName)
    {
        var subdirPath = Path.Combine(_iterator.CurrentDirectory, subdirName);
        if (Directory.Exists(subdirPath))
        {
            _iterator.ChangeDirectory(subdirPath);
        }
        else
        {
            Console.WriteLine($"Subdirectory '{subdirName}' not found");
        }
    }

    public void ToggleRecursive()
    {
        _iterator.IncludeSubdirectories = !_iterator.IncludeSubdirectories;
        Console.WriteLine($"🔄 Recursive browsing: {(_iterator.IncludeSubdirectories ? "ON" : "OFF")}");
    }
}

// Usage
var browser = new FileSystemBrowser(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

Console.WriteLine("=== File System Iterator ===");
browser.Browse();

browser.SearchFiles("*.txt");
browser.SearchFiles("*.pdf");

browser.ToggleRecursive();
// browser.Browse(); // Would show all subdirectories too
```

### 2. **Social Media Feed Iterator**
```csharp
// Social media post
public class SocialPost
{
    public string Id { get; set; }
    public string Author { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public List<string> Tags { get; set; }
    public int Likes { get; set; }
    public int Shares { get; set; }
    public string MediaType { get; set; } // "text", "image", "video"

    public SocialPost(string id, string author, string content, string mediaType = "text")
    {
        Id = id;
        Author = author;
        Content = content;
        MediaType = mediaType;
        Timestamp = DateTime.Now;
        Tags = new List<string>();
        Likes = 0;
        Shares = 0;
    }

    public override string ToString()
    {
        var mediaIcon = MediaType switch
        {
            "image" => "🖼️",
            "video" => "🎥",
            _ => "📝"
        };

        var tagsStr = Tags.Count > 0 ? $" {string.Join(" ", Tags.Select(t => $"#{t}"))}" : "";
        return $"{mediaIcon} @{Author}: {Content}{tagsStr} (❤️{Likes} 🔄{Shares})";
    }
}

// Feed iterator interface
public interface IFeedIterator : IIterator<SocialPost>
{
    void SetFilter(Func<SocialPost, bool> filter);
    void SetSortOrder(Comparison<SocialPost> comparison);
    void LoadMorePosts();
    bool HasMoreToLoad { get; }
}

// Timeline feed iterator (chronological)
public class TimelineFeedIterator : IFeedIterator
{
    private List<SocialPost> _allPosts;
    private List<SocialPost> _filteredPosts;
    private int _position;
    private Func<SocialPost, bool> _filter;
    private Comparison<SocialPost> _sortOrder;

    public TimelineFeedIterator(List<SocialPost> posts)
    {
        _allPosts = posts;
        _filter = post => true; // No filter by default
        _sortOrder = (x, y) => y.Timestamp.CompareTo(x.Timestamp); // Newest first
        _position = -1;
        ApplyFilterAndSort();
    }

    public bool HasMoreToLoad { get; private set; } = false;

    public SocialPost Current
    {
        get
        {
            if (_position < 0 || _position >= _filteredPosts.Count)
                throw new InvalidOperationException("Iterator not positioned on valid element");
            return _filteredPosts[_position];
        }
    }

    public bool HasNext()
    {
        return _position + 1 < _filteredPosts.Count;
    }

    public SocialPost Next()
    {
        if (!HasNext())
            throw new InvalidOperationException("No more posts");

        _position++;
        Console.WriteLine($"📱 Loading post {_position + 1}/{_filteredPosts.Count}");
        return Current;
    }

    public bool HasPrevious()
    {
        return _position > 0;
    }

    public SocialPost Previous()
    {
        if (!HasPrevious())
            throw new InvalidOperationException("No previous posts");

        _position--;
        return Current;
    }

    public void Reset()
    {
        _position = -1;
    }

    public void SetFilter(Func<SocialPost, bool> filter)
    {
        _filter = filter ?? (post => true);
        ApplyFilterAndSort();
        Reset();
        Console.WriteLine($"🔍 Filter applied. {_filteredPosts.Count} posts match criteria");
    }

    public void SetSortOrder(Comparison<SocialPost> comparison)
    {
        _sortOrder = comparison ?? ((x, y) => y.Timestamp.CompareTo(x.Timestamp));
        ApplyFilterAndSort();
        Reset();
        Console.WriteLine($"📊 Sort order applied. {_filteredPosts.Count} posts sorted");
    }

    public void LoadMorePosts()
    {
        // Simulate loading more posts from server
        HasMoreToLoad = false;
        Console.WriteLine("📡 Loaded more posts from server");
    }

    private void ApplyFilterAndSort()
    {
        _filteredPosts = _allPosts.Where(_filter).ToList();
        _filteredPosts.Sort(_sortOrder);
    }
}

// Trending feed iterator (popularity-based)
public class TrendingFeedIterator : IFeedIterator
{
    private List<SocialPost> _allPosts;
    private List<SocialPost> _trendingPosts;
    private int _position;
    private Func<SocialPost, bool> _filter;

    public TrendingFeedIterator(List<SocialPost> posts)
    {
        _allPosts = posts;
        _filter = post => true;
        _position = -1;
        CalculateTrending();
    }

    public bool HasMoreToLoad { get; private set; } = true;

    public SocialPost Current
    {
        get
        {
            if (_position < 0 || _position >= _trendingPosts.Count)
                throw new InvalidOperationException("Iterator not positioned on valid element");
            return _trendingPosts[_position];
        }
    }

    public bool HasNext()
    {
        return _position + 1 < _trendingPosts.Count;
    }

    public SocialPost Next()
    {
        if (!HasNext())
            throw new InvalidOperationException("No more trending posts");

        _position++;
        Console.WriteLine($"🔥 Loading trending post {_position + 1}/{_trendingPosts.Count}");
        return Current;
    }

    public bool HasPrevious()
    {
        return _position > 0;
    }

    public SocialPost Previous()
    {
        if (!HasPrevious())
            throw new InvalidOperationException("No previous posts");

        _position--;
        return Current;
    }

    public void Reset()
    {
        _position = -1;
    }

    public void SetFilter(Func<SocialPost, bool> filter)
    {
        _filter = filter ?? (post => true);
        CalculateTrending();
        Reset();
        Console.WriteLine($"🔍 Trending filter applied. {_trendingPosts.Count} posts match criteria");
    }

    public void SetSortOrder(Comparison<SocialPost> comparison)
    {
        // Trending feed always sorts by engagement score
        Console.WriteLine("⚠️ Trending feed uses its own sort order based on engagement");
    }

    public void LoadMorePosts()
    {
        HasMoreToLoad = false;
        Console.WriteLine("📡 Loaded more trending posts");
    }

    private void CalculateTrending()
    {
        // Calculate engagement score and sort by trending
        _trendingPosts = _allPosts
            .Where(_filter)
            .Where(post => DateTime.Now.Subtract(post.Timestamp).TotalHours <= 24) // Last 24 hours
            .OrderByDescending(post => CalculateEngagementScore(post))
            .ToList();
    }

    private double CalculateEngagementScore(SocialPost post)
    {
        var hoursOld = DateTime.Now.Subtract(post.Timestamp).TotalHours;
        var timeDecay = Math.Max(0.1, 1.0 - (hoursOld / 24.0)); // Decay over 24 hours
        return (post.Likes * 1.0 + post.Shares * 2.0) * timeDecay;
    }
}

// Social media feed aggregator
public class SocialMediaFeed
{
    private List<SocialPost> _posts;

    public SocialMediaFeed()
    {
        _posts = new List<SocialPost>();
    }

    public void AddPost(SocialPost post)
    {
        _posts.Add(post);
        Console.WriteLine($"📝 New post added: {post}");
    }

    public IFeedIterator CreateTimelineIterator()
    {
        Console.WriteLine("⏰ Creating timeline iterator (chronological)");
        return new TimelineFeedIterator(_posts);
    }

    public IFeedIterator CreateTrendingIterator()
    {
        Console.WriteLine("🔥 Creating trending iterator (popularity-based)");
        return new TrendingFeedIterator(_posts);
    }

    // Convenience methods for common filters
    public IFeedIterator CreateAuthorIterator(string author)
    {
        var iterator = CreateTimelineIterator();
        iterator.SetFilter(post => post.Author.Equals(author, StringComparison.OrdinalIgnoreCase));
        return iterator;
    }

    public IFeedIterator CreateTagIterator(string tag)
    {
        var iterator = CreateTimelineIterator();
        iterator.SetFilter(post => post.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase));
        return iterator;
    }

    public IFeedIterator CreateMediaTypeIterator(string mediaType)
    {
        var iterator = CreateTimelineIterator();
        iterator.SetFilter(post => post.MediaType.Equals(mediaType, StringComparison.OrdinalIgnoreCase));
        return iterator;
    }

    public void PrintFeedStats()
    {
        Console.WriteLine($"\n📊 Feed Statistics:");
        Console.WriteLine($"   Total posts: {_posts.Count}");
        
        var mediaTypes = _posts.GroupBy(p => p.MediaType);
        Console.WriteLine($"   Media types: {string.Join(", ", mediaTypes.Select(g => $"{g.Key} ({g.Count()})"))}");
        
        var authors = _posts.GroupBy(p => p.Author);
        Console.WriteLine($"   Authors: {string.Join(", ", authors.Select(g => $"{g.Key} ({g.Count()})"))}");
        
        var totalEngagement = _posts.Sum(p => p.Likes + p.Shares);
        Console.WriteLine($"   Total engagement: {totalEngagement}");
    }
}

// Usage
var feed = new SocialMediaFeed();

Console.WriteLine("\n=== Social Media Feed Iterator ===");

// Add sample posts
feed.AddPost(new SocialPost("1", "alice", "Just finished a great book! 📚", "text"));
feed.AddPost(new SocialPost("2", "bob", "Check out this sunset! 🌅", "image"));
feed.AddPost(new SocialPost("3", "charlie", "New coding tutorial is live! 🎥", "video"));
feed.AddPost(new SocialPost("4", "alice", "Coffee and code ☕💻", "image"));
feed.AddPost(new SocialPost("5", "diana", "Thoughts on the latest tech trends...", "text"));

// Add engagement to posts
feed._posts[1].Likes = 25; feed._posts[1].Shares = 5; // Bob's sunset
feed._posts[1].Tags.AddRange(new[] { "sunset", "photography", "nature" });

feed._posts[2].Likes = 45; feed._posts[2].Shares = 12; // Charlie's tutorial
feed._posts[2].Tags.AddRange(new[] { "coding", "tutorial", "programming" });

feed._posts[3].Likes = 15; feed._posts[3].Shares = 2; // Alice's coffee
feed._posts[3].Tags.AddRange(new[] { "coffee", "coding", "worklife" });

feed.PrintFeedStats();

// Test timeline iteration
Console.WriteLine("\n=== Timeline Feed (Chronological) ===");
var timelineIterator = feed.CreateTimelineIterator();
int count = 0;
while (timelineIterator.HasNext() && count < 3)
{
    timelineIterator.Next();
    count++;
}

// Test trending iteration
Console.WriteLine("\n=== Trending Feed (Popularity) ===");
var trendingIterator = feed.CreateTrendingIterator();
while (trendingIterator.HasNext())
{
    trendingIterator.Next();
}

// Test filtered iterations
Console.WriteLine("\n=== Alice's Posts ===");
var aliceIterator = feed.CreateAuthorIterator("alice");
while (aliceIterator.HasNext())
{
    aliceIterator.Next();
}

Console.WriteLine("\n=== Coding-related Posts ===");
var codingIterator = feed.CreateTagIterator("coding");
while (codingIterator.HasNext())
{
    codingIterator.Next();
}

Console.WriteLine("\n=== Image Posts ===");
var imageIterator = feed.CreateMediaTypeIterator("image");
while (imageIterator.HasNext())
{
    imageIterator.Next();
}
```

### 3. **Playlist Management System**
```csharp
// Song entity
public class Song
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Album { get; set; }
    public TimeSpan Duration { get; set; }
    public string Genre { get; set; }
    public int Year { get; set; }
    public int PlayCount { get; set; }

    public Song(string id, string title, string artist, string album, TimeSpan duration, string genre, int year)
    {
        Id = id;
        Title = title;
        Artist = artist;
        Album = album;
        Duration = duration;
        Genre = genre;
        Year = year;
        PlayCount = 0;
    }

    public void Play()
    {
        PlayCount++;
        Console.WriteLine($"🎵 Now playing: {this}");
    }

    public override string ToString()
    {
        return $"'{Title}' by {Artist} ({Album}, {Duration:mm\\:ss}) [Played {PlayCount} times]";
    }
}

// Playlist iterator interface
public interface IPlaylistIterator : IIterator<Song>
{
    bool IsShuffled { get; set; }
    bool IsRepeating { get; set; }
    void Shuffle();
    void Skip();
    void GoToSong(string songId);
    TimeSpan TotalDuration { get; }
}

// Sequential playlist iterator
public class SequentialPlaylistIterator : IPlaylistIterator
{
    private readonly List<Song> _originalOrder;
    private List<Song> _currentOrder;
    private int _position;
    private Random _random;

    public SequentialPlaylistIterator(List<Song> songs)
    {
        _originalOrder = new List<Song>(songs);
        _currentOrder = new List<Song>(songs);
        _position = -1;
        _random = new Random();
        IsShuffled = false;
        IsRepeating = false;
    }

    public bool IsShuffled { get; set; }
    public bool IsRepeating { get; set; }

    public TimeSpan TotalDuration => TimeSpan.FromTicks(_originalOrder.Sum(s => s.Duration.Ticks));

    public Song Current
    {
        get
        {
            if (_position < 0 || _position >= _currentOrder.Count)
                throw new InvalidOperationException("Iterator not positioned on valid song");
            return _currentOrder[_position];
        }
    }

    public bool HasNext()
    {
        if (IsRepeating)
            return _currentOrder.Count > 0;
        
        return _position + 1 < _currentOrder.Count;
    }

    public Song Next()
    {
        if (_currentOrder.Count == 0)
            throw new InvalidOperationException("Playlist is empty");

        if (IsRepeating && _position + 1 >= _currentOrder.Count)
        {
            _position = 0; // Loop back to beginning
            Console.WriteLine("🔄 Playlist repeating - back to first song");
        }
        else if (!HasNext())
        {
            throw new InvalidOperationException("No more songs in playlist");
        }
        else
        {
            _position++;
        }

        var song = Current;
        song.Play();
        return song;
    }

    public bool HasPrevious()
    {
        if (IsRepeating)
            return _currentOrder.Count > 0;
            
        return _position > 0;
    }

    public Song Previous()
    {
        if (_currentOrder.Count == 0)
            throw new InvalidOperationException("Playlist is empty");

        if (IsRepeating && _position <= 0)
        {
            _position = _currentOrder.Count - 1; // Loop to end
            Console.WriteLine("🔄 Playlist repeating - jumping to last song");
        }
        else if (!HasPrevious())
        {
            throw new InvalidOperationException("No previous songs");
        }
        else
        {
            _position--;
        }

        var song = Current;
        song.Play();
        return song;
    }

    public void Reset()
    {
        _position = -1;
        Console.WriteLine("⏮️ Playlist reset to beginning");
    }

    public void Shuffle()
    {
        if (IsShuffled)
        {
            // Un-shuffle: restore original order
            _currentOrder = new List<Song>(_originalOrder);
            IsShuffled = false;
            Console.WriteLine("📑 Playlist unshuffled - restored to original order");
        }
        else
        {
            // Shuffle the current order
            for (int i = _currentOrder.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (_currentOrder[i], _currentOrder[j]) = (_currentOrder[j], _currentOrder[i]);
            }
            IsShuffled = true;
            Console.WriteLine("🔀 Playlist shuffled");
        }
        
        Reset();
    }

    public void Skip()
    {
        if (HasNext())
        {
            Next();
            Console.WriteLine("⏭️ Skipped to next song");
        }
        else
        {
            Console.WriteLine("⏭️ No more songs to skip to");
        }
    }

    public void GoToSong(string songId)
    {
        for (int i = 0; i < _currentOrder.Count; i++)
        {
            if (_currentOrder[i].Id == songId)
            {
                _position = i;
                Console.WriteLine($"⏯️ Jumped to song: {Current}");
                return;
            }
        }
        
        Console.WriteLine($"❌ Song with ID '{songId}' not found in playlist");
    }
}

// Smart playlist iterator (dynamic filtering)
public class SmartPlaylistIterator : IPlaylistIterator
{
    private readonly List<Song> _allSongs;
    private List<Song> _filteredSongs;
    private int _position;
    private Func<Song, bool> _filter;
    private Random _random;

    public SmartPlaylistIterator(List<Song> allSongs, Func<Song, bool> filter)
    {
        _allSongs = allSongs;
        _filter = filter;
        _position = -1;
        _random = new Random();
        IsShuffled = false;
        IsRepeating = true; // Smart playlists typically repeat
        UpdateFilteredSongs();
    }

    public bool IsShuffled { get; set; }
    public bool IsRepeating { get; set; }

    public TimeSpan TotalDuration => TimeSpan.FromTicks(_filteredSongs.Sum(s => s.Duration.Ticks));

    public Song Current
    {
        get
        {
            if (_position < 0 || _position >= _filteredSongs.Count)
                throw new InvalidOperationException("Iterator not positioned on valid song");
            return _filteredSongs[_position];
        }
    }

    public bool HasNext()
    {
        UpdateFilteredSongs(); // Re-evaluate filter
        return _filteredSongs.Count > 0;
    }

    public Song Next()
    {
        UpdateFilteredSongs();
        
        if (_filteredSongs.Count == 0)
            throw new InvalidOperationException("No songs match the smart playlist criteria");

        if (IsShuffled)
        {
            _position = _random.Next(_filteredSongs.Count);
        }
        else
        {
            _position = (_position + 1) % _filteredSongs.Count;
        }

        var song = Current;
        song.Play();
        Console.WriteLine($"🎯 Smart playlist: {song}");
        return song;
    }

    public bool HasPrevious()
    {
        UpdateFilteredSongs();
        return _filteredSongs.Count > 0;
    }

    public Song Previous()
    {
        UpdateFilteredSongs();
        
        if (_filteredSongs.Count == 0)
            throw new InvalidOperationException("No songs match the smart playlist criteria");

        if (IsShuffled)
        {
            _position = _random.Next(_filteredSongs.Count);
        }
        else
        {
            _position = _position <= 0 ? _filteredSongs.Count - 1 : _position - 1;
        }

        var song = Current;
        song.Play();
        return song;
    }

    public void Reset()
    {
        _position = -1;
        Console.WriteLine("⏮️ Smart playlist reset");
    }

    public void Shuffle()
    {
        IsShuffled = !IsShuffled;
        Console.WriteLine($"🔀 Smart playlist shuffle: {(IsShuffled ? "ON" : "OFF")}");
    }

    public void Skip()
    {
        if (HasNext())
        {
            Next();
            Console.WriteLine("⏭️ Smart playlist: skipped to next qualifying song");
        }
    }

    public void GoToSong(string songId)
    {
        UpdateFilteredSongs();
        
        for (int i = 0; i < _filteredSongs.Count; i++)
        {
            if (_filteredSongs[i].Id == songId)
            {
                _position = i;
                Console.WriteLine($"⏯️ Smart playlist: jumped to {Current}");
                return;
            }
        }
        
        Console.WriteLine($"❌ Song '{songId}' not available in current smart playlist filter");
    }

    private void UpdateFilteredSongs()
    {
        var previousCount = _filteredSongs?.Count ?? 0;
        _filteredSongs = _allSongs.Where(_filter).ToList();
        
        if (_filteredSongs.Count != previousCount)
        {
            Console.WriteLine($"🎯 Smart playlist updated: {_filteredSongs.Count} songs match criteria");
        }
    }
}

// Music library and playlist manager
public class MusicLibrary
{
    private List<Song> _allSongs;
    private Dictionary<string, List<Song>> _playlists;

    public MusicLibrary()
    {
        _allSongs = new List<Song>();
        _playlists = new Dictionary<string, List<Song>>();
    }

    public void AddSong(Song song)
    {
        _allSongs.Add(song);
        Console.WriteLine($"🎵 Added to library: {song}");
    }

    public void CreatePlaylist(string name, List<string> songIds)
    {
        var songs = _allSongs.Where(s => songIds.Contains(s.Id)).ToList();
        _playlists[name] = songs;
        Console.WriteLine($"📃 Created playlist '{name}' with {songs.Count} songs");
    }

    public IPlaylistIterator GetPlaylistIterator(string playlistName)
    {
        if (_playlists.ContainsKey(playlistName))
        {
            return new SequentialPlaylistIterator(_playlists[playlistName]);
        }
        throw new ArgumentException($"Playlist '{playlistName}' not found");
    }

    public IPlaylistIterator CreateGenreIterator(string genre)
    {
        Console.WriteLine($"🎼 Creating smart playlist for genre: {genre}");
        return new SmartPlaylistIterator(_allSongs, song => 
            song.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));
    }

    public IPlaylistIterator CreatePopularSongsIterator(int minPlayCount = 5)
    {
        Console.WriteLine($"⭐ Creating smart playlist for popular songs (min {minPlayCount} plays)");
        return new SmartPlaylistIterator(_allSongs, song => song.PlayCount >= minPlayCount);
    }

    public IPlaylistIterator CreateRecentSongsIterator(int fromYear)
    {
        Console.WriteLine($"📅 Creating smart playlist for recent songs (from {fromYear})");
        return new SmartPlaylistIterator(_allSongs, song => song.Year >= fromYear);
    }

    public void PrintLibraryStats()
    {
        Console.WriteLine($"\n🎵 Music Library Statistics:");
        Console.WriteLine($"   Total songs: {_allSongs.Count}");
        Console.WriteLine($"   Total playlists: {_playlists.Count}");
        
        var genres = _allSongs.GroupBy(s => s.Genre);
        Console.WriteLine($"   Genres: {string.Join(", ", genres.Select(g => $"{g.Key} ({g.Count()})"))}");
        
        var totalDuration = TimeSpan.FromTicks(_allSongs.Sum(s => s.Duration.Ticks));
        Console.WriteLine($"   Total duration: {totalDuration:hh\\:mm\\:ss}");
        
        var mostPlayed = _allSongs.OrderByDescending(s => s.PlayCount).FirstOrDefault();
        Console.WriteLine($"   Most played: {mostPlayed?.Title} ({mostPlayed?.PlayCount} plays)");
    }
}

// Usage
var library = new MusicLibrary();

Console.WriteLine("\n=== Music Playlist Iterator ===");

// Add songs to library
library.AddSong(new Song("1", "Bohemian Rhapsody", "Queen", "A Night at the Opera", TimeSpan.FromMinutes(6), "Rock", 1975));
library.AddSong(new Song("2", "Hotel California", "Eagles", "Hotel California", TimeSpan.FromMinutes(6.5), "Rock", 1976));
library.AddSong(new Song("3", "Billie Jean", "Michael Jackson", "Thriller", TimeSpan.FromMinutes(4.5), "Pop", 1982));
library.AddSong(new Song("4", "Smells Like Teen Spirit", "Nirvana", "Nevermind", TimeSpan.FromMinutes(5), "Grunge", 1991));
library.AddSong(new Song("5", "Shape of You", "Ed Sheeran", "÷", TimeSpan.FromMinutes(4), "Pop", 2017));
library.AddSong(new Song("6", "Blinding Lights", "The Weeknd", "After Hours", TimeSpan.FromMinutes(3.5), "Pop", 2019));

library.PrintLibraryStats();

// Create a regular playlist
library.CreatePlaylist("Classic Rock", new List<string> { "1", "2", "4" });

// Test regular playlist iteration
Console.WriteLine("\n=== Classic Rock Playlist ===");
var rockPlaylist = library.GetPlaylistIterator("Classic Rock");
rockPlaylist.IsRepeating = false;

Console.WriteLine($"Total duration: {rockPlaylist.TotalDuration:mm\\:ss}");

while (rockPlaylist.HasNext())
{
    rockPlaylist.Next();
}

// Test shuffle
Console.WriteLine("\n--- Testing Shuffle ---");
rockPlaylist.Shuffle();
rockPlaylist.Reset();
while (rockPlaylist.HasNext())
{
    rockPlaylist.Next();
}

// Test smart playlists
Console.WriteLine("\n=== Smart Playlist - Pop Songs ===");
var popIterator = library.CreateGenreIterator("Pop");
for (int i = 0; i < 5 && popIterator.HasNext(); i++)
{
    popIterator.Next();
}

// Simulate some plays to test popular songs
Console.WriteLine("\n--- Simulating song plays ---");
library._allSongs.First(s => s.Id == "3").PlayCount = 10; // Billie Jean
library._allSongs.First(s => s.Id == "5").PlayCount = 8;  // Shape of You
library._allSongs.First(s => s.Id == "6").PlayCount = 6;  // Blinding Lights

Console.WriteLine("\n=== Smart Playlist - Popular Songs ===");
var popularIterator = library.CreatePopularSongsIterator(5);
while (popularIterator.HasNext())
{
    popularIterator.Next();
    // Break after a few to avoid infinite loop (smart playlist repeats)
    if (popularIterator.Current.PlayCount <= 10) break;
}
```

## Benefits
- **Encapsulation**: Hides internal collection structure
- **Uniform Interface**: Same interface for different collection types
- **Multiple Traversals**: Support concurrent iterations over same collection
- **Flexible Iteration**: Different algorithms (forward, reverse, filtered)
- **Separation of Concerns**: Iteration logic separated from collection logic

## Drawbacks
- **Memory Overhead**: Additional iterator objects
- **Complexity**: Can be overkill for simple scenarios
- **Performance**: Indirect access through iterator methods
- **State Management**: Need to manage iterator state properly

## When to Use
✅ **Use When:**
- You need to traverse collections without exposing structure
- Multiple traversal algorithms are needed
- You want to support concurrent iterations
- Collection implementation might change
- You need filtered or transformed iteration

❌ **Avoid When:**
- Simple, stable collections with basic iteration needs
- Performance is critical and direct access is faster
- Memory usage is severely constrained
- Only one traversal method is ever needed

## Iterator vs Other Patterns

| Pattern | Purpose | Key Difference |
|---------|---------|----------------|
| **Iterator** | Traverse collections sequentially | Focuses on sequential access |
| **Visitor** | Operate on object structure | Focuses on operations across types |
| **Strategy** | Select algorithm at runtime | Focuses on algorithm selection |
| **Command** | Encapsulate requests as objects | Focuses on request encapsulation |

## Best Practices
1. **Fail-Fast**: Throw exceptions for invalid states immediately
2. **Thread Safety**: Consider thread safety for concurrent access
3. **Resource Management**: Properly dispose of resources in iterators
4. **Reset Capability**: Provide way to reset iteration
5. **Clear State**: Make iterator state and position clear

## Common Mistakes
1. **Modifying Collection**: Modifying collection during iteration
2. **State Confusion**: Not properly managing iterator position
3. **Resource Leaks**: Not disposing of iterator resources
4. **Thread Issues**: Not considering thread safety

## Modern C# Features
```csharp
// Using yield return for easier iterator implementation
public class YieldIterator<T> : IEnumerable<T>
{
    private readonly List<T> _items;

    public YieldIterator(List<T> items)
    {
        _items = items;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var item in _items)
        {
            Console.WriteLine($"Yielding: {item}");
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

// Using IAsyncEnumerable for async iteration
public class AsyncIterator<T> : IAsyncEnumerable<T>
{
    private readonly List<T> _items;

    public AsyncIterator(List<T> items)
    {
        _items = items;
    }

    public async IAsyncEnumerator<T> GetAsyncEnumerator(
        CancellationToken cancellationToken = default)
    {
        foreach (var item in _items)
        {
            await Task.Delay(100, cancellationToken); // Simulate async operation
            yield return item;
        }
    }
}

// Using LINQ for functional iteration
public static class IteratorExtensions
{
    public static IEnumerable<T> WhereWithLogging<T>(this IEnumerable<T> source, 
        Func<T, bool> predicate)
    {
        foreach (var item in source)
        {
            bool matches = predicate(item);
            Console.WriteLine($"Filtering {item}: {(matches ? "KEEP" : "SKIP")}");
            if (matches)
                yield return item;
        }
    }
}
```

## Testing Iterators
```csharp
[Test]
public void Iterator_HasNext_ReturnsTrueWhenElementsAvailable()
{
    // Arrange
    var collection = new BookCollection();
    collection.AddBook(new Book("Test", "Author", "Genre", 100, DateTime.Now));
    var iterator = collection.CreateIterator();

    // Act & Assert
    Assert.IsTrue(iterator.HasNext());
}

[Test]
public void Iterator_Next_ThrowsExceptionWhenNoMoreElements()
{
    // Arrange
    var collection = new BookCollection();
    var iterator = collection.CreateIterator();

    // Act & Assert
    Assert.Throws<InvalidOperationException>(() => iterator.Next());
}
```

## Summary
The Iterator pattern is like having a universal remote control for any collection - it provides a standard interface to navigate through elements regardless of how the collection is internally organized. Whether you're browsing through a photo album, reading a book page by page, or scrolling through a social media feed, the iterator pattern ensures you can move through content without knowing the underlying storage structure.

The pattern is especially powerful when you need different ways to traverse the same data (forward, backward, filtered, sorted) or when multiple people need to be at different positions in the same collection simultaneously. Modern C# makes this even easier with built-in interfaces like `IEnumerable<T>` and language features like `yield return`.

The key insight is that by separating the traversal logic from the collection itself, you gain flexibility in how data is accessed while keeping the collection's internal structure hidden and protected.
