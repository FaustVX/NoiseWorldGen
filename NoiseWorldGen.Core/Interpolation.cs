using System.Collections;

namespace NoiseWorldGen.Core;

public class Interpolation : IEnumerable<(float noise, int value)>
{
    private readonly IDictionary<float, int> _values;

    public Interpolation(int start, int end)
    {
        _values = new Dictionary<float, int>(2)
        {
            [-1] = start,
            [1] = end,
        };
    }

    public void Add(float noiseValue, int value)
        => _values[noiseValue] = value;

    public int Lerp(float noise)
    {
        if (noise <= -1)
            return _values[-1];
        if (noise >= 1)
            return _values[1];
        if (_values.TryGetValue(noise, out var value))
            return value;
        var (start, end) = GetInterval(noise);
        var noiseInterpolation = (noise - start.noise) / (end.noise - start.noise);
        return (int)((end.value - start.value) * noiseInterpolation) + start.value;

        ((float noise, int value) start, (float noise, int value) end) GetInterval(float value)
        {
            var limits = (_values.Where(kvp => kvp.Key <= value).MaxBy(kvp => kvp.Key), _values.Where(kvp => kvp.Key >= value).MinBy(kvp => kvp.Key));
            return ((limits.Item1.Key, limits.Item1.Value), (limits.Item2.Key, limits.Item2.Value));
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public IEnumerator<(float noise, int value)> GetEnumerator()
        => _values.Select(static kvp => (kvp.Key, kvp.Value)).GetEnumerator();
}
