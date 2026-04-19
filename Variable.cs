using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CySiLang.CLI
{
    public static class Variable
    {
        public static Dictionary<string,string> VariableDictionary = new Dictionary<string, string>();
        public static string[] TempCodeVariableSlot = new string[65535];
        public static string TempStringVariableSlot_1 = "";
        public static string TempStringVariableSlot_2 = "";
        public static int[] TempIntArrayVariableSlot = new int[255];
        public static int TempIntVariableSlot = 0;
        public static class JobStream
        {
            public static long PC;
            public static long RunTimeCount;
        }
        public static class BoolSlot
        {
            public static bool Deflault = false;
            public static bool Error = false;
            public static bool CLIError = false;
            public static bool DevEnvExist = false;
            public static bool InternetEnabled = false;
            public static bool[] SpecialBoolSlot = new bool[15];
            public static bool[] EBoolSlot = new bool[256];
        }
    }
}
