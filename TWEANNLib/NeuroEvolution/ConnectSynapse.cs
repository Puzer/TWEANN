using TWEANNLib.NetworkNodes;

namespace TWEANNLib.NeuroEvolution
{
    public sealed class ConnectSynapse : BaseMutatuion
    {
        private IComputingNode neuronFrom;
        private Neuron neuronTo;
        private Synapse insertedSynapse;

        protected override void Mutation()
        {
            do
            {
                neuronFrom = Network.GetRandomNode();
                neuronTo = Network.GetRandomNeuron();
            } while (!(neuronFrom == neuronTo || neuronTo.InputSynapses.Exists(n => n._input == neuronFrom)));


            insertedSynapse = new Synapse(neuronFrom) { Weight = NeuralHelper.GetRandomWeigth() }; ;
            neuronTo.InputSynapses.Add(insertedSynapse);
            Network.Synapses.Add(insertedSynapse);
        }

        protected override void Rollback()
        {
            neuronTo.InputSynapses.Remove(insertedSynapse);
            Network.Synapses.Remove(insertedSynapse);
        }
    }
}