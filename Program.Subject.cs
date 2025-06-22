using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

public partial class Program
{
	public static void ForwardSubject()
	{
		Subject<int> s = new();

		s.Subscribe(x => WriteLine($"Sub1: {x}"));
		s.OnNext(1);
		s.OnNext(2);
		s.OnNext(3);

		s.Subscribe(x => WriteLine($"Sub2: {x}"));
		s.OnNext(4);
	}

	public static void ReplaySubject()
	{
		ReplaySubject<int> s = new(bufferSize: 2);

		s.Subscribe(x => WriteLine($"Sub1: {x}"));
		s.OnNext(1);
		s.OnNext(2);
		s.OnNext(3);

		s.Subscribe(x => WriteLine($"Sub2: {x}"));
		s.OnNext(4);
		s.OnCompleted();
		s.Subscribe(x => WriteLine($"Sub3: {x}"));
	}

	public static void BehaviorSubject()
	{
		BehaviorSubject<int> s = new(0);

		s.Subscribe(x => WriteLine($"Sub1: {x}"));
		s.OnNext(1);
		s.OnNext(2);
		s.OnNext(3);

		s.Subscribe(x => WriteLine($"Sub2: {x}"));
		s.OnNext(4);
		s.OnCompleted();
		s.Subscribe(x => WriteLine($"Sub3: {x}"));
	}

	public static void AsyncSubjectExample()
	{
		AsyncSubject<string> subject = new();
		subject.OnNext("A");
		subject.Subscribe(x => WriteLine($"Sub1: {x}"));
		subject.OnNext("B");
		subject.OnNext("C");
		subject.OnCompleted();
		subject.Subscribe(x => WriteLine($"Sub2: {x}"));
	}
}
