using System;

namespace Sem_Projec
{
    class Analyzer
    {
        public static string CountKeywords(string code)
        {
            string output = "";
            output += "--- Keyword Usage ---\n";

            string[] delims = { " ", "\n", "\t", "(", ")", "{", "}", ";" };
            string[] tokens = code.Split(delims, StringSplitOptions.RemoveEmptyEntries);
            string[] targets = { "for", "while", "if", "int", "return" };

            foreach (string target in targets)
            {
                int count = 0;
                for (int i = 0; i < tokens.Length; i++)
                {
                    if (tokens[i] == target) count++;
                }
                if (count > 0) output += $"{target} : {count}\n";
            }
            return output;
        }

        public static string ShowStructure(string code)
        {
            string output = "";
            output += "\n--- Code Structure ---\n";

            string[] delims = { " ", "\n", "\t", "(", ")", "{", "}", ";" };
            string[] tokens = code.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            int indent = 0;

            foreach (var token in tokens)
            {
                if (token == "for" || token == "while" || token == "if")
                {
                    output += new string(' ', indent * 2) + "|_ " + token.ToUpper() + "\n";
                    indent++;
                }
            }
            return output;
        }

        public static string EstimateTimeComplexity(string code)
        {
            int[] layers = new int[10];
            int depth = 0;
            int nextType = 0;
            int maxN = 0;
            int maxLog = 0;

            for (int i = 0; i < code.Length; i++)
            {
                if (IsKeyword(code, i, "for") || IsKeyword(code, i, "while"))
                {
                    nextType = GetLoopType(code, i);
                }
                else if (code[i] == '{')
                {
                    depth++;
                    layers[depth] = nextType;
                    nextType = 0;
                    CheckMax(layers, depth, ref maxN, ref maxLog);
                }
                else if (code[i] == '}')
                {
                    if (depth > 0)
                    {
                        layers[depth] = 0;
                        depth--;
                    }
                }
            }
            return PrintResult(maxN, maxLog);
        }

        static int GetLoopType(string code, int index)
        {
            int start = code.IndexOf('(', index);
            int end = code.IndexOf(')', start);
            if (start == -1 || end == -1) return 1;

            string header = code.Substring(start, end - start);
            if (header.Contains("*=") || header.Contains("/=")) return 2;
            return 1;
        }

        static void CheckMax(int[] layers, int depth, ref int maxN, ref int maxLog)
        {
            int currentN = 0;
            int currentLog = 0;

            for (int k = 1; k <= depth; k++)
            {
                if (layers[k] == 1) currentN++;
                if (layers[k] == 2) currentLog++;
            }

            if (currentN > maxN)
            {
                maxN = currentN;
                maxLog = currentLog;
            }
            else if (currentN == maxN && currentLog > maxLog)
            {
                maxLog = currentLog;
            }
        }

        static string PrintResult(int n, int log)
        {
            string s = "";

            if (n == 0 && log == 0) return "\n--- Complexity: O(1) ---\n";

            if (n > 0)
            {
                s += (n == 1) ? "n" : "n^" + n;
            }

            if (log > 0)
            {
                if (s != "") s += " * ";
                s += (log == 1) ? "log(n)" : "log(n)^" + log;
            }

            return $"\n--- Estimated Time Complexity: O({s}) ---\n";
        }

        static bool IsKeyword(string text, int index, string word)
        {
            if (index + word.Length > text.Length) return false;
            return text.Substring(index, word.Length) == word;
        }
    }
}