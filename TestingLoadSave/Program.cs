using Microsoft.Psi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingLoadSave
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("type 's' for creating and saving a store, type 'l' to load a store");
                char input = (char)Console.Read();
                Console.ReadLine();

                if (input == 's')
                {
                    using (var p = Pipeline.Create())
                    {
                        var store1 = Store.Create(p, "loadSaveTesting1", @"C:\Users\11702349\Documents\3tin\stage\psiStroreRoomData");
                        var store2 = Store.Create(p, "loadSaveTesting2", @"C:\Users\11702349\Documents\3tin\stage\psiStroreRoomData");

                        var sequence1 = Generators.Sequence(p, 0d, x => x + 0.2, 100, TimeSpan.FromMilliseconds(100));
                        sequence1
                            .Select(t => Math.Sin(t))
                            .Do(t => Console.WriteLine($"Sin: {t}"));
                        var sin1 = sequence1.Select(t => Math.Sin(t));

                        sequence1.Write("Sequence1", store1);
                        sin1.Write("Sin1", store1);

                        var sequence2 = Generators.Sequence(p, 0d, x => x + 0.1, 100, TimeSpan.FromMilliseconds(100));
                        sequence2
                            .Select(t => Math.Sin(t))
                            .Do(t => Console.WriteLine($"Sin: {t}"));
                        var sin2 = sequence2.Select(t => Math.Sin(t));

                        sequence2.Write("Sequence2", store2);
                        sin2.Write("Sin2", store2);

                        p.Run();
                    }

                }

                else if (input == 'l')
                {
                    using (var p = Pipeline.Create())
                    {
                        var store1 = Store.Open(p, "loadSaveTesting1", @"C:\Users\11702349\Documents\3tin\stage\psiStroreRoomData");
                        var store2 = Store.Open(p, "loadSaveTesting2", @"C:\Users\11702349\Documents\3tin\stage\psiStroreRoomData");

                        var sin1 = store1.OpenStream<double>("Sin1");
                        sin1.Do((x,e) => Console.WriteLine("sin1 - value: " + x + "    time: " + e.OriginatingTime));

                        var sin2 = store2.OpenStream<double>("Sin2");
                        sin2.Do((x, e) => Console.WriteLine("sin2 - value: " + x + "    time: " + e.OriginatingTime));

                        var joinedSins = sin1.Out.Join(sin2.Out);

                        joinedSins.Out.Process<(double, double), double>(
                            (x, e, o) =>
                            {
                                Console.WriteLine("sin1: " + x.Item1);
                                Console.WriteLine("sin2: " + x.Item2);
                                var sum = x.Item1 + x.Item2;
                                Console.WriteLine("sum: " + sum);

                                o.Post(sum, e.OriginatingTime);
                            });
                        p.Run();
                    }
                }
            }
        }
            
    }
}
