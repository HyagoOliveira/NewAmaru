
public interface IDynamicActor {
    void SetInitialValues();
    void LoadPlatformStates();
    void UpdateActor();
    void UpdatePhysics();

    void ApplyHorizontalMovement();
    void ApplyVerticalMovement();
    void ApplyGroundMovement();
    void ApplyMovement();
    void ApplyUpdateStateMachine();
    void ApplyUpdateGroundStateMachine();
    void ApplyUpdateAirStateMachine();
    void ApplySlopeLimit();
    void ApplyMeshRotation();
    void ApplyJump();
    void ApplyAnimation();
    void ApplyRespawn();
    void OnFellOffWorld();
    void PlayAudio(string name);
    void InstanciatePrefab(string name);
}
