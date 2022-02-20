using NoiseWorldGen.Core;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var noise = new Noise(new(), 5, 150, .025);
var values = Enumerable.Repeat(noise, 50)
    .Select(static (n, x) => n.Generate(x))
    .Select(static i => (int)i)
    .ToArray();

for (int i = 0; i < values.Length; i++)
    Console.WriteLine(new string('*', Math.Abs(values[i])));
