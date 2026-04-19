using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CySiLang.CLI
{
    public static class StandardGrammer
    {
        public static class GerneralFunctions
        {
            public static void Exit(int code)
            {
                Program.RunTimer.Stop();
                Variable.JobStream.RunTimeCount = Program.RunTimer.ElapsedMilliseconds;
                Console.WriteLine();
                Console.WriteLine($"Run time {Variable.JobStream.RunTimeCount} ms");
                Environment.Exit(code);
            }
        }
        public static Dictionary<string, Dictionary<string, Dictionary<string, Func<string, string?, string?>>>> Grammar = new Dictionary<string, Dictionary<string, Dictionary<string, Func<string, string?, string?>>>>()
        {
            {
                "Slot",
                new Dictionary<string, Dictionary<string, Func<string, string?, string?>>>()
                {
                    {
                        "IntArray-EAX",
                        new Dictionary<string, Func<string, string?, string ?>>()
                        {/*
                                    {"Add",async (stdin, specin)=>{Variable.TempIntArrayVariableSlot.Add(int.Parse(stdin)); return Task.FromResult<string?>(stdin); } },
                             {"Find",async (stdin, specin)=>{return Task.FromResult<string?>(Variable.TempIntArrayVariableSlot.Index(stdin).ToString());  } }
                        */}
                    }
                }
            },
            {
                "System",
                new Dictionary<string, Dictionary<string, Func<string, string?, string?>>>()
                {
                    {
                        "LocationIP-Get",
                        new Dictionary<string, Func<string, string?, string?>>()
                        {
                            { "", (stdin, specin) => string.Empty },
                            { "Find", (stdin, specin) => $"{specin}" } //specin为"<>"里的内容，stdin为"()"里的内容
                        }
                    }
                }
            },
            {
                "End",
                new Dictionary<string, Dictionary<string, Func<string, string?, string?>>>()
                {
                    {
                        "Return",
                        new Dictionary<string, Func<string, string?, string?>>()
                        {
                            { "EnvirmentEnd", (stdin, specin) => { GerneralFunctions.Exit(0); return null; } }
                        }
                    }
                }
            },
            {
                "JobStream",
                new Dictionary<string, Dictionary<string, Func<string, string?, string?>>>()
                {
                    {
                        "Sleep",
                        new Dictionary<string, Func<string, string?, string?>>()
                        {
                            {
                                "",
                                (stdin, specin) =>
                                {
                                    int time = 0;
                                    if (int.TryParse(specin, out var timec)) time = timec;
                                    else if (int.TryParse(stdin, out var timei)) time = timei;
                                    else time = 1;
                                    Thread.Sleep(time);
                                    return null;
                                }
                            }
                        }
                    }
                }
            },
            {
                "IO",
                new Dictionary<string, Dictionary<string, Func<string, string?, string?>>>()
                {
                    {
                        "Print",
                        new Dictionary<string, Func<string, string?, string?>>()
                        {
                            {
                                "",
                                (stdin, specin) => { Console.Write(specin); return null; }
                            }
                        }
                    }
                }
            }
        };
    }
}
