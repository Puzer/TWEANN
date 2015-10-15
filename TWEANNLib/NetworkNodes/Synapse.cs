using System;

namespace TWEANNLib.NetworkNodes
{
    [Serializable]
    internal class Synapse : IComputingNode
    {
        internal IComputingNode _input;
        private double _weight;
        internal double Weight
        {
            get
            {
                return _weight;
            }

            set
            {
                if (Math.Abs(value) > 2)
                    throw new Exception("The weight of the synapse must be in the range from -2 to 2");
                _weight = value;
            }
        }

        internal Synapse(IComputingNode input)
        {
            _input = input;
        }

        public double Compute()
        {
            return _input.Compute() * Weight;
        }

        public void PassMetric(int i)
        {
            _input.PassMetric(i);
        }

        public bool HasComputed => _input.HasComputed;
    }
}