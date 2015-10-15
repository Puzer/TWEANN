using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TWEANNLib.NetworkNodes;

namespace TWEANNLib
{
    public static class NeuralBuilder
    {
        public static NeuralNetwork BuildFullConnected(Func<double, double> activationFunc, int receptorCount,
            params int[] neuronsOnLayer)
        {

            var receptors = new List<IReceptor>();
            var neurons = new List<Neuron>();
            var synapses = new List<Synapse>();
            var effectors = new List<Effector>();


            //Создается нужное количество входных рецепторов
            for (int i = 0; i < receptorCount; i++)
                receptors.Add(new Receptor());


            var bias = new BiasReceptor();

            //Проходимся по слоям
            for (int layer = 0; layer < neuronsOnLayer.Length; layer++)
            {
                //Создаем нужное количество нейронов
                for (int neuronId = 0; neuronId < neuronsOnLayer[layer]; neuronId++)
                {
                    //Если слой первый - синапсы соеденяются с рецепторами, если нет - с предыдущем слоем
                    List<Synapse> synapsesToCurrNeuron;
                    if (layer == 0)
                        synapsesToCurrNeuron = receptors.Select(receptor => new Synapse(receptor)).ToList();
                    else
                        synapsesToCurrNeuron =
                            neurons.Where(neuron => neuron.LayerId == layer - 1)
                                .Select(neuron => new Synapse(neuron))
                                .ToList();

                    //В текущий нейрон добавляется входящих в него синапсов, включая bias
                    synapsesToCurrNeuron.Add(new Synapse(bias)); //bias (аффиново преобразование)

                    var curNeuron = new Neuron(activationFunc, synapsesToCurrNeuron, layer);

                    neurons.Add(curNeuron);
                    synapses.AddRange(synapsesToCurrNeuron);

                    if (layer == neuronsOnLayer.Length - 1)
                    {
                        effectors.Add(new Effector(curNeuron));
                    }
                }
            }
            RandomizeWigth(synapses);

            receptors.Add(bias);
            return new NeuralNetwork(receptors, synapses, neurons, effectors);


        }

        public static NeuralNetwork ConnectNeuralNetwork(NeuralNetwork inNetwork, NeuralNetwork outNetwork)
        {
            /*Последовательное соединение нейронных сетей
             * inNetwork - сеть в которую входят входные данные
             * outNetwork - сеть откуда выходит результат
            */
            /*
            if (inNetwork.EffectorCount != outNetwork.ReceptorCount)
                throw new Exception("Должно быть одинаковое число входов одной и выходов другой сети");

            var maxLayerInNetwork = inNetwork.Neurons.Max(n => n.LayerId);

            for (int i = 0; i < inNetwork.EffectorCount; i++)
            {
                outNetwork.Receptors[i].RecurrentCallback =
                    inNetwork.Neurons.Where(n => n.LayerId == maxLayerInNetwork).ToArray()[i]
                        .Compute;

                outNetwork.Receptors[i].Recurrent = true;
            }

            foreach (var neuron in outNetwork.Neurons)
                neuron.LayerId += maxLayerInNetwork + 1;

            List<Receptor> receptors = new List<Receptor>();
            List<Neuron> neurons = new List<Neuron>();
            List<Synapse> synapses = new List<Synapse>();

            receptors.AddRange(inNetwork.Receptors);
            //receptors.AddRange(outNetwork._receptors);

            neurons.AddRange(inNetwork.Neurons);
            neurons.AddRange(outNetwork.Neurons);

            synapses.AddRange(inNetwork.Synapses);
            synapses.AddRange(outNetwork.Synapses);

            return new NeuralNetwork(receptors, neurons, synapses);
            */
            return null;

        }

        public static NeuralNetwork ConnectMoreToOne(NeuralNetwork outNetwork, params NeuralNetwork[] networks)
        {
            /*
    //TODO: пофиксить Id нейронов
    if (networks.Sum(n => n.Effectors.Count) != outNetwork.Receptors.Count(r => typeof(Receptor)==r.GetType()))
                throw new Exception("Количество входов выходной сети должно равняться сумме всех выходов входных");

            if (networks.Max(n => n.Receptors.Count(r => typeof(Receptor) == r.GetType())) != networks.Min(n => n.Receptors.Count(r => typeof(Receptor) == r.GetType())))
                throw new Exception("Должно быть одинаковое число входов выходной сети (для объединения в один)");

            Receptor alphaReceptor = new Receptor(); //TODO: входных рецепторов может быть много



            List<Neuron> neuronsOnLatLayer = new List<Neuron>();
            var maxOfAllLayer = 0;

            List<Receptor> receptors = new List<Receptor>();
            List<Neuron> neurons = new List<Neuron>();
            List<Synapse> synapses = new List<Synapse>();

            foreach (var network in networks)
            {
                var maxLayer = network.Neurons.Max(n => n.LayerId);
                neuronsOnLatLayer.AddRange(network.Neurons.Where(n => n.LayerId == maxLayer).ToList());

                maxOfAllLayer = maxLayer > maxOfAllLayer ? maxLayer : maxOfAllLayer;

                foreach (var receptor in network.Receptors)
                {
                    receptor.RecurrentCallback = alphaReceptor.Compute;
                    receptor.Recurrent = true;
                }

                receptors.AddRange(network.Receptors);
                neurons.AddRange(network.Neurons);
                synapses.AddRange(network.Synapses);
            }

            for (int i = 0; i < outNetwork.ReceptorCount; i++)
            {
                var receptor = outNetwork.Receptors[i];
                receptor.RecurrentCallback = neuronsOnLatLayer[i].Compute;
                receptor.Recurrent = true;
            }

            foreach (var neuron in outNetwork.Neurons)
                neuron.LayerId += maxOfAllLayer + 1;


            receptors.Add(alphaReceptor);
            receptors.AddRange(outNetwork.Receptors);

            neurons.AddRange(outNetwork.Neurons);
            synapses.AddRange(outNetwork.Synapses);

            return new NeuralNetwork(receptors, neurons, synapses);
            
          */
            return null;
        }

        private static void RandomizeWigth(List<Synapse> synapses)
        {
            synapses.ForEach(s => s.Weight = NeuralHelper.GetRandomWeigth());
        }

        public static double TanhAprox(double x)
        {
            if (x > 4)
                return 1;
            if (x < -4)
                return -1;

            var x2 = x * x;
            return x / (1 + x2 / (3 + x2 / (5 + x2 / (7 + x2 / (9 + x2 / 11)))));
        }

        public static void Save(NeuralNetwork network, string file)
        {
            using (var serializationStream = File.Create(file))
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(serializationStream, network);
                serializationStream.Close();
            }
        }

        public static NeuralNetwork Open(string file)
        {
            if (!File.Exists(file))
                throw new Exception("Serialization file not found");

            NeuralNetwork network = null;
            using (var serializationStream = File.OpenRead(file))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                network = (NeuralNetwork) serializer.Deserialize(serializationStream);
                serializationStream.Close();
            }
            return network;
        }

        public static NeuralNetwork Clone(this NeuralNetwork source)
        {
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (NeuralNetwork)formatter.Deserialize(stream);
            }
        }

        public static void BuildDOTGraph(NeuralNetwork network, string file)
        {
            var sb = new StringBuilder();
            var minWeigth = Math.Abs(network.Synapses.Min(s => s.Weight));

            sb.AppendLine("digraph NeuralNetwork {");
            foreach (var receptor in network.Receptors)
            {
                foreach (var neuron in network.Neurons.Where(n => n.InputSynapses.Exists(s => s._input == receptor)))
                {
                    var weigth = neuron.InputSynapses.Find(s => s._input == receptor).Weight;
                    sb.AppendLine("R_" + receptor.GetType().Name + "_" + receptor.GetHashCode() + " -> "+getNeuronName(neuron)+" [weight = " + (minWeigth + weigth).ToString("F4").Replace(',', '.') + " color=\"" + (weigth > 0 ? "red" : "blue") + "\" group=\"receptor\"];");
                }
            }

            foreach (var neuron1 in network.Neurons)
            {
                foreach (var neuron2 in network.Neurons.Where(n => n.InputSynapses.Exists(s => s._input == neuron1)))
                {
                    var weigth = neuron2.InputSynapses.Find(s => s._input == neuron1).Weight;
                    sb.AppendLine(getNeuronName(neuron1)+" -> " + getNeuronName(neuron2)  + " [weight = " + (minWeigth + weigth).ToString("F4").Replace(',', '.') + " color=\"" + (weigth > 0 ? "red" : "blue") + "\" ];");
                }
            }

            foreach (var effector in network.Effectors)
            {
                sb.AppendLine(getNeuronName((Neuron)(effector._input)) + " -> E_" + effector.GetHashCode() + " [weight = 1 ];");
            }


            sb.AppendLine("}");
            File.WriteAllText(file, sb.ToString());
        }

        private static string getNeuronName(Neuron neuron)
        {
            return "N_" + "Dist_"+ neuron.Distance + "_" + neuron.GetHashCode();
        }

    }
    internal static class NeuralHelper
    {

#if DEBUG
        internal static readonly Random Rnd = new Random(1);
#else
        private static readonly Random Rnd = new Random();
#endif

        internal static Synapse GetRandomSynapse(this NeuralNetwork network)
        {
            return network.Synapses[Rnd.Next(0, network.Synapses.Count)];
        }
        internal static Neuron GetRandomNeuron(this NeuralNetwork network)
        {
            return network.Neurons[Rnd.Next(0, network.Neurons.Count)];
        }
        internal static IComputingNode GetRandomNode(this NeuralNetwork network)
        {
            return network.Synapses[Rnd.Next(0, network.Synapses.Count)]._input;
        }

        internal static double GetRandomWeigth()
        {
            return (2 - Rnd.NextDouble() * 4);
        }
    }
}