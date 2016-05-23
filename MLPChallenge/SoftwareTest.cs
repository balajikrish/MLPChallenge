using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareTest
{
    /**
     * Welcome to the Software Test. Please make sure you
     * read the instructions carefully.
     *
     * FAQ:
     * Can I use linq? Yes.
     * Can I cheat and look things up on Stack Overflow? Yes.
     * Can I use a database? No.
     */

    /// There are two challenges in this file
    /// The first one should takes ~10 mins with the
    /// second taking between ~30-40 mins.
    public interface IChallenge
    {
        /// Are you a winner?
        bool Winner();
    }

    /// Lets find out
    public class Program
    {
        /// <summary>
        /// Challenge Uno - NumberCalculator
        ///
        /// Fill out the TODOs with your own code and make any
        /// other appropriate improvements to this class.
        /// </summary>
        public class NumberCalculator : IChallenge
        {
            public int FindMax(int[] numbers)
            {
                // TODO: Find the highest number
                int max = 0;

                foreach (var num in numbers)
                    if (num > max)
                        max = num;

                return max;
            }

            public int[] FindMax(int[] numbers, int n)
            {
                return Sort(numbers).OrderByDescending(r => r).Take(n).ToArray();

            }

            public int[] Sort(int[] numbers)
            {
                // TODO: Sort the numbers
                //assuming that the source int array has to remain unaltered, I am creating a new int[] sequence and returning it.
                //I can also modify the original numbers array and need not return it since its a reference type.
                int[] sorted = new int[numbers.Length];
                Array.Copy(numbers, sorted, numbers.Length);
                QuickSort(sorted, 0, sorted.Length - 1);
                return sorted;
            }

            #region sort-helpermethods

            void QuickSort(int[] numbers, int low, int high)
            {
                //empty array input
                if (numbers.Length == 0)
                    return;

                int lo = low;
                int hi = high;

                //any random pivot element - choosing the first element
                int pivot = numbers[low];

                while (lo <= hi)
                {
                    while ((lo < high) && (numbers[lo] < pivot))
                        ++lo;

                    while ((hi > low) && (numbers[hi] > pivot))
                        --hi;

                    if (lo <= hi)
                    {
                        swap(numbers, lo, hi);
                        ++lo;
                        --hi;
                    }
                }

                if (low < hi)
                    QuickSort(numbers, low, hi);

                if (lo < high)
                    QuickSort(numbers, lo, high);
            }


            public static void swap(int[] listToSort, int iIndex, int jIndex)
            {
                int temp = listToSort[iIndex];
                listToSort[iIndex] = listToSort[jIndex];
                listToSort[jIndex] = temp;
            }

            #endregion

            public bool Winner()
            {
                var numbers = new[] { 5, 7, 5, 3, 6, 7, 9 };
                var sorted = Sort(numbers);
                var maxes = FindMax(numbers, 2);

                // TODO: Are the following test cases sufficient, to prove your code works
                // as expected? If not either write more test cases and/or describe what
                // other tests cases would be needed.

                //I have added test cases to check boundary conditions, in order sorting.

                bool testCase1= sorted.First() == 3
                       && sorted.Last() == 9
                       && FindMax(numbers) == 9
                       && maxes[0] == 9
                       && maxes[1] == 7;

                //empty array input
                numbers = new int[0] {};
                sorted = Sort(numbers);
                maxes = FindMax(numbers, 2);

                bool testCase2 = sorted.Count()== 0
                      && FindMax(numbers) == 0
                       && maxes.Count() == 0;

                //input in order
                numbers = new int[] {3,5,5,6,7,7,9};
                sorted = Sort(numbers);
                maxes = FindMax(numbers, 2);

                bool testCase3 = sorted.First() == 3
                       && sorted.Last() == 9
                       && FindMax(numbers) == 9
                       && maxes[0] == 9
                       && maxes[1] == 7;


                return testCase1 && testCase2 && testCase3;
            }
        }

        /// <summary>
        /// Challenge Due - Run Length Encoding
        ///
        /// RLE is a simple compression scheme that encodes runs of data into
        /// a single data value and a count. It's useful for data that has lots
        /// of contiguous values (for example it was used in fax machines), but
        /// also has lots of downsides.
        ///
        /// For example, aaaaaaabbbbccccddddd would be encoded as
        ///
        /// 7a4b4c5d
        ///
        /// You can find out more about RLE here...
        /// http://en.wikipedia.org/wiki/Run-length_encoding
        ///
        /// In this exercise you will need to write an RLE **Encoder** which will take
        /// a byte array and return an RLE encoded byte array.
        /// </summary>
        public class RunLengthEncodingChallenge : IChallenge
        {
            public byte[] Encode(byte[] original)
            {
                // TODO: Write your encoder here
                int currIndex = 0;
                List<byte> encoded = new List<byte>();
                while (currIndex < original.Length)
                {
                    var currByte = original[currIndex];
                    byte count = 0;
                    int compareIndex = currIndex;
                    while (compareIndex < original.Length && original[compareIndex] == currByte)
                    {
                        compareIndex++;
                        count++;
                    }
                    encoded.Add(count);
                    encoded.Add(currByte);
                    currIndex = compareIndex;
                }
                return encoded.ToArray();
            }

            public bool Winner()
            {
                // TODO: Are the following test cases sufficient, to prove your code works
                // as expected? If not either write more test cases and/or describe what
                // other tests cases would be needed.

                var testCases = new[]
                {
                    new Tuple<byte[], byte[]>(new byte[]{0x01, 0x02, 0x03, 0x04}, new byte[]{0x01, 0x01, 0x01, 0x02, 0x01, 0x03, 0x01, 0x04}),
                    new Tuple<byte[], byte[]>(new byte[]{0x01, 0x01, 0x01, 0x01}, new byte[]{0x04, 0x01}),
                    new Tuple<byte[], byte[]>(new byte[]{0x01, 0x01, 0x02, 0x02}, new byte[]{0x02, 0x01, 0x02, 0x02}),
                    new Tuple<byte[], byte[]>(new byte[]{}, new byte[]{}),
                    new Tuple<byte[], byte[]>(new byte[]{0x01, 0x01, 0x01, 0x01,0x01, 0x01,0x01, 0x01,0x01, 0x01,0x01}, new byte[]{0x0B, 0x01}),
                };

                // TODO: What limitations does your algorithm have (if any)?
                // TODO: What do you think about the efficiency of this algorithm for encoding data?

                foreach (var testCase in testCases)
                {
                    var encoded = Encode(testCase.Item1);
                    var isCorrect = encoded.SequenceEqual(testCase.Item2);

                    if (!isCorrect)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public static void Main(string[] args)
        {
            var challenges = new IChallenge[]
            {
                new NumberCalculator(),
                new RunLengthEncodingChallenge()
            };

            foreach (var challenge in challenges)
            {
                var challengeName = challenge.GetType().Name;

                var result = challenge.Winner()
                    ? string.Format("You win at challenge {0}", challengeName)
                    : string.Format("You lose at challenge {0}", challengeName);

                Console.WriteLine(result);
            }

            Console.ReadLine();
        }
    }
}
