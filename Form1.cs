using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace LR2
{
    public partial class Form1 : Form
    {
        private static int count;
        private static Button[,] buttons;
        private static Bitmap BH = new Bitmap("C:\\Users\\Xpyct\\source\\repos\\LR2\\img\\BH-removebg-preview.png");
        private static Bitmap BE = new Bitmap("C:\\Users\\Xpyct\\source\\repos\\LR2\\img\\BE-removebg-preview.png");
        private static Bitmap WH = new Bitmap("C:\\Users\\Xpyct\\source\\repos\\LR2\\img\\WH-removebg-preview.png");
        private static Bitmap WE = new Bitmap("C:\\Users\\Xpyct\\source\\repos\\LR2\\img\\WE-removebg-preview.png");
        private static Chest board;
        public Form1(int c)
        {
            InitializeComponent();
            count = c;
            this.Size = new Size(50 * count, 50 * count);
        }
        public static void ChangeFigures(Figure figure, int FX, int FY, int NX, int NY)
        {
            buttons[NX, NY].BackgroundImage = null;
            if (figure.Name == Figures.Elephant && figure.Color == Colors.Black)
                buttons[FX, FY].BackgroundImage = BE;
            else if (figure.Name == Figures.Hourse && figure.Color == Colors.Black)
                buttons[FX, FY].BackgroundImage = BH;
            else if (figure.Name == Figures.Hourse && figure.Color == Colors.White)
                buttons[FX, FY].BackgroundImage = WH;
            else buttons[FX, FY].BackgroundImage = WE;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            buttons = new Button[count, count];
            board = new Chest(count); 
            for (var i = 0; i < board.BoardLength; i++)
            {
                for (var j = 0; j < board.BoardLength; j++)
                { 
                    var button = new Button();
                    InitializeComponent();
                    button.Name = i+" "+j;
                    button.Text = " ";
                    button.Size = new Size(50, 50);
                    button.Location = new Point(i * 50, j * 50);
                    button.BackgroundImageLayout = ImageLayout.Zoom;
                    if ((i+j)%2 == 0 ) button.BackColor = Color.Brown;
                    else button.BackColor = Color.White;
                    if (board.Board1[i, j].figure != null)
                    {
                        if (board.Board1[i, j].figure.Name == Figures.Elephant && board.Board1[i, j].figure.Color == Colors.Black)
                            button.BackgroundImage = BE;
                        else if (board.Board1[i, j].figure.Name == Figures.Hourse && board.Board1[i, j].figure.Color == Colors.Black)
                            button.BackgroundImage = BH;
                        else if (board.Board1[i, j].figure.Name == Figures.Hourse && board.Board1[i, j].figure.Color == Colors.White)
                            button.BackgroundImage = WH;
                        else button.BackgroundImage = WE;
                    }
                    this.Controls.Add(button);
                    button.Click += new EventHandler(Form1_Click);
                    buttons[i, j] = button;
                }
            }
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            var i = 0;
            while (true)
            {
                if (!board.IsEnd())
                {
                    foreach (var figure in board.figures)
                    {
                        if (figure.Status == 0)
                        {
                            figure.thread = new Thread(() => board.GetNextMove(figure));
                            if (i == 0) figure.thread.Start();
                            figure.thread.Join();
                        };
                    }
                }
                else break;
            }
            MessageBox.Show("Игра завершилась!");   
        }
    }
}
