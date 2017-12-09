using Microsoft.Win32;
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
        private List<char> OutCharsList;
        private string CurrentPath = "";

        private void CountOutChars()
        {
            var l = new List<char>();
            foreach (var item in ElementsList)
            {
                if (l.IndexOf(item.Output) == -1)
                {
                    l.Add(item.Output);
                }
            }
            OutCharsList = l;
        }

        public MainWindow()
        {
            InitializeComponent();
            //CountOutChars();
        }

        private void ShowAllElements(List<ElementVar> list, double w)
        {
            CountOutChars();

            Father.RowDefinitions.Clear();
            Father.ColumnDefinitions.Clear();
            Father.Children.Clear();
            int c = (int)w / 100;

            for (int i = 0; i < c; i++)
                Father.ColumnDefinitions.Add(new ColumnDefinition());
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
            //general properties
            test.Background = Brushes.Gray;
            test.VerticalAlignment = VerticalAlignment.Top;
            test.HorizontalAlignment = HorizontalAlignment.Left;
            test.Margin = new Thickness(2);
            test.MouseLeftButtonDown += Father_MouseLeftButtonDown;
            test.Cursor = Cursors.Hand;
            //rectangles of the information
            for (int i = 0; i < 8; i++)
            {
                test.RowDefinitions.Add(new RowDefinition());
                test.ColumnDefinitions.Add(new ColumnDefinition());
                for (int j = 0; j < 8; j++)
                {
                    var rect = new Rectangle();
                    rect.Height = 10;
                    rect.Width = 10;
                    if (el.Picture[i * 8 + j]) rect.Fill = new SolidColorBrush(Colors.Orange);
                    else rect.Fill = new SolidColorBrush(Colors.White);

                    rect.Margin = new Thickness(1);
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                    test.Children.Add(rect);
                }
            }
            test.RowDefinitions.Add(new RowDefinition());
            var n = new Grid();
            Grid.SetColumnSpan(n, 8);
            Grid.SetRow(n, 8);
            for (int i = 0; i < 3; i++)
                n.ColumnDefinitions.Add(new ColumnDefinition());
            test.Children.Add(n);

            //for delete icon
            var g = DeleteElementButton();
            Grid.SetColumn(g, 1);
            n.Children.Add(g);

            //for left arrow icon
            var l = LeftArrowButton();
            Grid.SetColumn(l, 0);
            n.Children.Add(l);

            //for right arrow icon
            var r = RightArrowButton();
            Grid.SetColumn(r, 2);
            n.Children.Add(r);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ShowAllElements(ElementsList, e.NewSize.Width);
        }

        private void Father_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var index = Father.Children.IndexOf((Grid)sender);
            Element el;
            if (index == ElementsList.Count)
                el = new Element(new ElementVar());
            else
                el = new Element(ElementsList[index]);
            el.ShowDialog();
            if (index == ElementsList.Count)
                ElementsList.Add(el.elvar);
            else
                ElementsList[index] = el.elvar;
            //CountOutChars();
            ShowAllElements(ElementsList, Width);
        }

        #region FileMethods
        private void OpenFile()
        {
            var c = new OpenFileDialog();
            c.DefaultExt = ".neuro";
            c.Filter = "Neuro |*.neuro";
            c.ShowDialog();
            if (c.FileName != "")
            {
                ReadFromFile(c.FileName);
                CurrentPath = c.FileName;
            }
            ShowAllElements(ElementsList, Width);
        }

        private void ReadFromFile(string path)
        {
            ElementsList.Clear();
            var f = File.ReadAllLines(path);
            for (int i = 5; i < f.Length; i++)
            {
                var m = new bool[64];
                for (int j = 0; j < f[i].Length - 1 & j < 64; j++)
                {
                    m[j] = f[i][j] == '1';
                }
                var c = new ElementVar(m);
                c.Output = f[i][f[i].Length - 1];
                ElementsList.Add(c);
            }
        }

        private void WriteToFile(string path)
        {
            var s = new string[ElementsList.Count + 5];
            var r = File.ReadAllLines(path);
            for (int i = 0; i < 5; i++)
                s[i] = r[i];

            for (int i = 0; i < ElementsList.Count; i++)
            {
                string k = "";
                foreach (var item in ElementsList[i].Picture)
                {
                    k += item ? "1" : "0";
                }
                k += ";" + ElementsList[i].Output;
                s[i + 5] = k;
            }
            File.WriteAllLines(path, s);
        }
        #endregion

        #region MenuItemsMethods
        private void TeachItem_Click(object sender, RoutedEventArgs e)
        {
            var n = new NeuroNetWindow(ElementsList, OutCharsList);
            n.Show();
        }

        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void SaveMenu_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPath != "")
            {
                WriteToFile(CurrentPath);
                MessageBox.Show("Изменения сохранены в файл");
            }
            else SaveAsMenu_Click(sender, e);       
        }

        private void SaveAsMenu_Click(object sender, RoutedEventArgs e)
        {
            var c = new SaveFileDialog();

            c.DefaultExt = ".neuro";
            c.Filter = "Neuro |*.neuro";
            c.FileName = "Doc1.neuro";

            if (c.ShowDialog().Value)
            {
                MessageBox.Show(c.FileName);
            }
        }
        #endregion

        #region ButtonMethods
        private Grid AddElementButton()
        {
            var i = new Image();
            i.Margin = new Thickness(10);
            i.Source = new BitmapImage(new Uri("pack://application:,,,/img/add.png"));
            var res = new Grid
            {
                Background = Brushes.Gray,
                Width = 80,
                Height = 80,
                Cursor = Cursors.Hand,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10)
        };
            res.MouseLeftButtonDown += Father_MouseLeftButtonDown;
            res.Children.Add(i);
            return res;
        }

        private Grid DeleteElementButton()
        {
            var g = new Grid
            {
                Background = Brushes.White,
                Margin = new Thickness(1, 0, 1, 1),
            };
            var im = new Image
            {
                Margin = new Thickness(2),
                Source = new BitmapImage(new Uri("pack://application:,,,/img/delete.jpg")),
                Height = 20,
                Width = 20
            };
            var btn = new Button();
            btn.Content = im;
            btn.Click += Delete_Click;
            g.Children.Add(btn);
            return g;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var conf = new Confirmation();
            conf.ShowDialog();
            if (!conf.DialogResult.Value)
                return;
            var b = (Grid)((Button)sender).Parent;
            int c = -1;
            while (c == -1)
            {
                c = Father.Children.IndexOf(b);
                b = (Grid)b.Parent;
            }
            ElementsList.RemoveAt(c);
            ShowAllElements(ElementsList, Width);
        }

        private Grid RightArrowButton()
        {
            var g = new Grid
            {
                Background = Brushes.White,
                Margin = new Thickness(1, 0, 1, 1),
            };
            var im = new Image
            {
                Margin = new Thickness(2),
                Source = new BitmapImage(new Uri("pack://application:,,,/img/arrow-right.png")),
                Height = 20,
                Width = 20
            };
            var btn = new Button();
            btn.Content = im;
            btn.Click += RightMove_Click;
            g.Children.Add(btn);
            return g;
        }

        public void RightMove_Click(object sender, RoutedEventArgs e)
        {
            var b = (Grid)((Button)sender).Parent;
            int c = -1;
            while (c == -1)
            {
                c = Father.Children.IndexOf(b);
                b = (Grid)b.Parent;
            }
            if (c == ElementsList.Count)
                return;
            var el = ElementsList[c];
            ElementsList.RemoveAt(c);

            if (c == ElementsList.Count)
                ElementsList.Add(el);
            else
                ElementsList.Insert(c + 1, el);
            ShowAllElements(ElementsList, Width);
        }

        private Grid LeftArrowButton()
        {
            var g = new Grid
            {
                Background = Brushes.White,
                Margin = new Thickness(1, 0, 1, 1),
            };
            var im = new Image
            {
                Margin = new Thickness(2),
                Source = new BitmapImage(new Uri("pack://application:,,,/img/arrow-left.png")),
                Height = 20,
                Width = 20
            };
            var btn = new Button();
            btn.Content = im;
            btn.Click += LeftMove_Click;
            g.Children.Add(btn);
            return g;
        }

        private void LeftMove_Click(object sender, RoutedEventArgs e)
        {
            var b = (Grid)((Button)sender).Parent;
            int c = -1;
            while (c == -1)
            {
                c = Father.Children.IndexOf(b);
                b = (Grid)b.Parent;
            }
            if (c == 0)
                return;
            var el = ElementsList[c];
            ElementsList.RemoveAt(c);
            ElementsList.Insert(c - 1, el);
            ShowAllElements(ElementsList, Width);
        }
        #endregion
    }
}
