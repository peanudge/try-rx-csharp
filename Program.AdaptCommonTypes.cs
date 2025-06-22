
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

public partial class Program
{
	static void ObservableFromAction()
	{
		var start = Observable.Start(() =>
		{
			WriteLine("Working away");
			Enumerable
				.Range(0, 10)
				.ForEach((_) =>
				{
					Thread.Sleep(100);
					Write(".");
				});
		});

		start.Subscribe(
			unit => WriteLine("Unit published"),
			() => WriteLine("Action Completed")
		);
	}

	static void ObservableFromFunc()
	{
		var start = Observable.Start(() =>
		{
			WriteLine("Working away");
			Enumerable
				.Range(0, 10)
				.ForEach((_) =>
				{
					Thread.Sleep(100);
					Write(".");
				});
			return "Published Value";
		});

		var deferredStart = Observable.Defer(() =>
		{
			WriteLine("Subscribe Start");
			return start;
		});

		deferredStart.Subscribe(
			WriteLine,
			() => WriteLine("Func Completed")
		);

		Thread.Sleep(1000);

		deferredStart.Subscribe(
			WriteLine,
			() => WriteLine("Func Completed")
		);
	}

	public static void ObservableFromTask()
	{
		var t = Task.Run(() =>
		{
			WriteLine("Task Running...");
			return "Test";
		});

		IObservable<string> source = t.ToObservable();

		source.Subscribe(WriteLine, () => WriteLine("Completed"));
		source.Subscribe(WriteLine, () => WriteLine("Completed"));
	}


	public static void ObservableFromTaskPerSubscription()
	{

		IObservable<string> source = Observable.FromAsync(async () =>
		{
			WriteLine("Task Running...");
			await Task.Delay(50);
			return "Test";
		});

		source.Subscribe(WriteLine, () => WriteLine("Completed"));
		source.Subscribe(WriteLine, () => WriteLine("Completed"));
	}


}
