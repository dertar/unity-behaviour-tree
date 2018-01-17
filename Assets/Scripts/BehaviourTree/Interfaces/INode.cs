using Ai.BehaviourTree;
namespace Ai
{
    namespace BehaviourTree
    {
        public interface INode
        {
            ENodeState Behaviour(Context data);
            void ResetHandle();
            void OnInit(Context data);

            ENodeState OnBehaviour(Context data);
            void OnReset();
        }
    }

}
