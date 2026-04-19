using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;

namespace CySiLang.CLI
{
    /// <summary>
    /// Program Language CLI
    /// Language: Cyclic single-threaded command-configured fast temporary programming language
    /// </summary>
    public static class Program
    {
        public static System.Diagnostics.Stopwatch RunTimer = new();
        public static async Task CLIContractBus(string[] args) 
        {
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                StandardGrammer.GerneralFunctions.Exit(0);
            };
            if (args == null || args.Length == 0) throw new ArgumentException("Please input enough arguments");
            //await Run(args[0]);
            async Task Run(string FilePath)
            {
                try
                {
                    if (!File.Exists(FilePath)) throw new FileNotFoundException($"File not found: {FilePath}");
                    var AllChar = File.ReadAllLines(FilePath,Encoding.UTF8);
                    List<string> CodeLines = new List<string>();
                    foreach (string line in AllChar)
                    {
                        try
                        {
                            if (line.StartsWith("[") && line.EndsWith("]"))
                            {
                                if (line.Length > 2)
                                {
                                    char Command = line[1];
                                    if (char.IsUpper(Command) && !line.StartsWith("[[") && !line.EndsWith("]]") && line.Contains("::"))
                                    {
                                        CodeLines.Add(line);
                                    }
                                    else throw new Exception("Error:Syntax is invalid");
                                }
                                else throw new Exception("Error:Syntax is invalid");
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Warning:Skipping invalid line: " + line);
                        }
                    }
                    if (CodeLines.Count == 0) throw new Exception("Error:No code found.");
                    RunTimer.Start();
                    while (true)
                    {
                        try
                        {
                            foreach (string line in CodeLines)
                            {
                                var Command = line.Substring(1, line.Length - 2);
                                var Token = Command.Split("::", 2);
                                if (Token.Length != 2) throw new Exception("Error:Syntax is invalid - '" + line+"'");
                                if (StandardGrammer.Grammar.TryGetValue(Token[0], out var Grammar))
                                {
                                    int AngleLeftCount = Token[1].Count(x => x == '<');
                                    if (AngleLeftCount > 1 || AngleLeftCount < 0) throw new Exception($"Error:Syntax is invalid - '{line}'");
                                    int AngleRightCount = Token[1].Count(x => x == '>');
                                    if (AngleRightCount > 1 || AngleRightCount < 0) throw new Exception($"Error:Syntax is invalid - '{line}'");
                                    int ParenthesesLeftCount = Token[1].Count(x => x == '(');
                                    if (ParenthesesLeftCount > 1 || ParenthesesLeftCount < 0) throw new Exception($"Error:Syntax is invalid - '{line}'");
                                    int ParenthesesRightCount = Token[1].Count(x => x == ')');
                                    if (ParenthesesRightCount > 1 || ParenthesesRightCount < 0) throw new Exception($"Error:Syntax is invalid - ' {line} '");
                                    string ResultValue = Token[1];
                                    Match AngleBracket = Regex.Match(Token[1], @"<(.*?)>");
                                    Match Parentheses = Regex.Match(Token[1], @"\((.*?)\)");
                                    string Angle = AngleBracket.Success ? AngleBracket.Groups[1].Value : String.Empty;
                                    ResultValue = Regex.Replace(ResultValue, @"<(.*?)>", "");
                                    string ParenthesesValue = Parentheses.Success ? Parentheses.Groups[1].Value : String.Empty;
                                    ResultValue = Regex.Replace(ResultValue, @"\((.*?)\)", "");
                                    if (!string.IsNullOrEmpty(Angle) && Variable.VariableDictionary.TryGetValue(Angle, out var Value)) Angle = Value;
                                    int VarPointCount = Regex.Matches(ResultValue, "=>").Count;
                                    if (VarPointCount > 1 || VarPointCount < 0) throw new Exception($"Error:Reassignment is not allowed - '{ResultValue}' in namespace '{Token[0]}'");
                                    int VarPointIndex = ResultValue.IndexOf("=>");
                                    string VarName = ResultValue.Substring(VarPointIndex + 2);
                                    ResultValue = Regex.Replace(ResultValue, "=>" + VarName, "");
                                    int PointCount = ResultValue.Count(x => x == '.');
                                    if (PointCount > 1 || PointCount < 0) throw new Exception($"Error:Syntax is invalid - '{ResultValue}' in namespace '{Token[0]}'");
                                    if (PointCount == 1)
                                    {
                                        var CodePart = ResultValue.Split('.');
                                        if (Grammar.TryGetValue(CodePart[0], out var Code))
                                        {
                                            if (Code.TryGetValue(CodePart[1], out var CodeProgram))
                                            {
                                                string CodeResult = CodeProgram(ParenthesesValue, Angle) ?? string.Empty;
                                                if (VarPointCount == 1)
                                                {
                                                    if (Variable.VariableDictionary.ContainsKey(VarName))
                                                    {
                                                        Variable.VariableDictionary[VarName] = CodeResult ?? string.Empty;
                                                    }
                                                    else
                                                    {
                                                        Variable.VariableDictionary.Add(VarName, CodeResult ?? string.Empty);
                                                    }
                                                }
                                            }
                                            else throw new Exception($"Error:TailSysntax is invalid - '{CodePart[1]}' in namespace '{Token[0]}'");
                                        }
                                        else throw new Exception($"Error:HeadSysntax is invalid - '{ResultValue}' in namespace '{Token[0]}'");
                                    }
                                    else
                                    {
                                        if (Grammar.TryGetValue(ResultValue, out var Code))
                                        {
                                            string CodeResult = Code[""](ParenthesesValue, Angle) ?? string.Empty;
                                            if (VarPointCount == 1)
                                            {
                                                if (Variable.VariableDictionary.ContainsKey(VarName))
                                                {
                                                    Variable.VariableDictionary[VarName] = CodeResult ?? string.Empty;
                                                }
                                                else
                                                {
                                                    Variable.VariableDictionary.Add(VarName, CodeResult ?? string.Empty);
                                                }
                                            }
                                        }
                                    }
                                }
                                else throw new Exception($"Error:Namespace {Token[0]} is invalid");

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ErrorMessage.Error(ex.ToString()));
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ErrorMessage.Error(ex.ToString()));
                }
            }
        }

        public static async Task MakeProgramPackage(string SourcePath, string OutputPath)
        {

        }
        public static async Task IDLE()
        {

        }
        public static async Task InitDev(string ProjectName,string ProjectPath)
        {
            if (!Directory.Exists(Path.Combine(ProjectPath, ProjectName)))
            {

            }
        }
        public static async Task FileDownloader(string Url,string SavePath)
        {

        }


        public static async Task Main(string[] args)
        {
            const string Version = "Ver[1.0.0]-alpha-build[00065]";
            if (args.Length <= 0)
            {
                var ConsoleCommand = new List<string>()
                {
                    "run",//直接运行脚本
                    "dicget",//获取库
                    "exit",//退出cli
                    "help",//展示帮助
                    "clear",//清理屏幕
                    "idle",//打开交互式脚本制作器
                    "rcpl",//同上
                    "mkpgpk",//打包一个有Packages.INI和Project.JSON的项目文件夹为一个此操作系统可执行文件
                    "initdev",//构造一个项目文件
                    "fdl"//文件下载器
                };
                Console.Title = "CySiLang - CLI";
                Console.OutputEncoding = Encoding.UTF8;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"All configuration items have been loaded. Welcome back, {Environment.UserName}.");
                Console.WriteLine("Cyclic single-threaded command-configured fast temporary programming language - CLI");
                Console.WriteLine(Version);
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("=<Tips>======================================");
                Console.WriteLine("Type 'exit' to exit.");
                Console.WriteLine("Type 'run' to run a CySiLang file.");
                Console.WriteLine("Type 'idle'or 'rcpl' to run the IDLE.");
                Console.WriteLine("Type 'dicget' to get the Extension library.");
                Console.WriteLine("Type 'help' to get the help.");
                Console.WriteLine("Type 'clear' to clear the console.");
                Console.WriteLine("=<Console Mode>==============================");
                Console.ResetColor();
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write($"CySiLang|{Version}> ");
                    Console.ResetColor();
                    string Input = Console.ReadLine() ?? string.Empty;
                    if (!string.IsNullOrEmpty(Input)&ConsoleCommand.Contains(Input.ToLower()))
                    {
                        CommandParser(Input.Split(' '));
                    }else continue;
                }
                void CommandParser(string[] args)
                {

                }
            }
            else
            {

            }
            
        }
    }
}
