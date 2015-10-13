using System;
using System.Collections.Generic;
using TWEANNLib.NetworkNodes;

namespace TWEANNLib.NeuroEvolution
{
    public sealed class WeigthMutate : BaseMutatuion
    {
        private Synapse modifedSynapse;
        private double oldWeigth;

        private Dictionary<Synapse, double> successMutations;
        private Dictionary<Synapse, double> unsuccessMutations;
        private bool canImproveNext = true;

        protected override void Mutation()
        {
            canImproveNext = true;

            modifedSynapse = Network.GetRandomSynapse();
            oldWeigth = modifedSynapse.Weight;

            modifedSynapse.Weight = NeuralHelper.GetRandomWeigth();
        }

        protected override void Rollback()
        {
            canImproveNext = false;
            modifedSynapse.Weight = oldWeigth;
        }

        public override void Successful()
        {
            if(!canImproveNext)
                throw new Exception();
        }

        public override void Improve()
        {
           var stepBackOldWeigth = oldWeigth;
            oldWeigth = modifedSynapse.Weight;
            var newWeigth = oldWeigth + (oldWeigth - stepBackOldWeigth)*0.9;

            if (Math.Round(newWeigth - oldWeigth, 5) == 0)
                canImproveNext = false;

            if (newWeigth > 2)
                newWeigth = 2;
            else if (newWeigth < -2)
                newWeigth = -2;

            modifedSynapse.Weight = newWeigth;
        }

        public override bool CanImprove => canImproveNext && !(Math.Abs(modifedSynapse.Weight) >= 2);
    }
}