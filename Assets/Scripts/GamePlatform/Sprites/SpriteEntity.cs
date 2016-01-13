using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteEntity : MonoBehaviour {

    public SpriteRenderer SpriteRenderer { get; protected set; }

    public float Width { get { return SpriteRenderer.bounds.size.x; } }
    public float Height { get { return SpriteRenderer.bounds.size.y; } }

    public float Angle
    {
        get { return transform.eulerAngles.z; }
        set { transform.eulerAngles = new Vector3(transform.transform.eulerAngles.x, transform.transform.eulerAngles.y, value); }
    }

    public Color Color
    {
        get { return SpriteRenderer.color; }
        set { SpriteRenderer.color = value; }
    }

    protected virtual void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
}
