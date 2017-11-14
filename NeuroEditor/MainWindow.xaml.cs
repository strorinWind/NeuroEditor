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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeuroEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowElement(new ElementVar(),test);
            ShowElement(new ElementVar(), test1);
        }


        private void ShowElement(ElementVar el,Grid test)
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
                    if (el.Picture[i*8+j])
                    {
                        rect.Fill = new SolidColorBrush(Colors.Orange);
                    }
                    else
                    {
                        rect.Fill = new SolidColorBrush(Colors.White);
                    }                 
                    rect.Margin = new Thickness(1);
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                    test.Children.Add(rect);
                }
            }
        }

        private void test_GotFocus(object sender, RoutedEventArgs e)
        {
            new Element().ShowDialog();
        }
    }
}
