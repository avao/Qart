namespace Qart.Core.Nullable
{
    public static class NullableExtensions
    {
        public static bool TryGet<T>(this T? nullable, out T value)
            where T : struct
        {
            if (nullable.HasValue)
            {
                value = nullable.Value;
                return true;
            }

            value = default;
            return false;
        }
    }
}
