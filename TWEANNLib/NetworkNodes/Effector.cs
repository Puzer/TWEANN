using System;

namespace TWEANNLib.NetworkNodes
{
    [Serializable]
    internal class Effector
    {
        internal readonly IComputingNode _input;

        internal Effector(IComputingNode input)
        {
            _input = input;
        }

        internal double Solve()
        {
            return _input.Compute();
        }
    }
}