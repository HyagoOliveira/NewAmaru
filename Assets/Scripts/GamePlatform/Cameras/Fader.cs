using System.Collections;
using UnityEngine;

/// <summary>
/// Fades in and out.
/// Put this script inside a Game Object controller.
/// </summary>
[RequireComponent(typeof(GUITexture))]
public class Fader : MonoBehaviour, ISingletonManager
{
    public static Fader Instance { get; private set; }

    public float speed = 0.8f;

    private bool fadingIn = false;
    private bool fadingOut = false;
    private FadeAction FadeInAction;
    private FadeAction FadeOutAction;
    private GUITexture guiTextureComponent;


    public delegate void FadeAction();

    void Awake()
    {
    	SetSingleton();    	
    }

    void Start()
    {
        Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        guiTextureComponent = GetComponent<GUITexture>();
        guiTextureComponent.texture = texture;
        guiTextureComponent.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
        guiTextureComponent.color = Color.clear;
        transform.position = Vector3.zero;
    }

    private void Update()
    {
    	if(fadingIn)
    	{
    		FadeToDark();    		
    	}
    	else if(fadingOut)
    	{
    		FadeToClear();    		
    	}
    }

    public void SetSingleton()
    {
    	if(Instance == null)
    		Instance = this;
    	else
    		DestroyImmediate (this);
    }

    public void ChangeScene(string newScene, FadeAction fadeOutAction)
    {
        FadeIn();
        this.FadeOutAction = fadeOutAction;
        AsyncOperation scene = Application.LoadLevelAsync(newScene);
        StartCoroutine(LoadSceneCoroutine(scene));
    }

    public void ChangeScene(int newScene, FadeAction fadeOutAction)
    {
        FadeIn();
        this.FadeOutAction = fadeOutAction;
        AsyncOperation scene = Application.LoadLevelAsync(newScene);
        StartCoroutine(LoadSceneCoroutine(scene));
    }

    public void FadeInOut(FadeAction fadeInAction, FadeAction fadeOutAction)
    {
    	this.FadeInAction = fadeInAction;
    	this.FadeOutAction = fadeOutAction;
    	this.FadeInAction += FadeOut;
    	FadeIn();
    }


    public void FadeIn(FadeAction Action)
    {
    	this.FadeInAction = Action;
    	FadeIn();
    }
    public void FadeIn()
    {
    	guiTextureComponent.enabled = true;
    	guiTextureComponent.color = Color.clear;
    	fadingOut = false;
    	fadingIn = true;
    }

    public void FadeOut(FadeAction Action)
    {
    	this.FadeOutAction = Action;  	
    	FadeOut();
    }
    public void FadeOut()
    {
    	guiTextureComponent.enabled = true;
    	guiTextureComponent.color = Color.black;
    	fadingOut = true;
    	fadingIn = false;
    }

    private void StartFadeInAction()
    {
    	if(this.FadeInAction != null)
    	{
    		this.FadeInAction ();
    		FadeInAction = null;
    	}
    }

    private void StartFadeOutAction()
    {
    	if(this.FadeOutAction != null)
    	{
    		this.FadeOutAction();
    		FadeOutAction = null;
    	}
    }

    private void FadeToClear()
    {
        Color c = guiTextureComponent.color;
        c.a -= speed * Time.deltaTime;
        guiTextureComponent.color = c;
        if (guiTextureComponent.color.a <= 0f)
		{
			guiTextureComponent.color = Color.clear;
			fadingOut = false;
    		guiTextureComponent.enabled = false;
			StartFadeOutAction();
		}
    }

    private void FadeToDark()
    {
        Color c = guiTextureComponent.color;
        c.a += speed * Time.deltaTime;
        guiTextureComponent.color = c;
    	if(guiTextureComponent.color.a >= 1f)
		{
			guiTextureComponent.color = Color.black;
			fadingIn = false;
			StartFadeInAction();
		}
    }

    private IEnumerator LoadSceneCoroutine(AsyncOperation scene)
    {
        //print("Before load: " + Time.time);
        //yield return new WaitUntil(() => scene.isDone);
        while (!scene.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }

        //print("After load: " + Time.time);

        FadeOut();
    }
}