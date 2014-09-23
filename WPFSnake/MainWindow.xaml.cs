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
        static Snake snake;

        Image Wall = new Image();

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

            snake = new Snake(new GameAPI.Point(2, 2)); //Создадим змею
           // snake.Vektor = MoveVektor.Right;
            map.Add(snake.CurentPosition, snake); // Добавим змею на карту

            PrintMap(map, this.Grid);
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

            Grid.Children.Clear();
            //snake.Move(map);
            PrintMap(map,Grid);
        }
        
    }


    class GameObject : Image
    { 
        public int Size = 40;

        /// <summary>Координаты текущей позиции</summary>
        public GameAPI.Point CurentPosition 
        {
            get { return new GameAPI.Point((int)this.Margin.Left / Size, (int)this.Margin.Top / Size); }
            protected set { this.Margin = new Thickness() { Left = Size * value.X, Top = Size * value.Y }; } 
        }

        /// <summary> Устанавливает Картинку объекта</summary>
        public String SetIcon 
        { 
            get { return this.Source.ToString(); }
            set { this.Source = (ImageSource)(new ImageSourceConverter()).ConvertFromString(value); }
        }

        public GameObject(GameAPI.Point p)
            : base()
        {
            this.CurentPosition = p;

            //this.Source = (ImageSource)(new ImageSourceConverter()).ConvertFromString(@"..\..\wall.png");
            this.Height = Size;
            this.Width = Size;

           
            this.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        }
    }

    class Wall : GameObject
    {
        public Wall(GameAPI.Point p)
            : base(p)
        { this.SetIcon = @"..\..\wall.png"; }
    }

    class Snake : GameObject
    {
        /// <summary>Направление движения</summary>
        public MoveVektor Vektor 
        {
            get { return v; }
            set 
            {
                v = value;
                MessageBox.Show(this.Margin.ToString());
                //this.RenderTransform = new RotateTransform((int)v * 90, (this.Margin.Left + 40 / 2), (this.Margin.Top + 40 / 2));
                
                this.RenderTransform = new RotateTransform( (int)v * 90);
                MessageBox.Show(this.Margin.ToString());
            } 
        }
        MoveVektor v;

        public Snake(GameAPI.Point p)
            : base(p)
        { 
            this.SetIcon = @"..\..\SnakeHead.png";
           

        }

        /// <summary>Метод движения</summary>
        public void Move(GameMap map)
        {
            GameAPI.Point? newPosition;

            //Шагнем героем по карте
            newPosition = map.Add(this.CurentPosition.Move(Vektor), this);
            
            if (newPosition != null)
            {
                //Если удалось шагнуть удалим себя со старой позиции
                map.Remove(this.CurentPosition);

                //Сохраним текущюю позицию персонажа
                this.CurentPosition = (GameAPI.Point)newPosition;

                //Событие
                //if (onMove != null) onMove(this, map);

            }

        }
    }

    struct SnakeBody
    {
        public override string ToString()
        {
            return "*";
        }
    }
}
