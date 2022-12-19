namespace WebParking.Utils
{
    public class EnumerableProccesor
    {
        public static IEnumerable<T> PushDefaultToStart<T>(IEnumerable<T> values) where T : new()
        {
            var list = values.ToList();
            list.Insert(0, new T() { });
            return list;
        }
    }
}
