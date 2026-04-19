using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CySiLang.CLI
{
    public class EditRW
    {
        public void Edit(string? path_e)//path_e is Path-Enough
        {
            Console.WriteLine("Message: Edit Mode (Read-Write)");
            Console.WriteLine("Message: Type '/help' for help.");
            Console.WriteLine("Message: Type '/exit' to exit.");
            StringBuilder Text_Temp = new StringBuilder();
            TextReader tr = new StreamReader(path_e);
            int Up_LineCount = 0;
            var Lines = tr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            int LineCount = Lines.Length;
            string CheckLock = Lines[0];
            tr.Close();
            if (CheckLock.StartsWith("@CySiLang_REwrite[locked]@CanTypewrite"))
            {
                Console.WriteLine("Message: The file is locked. Do you want to read the text? (Y/N)");
                string? Read_Choice = Console.ReadLine() ?? "y";
                if (Read_Choice.ToLower() == "y")
                {
                    Console.WriteLine("Message: The text is:");
                    StreamReader sr = new StreamReader(path_e);
                    Console.WriteLine(sr.ReadToEnd().Substring("@CySiLang_REwrite[locked]@CanTypewrite".Length));

                }
                else
                {
                    Console.WriteLine("Message: Exiting without reading.");
                }
                Console.WriteLine("Message: Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else if (CheckLock.StartsWith("@CySiLang_REwrite[locked]@CanNotTypewrite"))
            {
                Console.WriteLine("Message: The file is locked and cannot be edited. You can not read the text. Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else if (CheckLock.StartsWith("@CySiLang_REwrite[locked]@TipText"))
            {
                Console.WriteLine("Message: The file is locked. The text is:");
                Console.WriteLine(CheckLock.Substring("@CySiLang_REwrite[locked]@TipText".Length));
                Console.WriteLine("Message: Press any key to exit.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            StringBuilder TextInput = new StringBuilder();
            int intoLineCount = 0;
            bool PrintOrg=false;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Lines[0]+ Environment.NewLine);
            Console.ResetColor();
            while (true)
            {
                Up_LineCount++;
                if (PrintOrg)
                {
                    if (intoLineCount < Lines.Length)
                    {
                        Up_LineCount--;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(Lines[intoLineCount]+Environment.NewLine);
                        Console.ResetColor();
                    }
                    PrintOrg = false;
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[{Up_LineCount}|{LineCount}|{intoLineCount}]>>");
                Console.ResetColor();
                string? Input = Console.ReadLine() ?? "@[CySiLang_NULLinput]_Replace_NULLinput_[SrcInfoOfCySiLang]";
                if (Input.ToLower() == "/help")
                {
                    Console.WriteLine("Message: Edit Mode (Read-Write) Help");
                    Console.WriteLine("Message: Type '/help' for help.");
                    Console.WriteLine("Message: Type '/exit' to exit.");
                    Console.WriteLine("Message: Type '/clear' to clear the text.");
                    Console.WriteLine("Message: Type '/wexit' to exit and save the text.");
                    Console.WriteLine("Message: Type '/lockcre' to lock the file and save the text and exit. (The file will be locked and can not be edited, but can be read.)");
                    Console.WriteLine("Message: Type '/lockno' to lock the file and save the text and exit. (The file will be locked and can not be edited, and can not be read.)");
                    Console.WriteLine("Message: Type '/lockwt' to lock the file with a tip and save the text and exit. (The file will be locked and can not be edited, and can not be read. The tip will be shown at the beginning of the text.)");
                    Console.WriteLine("Tip: '[Up_LineCount|LineCount|intoLineCount]>>' means the current line number, total line number and the line number that has been read into the text.");
                    Console.WriteLine("About Lock: Locking the file will add a lock tag at the beginning of the file. The lock tag can be '@CySiLang_REwrite[locked]@CanTypewrite' or '@CySiLang_REwrite[locked]@CanNotTypewrite'. The former means the file is locked but can be read, while the latter means the file is locked and cannot be read.");
                    Console.WriteLine("About Lock with Tip: Locking the file with a tip will add a lock tag with a tip at the beginning of the file. The lock tag can be '@CySiLang_REwrite[locked]@TipText' followed by the tip text. The former means the file is locked and cannot be read, but the tip will be shown at the beginning of the text.");
                    Console.WriteLine("About \"[{Up_LineCount}|{LineCount}|{intoLineCount}]>>\" in the console: 'Up_LineCount' =>'Updated line count'.");
                    Console.WriteLine("Message: Press any key to continue.");
                    Console.ReadKey();
                    continue;
                }
                else if (Input.ToLower() == "/exit")
                {
                    Console.WriteLine("Message:Do you want to save the text? (Y/N)");
                    string? Save_Choice = Console.ReadLine() ?? "y";
                    if (Save_Choice.ToLower() == "y")
                    {
                        if (intoLineCount > Lines.Length)
                        {
                            Console.WriteLine("Message: No more lines to read. Saving the text.");
                        }
                        else
                        {
                            for (int i = intoLineCount; i < Lines.Length; i++)
                            {
                                TextInput.AppendLine(Lines[i]);
                            }
                        }
                        using (StreamWriter sw = new StreamWriter(path_e, false))
                        {
                            sw.Write(TextInput.ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Message: Exiting without saving.");
                    }
                    break;
                }
                else if (Input.ToLower() == "/wexit")
                {
                    if (intoLineCount > Lines.Length)
                    {
                        Console.WriteLine("Message: No more lines to read. Saving the text.");
                    }
                    else
                    {
                        for (int i = intoLineCount; i < Lines.Length; i++)
                        {
                            TextInput.AppendLine(Lines[i]);
                        }
                    }
                    using (StreamWriter sw = new StreamWriter(path_e, false))
                    {
                        sw.Write(TextInput.ToString());
                    }
                    break;
                }
                else if (Input.ToLower() == "/clear")
                {
                    TextInput.Clear();
                    Console.WriteLine("Message: Text cleared.");
                    intoLineCount = 0;
                }
                else if (Input.ToLower() == "/lockcre")
                {
                    TextInput.Insert(0, "@CySiLang_REwrite[locked]@CanTypewrite" + Environment.NewLine);
                    for (int i = intoLineCount; i < Lines.Length; i++)
                    {
                        TextInput.AppendLine(Lines[i]);
                    }
                    using (StreamWriter sw = new StreamWriter(path_e, false))
                    {
                        sw.Write(TextInput.ToString());
                    }
                    Console.WriteLine("Message: File locked and saved. Exiting.");
                    break;
                }
                else if (Input.ToLower() == "/lockno")
                {
                    TextInput.Insert(0, "@CySiLang_REwrite[locked]@CanNotTypewrite" + Environment.NewLine);
                    for (int i = intoLineCount; i < Lines.Length; i++)
                    {
                        TextInput.AppendLine(Lines[i]);
                    }
                    using (StreamWriter sw = new StreamWriter(path_e, false))
                    {
                        sw.Write(TextInput.ToString());
                    }
                    Console.WriteLine("Message: File locked and saved. Exiting.");
                    break;
                }
                else if (Input.ToLower() == "/lockwt")
                {
                    Console.WriteLine("Message: Please enter the tip text:");
                    string? Tip_Text = Console.ReadLine() ?? "This file is locked.";
                    TextInput.Insert(0, "@CySiLang_REwrite[locked]@TipText" + Tip_Text + Environment.NewLine);
                    for (int i = intoLineCount; i < Lines.Length; i++)
                    {
                        TextInput.AppendLine(Lines[i]);
                    }
                    using (StreamWriter sw = new StreamWriter(path_e, false))
                    {
                        sw.Write(TextInput.ToString());
                    }
                    Console.WriteLine("Message: File locked with tip and saved. Exiting.");
                    break;
                }
                else if (string.IsNullOrEmpty(Input)||Input.StartsWith("@[CySiLang_NULLinput]_Replace_NULLinput_[SrcInfoOfCySiLang]"))
                {
                    if (intoLineCount < Lines.Length)
                    {
                        TextInput.AppendLine(Lines[intoLineCount]);
                        PrintOrg = true;
                        intoLineCount++;
                        continue;
                    }
                    else
                    {
                        intoLineCount++;
                        TextInput.AppendLine();
                    }
                }
                else
                {
                    intoLineCount++;
                    TextInput.AppendLine(Input);
                }
            }
        }
    }
}
