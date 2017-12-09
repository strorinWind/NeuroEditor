using AForge.Neuro;
using AForge.Neuro.Learning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NeuroEditor
{
    /// <summary>
    /// Логика взаимодействия для NeuroNetWindow.xaml
    /// </summary>
    public partial class NeuroNetWindow : Window
    {
        private List<ElementVar> VarList;
        private List<char> OutList;
        private ActivationNetwork network;

        public NeuroNetWindow(List<ElementVar> list, List<char> outlist)
        {
            InitializeComponent();
            VarList = list;
            OutList = outlist;
            network = new ActivationNetwork(
                new SigmoidFunction(), // sigmoid activation function
                64,
                50, 40,20, OutList.Count);
        }

        private void TeachNet_Click(object sender, RoutedEventArgs e)
        {
            double[][] inputs, outputs;
            inputs = new double[VarList.Count][];
            outputs = new double[VarList.Count][];
            for (int i = 0; i < VarList.Count; i++)
            {
                inputs[i] = new double[VarList[i].Picture.Length];
                for (int j = 0; j < inputs[i].Length; j++)
                {
                    inputs[i][j] = VarList[i].Picture[j] ? 1 : 0;
                }
                outputs[i] = new double[OutList.Count];
                for (int j = 0; j < outputs[i].Length; j++)
                {
                    outputs[i][j] = 0;
                }
                outputs[i][OutList.IndexOf(VarList[i].Output)] = 1;
            }


            BackPropagationLearning teacher = new BackPropagationLearning(network);

            double error = 1;
            int k = VarList.Count;
            
            while ((double)k/VarList.Count > 0.3)
            {
                error = teacher.RunEpoch(inputs, outputs);
                k = 0;
                for (int i = 0; i < inputs.Length; i++)
                {
                    var c = network.Compute(inputs[i]).ToList();
                    if (c.IndexOf(c.Max()) != outputs[i].ToList().IndexOf(1))
                    {
                        k++;
                        Console.Write(OutList[outputs[i].ToList().IndexOf(1)] + " ");
                    }
                   
                }
                //errors.Add(k);
                double c1 = (double)k / VarList.Count;
                Console.WriteLine(c1);
                Console.WriteLine(k + " " + error + " "+ k/VarList.Count+" "+VarList.Count);
            }
            MessageBox.Show("Обучение завершено");
        }
    }
}
