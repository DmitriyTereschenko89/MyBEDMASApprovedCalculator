using System;
using System.Text.RegularExpressions;

namespace MyBEDMASApprovedCalculator
{
    internal class Program
    {
        public class MyBEDMASApprovedCalculator
        {
            private enum Operations
            {
                OpenBracket = -1,
                Addition,
                Subtraction = 0,
                Division = 1,
                Multiplication = 1,
                PowerOf
            }
            private class MathExpression
            {
                private readonly string infixForm;
                private readonly Dictionary<char, Operations> operatorsMap;

                public MathExpression(string infixForm)
                {
                    this.infixForm = infixForm.Replace(" ", "");
                    operatorsMap = new Dictionary<char, Operations>()
                    {
                        ['+'] = Operations.Addition,
                        ['-'] = Operations.Subtraction,
                        ['*'] = Operations.Multiplication,
                        ['/'] = Operations.Division,
                        ['^'] = Operations.PowerOf
                    };
                }

                private static string GetNumericString(string infixForm, ref int index)
                {
                    int startIndex = index;
                    while (index < infixForm.Length && (char.IsDigit(infixForm[index]) || infixForm[index] == '.'))
                    {
                        ++index;
                    }
                    return infixForm[startIndex..index];
                }
                
                public List<string> GetPostFixForm()
                {
                    List<string> postFixForm = new();
                    Stack<(char, Operations)> operators = new();
                    int index = 0;
                    while (index < infixForm.Length)
                    {
                        if (char.IsDigit(infixForm[index]))
                        {
                            postFixForm.Add(GetNumericString(infixForm, ref index));
                            continue;
                        }
                        if (operatorsMap.TryGetValue(infixForm[index], out Operations oper))
                        {
                            while (operators.Count > 0 && operators.Peek().Item2 >= oper)
                            {
                                postFixForm.Add(operators.Pop().Item1.ToString());
                            }
                            operators.Push((infixForm[index], oper));
                        }
                        else if (infixForm[index] == '(')
                        {
                            operators.Push((infixForm[index], Operations.OpenBracket));
                        }
                        else
                        {
                            while (operators.Count > 0 && operators.Peek().Item2 != Operations.OpenBracket)
                            {
                                postFixForm.Add(operators.Pop().Item1.ToString());
                            }
                            operators.Pop();
                        }
                        ++index;
                    }
                    while (operators.Count > 0)
                    {
                        postFixForm.Add(operators.Pop().Item1.ToString());
                    }
                    return postFixForm;
                }

                private static double ExecuteOperation(Stack<double> st, string node)
                {
                    double a;
                    double b;
                    switch (node)
                    {
                        case "+":
                            a = st.Pop();
                            b = st.Pop();
                            return a + b;
                        case "-":
                            a = st.Pop();
                            b = st.Pop();
                            return b - a;
                        case "*":
                            a = st.Pop();
                            b = st.Pop();
                            return a * b;
                        case "/":
                            a = st.Pop();
                            b = st.Pop();
                            return b / a;
                        case "^":
                            a = st.Pop();
                            b = st.Pop();
                            return Math.Pow(b, a);
                        default:
                            return 0;

                    }
                }

                public double Calculate()
                {
                    List<string> rpe = GetPostFixForm();
                    Stack<double> st = new();
                    foreach (string node in rpe)
                    {
                        if (double.TryParse(node, out double value))
                        {
                            st.Push(value);
                        }
                        else
                        {
                            st.Push(ExecuteOperation(st, node));
                        }
                    }
                    return st.Pop();
                }

            }

            public static void Calculate(string s)
            {
                string[] mathExpressions = Regex.Split(s, @"(?:\r\n)|(?:\r)|(?:\n)");
                foreach (var infixForm in mathExpressions)
                {
                    MathExpression mathExpression = new(infixForm);
                    Console.WriteLine(mathExpression.Calculate());
                }
            }
        } 
        static void Main(string[] args)
        {
            //Easy Tests
            MyBEDMASApprovedCalculator.Calculate("3 + 5\r\n5 + 41\r\n5 - 3\r\n5 - 5\r\n3 * 5\r\n2 * 23\r\n123 / 3\r\n22 / 1");
            //Medium Tests
            MyBEDMASApprovedCalculator.Calculate("3 + 5 * 2\r\n5 - 3 * 8 / 8\r\n6*(2 + 3)\r\n2 ^ 5\r\n5 ^0\r\n23.2- 15.2\r\n22 / 5");
            //Hardest Tests
            MyBEDMASApprovedCalculator.Calculate("3 * (4 +       (2 / 3) * 6 - 5)\r\n123      -( 4^ (       3 -   1) * 8 - 8      /(     1 + 1 ) *(3 -1) )\r\n4 + 2 * ( (226 - (5 * 3) ^ 2) ^ 2 + (10.7 - 7.4) ^ 2 - 6.89)\r\n (226 - (5 * 3) ^ 2) ^ 2");
            //Random Tests
            MyBEDMASApprovedCalculator.Calculate("3 + 5 * 2");
            MyBEDMASApprovedCalculator.Calculate("(95   + 75+  47)  * 47  *95");
            MyBEDMASApprovedCalculator.Calculate("3 * (4 +       (2 / 3) * 6 - 5)\r\n123      -( 4^ (       3 -   1) * 8 - 8      /(     1 + 1 ) *(3 -1) )");
            MyBEDMASApprovedCalculator.Calculate("4^(3-1)*8-8/(1+1)*(3-1)");
        }
    }
}
