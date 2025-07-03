using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

public static class SampleExtension
{
	public static void Dump<T>(this IObservable<T> source, string name)
	{
		source.Subscribe(
			value => WriteLine($"{name} ---> {value}"),
			ex => WriteLine($"{name} failed --> {ex.Message}"),
			() => WriteLine($"{name} completed")
		);
	}

	public static IObservable<T> FirstOrEmpty<T>(this IObservable<T> src) => src.Take(1);
}
public partial class Program
{
	public static void FilteringOdds()
	{
		var xs = Observable
			.Range(0, 10);
		xs.Dump("Unfiltered");

		// var dropEverything = xs.IgnoreElements();
		var dropEverything = xs.Where(_ => false);
		dropEverything.Dump("IgnoreElements");


	}


	public static void FilteringType()
	{
		var source = Observable.Create<IAisMessage>((observer) =>
		{
			observer.OnNext(new AMessage("Hello"));
			observer.OnNext(new BMessage(1));
			observer.OnNext(new BMessage(2));
			return Disposable.Empty;
		});

		var bMessageSources = source.OfType<BMessage>().Select(_ => _.ToString());
		bMessageSources.Dump("Filter B Message");
	}

	interface IAisMessage { }
	class AMessage : IAisMessage
	{
		private readonly string _message;

		public AMessage(string message)
		{
			_message = message;
		}

		public new string ToString()
		{
			return _message;
		}
	}
	class BMessage : IAisMessage
	{
		private readonly int _speed;

		public BMessage(int speed)
		{
			_speed = speed;
		}

		public new string ToString()
		{
			return "Speed:" + _speed;
		}
	}

	public static void PositionalFilter()
	{
		var firstSource = Observable
			.Range(1, 10)
			.Take(3);

		firstSource.Dump("Pos Filter1");
	}

	public static void MultiSubscriptionExample()
	{
		var connectableSource = Observable
			.Create<int>(obs =>
			{
				obs.OnNext(1);
				obs.OnNext(2);
				obs.OnNext(3);
				obs.OnCompleted();
				return Disposable.Empty;
			})
			.PublishLast();
		connectableSource.Dump("Listen1");
		connectableSource.Dump("Listen2");
		connectableSource.Dump("Listen3");
		connectableSource.Connect();
	}
}

