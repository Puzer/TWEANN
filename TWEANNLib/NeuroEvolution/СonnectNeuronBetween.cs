using System;
using TWEANNLib.NetworkNodes;

namespace TWEANNLib.NeuroEvolution
{
    public sealed class СonnectNeuronBetween : BaseMutatuion
    {
        private Neuron insertedNeuron;

        private IComputingNode inputConnection;
        private Neuron outputNeuron;

        private Synapse synapseToOutput;
        private Synapse synapseToInput;

        private Synapse randomSynapse;
        public Func<double, double> ActivationFunc;

        protected override void Mutation()
        {

            randomSynapse = Network.GetRandomSynapse();



            outputNeuron = Network.Neurons.Find(n => n.InputSynapses.Contains(randomSynapse));
            inputConnection = randomSynapse._input;

            insertedNeuron = new Neuron(ActivationFunc);

            synapseToOutput = new Synapse(insertedNeuron) { Weight = NeuralHelper.GetRandomWeigth() };
            outputNeuron.InputSynapses.Add(synapseToOutput);

            synapseToInput = new Synapse(inputConnection) { Weight = NeuralHelper.GetRandomWeigth() };
            insertedNeuron.InputSynapses.Add(synapseToInput);

            outputNeuron.InputSynapses.Remove(randomSynapse);
            Network.Synapses.Remove(randomSynapse);


            Network.Neurons.Add(insertedNeuron);
            Network.Synapses.Add(synapseToOutput);
            Network.Synapses.Add(synapseToInput);
        }

        protected override void Rollback()
        {
            outputNeuron.InputSynapses.Remove(synapseToOutput);

            Network.Neurons.Remove(insertedNeuron);

            Network.Synapses.Remove(synapseToInput);
            Network.Synapses.Remove(synapseToOutput);

            outputNeuron.InputSynapses.Add(randomSynapse);
            Network.Synapses.Remove(randomSynapse);
        }

        public СonnectNeuronBetween(Func<double, double> activationFunc)
        {
            ActivationFunc = activationFunc;
        }
    }
}