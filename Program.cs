// QuiescentExample();
// BackPressureExample();

var numbers = new MySequenceOfNumbers();

numbers.Subscribe(
	number => WriteLine($"Received value :{number}"),
	() => WriteLine("Sequence terminated")
);

