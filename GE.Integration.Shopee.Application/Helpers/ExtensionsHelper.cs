namespace GE.Integration.Shopee.Application.Helpers
{
    public static class ExtensionsHelper
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            using var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return YieldBatchElements(enumerator, batchSize - 1);
            }
        }

        private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source, int batchSize)
        {
            yield return source.Current;
            for (var i = 0; i < batchSize && source.MoveNext(); i++)
            {
                yield return source.Current;
            }
        }

        public static string FormatLongArray(this long[] array)
        {
            string formattedString = string.Join(",", array);
            return formattedString;
        }

        public static long GenerateUniqueRandomNumber(long minValue, long maxValue)
        {
            Random rand = new Random();
            long randomNumber = rand.NextInt64(minValue, maxValue + 1);
            return randomNumber;
        }

        public static long NextInt64(this Random rnd, long minValue, long maxValue)
        {
            if (minValue > maxValue) 
                throw new ArgumentOutOfRangeException(nameof(minValue), "minValue cannot be greater than maxValue");

            ulong range = (ulong)(maxValue - minValue);
            ulong ulongRand;

            do
            {
                byte[] buf = new byte[8];
                rnd.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0) & ulong.MaxValue;
            } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % range) + 1) % range);

            return (long)(ulongRand % range) + minValue;
        }
    }
}
