using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BBG
{
    public partial class Form1 : Form
    {
        private Simulation simulation;
        private Timer timer;
        private const int NumRegularBalls = 10;
        private const int NumMonsterBalls = 2;
        private const int NumRepelentBalls = 3;
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            simulation = new Simulation(NumRegularBalls, NumMonsterBalls, NumRepelentBalls);

            timer = new Timer();
            timer.Interval = 50;
            timer.Tick += timer1_Tick;
            timer.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            simulation.Turn();
            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach (var ball in simulation.Balls)
            {
                DrawBall(e.Graphics, ball);
            }
        }

        private void DrawBall(Graphics g, Ball ball)
        {
            Brush brush = new SolidBrush(ball.Color);
            g.FillEllipse(brush, (float)(ball.X - ball.Radius), (float)(ball.Y - ball.Radius), (float)(ball.Radius * 2), (float)(ball.Radius * 2));
        }

        
    }
    public abstract class Ball
    {
        public double Radius { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public Color Color { get; set; }
        public double DX { get; set; }
        public double DY { get; set; }

        public Ball(double radius, double x, double y, Color color, double dx, double dy)
        {
            Radius = radius;
            X = x;
            Y = y;
            Color = color;
            DX = dx;
            DY = dy;
        }

        public abstract void HandleCollision(Ball other);
    }

    public class RegularBall : Ball
    {
        public RegularBall(double radius, double x, double y, Color color, double dx, double dy)
            : base(radius, x, y, color, dx, dy) { }

        public override void HandleCollision(Ball other)
        {
            // Logic for collision with other balls
        }
    }

    public class MonsterBall : Ball
    {
        public MonsterBall(double radius, double x, double y, Color color)
            : base(radius, x, y, color, 0, 0) { }

        public override void HandleCollision(Ball other)
        {
            // Logic for collision with other balls
        }
    }

    public class RepelentBall : Ball
    {
        public RepelentBall(double radius, double x, double y, Color color, double dx, double dy)
            : base(radius, x, y, color, dx, dy) { }

        public override void HandleCollision(Ball other)
        {
            // Logic for collision with other balls
        }
    }
    public class Simulation
{
    public List<Ball> Balls { get; private set; }
    private Random random;
    private const double CanvasWidth = 800;
    private const double CanvasHeight = 600;

    public Simulation(int numRegularBalls, int numMonsterBalls, int numRepelentBalls)
    {
        Balls = new List<Ball>();
        random = new Random();

        for (int i = 0; i < numRegularBalls; i++)
        {
            Balls.Add(CreateRandomBall<RegularBall>());
        }

        for (int i = 0; i < numMonsterBalls; i++)
        {
            Balls.Add(CreateRandomBall<MonsterBall>());
        }

        for (int i = 0; i < numRepelentBalls; i++)
        {
            Balls.Add(CreateRandomBall<RepelentBall>());
        }
    }

    private T CreateRandomBall<T>() where T : Ball
    {
        double radius = random.Next(10, 20);
        double x = random.NextDouble() * (CanvasWidth - 2 * radius) + radius;
        double y = random.NextDouble() * (CanvasHeight - 2 * radius) + radius;
        Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        double dx = random.NextDouble() * 2 - 1;
        double dy = random.NextDouble() * 2 - 1;

        if (typeof(T) == typeof(RegularBall))
            return new RegularBall(radius, x, y, color, dx, dy) as T;
        else if (typeof(T) == typeof(MonsterBall))
            return new MonsterBall(radius, x, y, color) as T;
        else if (typeof(T) == typeof(RepelentBall))
            return new RepelentBall(radius, x, y, color, dx, dy) as T;
        else
            throw new InvalidOperationException("Unknown ball type");
    }

    public void Turn()
    {
        foreach (var ball in Balls)
        {
            MoveBall(ball);
        }

        for (int i = 0; i < Balls.Count; i++)
        {
            for (int j = i + 1; j < Balls.Count; j++)
            {
                if (IsColliding(Balls[i], Balls[j]))
                {
                    Balls[i].HandleCollision(Balls[j]);
                    Balls[j].HandleCollision(Balls[i]);
                }
            }
        }

        Balls = Balls.Where(ball => !(ball is RegularBall regularBall && regularBall.Radius <= 0)).ToList();
    }

    private void MoveBall(Ball ball)
    {
        if (ball is MonsterBall) return;

        ball.X += ball.DX;
        ball.Y += ball.DY;

        if (ball.X - ball.Radius < 0 || ball.X + ball.Radius > CanvasWidth)
        {
            ball.DX = -ball.DX;
        }

        if (ball.Y - ball.Radius < 0 || ball.Y + ball.Radius > CanvasHeight)
        {
            ball.DY = -ball.DY;
        }
    }

    private bool IsColliding(Ball b1, Ball b2)
    {
        double dx = b1.X - b2.X;
        double dy = b1.Y - b2.Y;
        double distance = Math.Sqrt(dx * dx + dy * dy);
        return distance < b1.Radius + b2.Radius;
    }
}
}

