public abstract class BehaviourTree : AC_Component
{
    Node root = null;

    public override void ComponentStart()
    {
        root = SetupTree();
    }

    public override void ComponentUpdate()
    {
        if (root != null)
        {
            root.Evaluate();
        }
    }
    protected abstract Node SetupTree();
}
