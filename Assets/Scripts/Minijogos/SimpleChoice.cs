using UnityEngine;

[RequireComponent(typeof(GameplayItemComponent))]
public class SimpleChoice : Choice
{
    protected override void GoSelectionPositionAnimation()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
    }
}
