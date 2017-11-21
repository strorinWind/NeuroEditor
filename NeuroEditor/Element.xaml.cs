﻿using System;
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
    /// Логика взаимодействия для Element.xaml
    /// </summary>
    public partial class Element : Window
    {
        public bool[] m;

        public Element(bool[] mas)
        {
            InitializeComponent();
            m = mas;
            DrawGrigSurface(m);  
        }

        private void DrawGrigSurface(bool[] el)
        {
            Surface.Width = 265;
            Surface.Height = 265;
            Surface.Background = Brushes.Gray;
            for (int i = 0; i < 8; i++)
            {
                Surface.RowDefinitions.Add(new RowDefinition());
                Surface.ColumnDefinitions.Add(new ColumnDefinition());
                for (int j = 0; j < 8; j++)
                {
                    var rect = new Rectangle();
                    rect.Height = 30;
                    rect.Width = 30;
                    if (el[i * 8 + j]) rect.Fill = Brushes.Orange;
                    else rect.Fill = Brushes.White;

                    rect.MouseLeftButtonUp += Rect_MouseLeftButtonUp;
                    rect.Margin = new Thickness(1);
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                    Surface.Children.Add(rect);
                }
            }
        }

        private void Rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var r = (Rectangle)sender;
            if (r.Fill == Brushes.Orange)
            {
                r.Fill = Brushes.White;
            }
            else
            {
                r.Fill = Brushes.Orange;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int i = 0; i < 64; i++)
            {
                if (((Rectangle)Surface.Children[i]).Fill == Brushes.Orange)
                {
                    m[i] = true;
                }
                else
                {
                    m[i] = false;
                }
            }
        }
    }
}
