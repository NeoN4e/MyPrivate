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

    /// <summary>Преграда на карте</summary>
    public struct Wall
    {
        Char Simbol;

        /// <param name="s">Символ которым будет представленна преграда</param>
        public Wall(Char s = '#')
        { this.Simbol = s; }

        /// <summary>Строковое представление объекта</summary>
        public override string ToString()
        { return "" + Simbol; }
    }

    /// <summary>Герой который перемещаеться по карте/summary>
    class Herro
    {
        /// <summary>Направление движения</summary>
        public MoveVektor Vektor { get; set; }

        /// <summary>Координаты текущей позиции</summary>
        public Point CurentPosition { get; protected set;}

        public Herro(Point beginPoint)
        {   this.CurentPosition = beginPoint; }

        /// <summary>Строковое представление объекта</summary>
        public override string ToString()
        {
            switch (this.Vektor)
            {
                case MoveVektor.Right: return ">";
                case MoveVektor.Left: return "<";
                case MoveVektor.Top: return "^";
                case MoveVektor.Down: return "˅";

                case MoveVektor.Stop:
                default: return "0";
            }

        }

        /// <summary>Метод движения</summary>
        public void Move(GameMap map)
        {
            Point? newPosition; 

            //Шагнем героем по карте
            newPosition = map.Add( this.CurentPosition.Move(Vektor) ,this);

            
            if (newPosition != null)
            {
                //Если удалось шагнуть удалим себя со старой позиции
                map.Remove(this.CurentPosition);
                
                //Сохраним текущюю позицию персонажа
                this.CurentPosition = (Point)newPosition;
                
                //Событие
                //if (onMove != null) onMove(this, map);
 
            }

        }

        /// <summary>Событие движения</summary>
        //public event onMoveDelegate onMove;
            
        //    /// <param name="h">Герой</param>
        //    /// <param name="m">Карта по которой движемся</param>
        //    public delegate void onMoveDelegate(Herro h,GameMap m);
    }

    //Завершить игру по какойто причине
    class GameOverExeption : ApplicationException { }

    // <summary>Уровень пройден</summary>
    class GameWinExeption : ApplicationException { }
}
