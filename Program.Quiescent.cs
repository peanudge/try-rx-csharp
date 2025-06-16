using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

public partial class Program
{
	public static void QuiescentExample()
	{
		var exampleSources = Observable.Create(async (IObserver<int> observer) =>
		{
			observer.OnNext(1);
			await Task.Delay(100);
			observer.OnNext(2);
			await Task.Delay(2000);
			observer.OnNext(3);
			observer.OnNext(4);
			await Task.Delay(100);
			observer.OnNext(5);
			await Task.Delay(2000);
			observer.OnNext(6);
			return Disposable.Empty;
		});


		var subscription = exampleSources.Quiescent(TimeSpan.FromSeconds(1.5), Scheduler.Default)
			.Subscribe(
				value => { WriteLine($"({DateTime.Now}) [ {string.Join(", ", value)} ]"); },
				error => { WriteLine($"Sequence faulted with {error}"); },
				() => { WriteLine("Sequence terminated"); }
			);

		WriteLine("Subscribed");
		ReadLine();

		subscription.Dispose();
		WriteLine("Unsubscribed");

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
