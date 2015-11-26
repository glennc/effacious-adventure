using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using JsonConfigParser;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            Console.WriteLine("TokenizerTest: START");
            Console.WriteLine("TokenizerTest: RawTokenTest: START");
            var testString = "\"This is a string\" null false true \"another string\" { } [ ] ,";
            var times = new List<double>();
            for(int i = 0; i < 5; i++)
            {
                for (int j = 0;  j < 1000; j++)
                {
                    using(var stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(testString))))
                    {
                        var lexerUnderTest = new JsonLexer(stream);
                        var time = Time(() => {
                            Token token = null;
                            do
                            {
                                token = lexerUnderTest.GetNextToken();
                            } while(token.Type != TokenTypes.EOF);
                        });
                        times.Add(time.TotalMilliseconds);
                    }
                }
                Console.WriteLine($"TokenizerTest: {times.Average()}");
            }
            Console.WriteLine("TokenizerTest: RawTokenTest: STOP");
            Console.WriteLine("TokenizerTest: STOP");
        }
        
        public static TimeSpan Time(Action action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
}
