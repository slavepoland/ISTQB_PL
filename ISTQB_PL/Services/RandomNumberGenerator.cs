using System;
using System.Collections.Generic;

namespace ISTQB_PL.Services
{
    public class RandomNumberGenerator
    {
        public static List<string> GenerateUniqueRandomNumbers(int minValue, int maxValue, int count)
        {
            if (count > (maxValue - minValue + 1))
            {
                throw new ArgumentException("Count should not exceed the range of unique values.");
            }

            List<int> numbers = new List<int>();
            for (int i = minValue; i <= maxValue; i++)
            {
                numbers.Add(i);
            }

            List<string> uniqueRandomNumbers = new List<string>();
            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                int randomIndex = random.Next(0, numbers.Count);
                uniqueRandomNumbers.Add(numbers[randomIndex].ToString());
                numbers.RemoveAt(randomIndex);
            }

            return uniqueRandomNumbers;
        }
    }
}
