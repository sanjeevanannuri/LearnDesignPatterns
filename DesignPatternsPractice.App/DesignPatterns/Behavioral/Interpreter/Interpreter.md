# Interpreter Pattern

## Overview
The Interpreter pattern defines how to evaluate language grammar or expressions. It represents each grammar rule as a class and uses these classes to interpret sentences in the language. Think of it like building a **calculator** or **language translator** - you define rules for how different parts of an expression should be understood and processed.

## Problem It Solves
Imagine you need to evaluate mathematical expressions like "3 + 5 * 2" or process simple scripting commands:
- You have a specific language or expression format to parse
- The language has a simple grammar that can be represented as rules
- You need to interpret and execute these expressions dynamically
- Building a full parser would be overkill for simple languages

Without Interpreter pattern, you might write a monolithic parser:
```csharp
// BAD: Monolithic expression evaluator
public class ExpressionEvaluator
{
    public int Evaluate(string expression)
    {
        // Massive switch statement or if-else chain
        // Hard to extend, maintain, and test
        if (expression.Contains("+"))
        {
            var parts = expression.Split('+');
            return int.Parse(parts[0]) + int.Parse(parts[1]);
        }
        else if (expression.Contains("*"))
        {
            // More complex parsing logic...
        }
        // This becomes unwieldy quickly
    }
}
```

This approach becomes unmaintainable as the language grows in complexity.

## Real-World Analogy
Think of a **language translation system**:
1. **Grammar Rules**: Each language has specific rules (noun + verb + object)
2. **Expression Trees**: Sentences are broken down into grammatical components
3. **Interpreters**: Each grammatical rule has a specific way to be translated
4. **Context**: The current state of translation (vocabulary, previous sentences)

Or consider a **music notation system**:
- **Notes** (terminals): Individual musical notes like C, D, E
- **Phrases** (non-terminals): Combinations of notes that form melodies
- **Composition Rules**: How notes and phrases combine to create music
- **Performance**: The actual playing/interpretation of the written music

## Implementation Details

### Basic Structure
```csharp
// Context - contains global information
public class Context
{
    public Dictionary<string, int> Variables { get; set; }
    
    public Context()
    {
        Variables = new Dictionary<string, int>();
    }
}

// Abstract Expression
public abstract class AbstractExpression
{
    public abstract int Interpret(Context context);
}

// Terminal Expression - represents leaf nodes
public class NumberExpression : AbstractExpression
{
    private int _number;

    public NumberExpression(int number)
    {
        _number = number;
    }

    public override int Interpret(Context context)
    {
        return _number;
    }
}

// Non-terminal Expression - represents composite nodes
public class AddExpression : AbstractExpression
{
    private AbstractExpression _left;
    private AbstractExpression _right;

    public AddExpression(AbstractExpression left, AbstractExpression right)
    {
        _left = left;
        _right = right;
    }

    public override int Interpret(Context context)
    {
        return _left.Interpret(context) + _right.Interpret(context);
    }
}
```

### Key Components
1. **AbstractExpression**: Interface for interpreting expressions
2. **TerminalExpression**: Implements interpret for terminal symbols
3. **NonTerminalExpression**: Implements interpret for non-terminal symbols
4. **Context**: Contains information global to the interpreter
5. **Client**: Builds and interprets the expression tree

## Example from Our Code
```csharp
// Context for storing variables
public class InterpreterContext
{
    private Dictionary<string, int> _variables;

    public InterpreterContext()
    {
        _variables = new Dictionary<string, int>();
    }

    public void SetVariable(string name, int value)
    {
        _variables[name] = value;
        Console.WriteLine($"Variable '{name}' set to {value}");
    }

    public int GetVariable(string name)
    {
        if (_variables.ContainsKey(name))
        {
            return _variables[name];
        }
        throw new ArgumentException($"Variable '{name}' not found");
    }

    public void PrintVariables()
    {
        Console.WriteLine("Current variables:");
        foreach (var kvp in _variables)
        {
            Console.WriteLine($"  {kvp.Key} = {kvp.Value}");
        }
    }
}

// Abstract expression interface
public interface IExpression
{
    int Interpret(InterpreterContext context);
    string ToString();
}

// Terminal expressions - leaf nodes
public class NumberExpression : IExpression
{
    private readonly int _value;

    public NumberExpression(int value)
    {
        _value = value;
    }

    public int Interpret(InterpreterContext context)
    {
        return _value;
    }

    public override string ToString()
    {
        return _value.ToString();
    }
}

public class VariableExpression : IExpression
{
    private readonly string _variableName;

    public VariableExpression(string variableName)
    {
        _variableName = variableName;
    }

    public int Interpret(InterpreterContext context)
    {
        return context.GetVariable(_variableName);
    }

    public override string ToString()
    {
        return _variableName;
    }
}

// Non-terminal expressions - composite nodes
public class AddExpression : IExpression
{
    private readonly IExpression _left;
    private readonly IExpression _right;

    public AddExpression(IExpression left, IExpression right)
    {
        _left = left;
        _right = right;
    }

    public int Interpret(InterpreterContext context)
    {
        var leftValue = _left.Interpret(context);
        var rightValue = _right.Interpret(context);
        var result = leftValue + rightValue;
        
        Console.WriteLine($"  {_left} + {_right} = {leftValue} + {rightValue} = {result}");
        return result;
    }

    public override string ToString()
    {
        return $"({_left} + {_right})";
    }
}

public class SubtractExpression : IExpression
{
    private readonly IExpression _left;
    private readonly IExpression _right;

    public SubtractExpression(IExpression left, IExpression right)
    {
        _left = left;
        _right = right;
    }

    public int Interpret(InterpreterContext context)
    {
        var leftValue = _left.Interpret(context);
        var rightValue = _right.Interpret(context);
        var result = leftValue - rightValue;
        
        Console.WriteLine($"  {_left} - {_right} = {leftValue} - {rightValue} = {result}");
        return result;
    }

    public override string ToString()
    {
        return $"({_left} - {_right})";
    }
}

public class MultiplyExpression : IExpression
{
    private readonly IExpression _left;
    private readonly IExpression _right;

    public MultiplyExpression(IExpression left, IExpression right)
    {
        _left = left;
        _right = right;
    }

    public int Interpret(InterpreterContext context)
    {
        var leftValue = _left.Interpret(context);
        var rightValue = _right.Interpret(context);
        var result = leftValue * rightValue;
        
        Console.WriteLine($"  {_left} * {_right} = {leftValue} * {rightValue} = {result}");
        return result;
    }

    public override string ToString()
    {
        return $"({_left} * {_right})";
    }
}

public class DivideExpression : IExpression
{
    private readonly IExpression _left;
    private readonly IExpression _right;

    public DivideExpression(IExpression left, IExpression right)
    {
        _left = left;
        _right = right;
    }

    public int Interpret(InterpreterContext context)
    {
        var leftValue = _left.Interpret(context);
        var rightValue = _right.Interpret(context);
        
        if (rightValue == 0)
        {
            throw new DivideByZeroException("Cannot divide by zero");
        }
        
        var result = leftValue / rightValue;
        Console.WriteLine($"  {_left} / {_right} = {leftValue} / {rightValue} = {result}");
        return result;
    }

    public override string ToString()
    {
        return $"({_left} / {_right})";
    }
}

// Expression Builder - helps create complex expressions
public class ExpressionBuilder
{
    public static IExpression ParseExpression(string expression, InterpreterContext context)
    {
        // Simple parser for demonstration - handles basic expressions
        // In real implementation, you'd use a proper parser like ANTLR
        
        expression = expression.Replace(" ", ""); // Remove spaces
        
        // Handle parentheses by finding innermost expressions first
        while (expression.Contains("("))
        {
            int start = expression.LastIndexOf('(');
            int end = expression.IndexOf(')', start);
            
            if (end == -1)
                throw new ArgumentException("Mismatched parentheses");
                
            string innerExpression = expression.Substring(start + 1, end - start - 1);
            var innerResult = ParseSimpleExpression(innerExpression, context);
            
            // Create a temporary variable for the result
            string tempVar = $"temp{DateTime.Now.Ticks}";
            context.SetVariable(tempVar, innerResult.Interpret(context));
            
            // Replace the parentheses expression with the temp variable
            expression = expression.Substring(0, start) + tempVar + expression.Substring(end + 1);
        }
        
        return ParseSimpleExpression(expression, context);
    }

    private static IExpression ParseSimpleExpression(string expression, InterpreterContext context)
    {
        // Simple left-to-right parsing (doesn't handle operator precedence properly)
        // This is for demonstration - use a real parser for production code
        
        var tokens = TokenizeExpression(expression);
        
        if (tokens.Count == 1)
        {
            return CreateTerminalExpression(tokens[0], context);
        }
        
        IExpression result = CreateTerminalExpression(tokens[0], context);
        
        for (int i = 1; i < tokens.Count; i += 2)
        {
            if (i + 1 >= tokens.Count)
                throw new ArgumentException("Invalid expression format");
                
            string operatorToken = tokens[i];
            string operandToken = tokens[i + 1];
            
            IExpression rightOperand = CreateTerminalExpression(operandToken, context);
            
            result = operatorToken switch
            {
                "+" => new AddExpression(result, rightOperand),
                "-" => new SubtractExpression(result, rightOperand),
                "*" => new MultiplyExpression(result, rightOperand),
                "/" => new DivideExpression(result, rightOperand),
                _ => throw new ArgumentException($"Unknown operator: {operatorToken}")
            };
        }
        
        return result;
    }

    private static List<string> TokenizeExpression(string expression)
    {
        var tokens = new List<string>();
        var currentToken = "";
        
        foreach (char c in expression)
        {
            if (c == '+' || c == '-' || c == '*' || c == '/')
            {
                if (!string.IsNullOrEmpty(currentToken))
                {
                    tokens.Add(currentToken);
                    currentToken = "";
                }
                tokens.Add(c.ToString());
            }
            else
            {
                currentToken += c;
            }
        }
        
        if (!string.IsNullOrEmpty(currentToken))
        {
            tokens.Add(currentToken);
        }
        
        return tokens;
    }

    private static IExpression CreateTerminalExpression(string token, InterpreterContext context)
    {
        if (int.TryParse(token, out int number))
        {
            return new NumberExpression(number);
        }
        else
        {
            return new VariableExpression(token);
        }
    }
}

// Calculator - client that uses the interpreter
public class Calculator
{
    private InterpreterContext _context;

    public Calculator()
    {
        _context = new InterpreterContext();
    }

    public void SetVariable(string name, int value)
    {
        _context.SetVariable(name, value);
    }

    public int Calculate(string expression)
    {
        Console.WriteLine($"\nCalculating: {expression}");
        
        try
        {
            var parsedExpression = ExpressionBuilder.ParseExpression(expression, _context);
            Console.WriteLine($"Parsed as: {parsedExpression}");
            Console.WriteLine("Evaluation steps:");
            
            var result = parsedExpression.Interpret(_context);
            
            Console.WriteLine($"Final result: {result}");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 0;
        }
    }

    public void ShowVariables()
    {
        _context.PrintVariables();
    }
}

// Usage - demonstrating interpreter pattern
var calculator = new Calculator();

Console.WriteLine("=== Math Expression Interpreter ===");

// Set some variables
calculator.SetVariable("x", 10);
calculator.SetVariable("y", 5);
calculator.SetVariable("z", 3);

calculator.ShowVariables();

// Calculate various expressions
calculator.Calculate("5 + 3");
calculator.Calculate("x + y");
calculator.Calculate("x - y * z");
calculator.Calculate("x + y - z");
calculator.Calculate("x * y / z");

// More complex expressions
calculator.Calculate("(x + y) * z");
calculator.Calculate("x + (y * z)");
calculator.Calculate("(x - y) + (y * z)");

// Test error handling
calculator.Calculate("x + unknown");  // Variable not found
calculator.Calculate("x / 0");        // Division by zero
```

## Real-World Examples

### 1. **SQL Query Interpreter**
```csharp
// Context for SQL execution
public class SqlContext
{
    public Dictionary<string, List<Dictionary<string, object>>> Tables { get; set; }

    public SqlContext()
    {
        Tables = new Dictionary<string, List<Dictionary<string, object>>>();
    }

    public void AddTable(string tableName, List<Dictionary<string, object>> data)
    {
        Tables[tableName] = data;
    }

    public List<Dictionary<string, object>> GetTable(string tableName)
    {
        return Tables.ContainsKey(tableName) ? Tables[tableName] : new List<Dictionary<string, object>>();
    }
}

// SQL Expression interface
public interface ISqlExpression
{
    List<Dictionary<string, object>> Execute(SqlContext context);
}

// Terminal expressions
public class TableExpression : ISqlExpression
{
    private readonly string _tableName;

    public TableExpression(string tableName)
    {
        _tableName = tableName;
    }

    public List<Dictionary<string, object>> Execute(SqlContext context)
    {
        Console.WriteLine($"📊 Accessing table: {_tableName}");
        return context.GetTable(_tableName);
    }
}

// Non-terminal expressions
public class SelectExpression : ISqlExpression
{
    private readonly List<string> _columns;
    private readonly ISqlExpression _fromExpression;

    public SelectExpression(List<string> columns, ISqlExpression fromExpression)
    {
        _columns = columns;
        _fromExpression = fromExpression;
    }

    public List<Dictionary<string, object>> Execute(SqlContext context)
    {
        Console.WriteLine($"🔍 Selecting columns: {string.Join(", ", _columns)}");
        var sourceData = _fromExpression.Execute(context);
        
        var result = new List<Dictionary<string, object>>();
        
        foreach (var row in sourceData)
        {
            var newRow = new Dictionary<string, object>();
            foreach (var column in _columns)
            {
                if (column == "*")
                {
                    foreach (var kvp in row)
                    {
                        newRow[kvp.Key] = kvp.Value;
                    }
                }
                else if (row.ContainsKey(column))
                {
                    newRow[column] = row[column];
                }
            }
            result.Add(newRow);
        }
        
        return result;
    }
}

public class WhereExpression : ISqlExpression
{
    private readonly ISqlExpression _sourceExpression;
    private readonly string _column;
    private readonly string _operator;
    private readonly object _value;

    public WhereExpression(ISqlExpression sourceExpression, string column, string op, object value)
    {
        _sourceExpression = sourceExpression;
        _column = column;
        _operator = op;
        _value = value;
    }

    public List<Dictionary<string, object>> Execute(SqlContext context)
    {
        Console.WriteLine($"🔍 Filtering where {_column} {_operator} {_value}");
        var sourceData = _sourceExpression.Execute(context);
        
        var result = new List<Dictionary<string, object>>();
        
        foreach (var row in sourceData)
        {
            if (row.ContainsKey(_column))
            {
                var columnValue = row[_column];
                bool matches = _operator switch
                {
                    "=" => columnValue.Equals(_value),
                    ">" => Convert.ToDouble(columnValue) > Convert.ToDouble(_value),
                    "<" => Convert.ToDouble(columnValue) < Convert.ToDouble(_value),
                    ">=" => Convert.ToDouble(columnValue) >= Convert.ToDouble(_value),
                    "<=" => Convert.ToDouble(columnValue) <= Convert.ToDouble(_value),
                    "!=" => !columnValue.Equals(_value),
                    _ => false
                };

                if (matches)
                {
                    result.Add(row);
                }
            }
        }
        
        return result;
    }
}

public class OrderByExpression : ISqlExpression
{
    private readonly ISqlExpression _sourceExpression;
    private readonly string _column;
    private readonly bool _ascending;

    public OrderByExpression(ISqlExpression sourceExpression, string column, bool ascending = true)
    {
        _sourceExpression = sourceExpression;
        _column = column;
        _ascending = ascending;
    }

    public List<Dictionary<string, object>> Execute(SqlContext context)
    {
        Console.WriteLine($"📊 Ordering by {_column} {(_ascending ? "ASC" : "DESC")}");
        var sourceData = _sourceExpression.Execute(context);
        
        return _ascending 
            ? sourceData.OrderBy(row => row.ContainsKey(_column) ? row[_column] : null).ToList()
            : sourceData.OrderByDescending(row => row.ContainsKey(_column) ? row[_column] : null).ToList();
    }
}

// SQL Query Builder
public class SqlQueryBuilder
{
    public static ISqlExpression BuildQuery(string tableName, List<string> selectColumns = null, 
        string whereColumn = null, string whereOperator = null, object whereValue = null,
        string orderByColumn = null, bool orderAscending = true)
    {
        ISqlExpression query = new TableExpression(tableName);

        if (!string.IsNullOrEmpty(whereColumn) && !string.IsNullOrEmpty(whereOperator) && whereValue != null)
        {
            query = new WhereExpression(query, whereColumn, whereOperator, whereValue);
        }

        if (!string.IsNullOrEmpty(orderByColumn))
        {
            query = new OrderByExpression(query, orderByColumn, orderAscending);
        }

        if (selectColumns != null && selectColumns.Count > 0)
        {
            query = new SelectExpression(selectColumns, query);
        }

        return query;
    }
}

// Usage
var sqlContext = new SqlContext();

// Add sample data
var employees = new List<Dictionary<string, object>>
{
    new() { {"id", 1}, {"name", "Alice"}, {"age", 30}, {"department", "IT"}, {"salary", 70000} },
    new() { {"id", 2}, {"name", "Bob"}, {"age", 25}, {"department", "Sales"}, {"salary", 50000} },
    new() { {"id", 3}, {"name", "Charlie"}, {"age", 35}, {"department", "IT"}, {"salary", 80000} },
    new() { {"id", 4}, {"name", "Diana"}, {"age", 28}, {"department", "HR"}, {"salary", 60000} }
};

sqlContext.AddTable("employees", employees);

Console.WriteLine("\n=== SQL Query Interpreter ===");

// Build and execute queries
var query1 = SqlQueryBuilder.BuildQuery("employees", new List<string> { "name", "age" });
var result1 = query1.Execute(sqlContext);
Console.WriteLine("Results:");
foreach (var row in result1)
{
    Console.WriteLine($"  {row["name"]}, Age: {row["age"]}");
}

Console.WriteLine();

var query2 = SqlQueryBuilder.BuildQuery("employees", 
    new List<string> { "name", "salary" },
    "department", "=", "IT",
    "salary", false);
var result2 = query2.Execute(sqlContext);
Console.WriteLine("Results:");
foreach (var row in result2)
{
    Console.WriteLine($"  {row["name"]}, Salary: ${row["salary"]}");
}
```

### 2. **Configuration Language Interpreter**
```csharp
// Configuration context
public class ConfigContext
{
    public Dictionary<string, object> Settings { get; set; }
    public List<string> Errors { get; set; }

    public ConfigContext()
    {
        Settings = new Dictionary<string, object>();
        Errors = new List<string>();
    }

    public void SetSetting(string key, object value)
    {
        Settings[key] = value;
        Console.WriteLine($"⚙️ Setting {key} = {value}");
    }

    public T GetSetting<T>(string key, T defaultValue = default)
    {
        if (Settings.ContainsKey(key) && Settings[key] is T)
        {
            return (T)Settings[key];
        }
        return defaultValue;
    }

    public void AddError(string error)
    {
        Errors.Add(error);
        Console.WriteLine($"❌ Error: {error}");
    }
}

// Configuration expression interface
public interface IConfigExpression
{
    void Execute(ConfigContext context);
}

// Terminal expressions
public class SetExpression : IConfigExpression
{
    private readonly string _key;
    private readonly object _value;

    public SetExpression(string key, object value)
    {
        _key = key;
        _value = value;
    }

    public void Execute(ConfigContext context)
    {
        context.SetSetting(_key, _value);
    }
}

public class IncludeExpression : IConfigExpression
{
    private readonly string _filePath;

    public IncludeExpression(string filePath)
    {
        _filePath = filePath;
    }

    public void Execute(ConfigContext context)
    {
        Console.WriteLine($"📁 Including configuration from: {_filePath}");
        // In real implementation, you'd read and parse the included file
        context.SetSetting($"included_{Path.GetFileNameWithoutExtension(_filePath)}", true);
    }
}

// Non-terminal expressions
public class SectionExpression : IConfigExpression
{
    private readonly string _sectionName;
    private readonly List<IConfigExpression> _expressions;

    public SectionExpression(string sectionName)
    {
        _sectionName = sectionName;
        _expressions = new List<IConfigExpression>();
    }

    public void AddExpression(IConfigExpression expression)
    {
        _expressions.Add(expression);
    }

    public void Execute(ConfigContext context)
    {
        Console.WriteLine($"📂 Processing section: [{_sectionName}]");
        
        // Create a nested context for the section
        var sectionSettings = new Dictionary<string, object>();
        var sectionContext = new ConfigContext();
        
        foreach (var expression in _expressions)
        {
            expression.Execute(sectionContext);
        }
        
        // Merge section settings into main context with section prefix
        foreach (var setting in sectionContext.Settings)
        {
            context.SetSetting($"{_sectionName}.{setting.Key}", setting.Value);
        }
    }
}

public class ConditionalExpression : IConfigExpression
{
    private readonly string _condition;
    private readonly List<IConfigExpression> _trueExpressions;
    private readonly List<IConfigExpression> _falseExpressions;

    public ConditionalExpression(string condition)
    {
        _condition = condition;
        _trueExpressions = new List<IConfigExpression>();
        _falseExpressions = new List<IConfigExpression>();
    }

    public void AddTrueExpression(IConfigExpression expression)
    {
        _trueExpressions.Add(expression);
    }

    public void AddFalseExpression(IConfigExpression expression)
    {
        _falseExpressions.Add(expression);
    }

    public void Execute(ConfigContext context)
    {
        bool conditionResult = EvaluateCondition(_condition, context);
        Console.WriteLine($"🔍 Evaluating condition '{_condition}': {conditionResult}");
        
        var expressionsToExecute = conditionResult ? _trueExpressions : _falseExpressions;
        
        foreach (var expression in expressionsToExecute)
        {
            expression.Execute(context);
        }
    }

    private bool EvaluateCondition(string condition, ConfigContext context)
    {
        // Simple condition evaluation - in real implementation use a proper expression evaluator
        if (condition.StartsWith("env."))
        {
            string envVar = condition.Substring(4);
            return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(envVar));
        }
        
        if (condition.Contains("="))
        {
            var parts = condition.Split('=');
            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string expectedValue = parts[1].Trim();
                var actualValue = context.GetSetting<string>(key, "");
                return actualValue == expectedValue;
            }
        }
        
        // Default to checking if setting exists
        return context.Settings.ContainsKey(condition);
    }
}

// Configuration parser
public class ConfigParser
{
    public static List<IConfigExpression> ParseConfig(string[] configLines)
    {
        var expressions = new List<IConfigExpression>();
        var currentSection = (SectionExpression)null;
        var conditionalStack = new Stack<ConditionalExpression>();
        
        foreach (var line in configLines)
        {
            string trimmedLine = line.Trim();
            
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                continue;
                
            if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
            {
                // Section start
                string sectionName = trimmedLine.Substring(1, trimmedLine.Length - 2);
                currentSection = new SectionExpression(sectionName);
                expressions.Add(currentSection);
            }
            else if (trimmedLine.StartsWith("if "))
            {
                // Conditional start
                string condition = trimmedLine.Substring(3).Trim();
                var conditional = new ConditionalExpression(condition);
                conditionalStack.Push(conditional);
                
                if (currentSection != null)
                    currentSection.AddExpression(conditional);
                else
                    expressions.Add(conditional);
            }
            else if (trimmedLine == "else")
            {
                // Switch to false expressions in current conditional
                // This is simplified - real implementation would handle this better
            }
            else if (trimmedLine == "endif")
            {
                // End conditional
                if (conditionalStack.Count > 0)
                    conditionalStack.Pop();
            }
            else if (trimmedLine.StartsWith("include "))
            {
                // Include directive
                string filePath = trimmedLine.Substring(8).Trim();
                var includeExpr = new IncludeExpression(filePath);
                
                if (conditionalStack.Count > 0)
                    conditionalStack.Peek().AddTrueExpression(includeExpr);
                else if (currentSection != null)
                    currentSection.AddExpression(includeExpr);
                else
                    expressions.Add(includeExpr);
            }
            else if (trimmedLine.Contains("="))
            {
                // Setting assignment
                var parts = trimmedLine.Split('=', 2);
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    
                    // Parse value type
                    object parsedValue = ParseValue(value);
                    var setExpr = new SetExpression(key, parsedValue);
                    
                    if (conditionalStack.Count > 0)
                        conditionalStack.Peek().AddTrueExpression(setExpr);
                    else if (currentSection != null)
                        currentSection.AddExpression(setExpr);
                    else
                        expressions.Add(setExpr);
                }
            }
        }
        
        return expressions;
    }

    private static object ParseValue(string value)
    {
        value = value.Trim();
        
        // Remove quotes
        if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
            (value.StartsWith("'") && value.EndsWith("'")))
        {
            return value.Substring(1, value.Length - 2);
        }
        
        // Try parse as boolean
        if (bool.TryParse(value, out bool boolValue))
            return boolValue;
            
        // Try parse as integer
        if (int.TryParse(value, out int intValue))
            return intValue;
            
        // Try parse as double
        if (double.TryParse(value, out double doubleValue))
            return doubleValue;
            
        // Default to string
        return value;
    }
}

// Configuration manager
public class ConfigurationManager
{
    public ConfigContext LoadConfiguration(string[] configLines)
    {
        Console.WriteLine("🔧 Loading configuration...");
        
        var context = new ConfigContext();
        var expressions = ConfigParser.ParseConfig(configLines);
        
        foreach (var expression in expressions)
        {
            try
            {
                expression.Execute(context);
            }
            catch (Exception ex)
            {
                context.AddError($"Error executing expression: {ex.Message}");
            }
        }
        
        Console.WriteLine($"✅ Configuration loaded with {context.Settings.Count} settings and {context.Errors.Count} errors");
        return context;
    }
}

// Usage
var configManager = new ConfigurationManager();

var sampleConfig = new string[]
{
    "# Application Configuration",
    "app_name = MyApplication",
    "version = 1.0.0",
    "debug = true",
    "",
    "[database]",
    "host = localhost",
    "port = 5432",
    "name = myapp_db",
    "",
    "if env.PRODUCTION",
    "    ssl_enabled = true",
    "    connection_timeout = 30",
    "else",
    "    ssl_enabled = false",
    "    connection_timeout = 5",
    "endif",
    "",
    "[logging]",
    "level = INFO",
    "file = /var/log/myapp.log",
    "include logging_extra.conf",
    "",
    "if debug = true",
    "    level = DEBUG",
    "    console_output = true",
    "endif"
};

var config = configManager.LoadConfiguration(sampleConfig);

Console.WriteLine("\n📋 Final Configuration:");
foreach (var setting in config.Settings.OrderBy(s => s.Key))
{
    Console.WriteLine($"  {setting.Key} = {setting.Value}");
}
```

### 3. **Business Rule Engine**
```csharp
// Rule context
public class RuleContext
{
    public Dictionary<string, object> Facts { get; set; }
    public List<string> Actions { get; set; }
    public List<string> Logs { get; set; }

    public RuleContext()
    {
        Facts = new Dictionary<string, object>();
        Actions = new List<string>();
        Logs = new List<string>();
    }

    public void SetFact(string name, object value)
    {
        Facts[name] = value;
        Logs.Add($"📊 Fact set: {name} = {value}");
    }

    public T GetFact<T>(string name, T defaultValue = default)
    {
        if (Facts.ContainsKey(name) && Facts[name] is T)
        {
            return (T)Facts[name];
        }
        return defaultValue;
    }

    public void ExecuteAction(string action)
    {
        Actions.Add(action);
        Logs.Add($"⚡ Action executed: {action}");
    }

    public void Log(string message)
    {
        Logs.Add($"📝 {message}");
    }
}

// Rule expression interface
public interface IRuleExpression
{
    bool Evaluate(RuleContext context);
    void Execute(RuleContext context);
}

// Condition expressions
public class FactCondition : IRuleExpression
{
    private readonly string _factName;
    private readonly string _operator;
    private readonly object _value;

    public FactCondition(string factName, string op, object value)
    {
        _factName = factName;
        _operator = op;
        _value = value;
    }

    public bool Evaluate(RuleContext context)
    {
        var factValue = context.GetFact<object>(_factName);
        
        bool result = _operator switch
        {
            "=" => factValue?.Equals(_value) == true,
            "!=" => factValue?.Equals(_value) != true,
            ">" => Compare(factValue, _value) > 0,
            "<" => Compare(factValue, _value) < 0,
            ">=" => Compare(factValue, _value) >= 0,
            "<=" => Compare(factValue, _value) <= 0,
            "contains" => factValue?.ToString()?.Contains(_value?.ToString()) == true,
            _ => false
        };

        context.Log($"Evaluating: {_factName} {_operator} {_value} = {result}");
        return result;
    }

    public void Execute(RuleContext context)
    {
        // Conditions don't execute actions by themselves
    }

    private int Compare(object left, object right)
    {
        if (left is IComparable leftComp && right is IComparable rightComp)
        {
            return leftComp.CompareTo(Convert.ChangeType(rightComp, leftComp.GetType()));
        }
        return 0;
    }
}

// Logical expressions
public class AndExpression : IRuleExpression
{
    private readonly List<IRuleExpression> _conditions;

    public AndExpression(params IRuleExpression[] conditions)
    {
        _conditions = conditions.ToList();
    }

    public bool Evaluate(RuleContext context)
    {
        context.Log("Evaluating AND expression:");
        bool result = _conditions.All(c => c.Evaluate(context));
        context.Log($"AND result: {result}");
        return result;
    }

    public void Execute(RuleContext context)
    {
        foreach (var condition in _conditions)
        {
            condition.Execute(context);
        }
    }
}

public class OrExpression : IRuleExpression
{
    private readonly List<IRuleExpression> _conditions;

    public OrExpression(params IRuleExpression[] conditions)
    {
        _conditions = conditions.ToList();
    }

    public bool Evaluate(RuleContext context)
    {
        context.Log("Evaluating OR expression:");
        bool result = _conditions.Any(c => c.Evaluate(context));
        context.Log($"OR result: {result}");
        return result;
    }

    public void Execute(RuleContext context)
    {
        foreach (var condition in _conditions)
        {
            condition.Execute(context);
        }
    }
}

public class NotExpression : IRuleExpression
{
    private readonly IRuleExpression _condition;

    public NotExpression(IRuleExpression condition)
    {
        _condition = condition;
    }

    public bool Evaluate(RuleContext context)
    {
        context.Log("Evaluating NOT expression:");
        bool result = !_condition.Evaluate(context);
        context.Log($"NOT result: {result}");
        return result;
    }

    public void Execute(RuleContext context)
    {
        _condition.Execute(context);
    }
}

// Action expressions
public class ActionExpression : IRuleExpression
{
    private readonly string _action;

    public ActionExpression(string action)
    {
        _action = action;
    }

    public bool Evaluate(RuleContext context)
    {
        return true; // Actions always "evaluate" to true
    }

    public void Execute(RuleContext context)
    {
        context.ExecuteAction(_action);
    }
}

// Rule expression
public class RuleExpression : IRuleExpression
{
    private readonly IRuleExpression _condition;
    private readonly List<IRuleExpression> _actions;
    private readonly string _name;

    public RuleExpression(string name, IRuleExpression condition)
    {
        _name = name;
        _condition = condition;
        _actions = new List<IRuleExpression>();
    }

    public void AddAction(IRuleExpression action)
    {
        _actions.Add(action);
    }

    public bool Evaluate(RuleContext context)
    {
        context.Log($"🔍 Evaluating rule: {_name}");
        bool conditionMet = _condition.Evaluate(context);
        
        if (conditionMet)
        {
            context.Log($"✅ Rule '{_name}' condition met");
            foreach (var action in _actions)
            {
                action.Execute(context);
            }
        }
        else
        {
            context.Log($"❌ Rule '{_name}' condition not met");
        }
        
        return conditionMet;
    }

    public void Execute(RuleContext context)
    {
        Evaluate(context);
    }
}

// Business rule engine
public class BusinessRuleEngine
{
    private readonly List<RuleExpression> _rules;

    public BusinessRuleEngine()
    {
        _rules = new List<RuleExpression>();
    }

    public void AddRule(RuleExpression rule)
    {
        _rules.Add(rule);
    }

    public void ExecuteRules(RuleContext context)
    {
        context.Log("🚀 Starting rule engine execution");
        
        foreach (var rule in _rules)
        {
            try
            {
                rule.Execute(context);
            }
            catch (Exception ex)
            {
                context.Log($"❌ Error executing rule: {ex.Message}");
            }
        }
        
        context.Log($"✅ Rule engine execution completed. {context.Actions.Count} actions executed.");
    }
}

// Usage - E-commerce pricing rules
var ruleEngine = new BusinessRuleEngine();

// Rule 1: VIP customers get 20% discount
var vipRule = new RuleExpression("VIP Discount",
    new FactCondition("customer_type", "=", "VIP"));
vipRule.AddAction(new ActionExpression("apply_discount:20"));
vipRule.AddAction(new ActionExpression("send_notification:VIP discount applied"));

// Rule 2: Large orders (>$500) get free shipping
var freeShippingRule = new RuleExpression("Free Shipping",
    new FactCondition("order_total", ">", 500));
freeShippingRule.AddAction(new ActionExpression("set_shipping_cost:0"));
freeShippingRule.AddAction(new ActionExpression("add_message:Free shipping applied"));

// Rule 3: New customers get welcome bonus if order > $100
var welcomeRule = new RuleExpression("Welcome Bonus",
    new AndExpression(
        new FactCondition("customer_type", "=", "New"),
        new FactCondition("order_total", ">", 100)
    ));
welcomeRule.AddAction(new ActionExpression("apply_discount:10"));
welcomeRule.AddAction(new ActionExpression("send_email:welcome_bonus"));

// Rule 4: Weekend orders during promotion period get extra discount
var weekendRule = new RuleExpression("Weekend Promotion",
    new AndExpression(
        new FactCondition("is_weekend", "=", true),
        new FactCondition("promotion_active", "=", true),
        new NotExpression(new FactCondition("customer_type", "=", "VIP")) // VIP already gets discount
    ));
weekendRule.AddAction(new ActionExpression("apply_discount:5"));
weekendRule.AddAction(new ActionExpression("add_message:Weekend promotion applied"));

ruleEngine.AddRule(vipRule);
ruleEngine.AddRule(freeShippingRule);
ruleEngine.AddRule(welcomeRule);
ruleEngine.AddRule(weekendRule);

// Test scenario 1: VIP customer with large order
Console.WriteLine("=== Business Rule Engine ===");
Console.WriteLine("\n--- Scenario 1: VIP Customer ---");
var context1 = new RuleContext();
context1.SetFact("customer_type", "VIP");
context1.SetFact("order_total", 600);
context1.SetFact("is_weekend", true);
context1.SetFact("promotion_active", true);

ruleEngine.ExecuteRules(context1);

Console.WriteLine("\nActions executed:");
foreach (var action in context1.Actions)
{
    Console.WriteLine($"  • {action}");
}

// Test scenario 2: New customer weekend order
Console.WriteLine("\n--- Scenario 2: New Customer Weekend Order ---");
var context2 = new RuleContext();
context2.SetFact("customer_type", "New");
context2.SetFact("order_total", 150);
context2.SetFact("is_weekend", true);
context2.SetFact("promotion_active", true);

ruleEngine.ExecuteRules(context2);

Console.WriteLine("\nActions executed:");
foreach (var action in context2.Actions)
{
    Console.WriteLine($"  • {action}");
}

Console.WriteLine("\nExecution log:");
foreach (var log in context2.Logs)
{
    Console.WriteLine($"  {log}");
}
```

## Benefits
- **Extensibility**: Easy to add new grammar rules
- **Reusability**: Expression objects can be reused and combined
- **Composability**: Complex expressions built from simple ones
- **Testability**: Each expression can be tested independently
- **Flexibility**: Grammar can be modified without changing client code

## Drawbacks
- **Complexity**: Can become complex for large grammars
- **Performance**: Expression trees can be inefficient for large expressions
- **Memory Usage**: Each rule creates object instances
- **Maintenance**: Grammar changes may require multiple class modifications
- **Limited Scope**: Best for simple languages only

## When to Use
✅ **Use When:**
- You have a simple language or grammar to interpret
- The grammar is relatively stable
- You need to evaluate expressions repeatedly
- The language rules are well-defined and finite
- You want to make the grammar explicit in your code

❌ **Avoid When:**
- The language is complex (use a proper parser generator instead)
- Performance is critical (interpreter pattern can be slow)
- The grammar changes frequently
- You need advanced parsing features like error recovery
- The language is already well-supported by existing tools

## Interpreter vs Other Patterns

| Pattern | Purpose | Key Difference |
|---------|---------|----------------|
| **Interpreter** | Evaluates language expressions | Builds syntax tree for evaluation |
| **Strategy** | Selects algorithms at runtime | Focuses on algorithm selection |
| **Command** | Encapsulates requests as objects | Focuses on request encapsulation |
| **Visitor** | Operates on object structure | Focuses on operations across types |

## Best Practices
1. **Simple Grammar**: Keep the grammar as simple as possible
2. **Flyweight**: Use Flyweight pattern for terminal expressions
3. **Error Handling**: Provide clear error messages for invalid expressions
4. **Context Management**: Keep context lightweight and focused
5. **Performance**: Consider caching interpreted results
6. **Testing**: Test each expression type thoroughly

## Common Mistakes
1. **Complex Grammar**: Trying to interpret complex languages
2. **Poor Error Handling**: Not providing helpful error messages
3. **Memory Issues**: Not considering memory usage of expression trees
4. **Threading**: Not making expressions thread-safe when needed

## Modern C# Features
```csharp
// Using pattern matching in expressions
public class ModernExpression : IExpression
{
    public int Interpret(InterpreterContext context) => this switch
    {
        NumberExpression num => num.Value,
        VariableExpression var => context.GetVariable(var.Name),
        AddExpression add => add.Left.Interpret(context) + add.Right.Interpret(context),
        _ => throw new NotSupportedException()
    };
}

// Using records for immutable expressions
public record NumberExpr(int Value) : IExpression
{
    public int Interpret(InterpreterContext context) => Value;
}

public record AddExpr(IExpression Left, IExpression Right) : IExpression
{
    public int Interpret(InterpreterContext context) => 
        Left.Interpret(context) + Right.Interpret(context);
}

// Using expression trees for compilation
public class CompiledExpression
{
    private readonly Func<InterpreterContext, int> _compiled;

    public CompiledExpression(Expression<Func<InterpreterContext, int>> expression)
    {
        _compiled = expression.Compile();
    }

    public int Evaluate(InterpreterContext context) => _compiled(context);
}
```

## Testing Interpreters
```csharp
[Test]
public void NumberExpression_Interpret_ReturnsCorrectValue()
{
    // Arrange
    var expression = new NumberExpression(42);
    var context = new InterpreterContext();

    // Act
    var result = expression.Interpret(context);

    // Assert
    Assert.AreEqual(42, result);
}

[Test]
public void AddExpression_Interpret_AddsOperands()
{
    // Arrange
    var left = new NumberExpression(10);
    var right = new NumberExpression(5);
    var expression = new AddExpression(left, right);
    var context = new InterpreterContext();

    // Act
    var result = expression.Interpret(context);

    // Assert
    Assert.AreEqual(15, result);
}
```

## Summary
The Interpreter pattern is like building a custom language translator - you define the grammar rules as classes and create an interpreter that can understand and execute expressions in that language. It's perfect for domain-specific languages, configuration files, mathematical expressions, and business rules.

Think of it as creating your own mini-programming language where each grammar rule becomes a class that knows how to interpret its part of the language. The pattern shines when you have a simple, well-defined grammar that doesn't change frequently.

The key insight is that by representing each grammar rule as a separate class, you make the language rules explicit and easily testable, while also making it possible to combine simple expressions into complex ones through composition.
