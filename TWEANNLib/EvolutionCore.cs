using System;
using System.Collections.Generic;
using System.Threading;
using TWEANNLib.NeuroEvolution;

namespace TWEANNLib
{
    public class Species
    {
        public delegate void OnImprovementDelegate(Species sender, IMutation mutationType);

        public Func<NeuralNetwork, double> FitnessFunc { get; }
        public NeuralNetwork Network { get; }
        public Dictionary<IMutation, double> MutationsCollection { get; }
        public int Iterations { get; private set; }
        public double BestFit { get; private set; }
        public double Sensitivity = 0;
        public event OnImprovementDelegate OnImprovement;

#if DEBUG
        private readonly Random rnd = new Random(1);
#else
        private readonly Random rnd = new Random();
#endif

        public Species(Func<NeuralNetwork, double> fitnessFunc, NeuralNetwork network,
            Dictionary<IMutation, double> mutations)
        {
            FitnessFunc = fitnessFunc;
            Network = network;

            MutationsCollection = new Dictionary<IMutation, double>();

            if (mutations != null && mutations.Count > 0)
                foreach (var mutation in mutations)
                    MutationsCollection.Add(mutation.Key, mutation.Value);


            BestFit = fitnessFunc(network);
        }

        public Species(Func<NeuralNetwork, double> fitnessFunc, NeuralNetwork network)
            : this(fitnessFunc, network, null)
        {

        }

        public void Iteration()
        {
            foreach (var mutation in MutationsCollection)
            {
                if (rnd.NextDouble() < mutation.Value)
                {
                    mutation.Key.Mutate(Network);

                    var mutationFit = FitnessFunc(Network);

                    //Если тип мутации - добавлении нейрона, то мутация корректна сли только преодален порог Sensitivity
                    if (mutation.Key is ConnectNeuron ? (mutationFit - BestFit) > Sensitivity : mutationFit > BestFit)
                    {
                        BestFit = mutationFit;
                        while (mutation.Key.CanImprove)
                        {
                            mutation.Key.Improve();
                            mutationFit = FitnessFunc(Network);

                            Iterations++;

                            if (mutationFit > BestFit)
                                BestFit = mutationFit;
                            else
                                mutation.Key.RollbackMutation();
                        }

                        OnImprovement?.Invoke(this, mutation.Key);
                    }
                    else
                        mutation.Key.RollbackMutation();
                }
            }

            Iterations++;
        }
    }
}