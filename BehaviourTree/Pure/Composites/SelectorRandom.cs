namespace Ai
{
    namespace BehaviourTree
    {
        public class SelectorRandom : Selector
        {
            public SelectorRandom(string name, params Node[] nodes) : base(name, nodes)
            {
                MixNodes();
            }

            public override void OnReset()
            {
                MixNodes();
                ResetAllChildren();
            }
        }
    }
}
