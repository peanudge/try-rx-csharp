public partial class Program
{
	public static void BackPressureExample()
	{
		new MyObserver().Run();
	}

	private class GoUntilStopped
	{
		private readonly IObserver<int> observer;
		private bool running;
		public GoUntilStopped(IObserver<int> observer)
		{
			this.observer = observer;
		}

		public void Go()
		{
			this.running = true;
			for (int i = 0; this.running; i++)
			{
				this.observer.OnNext(i);
			}
			this.observer.OnCompleted();
		}

		public void Stop()
		{
			this.running = false;
			// this.observer.OnCompleted();
		}
	}

	public class MyObserver : IObserver<int>
	{
		private GoUntilStopped? runner;

		public void Run()
		{
			this.runner = new(this);
			WriteLine("Starting...");
			this.runner.Go();
			WriteLine("Finished");
		}

		public void OnCompleted()
		{
			WriteLine("OnCompleted");
		}

		public void OnError(Exception error) { }

		public void OnNext(int value)
		{
			WriteLine($"OnNext {value}");
			if (value > 3)
			{
				WriteLine($"OnNext calling Stop");
				this.runner?.Stop(); // runner call Observer's OnCompleted
			}
			WriteLine("OnNext returning");
		}
	}
}
