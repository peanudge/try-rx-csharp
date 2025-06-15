using System.Reactive.Concurrency;
using System.Reactive.Linq;

public partial class Program
{
	public static void QuiescentExample()
	{

		var source = Observable.Empty<int>()
			.Concat(Observable.Return(1).Delay(TimeSpan.FromSeconds(0)))
			.Concat(Observable.Return(2).Delay(TimeSpan.FromSeconds(0.1)))
			.Concat(Observable.Return(3).Delay(TimeSpan.FromSeconds(2)))
			.Concat(Observable.Return(4).Delay(TimeSpan.FromSeconds(0.1)))
			.Concat(Observable.Return(5).Delay(TimeSpan.FromSeconds(0.3)))
			.Concat(Observable.Return(6).Delay(TimeSpan.FromSeconds(1.9)));


		source.Quiescent(TimeSpan.FromSeconds(1.5), Scheduler.Default)
			.Subscribe(
				value => { WriteLine($"({DateTime.Now}) [ {string.Join(", ", value)} ]"); },
				error => { WriteLine($"Sequence faulted with {error}"); },
				() => { WriteLine("Sequence terminated"); }
			);


		ReadLine();
	}

}

public static class RxExt
{
	public static IObservable<IList<T>> Quiescent<T>(
		this IObservable<T> src,
		TimeSpan minimumInactivityPeriod,
		IScheduler scheduler)
	{
		IObservable<int> onoffs =
			from _ in src
			from delta in Observable.Return(1, scheduler)
				.Concat(Observable.Return(-1, scheduler).Delay(minimumInactivityPeriod, scheduler))
			select delta;

		IObservable<int> outstanding = onoffs.Scan(0, (total, delta) => total + delta);
		IObservable<int> zeroCrossings = outstanding.Where(total => total == 0);

		return src.Buffer(zeroCrossings);
	}
}
