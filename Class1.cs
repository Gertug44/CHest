using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace LR2
{
    public enum Colors { Black, White }
    public enum Figures { Elephant, Hourse}
    public enum variantOfCheck {OwnFigure,Free, Enemy }
    public class Figure
    {
        public int Status = 0;
        public Colors Color;
        public Figures Name;
        public Point point;
        public Thread thread;

        public Figure(Colors color, Figures name, Point point)
        {
            Color = color;
            Name = name;
            this.point = point;
        }
    }

    public class Cell
    {
        public Figure figure;
        public Cell() => figure = null;
        public variantOfCheck CheckCell(Colors color)
        {
            if (figure == null)
                return variantOfCheck.Free;
            else
                if (figure.Color == color)
                    return variantOfCheck.OwnFigure;
                else
                    return variantOfCheck.Enemy;
        }
    }
    class Chest
    {
        static readonly object Status = new object();
        public readonly Cell[,] Board1;
        public List<Figure> figures = new List<Figure>();
        public List<Figure> Blackfigures = new List<Figure>();
        public List<Figure> Whitefigures = new List<Figure>();
        public int BoardLength;

        public Chest(int boardLength)
        {
            Board1 = new Cell[boardLength, boardLength];
            BoardLength = boardLength;
            for (var i=0;i< boardLength; i++)
            {
                for (var j=0; j< boardLength; j++)
                {
                    Board1[i, j] = new Cell();
                    switch (i)
                    {
                        case 0:
                            Board1[i, j].figure = new Figure(Colors.White, Figures.Elephant, new Point(i, j));
                            Whitefigures.Add(Board1[i, j].figure);
                            figures.Add(Board1[i, j].figure);
                            break;
                        case 1:
                            Board1[i, j].figure = new Figure(Colors.White, Figures.Hourse, new Point(i, j));
                            Whitefigures.Add(Board1[i, j].figure);
                            figures.Add(Board1[i, j].figure);
                            break;
                        case 6:
                            Board1[i, j].figure = new Figure(Colors.Black, Figures.Hourse, new Point(i, j));
                            Blackfigures.Add(Board1[i, j].figure);
                            figures.Add(Board1[i, j].figure);
                            break;
                        case 7:
                            Board1[i, j].figure = new Figure(Colors.Black, Figures.Elephant, new Point(i, j));
                            Blackfigures.Add(Board1[i, j].figure);
                            figures.Add(Board1[i, j].figure);
                            break;
                        default:
                            Board1[i, j].figure = null;
                            break;
                    }
                }
            }
        }

        public void GetNextMove(Figure figure)
        {
            lock (Status)
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                if (figure.Status == 1 || IsEnd())
                {
                    figure.thread.Abort();
                    return;
                }
                else
                {
                    var possibleMove = new List<Point>();
                    if (figure.Name == Figures.Elephant)  possibleMove = GetPossibleHElephanteStep(figure);
                    else  possibleMove = GetPossibleHourseStep(figure);
                    if (possibleMove.Count > 0)
                    {
                        var move = new Random().Next(0, possibleMove.Count);
                        var newCell = possibleMove[move];
                        ChangeCell(figure, newCell);
                    }
                }
                stopWatch.Stop();
                if (figure.Name == Figures.Elephant && figure.Color == Colors.Black)
                    Console.Write("Ходит Чёрный Слон ");
                else if (figure.Name == Figures.Hourse && figure.Color == Colors.Black)
                    Console.Write("Ходит Чёрный конь ");
                else if (figure.Name == Figures.Hourse && figure.Color == Colors.White)
                    Console.Write("Ходит Белый конь ");
                else
                    Console.Write("Ходит Белый Слон ");
                Console.WriteLine(" Время хода: " + stopWatch.ElapsedTicks);
                Thread.Sleep(750);
            }
        }

        public bool IsEnd() => !(BlackAlive() && WhiteAlive());

        private bool BlackAlive()
        {
            foreach (var figure in figures)
                if (figure.Color == Colors.Black && figure.Status ==0)
                        return true;
            return false;
        }
        private bool WhiteAlive()
        {
            foreach (var figure in figures)
                if (figure.Status ==0   && figure.Color == Colors.White)
                    return true;
            return false;
        }

        private void ChangeCell (Figure figure, Point newCell)
        {
            var novX = figure.point.X;
            var novY = figure.point.Y;
            var futureX = newCell.X;
            var futureY = newCell.Y;
            figure.point.X = futureX;
            figure.point.Y = futureY;
            if (Board1[futureX, futureY].figure != null)
                Board1[futureX, futureY].figure.Status = 1;
            Board1[futureX, futureY].figure = figure;
            Board1[novX, novY].figure = null;
            Form1.ChangeFigures(figure, futureX, futureY, novX, novY);
        }

        private List<Point> GetPossibleHourseStep(Figure figure)
        {
            var poosibleMove = new Point[8] {
               new Point(1,-2), new Point(2,-1), new Point(-2,-1), new Point(-1,-2),
               new Point(-2,+1), new Point(2,1), new Point(-1,2), new Point(1,2)
            };
            return CheckCellsInDoard(poosibleMove, figure.Color, figure.point);
        }

        private List<Point> GetPossibleHElephanteStep(Figure figure)
        {
            var allMoves = new List<Point>();
            var finalMoves = new List<Point>();
            var movesRightUp = CheckCellsInDoard(new Point[1] { new Point(1, 1) }, figure.Color, figure.point);
            var movesLeftDown = CheckCellsInDoard(new Point[1] { new Point(-1, -1) }, figure.Color, figure.point);
            var movesRightDown = CheckCellsInDoard(new Point[1] { new Point(1, -1) }, figure.Color, figure.point);
            var movesLeftUp = CheckCellsInDoard(new Point[1] { new Point(-1, 1) }, figure.Color, figure.point);
            int countRightUp =1 , countLeftDown = 1, countLeftUp = 1 , countRightDown = 1;

            while (true)
            {  
                if (movesRightUp.Count != 0)
                {
                    allMoves.Add(movesRightUp[0]);
                    movesRightUp = CheckCellsInDoard(new Point[1] { new Point(1, 1) }, figure.Color, 
                        new Point(figure.point.X + countRightUp, figure.point.Y + countRightUp));
                    countRightUp++;
                }
                if (movesLeftDown.Count != 0)
                {
                    allMoves.Add(movesLeftDown[0]);
                    movesLeftDown = CheckCellsInDoard(new Point[1] { new Point(-1, -1) }, figure.Color,
                        new Point(figure.point.X - countLeftDown, figure.point.Y - countLeftDown));
                    countLeftDown++;
                }
                if (movesLeftUp.Count != 0)
                {
                    allMoves.Add(movesLeftUp[0]);
                    movesLeftUp = CheckCellsInDoard(new Point[1] { new Point(-1, 1) }, figure.Color, 
                        new Point(figure.point.X - countLeftUp, figure.point.Y + countLeftUp));
                    countLeftUp++;
                }
                if (movesRightDown.Count != 0)
                {
                    allMoves.Add(movesRightDown[0]);
                    movesRightDown = CheckCellsInDoard(new Point[1] { new Point(1, -1) }, figure.Color,
                        new Point(figure.point.X + countRightDown, figure.point.Y - countRightDown));
                    countRightDown++;
                }
                if (movesRightDown.Count + movesLeftUp.Count + movesLeftDown.Count + movesRightUp.Count == 0)
                    break;
            }
            foreach (var move in allMoves)
            {
                var answer = Board1[ move.X,  move.Y].CheckCell(figure.Color);
                switch (answer)
                {
                    case variantOfCheck.Free:
                        finalMoves.Add(new Point(move.X, move.Y));
                        break;
                    case variantOfCheck.OwnFigure: break;
                    case variantOfCheck.Enemy:
                        return new List<Point>() { new Point(move.X,  move.Y) };
                }
            }
            return finalMoves;
        }

        private List<Point> CheckCellsInDoard(Point[] poosibleMove, Colors color, Point figurePoint)
        {
            var x = figurePoint.X;
            var y = figurePoint.Y;
            var moves = new List<Point>();
            foreach (var move in poosibleMove)
            {
                if (x + move.X > -1 && y + move.Y > -1 && x + move.X < BoardLength && y + move.Y < BoardLength)
                {
                    var answer = Board1[x + move.X, y + move.Y].CheckCell(color);
                    switch (answer)
                    {
                        case variantOfCheck.Free:
                            moves.Add(new Point(x + move.X, y + move.Y));
                            break;
                        case variantOfCheck.OwnFigure: break;
                        case variantOfCheck.Enemy:
                            return new List<Point>() { new Point(x + move.X, y + move.Y) };
                    }
                }
            }
            return moves;
        }
    }
}
