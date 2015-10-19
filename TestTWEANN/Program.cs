using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TWEANNLib;
using TWEANNLib.NeuroEvolution;

namespace TestTWEANN
{
    class Program
    {
        static Func<double, double> func = x => Math.Sin(x * 10);
        private static Dictionary<IMutation, double> imrovement = new Dictionary<IMutation, double>();
        private static DateTime startTime;

        private static void Main(string[] args)
        {
            var network = NeuralBuilder.BuildFullConnected(Math.Tanh, 1, 20, 20, 1);
            
           



            var mutationDic = new Dictionary<IMutation, double>
            {
                {new WeigthMutate(), 1},
                {new ConnectSynapse(), 0.5},
                {new RemoveSynapse(), 0.5},
               // {new ConnectNeuron(Math.Tanh), 0.4},
                 {new MultiMutate(new[] {new WeigthMutate(), new WeigthMutate()}),  0.5},
                 {new MultiMutate(new[] { new ConnectSynapse(), new ConnectSynapse()}),  0.2}
            };

            var specie = new Species(FitFunc, network, mutationDic);
            specie.OnImprovement += OnImprovement;
            specie.Sensitivity = 0.0001;

            lastFit = FitFunc(network);
            startTime = DateTime.Now;

            while (true)
            {
                specie.Iteration();
                specie.Sensitivity *= 0.9999;
                if (network.LoopDetected())
                    Console.WriteLine("LOOP!");
            }
            network.MetricsRecitate();
            NeuralBuilder.BuildDOTGraph(network, "outgraph.dot");
        }

        private static double lastFit;
        static void OnImprovement(Species sender, IMutation mutationType)
        {
            Console.Clear();

            var improve = sender.BestFit - lastFit;

            if (imrovement.ContainsKey(mutationType))
                imrovement[mutationType] += improve;
            else
                imrovement.Add(mutationType,improve);

            
            Console.WriteLine("Iteration: {0} Seconds elapsed: {1}", sender.Iterations, (DateTime.Now- startTime).TotalSeconds);
            Console.WriteLine("Fit: {0}",sender.BestFit);
            Console.WriteLine("Neurons: {0}, Synapses: {1} \r\n", sender.Network.NeuronsCount, sender.Network.SynapsesCount);

            foreach (var value in imrovement.OrderByDescending(a=>a.Value))
                Console.WriteLine(value.Key.GetType().Name+" "+value.Value/sender.MutationsCollection[value.Key]);

          
        }


        static double FitFunc(NeuralNetwork nn)
        {
            const double step = 0.01d;
            const double start = -1;
            const double end = 1;

            var iterationCount = Math.Abs(start - end) / step;

            double errorRate = 0d;
            for (double i = start; i <= end; i += step)
            {
                errorRate += Math.Abs(nn.Solve(i) - func(i));
            }
            return 1d / (1 + errorRate / iterationCount);
        }

    }
}
