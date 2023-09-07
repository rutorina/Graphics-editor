using System;
using System.Collections.Generic;
using System.Drawing;


namespace kursovaya
{
    public class MyObjects
    {
        public static Graphics g;
        public static Color bgColor;
        public Color color;
        public int Layer;
        public string Text;
        public List<Point> Vertexes = new List<Point>();
        public int PenWidth;
        public Color PenColor;
        public Size size;
        public string image;
        public Font font;
        public SolidBrush brush;
        public string Type = "default";
        public bool Visible = true;
        public static int LayerCount = 0;

        public void move(int dx, int dy)
        {
            Point p = new Point();
            for (int i = 0; i < Vertexes.Count; i++)
            {
                p.X = Vertexes[i].X + dx;
                p.Y = Vertexes[i].Y + dy;
                Vertexes[i] = p;
            }
        }

        public MyObjects()
        { }

        public MyObjects(MyObjects m)
        {
            SetParams(m);
        }

        public void SetParams(MyObjects m)
        {
            Layer = m.Layer;
            Text = m.Text;
            Vertexes = m.Vertexes;
            color = m.color;
            PenColor = m.PenColor;
            PenWidth = m.PenWidth;
            size = m.size;
            image = m.image;
            font = m.font;
            brush = m.brush;
            Type = m.Type;
            Visible = m.Visible;
            LayerCount++;
        }

        public void MyCreate(List<MyObjects> m)
        {
            bool vis;
            List<Point> V;
            for (int i = 0; i < m.Count; i++)
            {
                switch (m[i].Type)
                {
                    case "Image":
                        vis = m[i].Visible;
                        V = m[i].Vertexes;
                        m[i] = new MyImage(m[i]);
                        m[i].Visible = vis;
                        m[i].Vertexes = V;
                        break;
                    case "Text":
                        vis = m[i].Visible;
                        V = m[i].Vertexes;
                        m[i] = new MyText(m[i]);
                        m[i].Visible = vis;
                        m[i].Vertexes = V;
                        break;
                    case "Triangle":
                        vis = m[i].Visible;
                        V = m[i].Vertexes;
                        m[i] = new MyTriangle(m[i]);
                        m[i].Visible = vis;
                        m[i].Vertexes = V;
                        break;
                    case "Rectangle":
                        vis = m[i].Visible;
                        V = m[i].Vertexes;
                        m[i] = new MyRectangle(m[i]);
                        m[i].Visible = vis;
                        m[i].Vertexes = V;
                        break;
                    case "Ellipse":
                        vis = m[i].Visible;
                        V = m[i].Vertexes;
                        m[i] = new MyEllipse(m[i]);
                        m[i].Visible = vis;
                        m[i].Vertexes = V;
                        break;
                    case "Line":
                        vis = m[i].Visible;
                        V = m[i].Vertexes;
                        m[i] = new MyLine(m[i]);
                        m[i].Visible = vis;
                        m[i].Vertexes = V;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class MyImage : MyObjects
    {
        public MyImage()
        { 
        }

        public MyImage(MyObjects m) : base(m)
        {
            Type = "Image";
        }

        public void draw(Point p, Size size, string image)
        {
            Type = "Image";
            Vertexes.Clear();
            Vertexes.Add(p);
            this.image = image;
            this.size = size;
            if (size == null)
            {
                g.DrawImage(Image.FromFile(image), p);
            }
            else
            {
                g.DrawImage(Image.FromFile(image), p.X, p.Y, size.Width, size.Height);
            }
        }
    }

    public class MyText : MyObjects
    {
        public MyText()
        {
        }

        public MyText(MyObjects m) : base(m)
        {
            Type = "Text";
        }

        public void draw(Point p, Font font, string text, Color color)
        {
            
            Type = "Text";
            Text = text;
            this.font = font;
            brush = new SolidBrush(color);
            g.DrawString(text, font, brush, p);
            Vertexes.Add(p);
            size = new Size((int)font.Size * text.Length, (int)font.Size);
            this.color = color;
        }
    }

    public class MyTriangle : MyObjects
    {
        public MyTriangle()
        {
        }

        public MyTriangle(MyObjects m) : base(m)
        {
            Type = "Triangle";
        }

        public void drawPoints()
        {
            foreach (Point p in Vertexes)
            {
                g.DrawEllipse(new Pen(PenColor, PenWidth), p.X - 5, p.Y - 5, 10, 10);
            }
        }

        public void draw(Point A, Point B, Point C, Color col, int thickness)
        {
            Vertexes.Clear();
            Type = "Triangle";
            Pen pen = new Pen(col, thickness);
            PenColor = col;
            PenWidth = thickness;
            Vertexes.Add(A);
            Vertexes.Add(B);
            Vertexes.Add(C);
            color = col;
            size.Width = Math.Abs(B.X - A.X);
            size.Height = Math.Abs(A.Y - C.Y);
            g.DrawLine(pen, A, B);
            g.DrawLine(pen, A, C);
            g.DrawLine(pen, C, B);
        }
    }

    public class MyRectangle : MyObjects
    {
        public MyRectangle()
        {
        }

        public MyRectangle(MyObjects m) : base(m)
        {
            Type = "Rectangle";
        }

        public void draw(Point p, Color color, int thickness, Size size)
        {
            Vertexes.Clear();
            Type = "Rectangle";
            Pen pen = new Pen(color, thickness);
            PenColor = color;
            PenWidth = thickness;
            g.DrawRectangle(pen, p.X, p.Y, size.Width, size.Height);
            Vertexes.Add(p);
            this.size = size;
            this.color = color;
        }
    }

    public class MyEllipse : MyObjects
    {
        public MyEllipse()
        {
        }

        public MyEllipse(MyObjects m) : base(m)
        {
            Type = "Ellipse";
        }

        public void draw(Point p, Color color, int thickness, Size size)
        {
            Type = "Ellipse";
            Vertexes.Clear();
            Pen pen = new Pen(color, thickness);
            PenColor = color;
            PenWidth = thickness;
            g.DrawEllipse(pen, p.X, p.Y, size.Width, size.Height);
            Vertexes.Add(p);
            this.size = size;
            this.color = color;
        }
    }

    public class MyLine : MyObjects
    {
        public MyLine()
        {
        }   

        public MyLine(MyObjects m) : base(m)
        {
            Type = "Line";
        }

        public void drawPoints()
        {
            foreach (Point p in Vertexes)
            {
                g.DrawEllipse(new Pen(PenColor,PenWidth), p.X - 5, p.Y - 5, 10, 10);
            }
        }

        public void draw(Point A, Point B, Color color, int thickness)
        {
            Type = "Line";
            Vertexes.Clear();
            Pen pen = new Pen(color, thickness);
            PenColor = color;
            PenWidth = thickness;
            g.DrawLine(pen, A, B);
            Vertexes.Add(A);
            Vertexes.Add(B);
            this.color = color;
            size.Width = Math.Abs(B.X - A.X);
            size.Height = Math.Abs(B.Y - A.Y);
        }
    }
 
}
