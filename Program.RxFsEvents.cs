using System.Reactive.Linq;

public partial class Program
{
	public static void SimpleFsWatcherExample()
	{
		FileSystemWatcher watcher = new("/Users/sonjiho/Workspace/TryRx");
		IObservable<FileSystemEventArgs> changes = Observable
			.FromEventPattern<FileSystemEventArgs>(watcher, nameof(watcher.Changed))
			.Select(ep => ep.EventArgs);

		watcher.EnableRaisingEvents = true;
	}

	public static void ComplicatedFsWatcherExample()
	{
		ObserveFileSystem("/Users/sonjiho/Workspace/TryRx")
			.Subscribe(a =>
			{
				// TODO:
			});
	}

	static IObservable<FileSystemEventArgs> ObserveFileSystem(string folder)
	{
		return Observable.Defer(() =>
		{

			FileSystemWatcher fsw = new(folder);
			fsw.EnableRaisingEvents = true;
			return Observable.Return(fsw);
		}).SelectMany(fsw =>
			Observable.Merge([
				Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
					h => fsw.Created += h, h => fsw.Created -= h
				),
				Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
					h => fsw.Changed += h, h => fsw.Changed -= h
				),
				Observable.FromEventPattern<RenamedEventHandler, FileSystemEventArgs>(
					h => fsw.Renamed += h, h => fsw.Renamed -= h
				),
				Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
					h => fsw.Created += h, h => fsw.Created -= h
				),
			])
			.Select(ep => ep.EventArgs)
			.Finally(() => fsw.Dispose()))
		.Publish()
		.RefCount();
	}
}
