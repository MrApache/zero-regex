using ZeroRegex;
using System;

namespace ZeroRegex.Launcher
{
    public partial struct Foo
    {
        public Match MatchXD(ReadOnlySpan<char> input)
        {
            int matchStart = -1;
            int matchLength = 0;

            int start = 0;
            int length = 0;
            if (!FindMatchMultipleTimes_127(ref start, ref length, ref input))
            {
                return new Match(0, 0, false);
            }
            matchStart = start;
            matchLength = start + length - matchStart;

            start = matchStart + matchLength;
            length = 0;
            if (!CharMatch_128(ref start, ref length, ref input))
            {
                return new Match(start, length, false);
            }
            matchLength = start + length - matchStart;
            Console.WriteLine($"{start}, {length}, CharMatch_128");

            start = matchStart + matchLength;
            length = 0;
            if (!Or_149(ref start, ref length, ref input))
            {
                return new Match(start, length, false);
            }

            matchLength = start + length - matchStart;
            Console.WriteLine($"{start}, {length}, Or_149");
            return new Match(matchStart, matchLength, true);
        }

        private bool FindMatchInClass_120(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int currentIndex = start + length;
            if (currentIndex >= input.Length)
                return false;
            char currentChar = input[currentIndex];
            if (currentChar >= 'a' && currentChar <= 'z')
            {
                length++;
                return true;
            }

            if (currentChar >= 'A' && currentChar <= 'Z')
            {
                length++;
                return true;
            }

            if (currentChar >= '0' && currentChar <= '9')
            {
                length++;
                return true;
            }

            if (currentChar >= '_' && currentChar <= '_')
            {
                length++;
                return true;
            }

            return false;
        }

        private bool FindMatchInClass_121(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int currentIndex = start + length;
            if (currentIndex >= input.Length)
                return false;
            char currentChar = input[currentIndex];
            if (currentChar >= '0' && currentChar <= '9')
            {
                length++;
                return true;
            }

            if (currentChar >= 'A' && currentChar <= 'Z')
            {
                length++;
                return true;
            }

            if (currentChar >= '_' && currentChar <= '_')
            {
                length++;
                return true;
            }

            if (currentChar >= 'a' && currentChar <= 'z')
            {
                length++;
                return true;
            }

            return false;
        }

        private bool FindMatchMultipleTimes_122(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int count = 0;
            for (int j = 0; j < 2147483647; j++)
            {
                if (FindMatchInClass_121(ref start, ref length, ref input))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count >= 1 && count <= 2147483647;
        }

        private bool EvaluateGroup_123(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            if (!FindMatchInClass_120(ref start, ref length, ref input))
            {
                return false;
            }

            if (!FindMatchMultipleTimes_122(ref start, ref length, ref input))
            {
                return false;
            }

            return true;
        }

        private bool FindMatchInClass_124(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int currentIndex = start + length;
            if (currentIndex >= input.Length)
                return false;
            char currentChar = input[currentIndex];
            if (currentChar >= '-' && currentChar <= '.')
            {
                length++;
                return true;
            }

            return false;
        }

        private bool FindMatchMultipleTimes_125(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int count = 0;
            for (int j = 0; j < 1; j++)
            {
                if (FindMatchInClass_124(ref start, ref length, ref input))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count >= 0 && count <= 1;
        }

        private bool EvaluateGroup_126(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            if (!EvaluateGroup_123(ref start, ref length, ref input))
            {
                return false;
            }

            if (!FindMatchMultipleTimes_125(ref start, ref length, ref input))
            {
                return false;
            }

            return true;
        }

        private bool FindMatchMultipleTimes_127(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int count = 0;
            for (int j = 0; j < 2147483647; j++)
            {
                if (EvaluateGroup_126(ref start, ref length, ref input))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count >= 1 && count <= 2147483647;
        }

        private bool CharMatch_128(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int pointer = start + length;
            if (pointer >= input.Length)
                return false;
            return input[start + length++] == '@';
        }

        private bool FindMatchInClass_129(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int currentIndex = start + length;
            if (currentIndex >= input.Length)
                return false;
            char currentChar = input[currentIndex];
            if (currentChar >= '0' && currentChar <= '9')
            {
                length++;
                return true;
            }

            return false;
        }

        private bool FindMatchMultipleTimes_130(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int count = 0;
            for (int j = 0; j < 3; j++)
            {
                if (FindMatchInClass_129(ref start, ref length, ref input))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count >= 1 && count <= 3;
        }

        private bool CharMatch_131(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int pointer = start + length;
            if (pointer >= input.Length)
                return false;
            return input[start + length++] == '.';
        }

        private bool FindMatchInClass_132(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int currentIndex = start + length;
            if (currentIndex >= input.Length)
                return false;
            char currentChar = input[currentIndex];
            if (currentChar >= '0' && currentChar <= '9')
            {
                length++;
                return true;
            }

            return false;
        }

        private bool FindMatchMultipleTimes_133(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int count = 0;
            for (int j = 0; j < 3; j++)
            {
                if (FindMatchInClass_132(ref start, ref length, ref input))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count >= 1 && count <= 3;
        }

        private bool CharMatch_134(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int pointer = start + length;
            if (pointer >= input.Length)
                return false;
            return input[start + length++] == '.';
        }

        private bool FindMatchInClass_135(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int currentIndex = start + length;
            if (currentIndex >= input.Length)
                return false;
            char currentChar = input[currentIndex];
            if (currentChar >= '0' && currentChar <= '9')
            {
                length++;
                return true;
            }

            return false;
        }

        private bool FindMatchMultipleTimes_136(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int count = 0;
            for (int j = 0; j < 3; j++)
            {
                if (FindMatchInClass_135(ref start, ref length, ref input))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count >= 1 && count <= 3;
        }

        private bool CharMatch_137(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int pointer = start + length;
            if (pointer >= input.Length)
                return false;
            return input[start + length++] == '.';
        }

        private bool FindMatchInClass_138(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int currentIndex = start + length;
            if (currentIndex >= input.Length)
                return false;
            char currentChar = input[currentIndex];
            if (currentChar >= '0' && currentChar <= '9')
            {
                length++;
                return true;
            }

            return false;
        }

        private bool FindMatchMultipleTimes_139(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int count = 0;
            for (int j = 0; j < 3; j++)
            {
                if (FindMatchInClass_138(ref start, ref length, ref input))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count >= 1 && count <= 3;
        }

        private bool EvaluateGroup_140(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            if (!FindMatchMultipleTimes_130(ref start, ref length, ref input))
            {
                return false;
            }

            if (!CharMatch_131(ref start, ref length, ref input))
            {
                return false;
            }

            if (!FindMatchMultipleTimes_133(ref start, ref length, ref input))
            {
                return false;
            }

            if (!CharMatch_134(ref start, ref length, ref input))
            {
                return false;
            }

            if (!FindMatchMultipleTimes_136(ref start, ref length, ref input))
            {
                return false;
            }

            if (!CharMatch_137(ref start, ref length, ref input))
            {
                return false;
            }

            if (!FindMatchMultipleTimes_139(ref start, ref length, ref input))
            {
                return false;
            }

            return true;
        }

        private bool FindMatchInClass_141(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int currentIndex = start + length;
            if (currentIndex >= input.Length)
                return false;
            char currentChar = input[currentIndex];
            if (currentChar >= '-' && currentChar <= '-')
            {
                length++;
                return true;
            }

            if (currentChar >= '0' && currentChar <= '9')
            {
                length++;
                return true;
            }

            if (currentChar >= 'A' && currentChar <= 'Z')
            {
                length++;
                return true;
            }

            if (currentChar >= 'a' && currentChar <= 'z')
            {
                length++;
                return true;
            }

            return false;
        }

        private bool FindMatchMultipleTimes_142(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int count = 0;
            for (int j = 0; j < 2147483647; j++)
            {
                if (FindMatchInClass_141(ref start, ref length, ref input))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count >= 1 && count <= 2147483647;
        }

        private bool CharMatch_143(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int pointer = start + length;
            if (pointer >= input.Length)
                return false;
            return input[start + length++] == '.';
        }

        private bool EvaluateGroup_144(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            if (!FindMatchMultipleTimes_142(ref start, ref length, ref input))
            {
                return false;
            }

            if (!CharMatch_143(ref start, ref length, ref input))
            {
                return false;
            }

            return true;
        }

        private bool FindMatchMultipleTimes_145(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int count = 0;
            for (int j = 0; j < 2147483647; j++)
            {
                if (EvaluateGroup_144(ref start, ref length, ref input))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count >= 1 && count <= 2147483647;
        }

        private bool FindMatchInClass_146(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int currentIndex = start + length;
            if (currentIndex >= input.Length)
                return false;
            char currentChar = input[currentIndex];
            if (currentChar >= 'A' && currentChar <= 'Z')
            {
                length++;
                return true;
            }

            if (currentChar >= 'a' && currentChar <= 'z')
            {
                length++;
                return true;
            }

            return false;
        }

        private bool FindMatchMultipleTimes_147(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int count = 0;
            for (int j = 0; j < 2147483647; j++)
            {
                if (FindMatchInClass_146(ref start, ref length, ref input))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            return count >= 2 && count <= 2147483647;
        }

        private bool EvaluateGroup_148(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            if (!FindMatchMultipleTimes_145(ref start, ref length, ref input))
            {
                return false;
            }

            if (!FindMatchMultipleTimes_147(ref start, ref length, ref input))
            {
                return false;
            }

            return true;
        }

        private bool Or_149(ref int start, ref int length, ref ReadOnlySpan<char> input)
        {
            int tempStart = start;
            int tempLength = length;
            if (!EvaluateGroup_140(ref start, ref length, ref input))
            {
                start = tempStart;
                length = tempLength;
                return EvaluateGroup_148(ref start, ref length, ref input);
            }

            return true;
        }
    }
}