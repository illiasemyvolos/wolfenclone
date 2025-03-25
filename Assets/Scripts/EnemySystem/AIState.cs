public abstract class AIState
{
    protected AIController ai;

    public AIState(AIController ai)
    {
        this.ai = ai;
    }

    /// <summary>
    /// Called when the state is entered.
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// Called periodically by the AIController.
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Called when the state is exited.
    /// </summary>
    public abstract void Exit();
}