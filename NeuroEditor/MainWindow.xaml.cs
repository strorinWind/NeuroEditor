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
                Father.RowDefinitions.Add(new RowDefinition());

                Grid.SetRow(g, y);
                Grid.SetColumn(g, x);
                Father.Children.Add(g);
                if (x == c - 1)
                {
                    x = 0;
                    y++;
                    Father.RowDefinitions.Add(new RowDefinition());
                }
                else x++;
                ShowElement(item, g);
            }
            var b = AddElementButton();
            Grid.SetRow(b, y);
            Grid.SetColumn(b, x);
            Father.Children.Add(b);
        }

        private void ShowElement(ElementVar el, Grid test)
        {
            test.Background = Brushes.Gray;
            test.VerticalAlignment = VerticalAlignment.Top;
            test.HorizontalAlignment = HorizontalAlignment.Left;
            test.Margin = new Thickness(2);
            test.MouseLeftButtonDown += Father_MouseLeftButtonDown;
            test.Cursor = Cursors.Hand;

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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ShowAllElements(ElementsList, e.NewSize.Width);       
        }

        private Grid AddElementButton()
        {
            var res = new Grid();
            res.Background = Brushes.Gray;
            res.Width = 80;
            res.Height = 80;
            var i = new Image();
            i.Margin = new Thickness(10);
            i.Source = new BitmapImage(new Uri("pack://application:,,,/img/add.png"));
            res.Children.Add(i);
            res.MouseLeftButtonDown += Father_MouseLeftButtonDown;

            res.Cursor = Cursors.Hand;
            return res;
        }

        private void Father_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var index = Father.Children.IndexOf((Grid)sender);
            Element el;
            if (index == ElementsList.Count)
                el = new Element(new bool[64]);
            else
                el = new Element(ElementsList[index].Picture);          
            el.ShowDialog();
            if (index == ElementsList.Count)
                ElementsList.Add(new ElementVar(el.m));
            else
                ElementsList[index].Picture = el.m;
            ShowAllElements(ElementsList, Width);
        }
    }
}
