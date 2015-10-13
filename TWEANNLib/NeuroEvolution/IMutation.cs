namespace TWEANNLib.NeuroEvolution
{
    public interface IMutation
    {
        void Mutate(NeuralNetwork network);

        //Call when a mutation unsuccessful
        NeuralNetwork RollbackMutation();

        //Call when a mutation successful
        void Successful();

        //Called after a successful mutation
        bool CanImprove { get; }

        void Improve();
    }
}