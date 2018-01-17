namespace Ai
{
    namespace BehaviourTree
    {
        abstract public class Decorator 
            : Node
            , IDecorator
        {
            protected Node child = null;

            public Decorator(Node node)
            {
                this.child = node;
            }

            public INode GetChild()
            {
                return child;
            }

            public override void OnReset()
            {
                child.ResetHandle();
            }
        }
    }
}
