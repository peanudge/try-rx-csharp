public static class EnumerableHelpers
{
	public static void ForEach<T>(this IEnumerable<T> source, Action<T> fn)
	{
		var enumerator = source.GetEnumerator();
		while (enumerator.MoveNext())
		{
			fn(enumerator.Current);
		}
	}
}
