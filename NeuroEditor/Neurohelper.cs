using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroEditor
{
    public class Neurohelper
    {
        //private bool[,] cells = new bool[8, 8];
        public NeuronNet net = new NeuronNet((8 * 8)); // создаем новую нейросеть

        private void SetInputs(bool[,] cells)  // на вход нейросети задает двоичные сигналы в соответсвии со значениями клеток(1 или 0)
        {
            int k = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    net.Inputs[k].Value = cells[i, j] ? 1 : 0;
                    k++;
                }
            }
        }

        public void TeachNet(string n, bool[,] cells)  // метод, который обучает нейросеть
        {
            var name = n;  // присваиваем переменной name текст 

            if (!net.Percs.Any(x => x.Name == name))  // проверка на наличие персептрона с таким же именем
            {
                net.AddNewPerc(name);  // если нет, то добавляется новый персептрон с именем значения name
                //frm.richTextBox1.AppendText(name + "\n");  // добавляем имя персептрона в richTextBox1
            }

            SetInputs(cells);  // подаем на вход двоичные сигналы
            net.TeachFromInput(name);  // обучаем нейронную сеть
        }

        private string ComputeNet(bool[,] cells)  // метод, который угадывает символ
        {
            SetInputs(cells);

            string str1 = "";

            for (int i = 0; i < net.NOuts; i++)  // перебор всех персептронов
            {
                if (net.Percs[i].Out == 1)  // если выход равен 1, то имя персептрона выводится на Messagebox
                {
                    /*str1 = "Символ: ";
                    str1 += net.Percs[i].Name + " ";*/
                    str1 += net.Percs[i].Name; 
                }
            }
            return str1;
        }
    }

    [Serializable]
    public class NeuronNet  // класс где задается структура нейросети
    {
        public List<Input> Inputs { get; set; }
        public List<Link> Links { get; set; }
        public List<Perc> Percs { get; set; }

        public int NInputs { get; set; }
        public int NOuts { get; set; }

        public NeuronNet(int nInputs) // метод для создания нейросети
        {
            NInputs = nInputs;
            NOuts = 0;

            Percs = new List<Perc>();
            Inputs = new List<Input>();

            Links = new List<Link>();

            for (int i = 0; i < nInputs; i++)
            {
                Inputs.Add(new Input { Index = i });  // индексация входных сигналов
            }
        }

        public void TeachFromInput(string name)  // метод для обучения нейросети
        {
            int index = Percs.IndexOf(Percs.First(x => x.Name == name));
            //коэффициент быстроты обучения сети
            float etha = 0.4f;

            int[] X = new int[NInputs];
            int[] D = new int[NOuts];
            int[] Y = new int[NOuts];
            int[] E = new int[NOuts];

            for (int i = 0; i < NInputs; i++)
            {
                if (Inputs[i].Index < 8 * 8)
                {
                    X[i] = Inputs[i].Value;
                }
            }

            D[index] = 1;


            for (int i = 0; i < NOuts; i++)
            {
                Y[i] = Percs[i].Out;
                E[i] = D[i] - Y[i];
            }


            for (int i = 0; i < NOuts; i++)
            {
                for (int j = 0; j < Percs[i].Links.Count; j++)
                {
                    Percs[i].Links[j].Weight += etha * E[i] * X[j];  // увеличение весов
                }
            }
        }

        public void AddNewPerc(string name)  // метод для добавления нового персептрона
        {
            NOuts++;

            var newPerc = new Perc(name);  // создание объекта персептрон

            for (int j = 0; j < NInputs; j++)
            {
                Link link = new Link(Inputs[j], 0);

                Links.Add(link);
                newPerc.Links.Add(link);
            }
            Percs.Add(newPerc);
        }
    }

    [Serializable]  // метка что класс сериализуется
    public class Input  // входной сигнал
    {
        public int Value { get; set; }
        public int Index { get; set; }
    }

    [Serializable]
    public class Link  // связь с входным сигналом и его весами
    {
        public Input Input { get; set; }
        public float Weight { get; set; }

        public Link(Input input, float weight)
        {
            Input = input;
            Weight = weight;
        }

        private Link()
        {
        }
    }

    [Serializable]
    public class Perc  // класс, где задается структура персептрона
    {
        public const double StepValue = 1;  //пороговое значение
        public readonly string Name;
        public List<Link> Links { get; set; }

        public Perc(string name)  // метод для создания объетка персептрон
        {
            Links = new List<Link>();
            Name = name;
        }

        private Perc()
        {
        }
        public float Sum  // метод сумматор
        {
            get
            {
                float _sum = 0;

                foreach (var link in Links)
                {
                    _sum += link.Input.Value * link.Weight; // взвешанные веса
                }
                return _sum;
            }
        }

        private bool StepFunction(float Sum)  // сравнение с пороговым значением
        {
            return Sum >= StepValue;
        }

        public int Out  // определения выхоного сигнала
        {
            get { return StepFunction(Sum) ? 1 : 0; }  //задаем выход
        }
    }
}
