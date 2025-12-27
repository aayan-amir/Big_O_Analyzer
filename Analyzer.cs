using System;

namespace Sem_Projec
{
    class Analyzer
    {
        /* ==================================================================================
           1. KEYWORD COUNT
           ----------------
           Counts how many times specific C# keywords (for, while, if, etc.) appear in the code.
           ================================================================================== */
        public static string CountKeywords(string code)
        {
            string output = "";
            output += "--- Keyword Usage ---\n";

            // Define characters that separate words in code (spaces, brackets, semicolons, etc.)
            string[] delims = { " ", "\n", "\t", "(", ")", "{", "}", ";" };

            // Split the raw code string into individual words/tokens based on the delimiters
            string[] tokens = code.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            // The specific keywords we are looking for
            string[] targets = { "for", "while", "if", "int", "return" };

            // Loop through each target keyword
            foreach (string target in targets)
            {
                int count = 0; // Reset count for the current target

                // Scan through every token in the code
                for (int i = 0; i < tokens.Length; i++)
                {
                    // If the token matches the keyword exactly, increment count
                    if (tokens[i] == target) count++;
                }

                // Only print the keyword if it was found at least once
                if (count > 0) output += $"{target} : {count}\n";
            }

            return output;
        }

        /* ==================================================================================
           2. CODE STRUCTURE VIEW
           ----------------------
           Creates a visual tree representation of the code's flow (loops and if-statements).
           ================================================================================== */
        public static string ShowStructure(string code)
        {
            string output = "";
            output += "\n--- Code Structure ---\n";

            string[] delims = { " ", "\n", "\t", "(", ")", "{", "}", ";" };
            string[] tokens = code.Split(delims, StringSplitOptions.RemoveEmptyEntries);

            int indent = 0; // Tracks how deep we are in the structure

            foreach (var token in tokens)
            {
                // We only care about control flow keywords
                if (token == "for" || token == "while" || token == "if")
                {
                    // Add indentation spaces based on current depth
                    output += new string(' ', indent * 2) + "|_ " + token.ToUpper() + "\n";

                    // Increase indentation for the next item (simulating nesting)
                    indent++;
                }
            }
            return output;
        }

        /* ==================================================================================
           3. SIMPLE TIME COMPLEXITY ESTIMATOR
           -----------------------------------
           Estimates Big O notation by analyzing nested loops.
           It handles both Linear loops (i++) and Logarithmic loops (i *= 2).
           ================================================================================== */
        public static string EstimateTimeComplexity(string code)
        {
            // Array to store the "type" of code block at every nesting level.
            // Index = Depth level.
            // Value = 0 (Regular code), 1 (Linear Loop), 2 (Log Loop)
            int[] layers = new int[10];

            // Keeps track of how many open curly braces '{' we are currently inside
            int depth = 0;

            // Temporary variable to remember if we just saw a loop header before entering '{'
            // 0 = No loop seen recently
            // 1 = Saw a Linear loop (e.g., for i++)
            // 2 = Saw a Log loop (e.g., for i*=2)
            int nextType = 0;

            // Variables to track the maximum complexity found in the entire code
            int maxN = 0;   // Maximum exponent for 'n' (e.g., n^2)
            int maxLog = 0; // Maximum exponent for 'log n'

            // Iterate through every character in the code string
            for (int i = 0; i < code.Length; i++)
            {
                // --- STEP 1: DETECT LOOP HEADERS ---
                // Check if the current word is "for" or "while"
                if (IsKeyword(code, i, "for") || IsKeyword(code, i, "while"))
                {
                    // Analyze the loop condition (...) to see if it is O(n) or O(log n)
                    // Store the result in 'nextType' until we hit the opening brace '{'
                    nextType = GetLoopType(code, i);
                }

                // --- STEP 2: ENTERING A SCOPE (Opening Brace) ---
                else if (code[i] == '{')
                {
                    depth++; // Go one level deeper

                    // Store the type of this new scope in our 'layers' array.
                    // If we saw a loop header recently, use that type. Otherwise, it's just 0.
                    layers[depth] = nextType;

                    // Reset nextType because we have consumed the pending loop information
                    nextType = 0;

                    // Now that we are inside a new scope, calculate the TOTAL complexity
                    // of the current nesting stack and update the global max if needed.
                    CheckMax(layers, depth, ref maxN, ref maxLog);
                }

                // --- STEP 3: EXITING A SCOPE (Closing Brace) ---
                else if (code[i] == '}')
                {
                    // If we are not at the root level, move back up one level
                    if (depth > 0)
                    {
                        layers[depth] = 0; // Clear the record for the level we are leaving
                        depth--;           // Decrease depth counter
                    }
                }
            }

            // Convert the final max values into a readable string like "O(n^2)"
            return PrintResult(maxN, maxLog);
        }

        /* ==================================================================================
           HELPER: GetLoopType
           -------------------
           Looks inside the loop header (...) to determine complexity.
           Returns: 1 for Linear (O(n)), 2 for Logarithmic (O(log n))
           ================================================================================== */
        static int GetLoopType(string code, int index)
        {
            // Find the parentheses for the loop: for (...)
            int start = code.IndexOf('(', index);
            int end = code.IndexOf(')', start);

            // Safety check: if parenthesis are missing, assume it's a standard Linear loop
            if (start == -1 || end == -1) return 1;

            // Extract the text inside the parentheses
            string header = code.Substring(start, end - start);

            // Check for multiplication (*=) or division (/=) assignment operators.
            // These indicate the loop variable changes exponentially (1, 2, 4, 8...), 
            // which means the complexity is Logarithmic.
            if (header.Contains("*=") || header.Contains("/=")) return 2;

            // Otherwise, assume it's a standard increment/decrement loop (Linear)
            return 1;
        }

        /* ==================================================================================
           HELPER: CheckMax
           ----------------
           Sums up the complexity of all currently active layers and updates the global maximum.
           ================================================================================== */
        static void CheckMax(int[] layers, int depth, ref int maxN, ref int maxLog)
        {
            int currentN = 0;
            int currentLog = 0;

            // Loop through all active depths (from 1 up to current depth)
            for (int k = 1; k <= depth; k++)
            {
                // If this layer is a Linear loop, add to n power
                if (layers[k] == 1) currentN++;

                // If this layer is a Log loop, add to log power
                if (layers[k] == 2) currentLog++;
            }

            // Compare current complexity with the highest one found so far.
            // PRIORITY RULE: Higher 'n' power is always more complex than 'log n'.
            // Example: O(n) > O(log n^5)

            if (currentN > maxN)
            {
                // Found a higher n-power (e.g., found n^2, previous max was n)
                maxN = currentN;
                maxLog = currentLog;
            }
            else if (currentN == maxN && currentLog > maxLog)
            {
                // n-power is equal, but found a higher log-power 
                // (e.g., found n*log(n), previous max was n)
                maxLog = currentLog;
            }
        }

        /* ==================================================================================
           HELPER: PrintResult
           -------------------
           Formats the integer counts into a standard Big O string.
           ================================================================================== */
        static string PrintResult(int n, int log)
        {
            string s = "";

            // Case: No loops found
            if (n == 0 && log == 0) return "\n--- Complexity: O(1) ---\n";

            // Format the 'n' part
            if (n > 0)
            {
                // If n=1, print "n". If n=2, print "n^2"
                s += (n == 1) ? "n" : "n^" + n;
            }

            // Format the 'log' part
            if (log > 0)
            {
                // Add multiplication symbol if we already have an 'n' part
                if (s != "") s += " * ";

                // If log=1, print "log(n)". If log=2, print "log(n)^2"
                s += (log == 1) ? "log(n)" : "log(n)^" + log;
            }

            return $"\n--- Estimated Time Complexity: O({s}) ---\n";
        }

        /* ==================================================================================
           HELPER: IsKeyword
           -----------------
           Checks if the word at a specific index matches a given keyword.
           ================================================================================== */
        static bool IsKeyword(string text, int index, string word)
        {
            // Ensure we don't read past the end of the string
            if (index + word.Length > text.Length) return false;

            // Check if the substring matches the keyword exactly
            return text.Substring(index, word.Length) == word;
        }
    }
}