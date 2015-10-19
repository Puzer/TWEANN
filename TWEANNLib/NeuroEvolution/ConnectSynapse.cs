using System;
using TWEANNLib.NetworkNodes;

namespace TWEANNLib.NeuroEvolution
{
    public sealed class ConnectSynapse : BaseMutatuion
    {
        private Neuron neuronFrom;
        private Neuron neuronTo;
        private Synapse insertedSynapse;

        protected override void Mutation()
        {
            //TODO: оптимизировать. Сделать добавление не только нейронов но и синапсов
            Network.MetricsRecitate();

            var rechoose = false;
            
            do
            {
                rechoose = false;
                neuronFrom = Network.GetRandomNeuron();
                neuronTo = Network.GetRandomNeuron();

                //Loop condition
                if (neuronFrom.Distance <= neuronTo.Distance)
                {
                    rechoose = true;
                    continue;
                }

                if (neuronTo.InputSynapses.Exists(n => n._input == neuronFrom))
                    rechoose = true;

                

            } while (rechoose);
           

            insertedSynapse = new Synapse(neuronFrom) { Weight = NeuralHelper.GetRandomWeigth() }; ;
            neuronTo.InputSynapses.Add(insertedSynapse);
            Network.Synapses.Add(insertedSynapse);

            Network.MetricsRecitate();

            if (Network.LoopDetected())
            {
                Rollback();
                Network.Neurons.ForEach(n => n.Reset());
            }
            
        }

        protected override void Rollback()
        {
            neuronTo.InputSynapses.Remove(insertedSynapse);
            Network.Synapses.Remove(insertedSynapse);

            Network.MetricsRecitate();
        }
    }
}