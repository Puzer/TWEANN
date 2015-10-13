using System;

namespace TWEANNLib.NetworkNodes
{
    internal interface IReceptor : IComputingNode
    {
    }

    [Serializable]
    internal class Receptor : IComputingNode, IReceptor
    {
        private double _charge;

        internal double Charge
        {
            get
            {
                return _charge;
            }

            set
            {
                if (Math.Abs(value) > 5)
                    throw new Exception("The input data must be in the range from -4 to 4");
                _charge = value;
            }
        }

        internal Receptor(double charge)
        {
            Charge = charge;
        }

        internal Receptor()
        {
        }

        public double Compute()
        {
            return Charge;
        }
    }

    [Serializable]
    internal class BiasReceptor : IComputingNode, IReceptor
    {
        public double Compute()
        {
            return 1;
        }
    }

    [Serializable]
    internal class DynamicReceptor : IComputingNode, IReceptor
    {
        internal Func<double> Callback;
        internal DynamicReceptor(Func<double> callback)
        {
            Callback = callback;
        }

        public double Compute()
        {
            return Callback();
        }
    }
}