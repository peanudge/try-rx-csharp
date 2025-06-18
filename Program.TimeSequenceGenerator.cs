using System.Reactive.Linq;

public partial class Program
{
	public static void IntervalSequenceExample()
	{
		IObservable<long> interval = Observable.Interval(TimeSpan.FromMilliseconds(250));
		var subscription = interval.Subscribe(WriteLine, () => WriteLine("completed"));
		Task.Delay(2000).ContinueWith(_ =>
		{
			subscription.Dispose();
		});


	}

	public static void TimerSequenceExample()
	{
		var timer = Observable.Timer(TimeSpan.FromSeconds(1));
		timer.Subscribe(WriteLine, () => WriteLine("Completed"));

		Interval(TimeSpan.FromSeconds(1))
			.Subscribe(WriteLine);

	}

	public static IObservable<long> IntervalByTimer(TimeSpan period)
	{
		return Observable.Timer(period, period);
	}

	public static void GenerateTimeSequenceExample()
	{
		Timer(TimeSpan.FromSeconds(1))
			.Subscribe(WriteLine);

		Random(TimeSpan.FromSeconds(1))
			.Subscribe(WriteLine);
	}

	public static IObservable<long> Timer(TimeSpan dueTime)
	{
		return Observable.Generate(
			0L,
			i => i < 1,
			i => i + 1,
			i => i,
			i => dueTime
		);
	}

	public static IObservable<long> Timer(TimeSpan dueTime, TimeSpan period)
	{
		return Observable.Generate(
			0L,
			i => true,
			i => i + 1,
			i => i,
			i => i == 0 ? dueTime : period
		);
	}

	public static IObservable<long> Interval(TimeSpan period)
	{
		return Observable.Generate(
			0L,
			i => true,
			i => i + 1,
			i => i,
			i => period
		);
	}

	public static IObservable<long> Random(TimeSpan period)
	{
		var rand = new Random();
		return Observable.Generate(
			rand.NextInt64(),
			i => true,
			i => rand.NextInt64(),
			i => i,
			i => period
		);
	}


}
