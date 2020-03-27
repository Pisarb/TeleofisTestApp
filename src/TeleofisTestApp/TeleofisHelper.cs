using System;

namespace TeleofisTestApp
{
    public static class TeleofisHelper
    {
        public static string GetNewImei()
        {
            var rnd = new Random();
            string imei = default;
            var digits = new int[14];
            for (int i = 0; i < 14; i++)
                digits[i] = rnd.Next(10);
            var digitsCopy = new int[14];
            Array.Copy(digits, digitsCopy, 14);
            int sum = GetImeiSum(digitsCopy) % 10;
            int lastNumber;
            if (sum == 0)
                lastNumber = 0;
            else
                lastNumber = 10 - sum;
            for (int i = 0; i < 14; i++)
                imei += digits[i].ToString();
            imei += lastNumber.ToString();
            return imei;
        }

        private static int GetImeiSum(int[] numbers)
        {
            int sum = 0;
            if (numbers.Length != 14)
                throw new Exception("Unexpected array length");
            for (int i = 0; i < 14; i++)
            {
                if (i % 2 == 1)
                    numbers[i] = 2 * numbers[i];
                sum += DigitsSum(numbers[i]);
            }
            return sum;
        }
        private static int DigitsSum(int number)
        {
            int sum = 0;
            while (number > 0)
            {
                sum += number % 10;
                number /= 10;
            }
            return sum;
        }
    }
}
