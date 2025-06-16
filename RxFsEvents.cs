
public class RxFsEvents : IObservable<FileSystemEventArgs>
{
	private readonly string _folder;

	public RxFsEvents(string folder)
	{
		_folder = folder;
	}

	public IDisposable Subscribe(IObserver<FileSystemEventArgs> observer)
	{
		FileSystemWatcher watcher = new(_folder);
		object sync = new();
		bool onErrorAlreadyCalled = false;
		void SendToObserver(object _, FileSystemEventArgs e)
		{
			lock (sync)
			{
				if (!onErrorAlreadyCalled)
				{
					observer.OnNext(e);
				}
			}
		}

		watcher.Created += SendToObserver;
		watcher.Changed += SendToObserver;
		watcher.Renamed += SendToObserver;
		watcher.Deleted += SendToObserver;

		watcher.Error += (_, e) =>
		{
			lock (sync)
			{
				if (!onErrorAlreadyCalled)
				{
					observer.OnError(e.GetException());
					onErrorAlreadyCalled = true;
					watcher.Dispose();
				}
			}
		};

		watcher.EnableRaisingEvents = true;
		return watcher;
	}
}


public class RxFsEventsMultiSubscriber : IObservable<FileSystemEventArgs>
{
	private readonly object _sync = new();

	private readonly List<Subscription> _subscribers = new();

	private readonly FileSystemWatcher _wacther;

	public RxFsEventsMultiSubscriber(string folder)
	{
		_wacther = new FileSystemWatcher(folder);
		_wacther.Created += SendEventToObservers;
		_wacther.Changed += SendEventToObservers;
		_wacther.Renamed += SendEventToObservers;
		_wacther.Deleted += SendEventToObservers;
	}

	void SendEventToObservers(object _, FileSystemEventArgs e)
	{
		lock (_sync)
		{
			foreach (var subscription in _subscribers)
			{
				subscription.Observer.OnNext(e);
			}
		}
	}

	void SendErrorToObservers(object _, ErrorEventArgs e)
	{
		Exception x = e.GetException();
		lock (_sync)
		{
			foreach (var subscription in _subscribers)
			{
				subscription.Observer.OnError(x);
			}
			_subscribers.Clear();
		}
	}

	public IDisposable Subscribe(IObserver<FileSystemEventArgs> observer)
	{
		Subscription sub = new(this, observer);
		lock (_sync)
		{
			_subscribers.Add(sub);

			if (_subscribers.Count == 1)
			{
				_wacther.EnableRaisingEvents = true;
			}
		}
		return sub;
	}

	private void Unsubscribe(Subscription sub)
	{
		lock (_sync)
		{
			_subscribers.Remove(sub);

			if (_subscribers.Count == 0)
			{
				_wacther.EnableRaisingEvents = false;
			}
		}
	}

	private class Subscription : IDisposable
	{
		private RxFsEventsMultiSubscriber? _parent;
		public IObserver<FileSystemEventArgs> Observer { get; }

		public Subscription(
			RxFsEventsMultiSubscriber rxFsEventsMultiSubscriber,
			IObserver<FileSystemEventArgs> observer)
		{
			_parent = rxFsEventsMultiSubscriber;
			Observer = observer;
		}

		public void Dispose()
		{
			_parent?.Unsubscribe(this);
			_parent = null;
		}
	}
}
