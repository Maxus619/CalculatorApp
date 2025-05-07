using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CalculatorApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CalculatorForm());
        }
    }

    // Использование паттерна Стратегия
    /// <summary>
    /// Абстрактный класс стратегий операторов
    /// </summary>
    public abstract class OperatorStrategy
    {
        public abstract int Priority { get; }
        public abstract bool IsLeftAssociative { get; }
        public abstract double Execute(double a, double b);
    }

    /// <summary>
    /// Стратегия сложения
    /// </summary>
    public class AddStrategy : OperatorStrategy
    {
        public override int Priority => 1;
        public override bool IsLeftAssociative => true;
        public override double Execute(double a, double b) => a + b;
    }

    /// <summary>
    /// Стратегия вычитания
    /// </summary>
    public class SubtractStrategy : OperatorStrategy
    {
        public override int Priority => 1;
        public override bool IsLeftAssociative => true;
        public override double Execute(double a, double b) => a - b;
    }

    /// <summary>
    /// Стратегия умножения
    /// </summary>
    public class MultiplyStrategy : OperatorStrategy
    {
        public override int Priority => 2;
        public override bool IsLeftAssociative => true;
        public override double Execute(double a, double b) => a * b;
    }
    
    /// <summary>
    /// Стратегия деления
    /// </summary>
    public class DivideStrategy : OperatorStrategy
    {
        public override int Priority => 2;
        public override bool IsLeftAssociative => true;
        public override double Execute(double a, double b)
        {
            if (b == 0)
                throw new DivideByZeroException();
            return a / b;
        }
    }

    // Использование паттерна Фабрика
    public static class OperatorFactory
    {
        private static readonly Dictionary<string, OperatorStrategy> strategies = new()
        {
            { "+", new AddStrategy() },
            { "-", new SubtractStrategy() },
            { "*", new MultiplyStrategy() },
            { "/", new DivideStrategy() }
        };

        public static bool IsOperator(string token) => strategies.ContainsKey(token);
        public static OperatorStrategy GetStrategy(string token) => strategies[token];
    }

    // Использование алгоритма сортировочной станции
    public static class ExpressionEvaluator
    {
        public static double Evaluate(string expression)
        {
            var outputQueue = new Queue<string>();
            var operatorStack = new Stack<string>();

            var tokens = Tokenize(expression);

            foreach (var token in tokens)
            {
                if (double.TryParse(token, out _))
                {
                    outputQueue.Enqueue(token);
                }
                else if (OperatorFactory.IsOperator(token))
                {
                    var op1 = OperatorFactory.GetStrategy(token);
                    while (operatorStack.Count > 0 && OperatorFactory.IsOperator(operatorStack.Peek()))
                    {
                        var op2 = OperatorFactory.GetStrategy(operatorStack.Peek());
                        if ((op1.IsLeftAssociative && op1.Priority <= op2.Priority) ||
                            (!op1.IsLeftAssociative && op1.Priority < op2.Priority))
                        {
                            outputQueue.Enqueue(operatorStack.Pop());
                            continue;
                        }
                        break;
                    }
                    operatorStack.Push(token);
                }
                else if (token == "(")
                {
                    operatorStack.Push(token);
                }
                else if (token == ")")
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                        outputQueue.Enqueue(operatorStack.Pop());
                    if (operatorStack.Count == 0 || operatorStack.Pop() != "(")
                        throw new FormatException("Несоответствующие скобки");
                }
                else
                {
                    throw new FormatException($"Неизвестный токен: {token}");
                }
            }

            while (operatorStack.Count > 0)
            {
                var op = operatorStack.Pop();
                if (op == "(" || op == ")")
                    throw new FormatException("Несоответствующие скобки");
                outputQueue.Enqueue(op);
            }

            var evaluationStack = new Stack<double>();
            while (outputQueue.Count > 0)
            {
                var tk = outputQueue.Dequeue();
                if (double.TryParse(tk, out double val))
                {
                    evaluationStack.Push(val);
                }
                else if (OperatorFactory.IsOperator(tk))
                {
                    if (evaluationStack.Count < 2)
                        throw new FormatException("Недостаточные значения");
                    double b = evaluationStack.Pop();
                    double a = evaluationStack.Pop();
                    var result = OperatorFactory.GetStrategy(tk).Execute(a, b);
                    evaluationStack.Push(result);
                }
            }

            if (evaluationStack.Count != 1)
                throw new FormatException("Некорректное выражение");

            return evaluationStack.Pop();
        }

        private static IEnumerable<string> Tokenize(string expr)
        {
            var tokens = new List<string>();
            var num = string.Empty;

            foreach (char c in expr)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    num += c;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(num))
                    {
                        tokens.Add(num);
                        num = string.Empty;
                    }
                    if ("+-*/()".Contains(c))
                        tokens.Add(c.ToString());
                    else if (!char.IsWhiteSpace(c))
                        throw new FormatException($"Неверный знак: {c}");
                }
            }
            if (!string.IsNullOrWhiteSpace(num))
                tokens.Add(num);

            return tokens;
        }
    }
}
