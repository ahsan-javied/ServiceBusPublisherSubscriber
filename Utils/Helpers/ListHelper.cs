
namespace Utils.Helpers
{
    public static class ListHelper
    {
        public static List<List<T>> SplitIntoBatches<T>(List<T> source, int batchSize)
        {
            List<List<T>> batches = new List<List<T>>();

            for (int i = 0; i < source.Count; i += batchSize)
            {
                List<T> batch = source.Skip(i).Take(batchSize).ToList();
                batches.Add(batch);
            }

            return batches;
        }
    }


}
