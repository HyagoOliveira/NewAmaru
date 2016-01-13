using UnityEngine;

public abstract class GameplayToolComponent : MonoBehaviour, IPauseableEntity
{
    public float speed = 6f;

    public Vector3 InicitalPos { get; protected set; }
    public bool Catched { get; protected set; }
    public bool Exiting { get; protected set; }
    protected Amaru amaru;

    protected virtual void Start()
    {
        Catched = false;
        Exiting = false;
        InicitalPos = transform.position;
    }


    public abstract void Drop();
    public abstract void Catch();

    public virtual void OnPause()
    {
        enabled = false;
    }

    public virtual void OnResume()
    {
        enabled = true;
    }
}
