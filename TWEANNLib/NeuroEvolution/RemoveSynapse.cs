using System.Linq;
using TWEANNLib.NetworkNodes;

namespace TWEANNLib.NeuroEvolution
{
    public sealed class RemoveSynapse : BaseMutatuion
    {

        private Synapse removedSynapse;
        private Neuron fromNeuron;
        private bool fail;

        protected override void Mutation()
        {

            removedSynapse = Network.GetRandomSynapse();

            if (Network.Synapses.Count < 3 || Network.Effectors.Exists(e => e._input == removedSynapse))
            {
                fail = true;
                return;

            }

            fromNeuron = Network.Neurons.First(n => n.InputSynapses.Contains(removedSynapse));

            fromNeuron.InputSynapses.Remove(removedSynapse);
            Network.Synapses.Remove(removedSynapse);
        }

        protected override void Rollback()
        {
            if (fail)
                return;


            fromNeuron.InputSynapses.Add(removedSynapse);
            Network.Synapses.Add(removedSynapse);

        }

    }
}