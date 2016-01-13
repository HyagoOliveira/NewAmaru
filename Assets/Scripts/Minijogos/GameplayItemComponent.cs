using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public abstract class GameplayItemComponent : MonoBehaviour, IGameplayAction
{
    public bool CanInteract { get; set; }
    public Vector3 InitialPosition { get; protected set; }
    public bool DoingAnimation { get; protected set; }

    protected AudioSource audioComponent;

    public delegate void ActionEvent();
    public event ActionEvent OnAction;

    protected void Start()
    {
        CanInteract = true;
        audioComponent = GetComponent<AudioSource>();
        InitialPosition = transform.position;
        DoingAnimation = false;
    }
	
	
	protected virtual void Update () {
        if (DoingAnimation)
        {
            OnAnimation();
        }
    }    

    protected virtual void PlayAudioEFX()
    {
        audioComponent.Play();
    }

    public virtual void OnConfirmAction()
    {
        if (OnAction == null)
            throw new UnityException("Ação do componente é nula.");

        OnAction();
    }

    public virtual void StopAnimation()
    {
        DoingAnimation = false;
    }

    public virtual void StartInteractionAnimation()
    {
        if (CanInteract && !DoingAnimation)
        {
            PlayAudioEFX();
            DoingAnimation = true;
        }
    }

    public abstract void OnAnimation();
    public abstract void OnAnimationFinish();
}
