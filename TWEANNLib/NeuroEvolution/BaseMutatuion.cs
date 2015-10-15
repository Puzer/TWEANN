
namespace TWEANNLib.NeuroEvolution
{
    public abstract class BaseMutatuion : IMutation
    {
        /*
        TODO: Написать тесты
        каждая мутация должна быть проверена на:
        1) после отката возвращаются те же значения что и были
        2) Проверка на случайное создание и оставление циклов
        */

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