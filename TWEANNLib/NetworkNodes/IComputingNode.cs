namespace TWEANNLib.NetworkNodes
{
    internal interface IComputingNode
    {
        double Compute();
        bool HasComputed { get; }
        void PassMetric(int distance);
    }

}