/// <summary>
/// Actor State.
/// This class helps Actor State Controller.
/// </summary>
public class ActorState
{
	public string Name { get; private set; }
	public string AnimationName { get; private set; }
	private static int index = -1;

	public int Index { get; set; }

	public delegate void StateActionEvent ();


	private event StateActionEvent enterStateAction;
	private event StateActionEvent onStateAction;	
	private event StateActionEvent exitStateAction;


	public float CurrentTime { get; set; }
	public float LastTime { get; set; }

	public ActorState (string name) : 
		this(name, name)
	{
	}


	public ActorState (string name, string animationName)
	{
		Name = name;
		AnimationName = animationName;
		Index = ++index;

		CurrentTime = 0f;
		LastTime = CurrentTime;
	}

	public void Update ()
	{
		if (onStateAction != null) {
			onStateAction ();
		}
	}

	public void OnEnterState ()
	{
		if (enterStateAction != null) {
			CurrentTime = 0f;
			enterStateAction ();
		}
	}

	public void OnExitState ()
	{
		if (exitStateAction != null) {
			exitStateAction ();
			LastTime = CurrentTime;
			CurrentTime = 0f;
		}
	}

	public void RegisterOnEnterState (StateActionEvent action)
	{
		enterStateAction += action;
	}

	public void RegisterOnState (StateActionEvent action)
	{
		onStateAction += action;
	}

	public void RegisterOnExitState (StateActionEvent action)
	{
		exitStateAction += action;
	}

	public override string ToString ()
	{
		return Name;
		//return string.Format ("[PlatformState: Name={0}, AnimationName={1}]", Name, AnimationName);
	}
	
}
