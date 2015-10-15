using System;
using System.Collections.Generic;
using System.Linq;
using TWEANNLib.NetworkNodes;

namespace TWEANNLib
{
    [Serializable]
    public sealed class NeuralNetwork
    {
        internal readonly List<IReceptor> Receptors;
        internal readonly List<Synapse> Synapses;
        internal readonly List<Neuron> Neurons;
        internal readonly List<Effector> Effectors;

        private readonly List<Receptor> pushableReceptors;
        
        public int InputCount
        {
            get
            {
                return Receptors.Count(r => r.GetType() == typeof(Receptor));
            }
        }
        public int OutputCount => Effectors.Count;

        public int SynapsesCount => Synapses.Count;
        public int NeuronsCount => Neurons.Count;

        internal NeuralNetwork(List<IReceptor> receptors, List<Synapse> synapses, List<Neuron> neurons, List<Effector> effectors)
        {
            Receptors = receptors;
            Synapses = synapses;
            Neurons = neurons;
            Effectors = effectors;

            pushableReceptors =
                Receptors.Where(r => r.GetType() == typeof(Receptor)).Select(r => (Receptor)r).ToList();
        }

        private void PushDataToReceptors(params double[] values)
        {
            /*
            TODO: params немного замедляет вычисления
            */

            if (values.Length != pushableReceptors.Count)
                throw new Exception("Count of receptors and input data does`t equal");

            for (int i = 0; i < values.Length; i++)
                pushableReceptors[i].Charge = values[i];
        }

        private IEnumerable<double> Solve()
        {
            Neurons.ForEach(n => n.Reset());

            /*// TODO: Паралельные вычисления ускоряют не всегда
              // Включать если скрытых слоёв больше 3 (?) и нейронов на одном слое больше ~100
               Neurons.Where(s=>s.LayerId==2).AsParallel().ForAll(a=>a.Compute()); */

            return Effectors.Select(e => e.Solve());
        }

        public IEnumerable<double> Solve(params double[] values)
        {
            /*
            TODO: params немного замедляет вычисления
            */


            PushDataToReceptors(values);
            return Solve();
        }

        public double Solve(double value)
        {
            if (pushableReceptors.Count!=1 && Effectors.Count!=1)
                throw new Exception("Count of receptors or effectors not equal 1");

            Neurons.ForEach(n => n.Reset());
            
            pushableReceptors[0].Charge = value;


            for (int i = Neurons.Max(n => n.Distance); i >= 0; i--)
            {
                var neuronsOnLayer = Neurons.Where(n => n.Distance == i);
                foreach (var neuron in neuronsOnLayer)
                    neuron.Compute();

            }

            var result = Effectors[0].Solve();
            if (LoopDetected())
            {
                NeuralBuilder.BuildDOTGraph(this, "outgraph.dot");
            }

            return result;
        }

        public void MetricsRecitate()
        {
            foreach (var effector in Effectors)
                effector._input.PassMetric(0);
        }

        public bool LoopDetected()
        {
            return Neurons.Exists(n => n.LoopDetected);
        }
    }

}