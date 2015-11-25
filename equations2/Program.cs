using System;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Phantasmagorical computaitonal contraption. Enter an expression:");
            while(true)
            {
                try
                {
                    var text = Console.ReadLine();
                    var lexer = new Lexer(text);
                    var interpreter = new Parser(lexer);
                    var result = interpreter.Expr();
                    Console.WriteLine($"Result: {result.ToString()}");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
