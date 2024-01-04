using System.Net;

namespace Expression_Evaluation;

class UserVariable
{
    private string? _value;
    private char? _symbol;

    public string? GetValue()
    {
        return _value;
    }

    public char? GetSymbol()
    {
        return _symbol;
    }

    public UserVariable(string value, char symbol)
    {
        _value = value;
        _symbol = symbol;
    }


    public static List<UserVariable> CreateList(List<UserVariable> variables)
    {
        const string reservedCharacters = "1234567890^+-/*<>=";
        Console.WriteLine("Input number of variables you would like to create");
        int numOfVariables;
        while (!int.TryParse(Console.ReadLine(), out numOfVariables))
            Console.WriteLine("Please input valid option");
        for (var i = 0; i < numOfVariables; i++)
        {
            {
                Console.WriteLine("Please input variable value (T/F for true and false)");
                var value = Console.ReadLine();
                while (value == null)
                {
                    Console.WriteLine("Please input valid option");
                    value = Console.ReadLine();
                }

                Console.WriteLine("Please input variable symbol");
                char symbol = Convert.ToChar(Console.ReadLine()!);
                while (symbol == null || reservedCharacters.Contains(symbol))
                {
                    Console.WriteLine("Please input valid option");
                    symbol = Convert.ToChar(Console.ReadLine()!);
                }

                var userVariable = new UserVariable(value, symbol);
                variables.Add(userVariable);
            }

        }

        return variables;
    }

    class ArithmeticOperator
    {
        private string? _symbol;
        private int? _priority;

        private ArithmeticOperator(string symbol, int priority)
        {
            _symbol = symbol;
            _priority = priority;
        }

        private static ArithmeticOperator _power = new ArithmeticOperator("^", 3);
        private static ArithmeticOperator _division = new ArithmeticOperator("/", 2);
        private static ArithmeticOperator _multiplication = new ArithmeticOperator("*", 2);
        private static ArithmeticOperator _addition = new ArithmeticOperator("+", 1);
        private static ArithmeticOperator _subtraction = new ArithmeticOperator("-", 1);

        private static List<ArithmeticOperator> _operators =
            [_power, _division, _multiplication, _addition, _subtraction];



        public string? GetSymbol()
        {
            return _symbol;
        }

        public int? GetPriority()
        {
            return _priority;
        }

        public static List<ArithmeticOperator> GetOperators()
        {
            return _operators;
        }
    }
    
    class ComparitiveOperator
    {
        private string? _symbol;

        private ComparitiveOperator(string symbol)
        {
            _symbol = symbol;
        }

        private static ComparitiveOperator _Less = new ComparitiveOperator("<");
        private static ComparitiveOperator _LessEqual = new ComparitiveOperator("\u00a3");
        private static ComparitiveOperator _Great = new ComparitiveOperator(">");
        private static ComparitiveOperator _GreatEqual = new ComparitiveOperator("\u00b3");
        private static ComparitiveOperator _Equal = new ComparitiveOperator("=");
        private static ComparitiveOperator _NotEqual = new ComparitiveOperator("\u00b9");

        private static List<ComparitiveOperator> _operators =
            [_Less, _LessEqual, _Great, _GreatEqual, _Equal, _NotEqual];



        public string? GetSymbol()
        {
            return _symbol;
        }

        public static List<ComparitiveOperator> GetOperators()
        {
            return _operators;
        }
    }

    class LogicOperator
    {
        private string? _symbol;
        private int? _priority;

        private LogicOperator(string symbol, int priority)
        {
            _symbol = symbol;
            _priority = priority;
        }

        private static LogicOperator _not = new LogicOperator("!", 3);
        private static LogicOperator _and = new LogicOperator("&", 2);
        private static LogicOperator _or = new LogicOperator("|", 1);

        private static List<LogicOperator> _operators = [_not, _and, _or];



        public string? GetSymbol()
        {
            return _symbol;
        }

        public int? GetPriority()
        {
            return _priority;
        }

        public static List<LogicOperator> GetOperators()
        {
            return _operators;
        }
    }

    internal static class Menu
    {
        public static void Main(string[] args)
        {
            var exit = true;
            var variables = new List<UserVariable>();
            Stack<char> operators = new Stack<char>();
            Stack<double> operands = new Stack<double>();
            Stack<string> arguements = new Stack<string>();
            while (exit)
            {
                operators.Clear();
                operands.Clear();
                arguements.Clear();
                Console.WriteLine("1: Edit Variable");
                Console.WriteLine("2: Resolve Arithmetic Expression");
                Console.WriteLine("3: Resolve Logic Expression");
                Console.WriteLine("4: Show Variables");
                Console.WriteLine("5: Exit");
                int input;
                while (!int.TryParse(Console.ReadLine(), out input))
                    Console.WriteLine("Please input valid option");
                switch (input)
                {
                    case 1:
                        EditVariables(variables);
                        break;
                    case 2:
                        ArithmeticOperation(variables, operators, operands);
                        break;
                    case 3:
                        LogicOperation(variables, operators, arguements);
                        break;
                    case 4:
                        ShowVariables(variables);
                        break;
                    case 5:
                        exit = false;
                        break;
                }
            }
        }

        private static void ArithmeticOperation(List<UserVariable> variables, Stack<char> operators,
            Stack<double> operands)
        {
            Console.WriteLine("Please input your expression");
            var expression = Console.ReadLine();
            const string allowedCharacters = "+-/*^";
            expression = string.Concat(expression!.Where(c => !char.IsWhiteSpace(c)));
            expression += " ";
            while (expression == null || allowedCharacters.Contains(expression))
            {
                Console.WriteLine("Please input valid option");
                expression = Console.ReadLine();
            }

            string number = "";
            if (expression[0] == '+' || expression[0] == '-')
            {
                number += expression[0];
            }

            foreach (char c in expression)
            {
                if (char.IsDigit(c))
                {
                    number += c;
                }
                else if (IsSymbol(variables, c))
                {
                    number += SymbolValue(variables, c)!;
                }
                else
                {
                    foreach (var op in ArithmeticOperator.GetOperators())
                    {
                        if (c.ToString() == op.GetSymbol())
                        {
                            operands.Push(double.Parse(number));
                            number = "";
                            if (operators.Count() == 0)
                            {
                                operators.Push(c);
                            }

                            else if (op.GetPriority() <= PeekPriority(operators))
                            {
                                operands = ResolveArithmetic(operands, operators, c);
                            }
                        }
                    }

                }

            }
            operands.Push(double.Parse(number));
            while (operators.Count != 0)
            {
                char temp = ' ';
                operands = ResolveArithmetic(operands, operators, temp);
                
            }
            Console.WriteLine(operands.Peek());
        }



    }

    private static int? PeekPriority(Stack<char> operators)
    {
        char symbol = operators.Peek();
        foreach (var op in ArithmeticOperator.GetOperators())
        {
            if (symbol.ToString() == op.GetSymbol())
            {
                return op.GetPriority();
            }
        }
        return 0;
    }


private static Stack<double> ResolveArithmetic(Stack<double> operands, Stack<char> operators, char c)
         {
             double num1 = operands.Peek();
             operands.Pop();
             double num2 = operands.Peek();
             operands.Pop();
             switch (operators.Peek())
             {
                 case '+':
                     operands.Push(num2 + num1);
                     operators.Pop();
                     if(c != ' '){operators.Push(c);}
                     break;

                 case '-':
                     operands.Push(num2 - num1);
                     operators.Pop();
                     if(c != ' '){operators.Push(c);}
                     break;

                 case '/':
                     operands.Push(num2 / num1);
                     operators.Pop();
                     if(c != ' '){operators.Push(c);}
                     break;

                 case '*':
                     operands.Push(num2 * num1);
                     operators.Pop();
                     if(c != ' '){operators.Push(c);}
                     break;
                 case '^':
                     operands.Push((Math.Pow(num2, num1)));
                     operators.Pop();
                     if(c != ' '){operators.Push(c);}
                     break;
             }

             return operands;
         }

     private static string? SymbolValue(List<UserVariable> variables, char c)
    {
        foreach (var variable in variables)
        {
            if (variable.GetSymbol() == c)
            {
                return variable.GetValue();
            }
        }

        return "0";
    }

    private static bool IsSymbol(List<UserVariable> variables, char c)
    {
        foreach (var variable in variables)
        {
            if (variable.GetSymbol() == c)
            {
                return true;
            }
        }

        return false;
    }

    private static void LogicOperation(List<UserVariable> variables, Stack<char> operators, Stack<string> arguements)
    {
        Console.WriteLine("Please input your expression");
             var expression = Console.ReadLine();
             expression = string.Concat(expression!.Where(c => !char.IsWhiteSpace(c)));
             expression += " ";
             while (expression == null)
             {
                 Console.WriteLine("Please input valid option");
                 expression = Console.ReadLine();
             }

             string symbol = "";
             if (expression[0] == '+' || expression[0] == '-')
             {
                 symbol += expression[0];
             }

             foreach (char c in expression)
             {
                 if (c == 'T' || c == 'F')
                 {
                     symbol += c;
                 }
                 else if (IsSymbol(variables, c))
                 {
                     symbol += SymbolValue(variables, c)!;
                 }
                 else
                 {
                     foreach (var op in LogicOperator.GetOperators())
                     {
                         if (c.ToString() == op.GetSymbol())
                         {
                             arguements.Push(symbol);
                             symbol = "";
                             if (operators.Count() == 0)
                             {
                                 operators.Push(c);
                             }

                             else if (op.GetPriority() <= PeekLogicPriority(operators))
                             {
                                 arguements = ResolveLogic(arguements, operators, c);
                             }
                         }
                     }

                 }

             }
             arguements.Push(symbol);
             while (operators.Count != 0)
             {
                 char temp = ' ';
                 arguements = ResolveLogic(arguements, operators, temp);
                
             }
             Console.WriteLine(arguements.Peek());
    }
    
    private static int? PeekLogicPriority(Stack<char> arguements)
    {
        char symbol = arguements.Peek();
        foreach (var op in LogicOperator.GetOperators())
        {
            if (symbol.ToString() == op.GetSymbol())
            {
                return op.GetPriority();
            }
        }
        return 0;
    }

    private static Stack<string> ResolveLogic(Stack<string> arguements, Stack<char> operators, char c)
    {
        string arg1 = arguements.Peek();
        arguements.Pop();
        string arg2 = arguements.Peek();
        
        switch (operators.Peek())
        {
            case '!':
                if (arg1 == "T")
                {
                    arguements.Push("F");
                }
                else
                {
                    arguements.Push("T");
                }
                operators.Push(c);
                break;
                 
            case '&':
                arguements.Pop();
                if (arg1 == "T" & arg2 == "T")
                {
                    arguements.Push("T");
                }
                else
                {
                    arguements.Push("F");
                }
                operators.Push(c);
                break;
                 
            case '|':
                arguements.Pop();
                if (arg1 == "T" | arg2 == "T")
                {
                    arguements.Push("T");
                }
                else
                {
                    arguements.Push("F");
                }
                operators.Push(c);
                break;
        }

        return arguements;
    }

    private static void ShowVariables(List<UserVariable> variables)
    {
        foreach (var variable in variables) {
            Console.WriteLine("Symbol is {1} and value is {0}", variable.GetValue(), variable.GetSymbol());
        }
    }

    static List<UserVariable> EditVariables(List<UserVariable> variables)
    {
        Console.WriteLine("1: Create Variables");
        Console.WriteLine("2: Clear Variables");
        int input;
        while (!int.TryParse(Console.ReadLine(), out input) || input > 2 || input < 1)
            Console.WriteLine("Please input valid option");
        if (input == 1)
        {
            CreateList(variables);
            return variables;
        }
        variables.Clear();
        return variables;

    }


}


