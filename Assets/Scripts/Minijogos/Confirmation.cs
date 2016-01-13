public abstract class Confirmation : SpriteEntity
{
    public ConfirmationType type;

    protected abstract void ConfirmAction();
}

public enum ConfirmationType
{
    POSITIVE,
    NEGATIVE
}
