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
        static GameAPI.Snake snake;
        static GameAPI.Mouse apple;

        /// <summary>Выводит на экран карту</summary>
        static void PrintMap(GameMap map, Grid Grid)
        {
            //Все обьекты на карте
            foreach (GameObject item in map)
            {
                if (item != null) //Console.Write(" ");
                    Grid.Children.Add(item);
            }
 
        }

        /// <summary>Метод генерация и размещения mouse</summary>
        static void RandomAppleGeneration()
        {
            GameAPI.Point p;

            //Генерируем координаты пока не пусто
            do
            {
                p = new GameAPI.Point(MyRandom.R.Next(map.XSize), MyRandom.R.Next(map.YSize));
            } while (map[p] != null);


            if (apple == null)
                apple = new GameAPI.Mouse(p);
            else
                apple.CurentPosition = p;

            apple.Vektor = (MoveVektor)MyRandom.R.Next(4);
            map.Add(apple.CurentPosition,apple);
        }

        /// <summary>Обработчик Столкновений</summary>
        static void Collision(GameAPI.GameMap map, GameAPI.Point p, Object NewObj, Object ExistObj)
        {
            if ((NewObj is GameAPI.Snake) && (ExistObj is GameAPI.Mouse))
            {
                //Удалим яблочко
                map.Remove(p);
                (NewObj as Snake).Grow(map); // Вырастим

                RandomAppleGeneration(); // Сгенерим новое яблочко
            }
            //else
               // throw new GameOverExeption();
        }

        static void BeginGame()
        {
            //System.Threading.Thread.Sleep(1000);
            //try
            //{
                System.Threading.Thread.Sleep(400);
                while (true)
                    snake.Move(map);
                
            //}
            //catch (GameOverExeption gwe)
            //{
            //    Grid.Children.Clear();
            //    //<Label Content="Geme over" Margin="121,90,122,126" FontSize="48" FontWeight="Bold" Foreground="#FFFA0000"/>
            //    Label l = new Label();
            //    l.Content = "Geme over";
            //    l.FontSize = 48;
            //    l.Foreground = Brushes.Red;

            //    Grid.Children.Add(l);
            //}
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
            Grid.Children.Clear();
            map = GameMap.LoadMapFromfile(@"..\..\Box.map");

            map.OnCollision += Collision; //Обработчик колизий

            snake = new GameAPI.Snake(new GameAPI.Point(2, 2)); //Создадим змею
            map.Add(snake.CurentPosition, snake); // Добавим змею на карту

            snake.OnGrow += (Object obj, EventArgs ea) => { Grid.Children.Add((SnakeBody)obj); };//Добавим в Грид Новые куски змеи...

            //SnakeBody sb = new SnakeBody(new GameAPI.Point(2, 3));
            //map.Add(sb.CurentPosition, sb);

            RandomAppleGeneration(); //Сгенерим яблочко

            PrintMap(map, this.Grid);

            //MessageBox.Show(this.Resources.Count.ToString());
            //System.Threading.Thread T = new System.Threading.Thread(BeginGame);
            //T.Start();
            //BeginGame();
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show(e.Key.ToString());
            switch (e.Key)
            {
                case Key.Up:    snake.Vektor = MoveVektor.Top; break;
                case Key.Down:  snake.Vektor = MoveVektor.Down; break;
                case Key.Left:  snake.Vektor = MoveVektor.Left; break;
                case Key.Right: snake.Vektor = MoveVektor.Right; break;
            }

            snake.Move(map);
        }
        
    }

    
   
}
