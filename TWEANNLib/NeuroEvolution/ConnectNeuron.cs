using System;
using TWEANNLib.NetworkNodes;

namespace TWEANNLib.NeuroEvolution
{
    public sealed class ConnectNeuron : BaseMutatuion
    {
        private Neuron insertedNeuron;

        private Neuron inputNeuron;
        private Neuron outputNeuron;

        private Synapse synapseToOutput;
        private Synapse synapseToInput;

        public Func<double, double> ActivationFunc;

        public ConnectNeuron(Func<double, double> activationFunc)
        {
            ActivationFunc = activationFunc;
        }

        protected override void Mutation()
        {

            do
            {
                outputNeuron = Network.GetRandomNeuron();
                inputNeuron = Network.GetRandomNeuron();
            } while ((inputNeuron == outputNeuron));

            insertedNeuron = new Neuron(ActivationFunc);

            synapseToOutput = new Synapse(insertedNeuron) { Weight = NeuralHelper.GetRandomWeigth() };
            outputNeuron.InputSynapses.Add(synapseToOutput);

            synapseToInput = new Synapse(inputNeuron) { Weight = NeuralHelper.GetRandomWeigth() };
            insertedNeuron.InputSynapses.Add(synapseToInput);



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

            inputNeuron = null;
            synapseToInput = null;
            synapseToOutput = null;
        }

    }
}