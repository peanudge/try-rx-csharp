using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

public partial class Program
{
	public static void ObservableRangeExample()
	{
		Observable.Range(10, 15)
			.Subscribe(WriteLine, () => WriteLine("Completed"));

		CustomRange1(10, 15)
			.Subscribe(WriteLine, () => WriteLine("Completed"));

		CustomRange2(10, 15)
			.Subscribe(WriteLine, () => WriteLine("Completed"));
	}

	public static IObservable<int> CustomRange1(int start, int count)
	{
		// it does not respect request to unsubscribe.
		// of course, RX will ignore when receive unsubscribe notification.
		return Observable.Create<int>(observer =>
		{
			for (int i = 0; i < count; i++)
			{
				observer.OnNext(start + i);
			}
			observer.OnCompleted();
			return Disposable.Empty;
		});
	}

	public static IObservable<int> CustomRange2(int start, int count)
	{
		int max = start + count;
		return Observable.Generate(
			start,
			value => value < max,
			value => value + 1,
			value => value
		);
	}

	public static IObservable<BigInteger> Fibonacci()
	{
		return Observable.Generate(
			(v1: new BigInteger(1), v2: new BigInteger(1)),
			value => true,
			value => (value.v2, value.v1 + value.v2),
			value => value.v1
		);
	}
}
