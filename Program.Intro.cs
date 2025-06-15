using System.Reactive.Linq;

public partial class Program
{
	public static void ObservableExample()
	{

		IObservable<long> ticks = Observable.Timer(
			dueTime: TimeSpan.Zero,
			period: TimeSpan.FromSeconds(1)
		);

		ticks.Subscribe(
			tick => WriteLine($"Tick {tick}")
		);


		var result = Enumerable.Range(1, 10)
			.ToObservable()
			.Aggregate((a, b) => a + b);
		WriteLine(result);
		ReadLine();

	}
}
