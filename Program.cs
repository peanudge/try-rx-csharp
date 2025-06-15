// QuiescentExample();
// BackPressureExample();


using System.Reactive.Linq;

var ticks = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1));

// var intervals = Observable.TimeInterval(ticks)
// 	.Subscribe(interval =>
// 	{
// 		WriteLine(interval);
// 	});

