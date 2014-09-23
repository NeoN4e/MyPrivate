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
using GameAPI;

namespace WPFSnake
{
    
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static GameAPI.GameMap map;
        Image Wall = new Image();

        /// <summary>Выводит на экран карту</summary>
        static void PrintMap(GameMap map, Grid Grid)
        {
            //Все обьекты на карте
            foreach (Wall item in map)
            {
                if (item != null) //Console.Write(" ");
                    Grid.Children.Add(item);
            }
 
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            map = GameMap.LoadMapFromfile("Box.map");
            PrintMap(map, this.Grid);
        }
        
    }

    class Wall: Image
    {
        static int imgNumbeg = 0;
   
        public int Size = 20;
        public Wall(int left , int top)
            : base()
        {
            this.Source = (ImageSource)(new ImageSourceConverter()).ConvertFromString(@"..\..\wall.png");
            this.Height = Size;
            this.Width = Size;
            this.Name = "img"+imgNumbeg++;

            this.Margin = new Thickness() { Left = Size * left, Top = Size*top};
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        }
    }
}
