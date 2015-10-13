using System;
using System.Collections.Generic;
using System.Linq;

namespace TWEANNLib.NetworkNodes
{
    [Serializable]
    internal sealed class Neuron : IComputingNode
    {
        internal readonly List<Synapse> InputSynapses;
        private readonly Func<double, double> _activationFunc;
        internal int LayerId;
        private double? _calculatedCompute;
        private bool _loopProtector;
        internal bool LoopDetected;

        internal Neuron(Func<double, double> activationFunc, IEnumerable<Synapse> inputSynapse, int layer)
        {
            _activationFunc = activationFunc;
            InputSynapses = new List<Synapse>();
            InputSynapses.AddRange(inputSynapse);
            LayerId = layer;

        }

        internal Neuron(Func<double, double> activationFunc, IEnumerable<Synapse> inputSynapse) : this(activationFunc, inputSynapse, 0)
        { }

        internal Neuron(Func<double, double> activationFunc)
        {
            InputSynapses = new List<Synapse>();
            _activationFunc = activationFunc;
        }

        internal void Reset()
        {
            _calculatedCompute = null;
            _loopProtector = false;
            LoopDetected = false;
        }

        public double Compute()
        {
            if (InputSynapses.Count == 0)
                return 0;

            if (_calculatedCompute.HasValue)
                return _calculatedCompute.Value;

            //защита от рекурсивных вызовов
            if (_loopProtector)
            {
                LoopDetected = true;
                return 0;
            }

            _loopProtector = true;

            _calculatedCompute = _activationFunc(InputSynapses.Sum(s => s.Compute()));

            _loopProtector = false;

            return _calculatedCompute.Value;
        }
    }
}


