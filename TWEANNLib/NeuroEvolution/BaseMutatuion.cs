
namespace TWEANNLib.NeuroEvolution
{
    public abstract class BaseMutatuion : IMutation
    {
        protected NeuralNetwork Network;
        public void Mutate(NeuralNetwork network)
        {
            Network = network;
            Mutation();
        }

        public NeuralNetwork RollbackMutation()
        {
            Rollback();
            return Network;
        }
        protected abstract void Mutation();
        protected abstract void Rollback();

        public virtual void Successful() { }

        public virtual bool CanImprove => false;

        public virtual void Improve() { }
    }
   
}