namespace NoiseWorldGen.OpenGL;

public class World
{
    private readonly Dictionary<(int x, int y), Chunck> Chunks = new()
    {
        [(0, 0)] = new(),
    };

    public Tile this[int x, int y]
    {
        get
        {
            var chunk = GetChunkAtPos(x, y, out var posX, out var posY);
            return chunk[posX, posY];
        }
        set
        {
            var chunk = GetChunkAtPos(x, y, out var posX, out var posY);
            chunk[posX, posY] = value;
        }
    }

    public Chunck GetChunkAtPos(int x, int y, out int posX, out int posY)
    {
        (var divX, posX) = ProperRemainder(x, Chunck.Size);
        (var divY, posY) = ProperRemainder(y, Chunck.Size);
        return GetOrCreateValue(Chunks, (divX, divY), out _);
    }
}
