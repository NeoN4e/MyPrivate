using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAPI
{
    //Направление движения
    public enum MoveVektor { Stop, Right, Down, Left, Top }

    /// <summary>Определяет координаты объекта на карте</summary>
    public struct Point : IEquatable<Point>
    {
        int x;
        public int X { get { return this.x; } }

        int y;
        public int Y { get { return this.y; } }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Point Move(MoveVektor Vektor)
        {
            switch (Vektor)
            {
                case MoveVektor.Right: return new Point(this.X + 1, this.Y);
                case MoveVektor.Left: return new Point(this.X - 1, this.Y);
                case MoveVektor.Top: return new Point(this.X, this.Y - 1);
                case MoveVektor.Down: return new Point(this.X, this.Y + 1);
            }

            return this;
        }

        /// <summary>Метод сравнения</summary>
        public bool Equals(Point p)
        {
            if (this.X == p.X && this.Y == p.Y) return true;
            else return false;
        }

        public override string ToString()
        {
            return String.Format("Point( X: {0} Y: {1})", this.X, this.Y); ;
        }
    }

    //Завершить игру по какойто причине
    class GameOverExeption : ApplicationException { }

    // <summary>Уровень пройден</summary>
    class GameWinExeption : ApplicationException { }


    class GameObject : System.Windows.Controls.Image
    {
        public int Size = 40;

        /// <summary>Координаты текущей позиции</summary>
        public GameAPI.Point CurentPosition
        {
            get { return new GameAPI.Point((int)this.Margin.Left / Size, (int)this.Margin.Top / Size); }
            set { this.Margin = new System.Windows.Thickness() { Left = Size * value.X, Top = Size * value.Y }; }
        }

        /// <summary> Устанавливает Картинку объекта</summary>
        public String SetIcon
        {
            get { return this.Source.ToString(); }
            set { this.Source = (System.Windows.Media.ImageSource)(new System.Windows.Media.ImageSourceConverter()).ConvertFromString(value); }
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

        /// <summary>Направление движения</summary>
        public MoveVektor Vektor
        {
            get { return v; }
            set
            {
                v = value;
                //Свойство RenderTransformOrigin использует значение структуры Point несколько нестандартным образом, так как Point не представляет абсолютное положение в системе координат. Вместо этого значения между 0 и 1 интерпретируются как коэффициент для диапазона текущего элемента по каждой из осей X и Y. Например, значение (0.5,0.5) вызовет центрирование преобразования прорисовки по элементу, а (1,1) поместит преобразование прорисовки в нижний правый угол элемента. NaN не является приемлемым значением. 
                this.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                this.RenderTransform = new System.Windows.Media.RotateTransform((int)v * 90);
            }
        }
        MoveVektor v;
    }

    class Wall : GameObject
    {
        public Wall(GameAPI.Point p)
            : base(p)
        { this.SetIcon = @"..\..\wall.png"; }
    }

    class Snake : GameObject
    {
        //Коллекция Хвоста
        Queue<GameAPI.SnakeBody> BodyQueue = new Queue<GameAPI.SnakeBody>();

        private bool mustGrow = false;

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

                //Отресуем Хвост
                if (mustGrow) //Нужно удленить змейку
                {
                    SnakeBody sb = new SnakeBody(this.CurentPosition);
                    sb.Vektor = this.Vektor;
                    BodyQueue.Enqueue(sb);
                    map.Add(sb.CurentPosition, sb); //Добавим хвостик на карту

                    //Событие ОнРасти
                    if (OnGrow != null) OnGrow(sb, null);
                    this.mustGrow = false;
                }
                else
                {
                    if (this.BodyQueue.Count > 0)
                    {
                        SnakeBody sb = BodyQueue.Dequeue();
                        map.Remove(sb.CurentPosition); // Удалим Хвостик с игровой карты

                        sb.CurentPosition = this.CurentPosition;
                        sb.Vektor = this.Vektor;
                        BodyQueue.Enqueue(sb);
                        map.Add(sb.CurentPosition, sb); //Добавим хвостик на карту
                    }
                }

                //Сохраним текущюю позицию персонажа
                this.CurentPosition = (GameAPI.Point)newPosition;
             }

        }

        /// <summary>Расти</summary>
        public void Grow(GameAPI.GameMap map)
        {
            this.mustGrow = true;
        }

        /// <summary>При росте змеии</summary>
        public event EventHandler OnGrow;
    }

    class SnakeBody : GameObject
    {
        public SnakeBody(GameAPI.Point p)
            : base(p)
        {
            this.SetIcon = @"..\..\SnakeBody.png";
        }
    }

    class Mouse : GameObject
    {
        public Mouse(GameAPI.Point p)
            : base(p)
        {
            this.SetIcon = @"..\..\Mouse.png";
        }
    }

}
