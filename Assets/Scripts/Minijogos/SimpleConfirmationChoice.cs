using Minijogos;
using UnityEngine;

[RequireComponent(typeof(GameplayItemComponent))]
public class SimpleConfirmationChoice : Confirmation
{
    public GameplayItemComponent component { get; private set; }

    protected void Start()
    {
        component = GetComponent<GameplayItemComponent>();
        component.OnAction += ConfirmAction;  
    }

    protected override void ConfirmAction()
    {
        switch (type)
        {
            case ConfirmationType.POSITIVE:
                GerenciadorTarefas.Instance.CurrentMinijogoGameplay.PositiveSelectionAction();
                break;
            case ConfirmationType.NEGATIVE:
                GerenciadorTarefas.Instance.CurrentMinijogoGameplay.NegativeSelectionAction();
                break;
            default:
                break;
        }
    }
}


