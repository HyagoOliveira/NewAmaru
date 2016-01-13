public class Minijogo
{
    public MinijogoType tipo;
    public int rank;

    public Minijogo(MinijogoType tipo)
    {
        this.tipo = tipo;
        rank = 0;
    }

    public override string ToString()
    {
        return tipo.ToString() + ", Rank: " + rank;
    }
}

public enum MinijogoType
{
    NONE,
    CUBE,
    JETPACK
}
