using TWEANNLib.NetworkNodes;

namespace TWEANNLib.NeuroEvolution
{
    public sealed class RandomSynapseReconnect : BaseMutatuion
    {
        private Synapse synapse;
        private IComputingNode oldDestination;

        protected override void Mutation()
        {
            synapse = Network.GetRandomSynapse();
            oldDestination = synapse._input;

            synapse._input = Network.GetRandomNode();
        }

        protected override void Rollback()
        {
            synapse._input = oldDestination;
        }

    }
}