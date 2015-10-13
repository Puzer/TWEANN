using System;
using TWEANNLib.NetworkNodes;

namespace TWEANNLib.NeuroEvolution
{
    public sealed class RemoveNeuron : BaseMutatuion
    {
        private Neuron removedNeuron;

        new public void Mutate(NeuralNetwork network)
        {
            Network = network.Clone();
        }

        protected override void Mutation()
        {
            //never call
            throw new NotImplementedException();
        }

        protected override void Rollback()
        {

        }
    }
}