using System.Reactive.Disposables;
using System.Reactive.Linq;

public partial class Program
{
	// Due to the large number of methods available for creating observable sequences.
	// we will break them down into categories.
	public static void SimpleFactoryMethodsEx()
	{
		Observable.Return("value").Subscribe(WriteLine);

		Observable.Empty<string>().Subscribe(WriteLine);

		Observable.Never<string>().Subscribe(WriteLine);

		Observable.Throw<string>(new Exception()).Subscribe(WriteLine, error => WriteLine(error.Message), () => { });

		var subscription = KeyPress().Subscribe(v =>
		{
			WriteLine($"{v}");
		});

		Task.Run(async () =>
		{
			await Task.Delay(3000);
			WriteLine("Disposed");
			subscription.Dispose();
		});

		var filePath = Path.Combine(Directory.GetCurrentDirectory(), "README.md");
		WriteLine("ReadFile:" + filePath);

		ReadFileLines(filePath)
			.Zip(Naturals())
			.Subscribe(value =>
			{
				Write($"#{value.Second,5}: ");
				WriteLine(value.First);
			});
	}


	private static IEnumerable<int> Naturals()
	{
		var i = 0;
		while (true)
		{
			yield return i++;
		}
	}

	private static IObservable<int> SomeNumbers()
	{
		return Observable.Create(
			(IObserver<int> observer) =>
			{
				observer.OnNext(1);
				observer.OnNext(2);
				observer.OnNext(3);
				observer.OnCompleted();
				return Disposable.Empty;
			});
	}

	private static IObservable<char> KeyPress()
	{
		return Observable.Create<char>(observer =>
			{
				CancellationTokenSource cts = new();
				Task.Run(() =>
				{
					WriteLine($"Cancellation: {cts.IsCancellationRequested}");
					while (!cts.IsCancellationRequested)
					{
						ConsoleKeyInfo ki = ReadKey();
						observer.OnNext(ki.KeyChar);
					}
				});
				return () => cts.Cancel();
			});
	}

	public static IObservable<string> ReadFileLines(string path)
	{
		return Observable.Create<string>(async (observer, cancellationToken) =>
		{
			using (StreamReader reader = File.OpenText(path))
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					string? line = await reader
						.ReadLineAsync(cancellationToken)
						.ConfigureAwait(false);
					if (line is null)
					{
						break;
					}
					observer.OnNext(line);
				}
				observer.OnCompleted();
			}
		});
	}

	public static IObservable<T> Empty<T>()
	{
		return Observable.Create<T>(o =>
		{
			o.OnCompleted();
			return Disposable.Empty;
		});
	}

	public static IObservable<T> Return<T>(T value)
	{
		return Observable.Create<T>(o =>
		{
			o.OnNext(value);
			o.OnCompleted();
			return Disposable.Empty;
		});
	}

	public static IObservable<T> Never<T>()
		=> Observable.Create<T>(o => Disposable.Empty);

	public static IObservable<T> Throws<T>(Exception exception)
		=> Observable.Create<T>(o =>
		{
			o.OnError(exception);
			return Disposable.Empty;
		});


	public static void DeferObservableExample()
	{
		WriteLine("Calling factory method");
		IObservable<int> s1 = WithoutDeferal();

		WriteLine("First subscription");
		s1.Subscribe(WriteLine);

		WriteLine("Second subscription");
		s1.Subscribe(WriteLine);


		WriteLine("Calling factory method with deferal");

		IObservable<int> s2 = WithDeferal();

		WriteLine("First subscription");
		s2.Subscribe(WriteLine);

		WriteLine("Second subscription");
		s2.Subscribe(WriteLine);
	}

	static IObservable<int> WithoutDeferal()
	{
		WriteLine("Doing some startup wrork...");
		return Observable.Range(1, 3);
	}

	static IObservable<int> WithDeferal()
	{
		return Observable.Defer(() =>
		{
			WriteLine("Doing some startup work...");
			return Observable.Range(1, 3);
		});
	}
}
