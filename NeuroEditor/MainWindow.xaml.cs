using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeuroEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ElementVar> ElementsList = new List<ElementVar>();


        public MainWindow()
        {
            InitializeComponent();

            var f = File.ReadAllLines("../../../testvar.neuro");
            for (int i = 5; i < f.Length; i++)
            {
                var m = new bool[64];
                for (int j = 0; j < f[i].Length - 1 & j<64; j++)
                {
                    m[j] = f[i][j] == '1';
                }
                ElementsList.Add(new ElementVar(m));
            }

            ShowAllElements(ElementsList, this.Width);
        }

        private void ShowAllElements(List<ElementVar> list, double w)
        {
            Father.RowDefinitions.Clear();
            Father.ColumnDefinitions.Clear();
            Father.Children.Clear();

            int c = (int)w / 100;
            for (int i = 0; i < c; i++)
            {
                Father.ColumnDefinitions.Add(new ColumnDefinition());
            }
            int x = 0, y = 0;
            foreach (var item in list)
            {
                var g = new Grid();
                g.Background = new SolidColorBrush(Colors.Gray);
                g.VerticalAlignment = VerticalAlignment.Top;
                g.HorizontalAlignment = HorizontalAlignment.Left;
                g.Margin = new Thickness(2);
                Father.RowDefinitions.Add(new RowDefinition());

                Grid.SetRow(g, y);
                Grid.SetColumn(g, x);
                Father.Children.Add(g);
                if (x == c)
                {
                    x = 0;
                    y++;
                    Father.RowDefinitions.Add(new RowDefinition());
                }
                else x++;
                ShowElement(item, g);
            }
        }

        private void ShowElement(ElementVar el, Grid test)
        {
            for (int i = 0; i < 8; i++)
            {
                test.RowDefinitions.Add(new RowDefinition());
                test.ColumnDefinitions.Add(new ColumnDefinition());
                for (int j = 0; j < 8; j++)
                {
                    var rect = new Rectangle();
                    rect.Height = 10;
                    rect.Width = 10;
                    if (el.Picture[i*8+j]) rect.Fill = new SolidColorBrush(Colors.Orange);
                    else rect.Fill = new SolidColorBrush(Colors.White);
                                  
                    rect.Margin = new Thickness(1);
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                    test.Children.Add(rect);
                }
            }
        }

        private void test_GotFocus(object sender, RoutedEventArgs e)
        {
            //new Element().ShowDialog();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ShowAllElements(ElementsList, e.NewSize.Width);
            
        }
    }
}
