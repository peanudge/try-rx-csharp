using System.Reactive.Linq;

public partial class Program
{
	public static void SelectExample()
	{
		IObservable<int> source = Observable.Range(1, 5);
		source
			.Select((i) => (char)(i + 64))
			.Select((c, idx) => new { Number = idx, Character = c })
			.Dump("char");
	}

	public static void SelectManyExample()
	{
		Observable.Range(1, 5)
			// .SelectMany(i => Observable.Repeat((char)(i + 64), i))
			.SelectMany(i => new string((char)(i + 64), i).ToObservable())
			.Dump("Sequence");
	}

	public static void SelectManyExampleWithDelay()
	{
		Observable.Range(1, 5)
			.SelectMany(i =>
				Observable.Repeat((char)(i + 64), i)
					.Delay(TimeSpan.FromMilliseconds(i * 100)))
			.Dump("chars");
	}

	public static void MaterializeExample()
	{
		Observable.Range(1, 3)
			.Materialize()
			.Dump("Materilie");
	}
}
