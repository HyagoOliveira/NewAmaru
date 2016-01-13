using Minijogos;
using UnityEngine;

public abstract class Choice : SpriteEntity
{
	public SpriteRenderer imageModelRenderer;
    public TextMesh textModelMesh;
    public GameplayItemComponent gameplayComponent;
    public string Model;// { get; private set; }

    protected bool moving = false;
    protected Vector3 targetPosition;

    public int Index{ get; set; }    
        
    protected abstract void GoSelectionPositionAnimation();  


    protected virtual void Start()
    {
    	FixTextOrder();
    	SetupGameplayComponent();
    	SetActivationComponent(true);

        if (GerenciadorTarefas.Instance.Data.TarefaAprendizadoAtual.IsFormaExibicaoPalavra())
        {
            textModelMesh.transform.localScale = Vector3.one * 0.6f;
        }
    }

    protected virtual void Update()
    {
        if (moving)
        {
        	if(IsOnTargetPosition())
        	{
        		moving = false;
        		transform.position = targetPosition;
        	}
        	else
        	{
        		GoSelectionPositionAnimation();
        	}
        }            
    }

    public virtual void SetModelImage(Sprite sprite, string name)
    {
        Model = name;
        imageModelRenderer.sprite = sprite;
    }

    public virtual void SetModelText(string text)
    {
        Model = text;
        textModelMesh.text = text;
    }

	public virtual void ResetPosition()
    {
        targetPosition = gameplayComponent.InitialPosition;
        moving = true;
        gameplayComponent.CanInteract = true;
    } 

    public virtual void SetSelectedPosition(Vector3 position)
    {
        targetPosition = position;
        moving = true;
    }

    protected virtual void SetupGameplayComponent()
    {
        if (gameplayComponent == null)
            gameplayComponent = GetComponent<GameplayItemComponent>();
        gameplayComponent.OnAction += ConfirmAction;        
    }

    protected virtual void SetActivationComponent(bool active)
    {
        gameplayComponent.CanInteract = active;        
    }

    protected virtual bool IsOnTargetPosition()
    {
    	return Vector3.Distance(transform.position, targetPosition) < 0.1f;
    }

    protected virtual void FixTextOrder()
    {
        Renderer textRenderer = textModelMesh.GetComponent<Renderer>();
        textRenderer.sortingLayerID = imageModelRenderer.sortingLayerID;
        textRenderer.sortingOrder = imageModelRenderer.sortingOrder + 1;
    }    

    protected virtual void ConfirmAction()
    {        
        SetActivationComponent(false);        
        GerenciadorTarefas.Instance.CurrentMinijogoGameplay.ChoiceSelectionAction(Index);
    }
}
