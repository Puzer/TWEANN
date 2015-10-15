using System.Collections.Generic;

namespace TWEANNLib.NeuroEvolution
{
    public sealed class MultiMutate : BaseMutatuion
    {
        public List<IMutation> mutations = new List<IMutation>();

        public MultiMutate(IEnumerable<IMutation> mutateEnumerable)
        {
            mutations.AddRange(mutateEnumerable);
        }

        protected override void Mutation()
        {
            foreach (var mutation in mutations)
                mutation.Mutate(Network);
        }

        protected override void Rollback()
        {
            for (int i = mutations.Count - 1; i >= 0; i--)
                mutations[i].RollbackMutation();
        }
    }
}