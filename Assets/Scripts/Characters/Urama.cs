using ALEPP;
using Minijogos;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider2D))]
public class Urama : SpriteEntity
{
    public static Urama Instance { get; private set; }

    public SpriteRenderer ballon;
    public SpriteRenderer imageModel;
    public SpriteRenderer speakerSprite;
    public TextMesh textModel;

    public float maxAmaruDistance = 10f;
	public float minAmaruDistance = 2f;

	public float speed = 1.2f;
	public float animationSpeed = 0.6f;
	public float animationAmplitude = 2f;

	private UramaState state = UramaState.FOLLOWING_AMARU;

	private Vector3 initialDistance;
    private Amaru amaru;
    private AudioSource audioSource;

    protected void Start()
    {
        SetSingleton();
        amaru = Amaru.Instance as Amaru;
        initialDistance = transform.position - amaru.transform.position;
        audioSource = GetComponent<AudioSource>();
        GetComponent<BoxCollider2D>().isTrigger = true;

        //put text on front of ballon
        FixTextOrder();

        SetBallonenable(false);
    }

    private void FixTextOrder()
    {
        Renderer textRenderer = textModel.GetComponent<Renderer>();
        Renderer ballonRenderer = ballon.GetComponent<Renderer>();
        textRenderer.sortingLayerID = ballonRenderer.sortingLayerID;
        textRenderer.sortingOrder = ballonRenderer.sortingOrder + 1;
        textModel.text = string.Empty;
    }

    private void SetBallonenable(bool enable, bool text = true, bool image = true, bool speaker = false)
    {
        ballon.enabled = enable;
        textModel.gameObject.SetActive(text);
        imageModel.enabled = image;
        speakerSprite.enabled = speaker;
    }

    private void SetSingleton()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    void Update(){
		switch(state){
			case UramaState.FOLLOWING_AMARU:
				FollowAmaru();
                ApplyFlipDirection();
                break;

			case UramaState.GIVING_INSTRUCTIONS:
				GoInstructionPosition();
				break;
		}		
	}

    public void GoToInstructionPlace()
    {
        state = UramaState.GIVING_INSTRUCTIONS;        
        TurnRight();
    }

    public void GoFollowAmaru()
    {
        state = UramaState.FOLLOWING_AMARU;
        HideModel();
    }

    public void ShowModel(TarefaAprendizado tarefa)
    {

        switch (tarefa.tipoTarefa.GetFormaExibicaoModelo())
        {
            case (char)DefinicaoTipoTarefa.SOM:
                audioSource.clip = GerenciadorTarefas.Instance.Data.GetAudioEnsino(tarefa.modelo.id);
                PlayAudioModel();
                SetBallonenable(true, false, false, true);
                break;

            case (char)DefinicaoTipoTarefa.IMAGEM:
                SetBallonenable(true);
                imageModel.sprite = GerenciadorTarefas.Instance.Data.GetImageEnsino(tarefa.modelo.id);
                break;

            case (char)DefinicaoTipoTarefa.PALAVRA:
                SetBallonenable(true);
                textModel.text = tarefa.modelo.nome;
                break;

            default:
                break;
        }
    }

    public void HideModel()
    {
        audioSource.clip = null;
        imageModel.sprite = null;
        textModel.text = string.Empty;
        SetBallonenable(false, false, false, false);
    }

	private void FollowAmaru(){
        Vector3 targetPosition = amaru.transform.position + initialDistance + PingPongAnimation();
		transform.position = Vector2.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
	}

	private Vector3 PingPongAnimation(){
		return Vector2.up * Mathf.PingPong(Time.time, animationAmplitude) * animationSpeed;
	}

	private void ApplyFlipDirection(){
        if (amaru.Direction == SideScrollingDirection.RIGHT && !isAmaruOnRight())
            initialDistance.x = Mathf.Abs(initialDistance.x) * -1f;
        else if(amaru.Direction == SideScrollingDirection.LEFT && !isAmaruOnLeft())
            initialDistance.x = Mathf.Abs(initialDistance.x);


        if (transform.localScale.x > 0 && isAmaruOnLeft())
            TurnLeft();
        else if (transform.localScale.x < 0 && isAmaruOnRight())
            TurnRight();
    }

    private void TurnLeft()
    {
        transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
    }

    private void TurnRight()
    {
        transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
    }

    private bool isAmaruOnRight(){
		return amaru.transform.position.x > transform.position.x;
	}

	private bool isAmaruOnLeft(){
		return amaru.transform.position.x < transform.position.x;
	}

	private void GoInstructionPosition(){
        Vector3 instructionPosition = Follow2DCamera.Instance.GetTopLeftWorldPosition(6f) + new Vector3(Width / 2f, -Height, 0f);
        transform.position = Vector3.Lerp(transform.position, instructionPosition, speed * Time.deltaTime);
    }

    private void OnMouseDown()
    {
        PlayAudioModel();
    }

    private void PlayAudioModel()
    {
        if (audioSource.clip != null)
        {
            BackgroundMusicManager.Instance.Pause(1f);
            audioSource.Stop();
            audioSource.Play();
        }
    }
}

public enum UramaState{
	FOLLOWING_AMARU,
	GIVING_INSTRUCTIONS
}