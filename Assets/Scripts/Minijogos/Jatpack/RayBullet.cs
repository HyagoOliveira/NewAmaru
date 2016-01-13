using UnityEngine;

public class RayBullet : MonoBehaviour {

    public float timeAlive = 10f;
    public float speed = 6f;
    public Vector3 direction = Vector2.right;
	
	// Update is called once per frame
	void Update () {
        transform.position += direction * speed * Time.deltaTime;	
	}
}
