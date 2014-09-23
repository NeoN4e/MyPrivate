using System;
using System.IO;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAPI
{
  
    /// <summary>Клас Определяющий игровую Карту</summary>
    class GameMap
    {
#region Статические методы
        /// <summary>Создаёт Объект Карты из Файла (Размер карты КвоСтрок Х КвоСимволов в последней строке)</summary>
        /// <param name="FileName">Путь к файлу</param>
        public static GameMap LoadMapFromfile(string FileName)
        {
            if (!  File.Exists(FileName)) return null;

            GameMap map = new GameMap(int.MaxValue, int.MaxValue);

            int X = -1; // Х координата
            int Y = -1; // У Координата
            String line;

            using (StreamReader sr = new StreamReader(FileName))
            {
                
                //Переберем все строки
                while (!sr.EndOfStream)
                {
                    ++Y;
                    X = -1;

                    line = sr.ReadLine();
                    foreach (Char Symbol in line)
                    {
                        ++X;
                        if (Symbol != ' ' && !Char.IsControl(Symbol) && !Char.IsSeparator(Symbol))
                        {
                            GameAPI.Point p = new Point(X, Y);
                            map.Add(p, new WPFSnake.Wall(p));
                        }
                    }

                }
                
                //Зададим размер карты
                map.y = ++Y;
                map.x = ++X;
            }

            return map;
        }
#endregion

#region Поля и Конструкторы
        /// <summary>Основное поле класса! Список объектов размещенных на карте</summary>
        Dictionary<Point,Object> GameObjectlist = new Dictionary<Point,object>();

        /// <summary>Размер игровой карты по Х(Ширине)</summary>
        public int XSize { get { return this.x; } }
        private int x;

        /// <summary>Размер игровой карты по У(Высоте)</summary>
        public int YSize { get { return this.y; } }
        private int y;

        /// <summary>Кончтруктор</summary>
        /// <param name="xSize">Размер карты по Х</param>
        /// <param name="ySize">Размер Карты по У</param>
        public GameMap(int xSize, int ySize)
        {
            this.x = xSize;
            this.y = ySize;
        }
#endregion

        /// <summary>Проверяет Существует ли точка в приделах(XSize,YSize)
        /// </summary>
        private bool ChekPointRange(ref Point p)
        {
            //движение по кругу при выходе за приделы карты
            if (p.X >= this.XSize) p = new Point(0,p.Y); 
            if (p.Y >= this.YSize) p = new Point(p.X,0);
           
            if (p.X < 0) p = new Point(this.XSize -1, p.Y);
            if (p.Y < 0) p = new Point(p.X , this.YSize -1);

            return true;
        }

        /// <summary>Определяет координаты обьекта на карте</summary>
        public Point? GetObjectPosition(object obj)
        {
            foreach (var item in this.GameObjectlist)
            {
                if (item.Value == obj) // нашли
                {
                    return item.Key;
                }
            }

            return null;//не нашли
        }

        /// <summary> Удаляет объект с позиции на карте</summary>
        /// <param name="p">позиция</param>
        public bool Remove(Point p)
        {
            bool rezalt = GameObjectlist.Remove(p);

            if (OnMapChange != null && rezalt) OnMapChange(this, p);

            return rezalt;
        }

        /// <summary>Размещает Объект в текущей позиции на карте
        /// Если занято - Event OnCollision
        /// </summary>
        /// <param name="obj">Добавляемый обьект</param>
        /// <param name="p">позиция</param>
        /// <return>Возвращает Позицию где оказался объект null если добавить не удалось</return>
        public Point? Add(Point p,object obj)
        {
            ChekPointRange(ref p);

            //проверим нет ли на указанной позиции обьекта
            if (this[p] != null)
            { 
                //Вызовем обработчик колизий
                if (OnCollision != null) OnCollision(this, p, obj, this[p]);
            }

            //Попытаемся добавить объект
            try
            {
                GameObjectlist.Add(p,obj);
            }
            catch (ArgumentException)
            {
                return null; // неудалось разместить объект
            }

            //Сгенерим событие
            if (OnMapChange != null) OnMapChange(this, p);
            return p;
        }

        /// <summary>Возвращает поочередно все элементы на карте</summary>
        public IEnumerator<Object> GetEnumerator()
        {
            //переберем строки карты
            for (int yPointer = 0; yPointer < this.YSize; yPointer++)
            {

                //перебираем колонки карты
                for (int хPointer = 0; хPointer < this.XSize; хPointer++)
                {
                    yield return this[new Point(хPointer, yPointer)];
                }
            }

        }

        /// <summary>Возвращает объект(или null) расположенный по заданным координатам</summary>
        /// <param name="p">Координаты типа Point</param>
        /// <returns>Объект Расположенный на карте</returns>
        public Object this[Point p]
        {
            get
            {
                try { return GameObjectlist[p]; }
                catch (KeyNotFoundException)
                    { return null; }
            }
        }

 #region Исключения события делегаты
        /// <summary>Событие при Добавлении, изменении, удалении объекта с карты</summary>
        public event OnChangeDelegate OnMapChange;
            /// <param name="m">Карта</param>
            /// <param name="p">Позиция на карте</param>
            public delegate void OnChangeDelegate(GameMap m, Point p);
        
        /// <summary>Возникает при попытке добавить на карту объект в позицию уже занятую другим объектом</summary>
        public event  OnCollisionDelegate OnCollision;
            /// <param name="m">Карта</param>
            /// <param name="p">Позиция на карте</param>
            /// <param name="NewObj">Помещаемый обьект на карту</param>
            /// <param name="ExistObj">Существующий объект на карте</param>
            public delegate void OnCollisionDelegate(GameMap m, Point p, Object NewObj,Object ExistObj);

 #endregion
    }
}
