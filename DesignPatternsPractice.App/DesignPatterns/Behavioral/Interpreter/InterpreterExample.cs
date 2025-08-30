namespace DesignPatterns.Behavioral.Interpreter
{
    // Context for the interpreter
    public class Context
    {
        private readonly Dictionary<string, int> _variables = new();

        public void SetVariable(string name, int value)
        {
            _variables[name] = value;
            Console.WriteLine($"Set variable {name} = {value}");
        }

        public int GetVariable(string name)
        {
            if (_variables.TryGetValue(name, out int value))
            {
                return value;
            }
            throw new ArgumentException($"Variable '{name}' not found");
        }

        public bool HasVariable(string name)
        {
            return _variables.ContainsKey(name);
        }

        public void PrintVariables()
        {
            Console.WriteLine("Current variables:");
            foreach (var variable in _variables)
            {
                Console.WriteLine($"  {variable.Key} = {variable.Value}");
            }
        }
    }

    // Abstract expression
    public interface IExpression
    {
        int Interpret(Context context);
    }

    // Terminal expressions
    public class NumberExpression : IExpression
    {
        private readonly int _number;

        public NumberExpression(int number)
        {
            _number = number;
        }

        public int Interpret(Context context)
        {
            Console.WriteLine($"Interpreting number: {_number}");
            return _number;
        }
    }

    public class VariableExpression : IExpression
    {
        private readonly string _variableName;

        public VariableExpression(string variableName)
        {
            _variableName = variableName;
        }

        public int Interpret(Context context)
        {
            Console.WriteLine($"Interpreting variable: {_variableName}");
            return context.GetVariable(_variableName);
        }
    }

    // Non-terminal expressions
    public class AddExpression : IExpression
    {
        private readonly IExpression _left;
        private readonly IExpression _right;

        public AddExpression(IExpression left, IExpression right)
        {
            _left = left;
            _right = right;
        }

        public int Interpret(Context context)
        {
            int leftValue = _left.Interpret(context);
            int rightValue = _right.Interpret(context);
            int result = leftValue + rightValue;
            Console.WriteLine($"Adding {leftValue} + {rightValue} = {result}");
            return result;
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

        public int Interpret(Context context)
        {
            int leftValue = _left.Interpret(context);
            int rightValue = _right.Interpret(context);
            int result = leftValue - rightValue;
            Console.WriteLine($"Subtracting {leftValue} - {rightValue} = {result}");
            return result;
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

        public int Interpret(Context context)
        {
            int leftValue = _left.Interpret(context);
            int rightValue = _right.Interpret(context);
            int result = leftValue * rightValue;
            Console.WriteLine($"Multiplying {leftValue} * {rightValue} = {result}");
            return result;
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

        public int Interpret(Context context)
        {
            int leftValue = _left.Interpret(context);
            int rightValue = _right.Interpret(context);
            if (rightValue == 0)
            {
                throw new DivideByZeroException("Cannot divide by zero");
            }
            int result = leftValue / rightValue;
            Console.WriteLine($"Dividing {leftValue} / {rightValue} = {result}");
            return result;
        }
    }

    // Expression parser
    public class ExpressionParser
    {
        public static IExpression ParseExpression(string expression)
        {
            var tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var stack = new Stack<IExpression>();

            foreach (var token in tokens)
            {
                if (IsOperator(token))
                {
                    if (stack.Count < 2)
                    {
                        throw new ArgumentException($"Invalid expression: not enough operands for operator '{token}'");
                    }

                    var right = stack.Pop();
                    var left = stack.Pop();

                    IExpression operation = token switch
                    {
                        "+" => new AddExpression(left, right),
                        "-" => new SubtractExpression(left, right),
                        "*" => new MultiplyExpression(left, right),
                        "/" => new DivideExpression(left, right),
                        _ => throw new ArgumentException($"Unknown operator: {token}")
                    };

                    stack.Push(operation);
                }
                else if (int.TryParse(token, out int number))
                {
                    stack.Push(new NumberExpression(number));
                }
                else
                {
                    stack.Push(new VariableExpression(token));
                }
            }

            if (stack.Count != 1)
            {
                throw new ArgumentException("Invalid expression: incorrect number of operands and operators");
            }

            return stack.Pop();
        }

        private static bool IsOperator(string token)
        {
            return token is "+" or "-" or "*" or "/";
        }
    }

    // Boolean expression interpreter
    public interface IBooleanExpression
    {
        bool Interpret(BooleanContext context);
    }

    public class BooleanContext
    {
        private readonly Dictionary<string, bool> _variables = new();

        public void SetVariable(string name, bool value)
        {
            _variables[name] = value;
            Console.WriteLine($"Set boolean variable {name} = {value}");
        }

        public bool GetVariable(string name)
        {
            if (_variables.TryGetValue(name, out bool value))
            {
                return value;
            }
            throw new ArgumentException($"Boolean variable '{name}' not found");
        }
    }

    public class BooleanConstant : IBooleanExpression
    {
        private readonly bool _value;

        public BooleanConstant(bool value)
        {
            _value = value;
        }

        public bool Interpret(BooleanContext context)
        {
            Console.WriteLine($"Interpreting boolean constant: {_value}");
            return _value;
        }
    }

    public class BooleanVariable : IBooleanExpression
    {
        private readonly string _variableName;

        public BooleanVariable(string variableName)
        {
            _variableName = variableName;
        }

        public bool Interpret(BooleanContext context)
        {
            Console.WriteLine($"Interpreting boolean variable: {_variableName}");
            return context.GetVariable(_variableName);
        }
    }

    public class AndExpression : IBooleanExpression
    {
        private readonly IBooleanExpression _left;
        private readonly IBooleanExpression _right;

        public AndExpression(IBooleanExpression left, IBooleanExpression right)
        {
            _left = left;
            _right = right;
        }

        public bool Interpret(BooleanContext context)
        {
            bool leftValue = _left.Interpret(context);
            bool rightValue = _right.Interpret(context);
            bool result = leftValue && rightValue;
            Console.WriteLine($"AND operation: {leftValue} && {rightValue} = {result}");
            return result;
        }
    }

    public class OrExpression : IBooleanExpression
    {
        private readonly IBooleanExpression _left;
        private readonly IBooleanExpression _right;

        public OrExpression(IBooleanExpression left, IBooleanExpression right)
        {
            _left = left;
            _right = right;
        }

        public bool Interpret(BooleanContext context)
        {
            bool leftValue = _left.Interpret(context);
            bool rightValue = _right.Interpret(context);
            bool result = leftValue || rightValue;
            Console.WriteLine($"OR operation: {leftValue} || {rightValue} = {result}");
            return result;
        }
    }

    public class NotExpression : IBooleanExpression
    {
        private readonly IBooleanExpression _expression;

        public NotExpression(IBooleanExpression expression)
        {
            _expression = expression;
        }

        public bool Interpret(BooleanContext context)
        {
            bool value = _expression.Interpret(context);
            bool result = !value;
            Console.WriteLine($"NOT operation: !{value} = {result}");
            return result;
        }
    }

    // SQL-like query interpreter
    public class SqlContext
    {
        public List<Dictionary<string, object>> Table { get; } = new();

        public void AddRow(Dictionary<string, object> row)
        {
            Table.Add(row);
        }

        public void PrintTable()
        {
            if (Table.Count == 0)
            {
                Console.WriteLine("Table is empty");
                return;
            }

            var columns = Table[0].Keys.ToList();
            
            // Print header
            Console.WriteLine(string.Join(" | ", columns.Select(c => c.PadRight(10))));
            Console.WriteLine(new string('-', columns.Count * 13 - 3));

            // Print rows
            foreach (var row in Table)
            {
                Console.WriteLine(string.Join(" | ", columns.Select(c => row[c].ToString()!.PadRight(10))));
            }
        }
    }

    public interface ISqlExpression
    {
        List<Dictionary<string, object>> Interpret(SqlContext context);
    }

    public class SelectAllExpression : ISqlExpression
    {
        public List<Dictionary<string, object>> Interpret(SqlContext context)
        {
            Console.WriteLine("Executing SELECT * FROM table");
            return new List<Dictionary<string, object>>(context.Table);
        }
    }

    public class WhereExpression : ISqlExpression
    {
        private readonly ISqlExpression _source;
        private readonly string _column;
        private readonly object _value;
        private readonly string _operator;

        public WhereExpression(ISqlExpression source, string column, string op, object value)
        {
            _source = source;
            _column = column;
            _operator = op;
            _value = value;
        }

        public List<Dictionary<string, object>> Interpret(SqlContext context)
        {
            Console.WriteLine($"Executing WHERE {_column} {_operator} {_value}");
            var sourceData = _source.Interpret(context);
            var result = new List<Dictionary<string, object>>();

            foreach (var row in sourceData)
            {
                if (row.ContainsKey(_column))
                {
                    bool matches = _operator switch
                    {
                        "=" => row[_column].Equals(_value),
                        ">" => Comparer<object>.Default.Compare(row[_column], _value) > 0,
                        "<" => Comparer<object>.Default.Compare(row[_column], _value) < 0,
                        ">=" => Comparer<object>.Default.Compare(row[_column], _value) >= 0,
                        "<=" => Comparer<object>.Default.Compare(row[_column], _value) <= 0,
                        "!=" => !row[_column].Equals(_value),
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
        private readonly ISqlExpression _source;
        private readonly string _column;
        private readonly bool _ascending;

        public OrderByExpression(ISqlExpression source, string column, bool ascending = true)
        {
            _source = source;
            _column = column;
            _ascending = ascending;
        }

        public List<Dictionary<string, object>> Interpret(SqlContext context)
        {
            Console.WriteLine($"Executing ORDER BY {_column} {(_ascending ? "ASC" : "DESC")}");
            var sourceData = _source.Interpret(context);
            
            var result = sourceData.OrderBy(row => row.ContainsKey(_column) ? row[_column] : null).ToList();
            if (!_ascending)
            {
                result.Reverse();
            }

            return result;
        }
    }

    // Simple scripting language interpreter
    public class ScriptContext
    {
        private readonly Dictionary<string, object> _variables = new();

        public void SetVariable(string name, object value)
        {
            _variables[name] = value;
            Console.WriteLine($"Script: Set {name} = {value}");
        }

        public object GetVariable(string name)
        {
            if (_variables.TryGetValue(name, out object? value))
            {
                return value;
            }
            throw new ArgumentException($"Script variable '{name}' not found");
        }

        public void PrintVariable(string name)
        {
            Console.WriteLine($"Script: {name} = {GetVariable(name)}");
        }
    }

    public interface IScriptCommand
    {
        void Execute(ScriptContext context);
    }

    public class AssignCommand : IScriptCommand
    {
        private readonly string _variableName;
        private readonly object _value;

        public AssignCommand(string variableName, object value)
        {
            _variableName = variableName;
            _value = value;
        }

        public void Execute(ScriptContext context)
        {
            context.SetVariable(_variableName, _value);
        }
    }

    public class PrintCommand : IScriptCommand
    {
        private readonly string _variableName;

        public PrintCommand(string variableName)
        {
            _variableName = variableName;
        }

        public void Execute(ScriptContext context)
        {
            context.PrintVariable(_variableName);
        }
    }

    public class AddCommand : IScriptCommand
    {
        private readonly string _resultVariable;
        private readonly string _leftVariable;
        private readonly string _rightVariable;

        public AddCommand(string resultVariable, string leftVariable, string rightVariable)
        {
            _resultVariable = resultVariable;
            _leftVariable = leftVariable;
            _rightVariable = rightVariable;
        }

        public void Execute(ScriptContext context)
        {
            var left = context.GetVariable(_leftVariable);
            var right = context.GetVariable(_rightVariable);
            
            if (left is int leftInt && right is int rightInt)
            {
                context.SetVariable(_resultVariable, leftInt + rightInt);
            }
            else if (left is string || right is string)
            {
                context.SetVariable(_resultVariable, left.ToString() + right.ToString());
            }
            else
            {
                throw new ArgumentException("Cannot add variables of these types");
            }
        }
    }

    public class ScriptInterpreter
    {
        public static List<IScriptCommand> ParseScript(string script)
        {
            var commands = new List<IScriptCommand>();
            var lines = script.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("//"))
                {
                    continue; // Skip empty lines and comments
                }

                var command = ParseCommand(trimmedLine);
                if (command != null)
                {
                    commands.Add(command);
                }
            }

            return commands;
        }

        private static IScriptCommand? ParseCommand(string commandLine)
        {
            var parts = commandLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length >= 3 && parts[1] == "=")
            {
                // Assignment: variable = value
                var variableName = parts[0];
                var value = ParseValue(parts[2]);
                return new AssignCommand(variableName, value);
            }
            else if (parts.Length == 2 && parts[0].ToLower() == "print")
            {
                // Print: print variable
                return new PrintCommand(parts[1]);
            }
            else if (parts.Length == 5 && parts[1] == "=" && parts[3] == "+")
            {
                // Addition: result = left + right
                return new AddCommand(parts[0], parts[2], parts[4]);
            }

            return null;
        }

        private static object ParseValue(string value)
        {
            if (int.TryParse(value, out int intValue))
            {
                return intValue;
            }
            if (value.StartsWith('"') && value.EndsWith('"') && value.Length >= 2)
            {
                return value[1..^1]; // Remove quotes
            }
            return value; // Treat as variable name
        }
    }
}
