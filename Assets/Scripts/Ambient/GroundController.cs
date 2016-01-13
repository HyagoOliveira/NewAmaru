using System.Linq;
using UnityEngine;

public class GroundController : MonoBehaviour {

    public string groundTag = "Ground";
    //public Transform mountainBackground
    public bool respawToRight = true;
    public bool respawToLeft = false;

    public static GroundController Instance { get; private set; }

    private int mostRightGroundIndex = 0;
	private int mostLeftGroundIndex = 0;

	private GameObject[] grounds;

	private Transform amaruTransform;

    private BoxCollider leftBoundaryCollider;
    private BoxCollider rightBoundaryCollider;
    private Follow2DCamera followCamera;

	void Start(){
        SetSingleton();
        amaruTransform = Amaru.Instance.transform;
        followCamera = Camera.main.GetComponent<Follow2DCamera>();

        FindGrounds();
        CreateBoundariesColliders();
    }

    private void SetSingleton()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    void Update()
    {
        if (respawToRight && NeedRespawnToRight())
        {
            SwapToRight();
        }

        if(respawToLeft && NeedRespawnToLeft())
        {
            SwapToLeft();
        }
    }

    private void SwapToLeft()
    {
        grounds[mostRightGroundIndex].transform.position = GetNextMostLeftGroundPosition();

        //swap now
        mostLeftGroundIndex = mostRightGroundIndex;
        mostRightGroundIndex = GetMostRightIndex();

        PlaceBoundaryColliders();
    }

    private void SwapToRight()
    {
        grounds[mostLeftGroundIndex].transform.position = GetNextMostRightGroundPosition();

        //swap now
        mostRightGroundIndex = mostLeftGroundIndex;
        mostLeftGroundIndex = GetMostLeftIndex();

        PlaceBoundaryColliders();
    }

    private bool NeedRespawnToRight()
    {
        return amaruTransform.transform.position.x > grounds[mostRightGroundIndex].transform.position.x;
    }

    private bool NeedRespawnToLeft()
    {
        return amaruTransform.transform.position.x < grounds[mostLeftGroundIndex].transform.position.x;
    }

    private void FindGrounds(){
		grounds = transform.Cast<Transform>().Where(c => c.gameObject.tag == groundTag).Select(c => c.gameObject).ToArray();
        //GameObject.FindGameObjectsWithTag(groundTag);
        
		if(grounds == null){
			throw new UnityException("No grounds with " + groundTag + " defined.");
		}
		
		mostLeftGroundIndex = grounds.Length - 1;
		mostRightGroundIndex = 0;

		for(int i=0; i < grounds.Length; i++){
            if (grounds[i].transform.position.x < grounds[mostLeftGroundIndex].transform.position.x)
                mostLeftGroundIndex = i;

            if (grounds[i].transform.position.x > grounds[mostRightGroundIndex].transform.position.x)
                mostRightGroundIndex = i;
        }
        
	}    

	private Vector3 GetNextMostRightGroundPosition(){
		return grounds[mostRightGroundIndex].transform.position + Vector3.right *
                grounds[mostRightGroundIndex].transform.GetComponent<Renderer>().bounds.size.x;
	}

    private Vector3 GetNextMostLeftGroundPosition()
    {
        return grounds[mostLeftGroundIndex].transform.position - Vector3.right *
                grounds[mostLeftGroundIndex].transform.GetComponent<Renderer>().bounds.size.x;
    }

    private int GetMostLeftIndex()
    {
        int index = grounds.Length - 1;
        for (int i = 0; i < grounds.Length; i++)
        {
            if (grounds[i].transform.position.x < grounds[index].transform.position.x)
                index = i;
        }

        return index;
    }

    private int GetMostRightIndex()
    {
        int index = 0;
        for (int i = 0; i < grounds.Length; i++)
        {
            if (grounds[i].transform.position.x > grounds[index].transform.position.x)
                index = i;
        }

        return index;
    }

    private void CreateBoundariesColliders()
    {
        leftBoundaryCollider = this.gameObject.AddComponent<BoxCollider>();
        rightBoundaryCollider = this.gameObject.AddComponent<BoxCollider>();

        leftBoundaryCollider.size = new Vector3(1f, 5f, 5f);
        rightBoundaryCollider.size = new Vector3(1f, 5f, 5f);

        PlaceBoundaryColliders();
    }

    private void PlaceLeftBoundaryCollider()
    {
        leftBoundaryCollider.center = new Vector3(grounds[mostLeftGroundIndex].transform.position.x -
            grounds[mostLeftGroundIndex].GetComponent<Renderer>().bounds.size.x / 3f, 3f, 0f);


        followCamera.LeftBoundary = leftBoundaryCollider.center.x + 5f;
    }

    private void PlaceRightBoundaryCollider()
    {
        rightBoundaryCollider.center = new Vector3(grounds[mostRightGroundIndex].transform.position.x +
            grounds[mostRightGroundIndex].GetComponent<Renderer>().bounds.size.x / 3f, 3f, 0f);

        followCamera.RightBoundary = rightBoundaryCollider.center.x -5f;
    }

    public void PlaceBoundaryColliders(Vector3 leftPosition, Vector3 rightPosition)
    {
        rightPosition.y = leftPosition.y = 3f;
        rightBoundaryCollider.center = rightPosition;
        leftBoundaryCollider.center = leftPosition;
    }

    public void PlaceBoundaryColliders()
    {
        PlaceLeftBoundaryCollider();
        PlaceRightBoundaryCollider();
    }
}