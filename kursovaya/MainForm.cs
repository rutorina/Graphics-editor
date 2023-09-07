using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Drawing.Imaging;

namespace kursovaya
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            MyObjects.g = Graphics.FromImage(bitmap);
            MyObjects.bgColor = pictureBox1.BackColor;
            pictureBox1.Image = bitmap;
        }
        Bitmap bitmap;
        bool move = false;
        bool scale = false;
        int vertexForScaling = -1;
        Point prevP;
        List<MyObjects> myObjects = new List<MyObjects>();
        MyObjects curObj = new MyObjects();
        
        private void button1_Click(object sender, EventArgs e)
        {
            MyEllipse myEllipse = new MyEllipse();
            myEllipse.draw(new Point(0, 0), Color.Black, 2, new Size(50, 50));
            myEllipse.Layer = MyObjects.LayerCount;
            MyObjects.LayerCount++;
            myObjects.Add(myEllipse);
            listBox1.Items.Add(myEllipse);
            this.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MyTriangle myTriangle = new MyTriangle();
            myTriangle.draw(new Point(0, 50), new Point(50, 50), new Point(25, 0), Color.Black, 2);
            myTriangle.Layer = MyObjects.LayerCount;
            MyObjects.LayerCount++;
            myObjects.Add(myTriangle);
            listBox1.Items.Add(myTriangle);
            this.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MyRectangle myRectangle = new MyRectangle();
            myRectangle.draw(new Point(0,0), Color.Black, 2, new Size(50,50));
            myRectangle.Layer = MyObjects.LayerCount;
            MyObjects.LayerCount++;
            myObjects.Add(myRectangle);
            listBox1.Items.Add(myRectangle);
            this.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MyLine myLine = new MyLine();
            myLine.draw(new Point(0, 0), new Point(50, 50), Color.Black, 2) ;
            myLine.Layer = MyObjects.LayerCount;
            MyObjects.LayerCount++;
            myObjects.Add(myLine);
            listBox1.Items.Add(myLine);
            this.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files (*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                MyImage myImage = new MyImage();
                Image image = Image.FromFile(openFileDialog1.FileName);
                myImage.draw(new Point(0, 0), image.Size, openFileDialog1.FileName);
                myImage.Layer = MyObjects.LayerCount;
                MyObjects.LayerCount++;
                myObjects.Add(myImage);
                listBox1.Items.Add(myImage);
                this.Refresh();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MyText myText = new MyText();
            myText.draw(new Point(0, 0), new Font("Microsoft Sans Serif", 14), "Text", Color.Black);
            myText.Layer = MyObjects.LayerCount;
            MyObjects.LayerCount++;
            myObjects.Add(myText);
            listBox1.Items.Add(myText);
            this.Refresh();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                curObj.font = fontDialog1.Font;
                ReDraw();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                curObj.color = colorDialog1.Color;
                ReDraw();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SaveForm f = new SaveForm();
            f.ShowDialog();
            switch (f.DialogResult)
            {
                case DialogResult.Yes:
                    {
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            serializer.Converters.Add(new JavaScriptDateTimeConverter());
                            serializer.NullValueHandling = NullValueHandling.Ignore;
                            using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                            {
                                using (JsonWriter writer = new JsonTextWriter(sw))
                                {
                                    serializer.Serialize(writer, myObjects);
                                    sw.Close();
                                    writer.Close();
                                }
                            }
                        }
                    }
                    break;
                case DialogResult.OK:
                    {
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            bitmap.Save(saveFileDialog1.FileName + ".png", ImageFormat.Png);
                        }
                    }
                    break;
                default:
                    break;
            }
            this.Refresh();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                MyObjects m = new MyObjects();
                MyObjects.g.Clear(MyObjects.bgColor);
                myObjects.Clear();
                StreamReader sr = new StreamReader(openFileDialog1.FileName);
                string output = sr.ReadLine();
                myObjects = JsonConvert.DeserializeObject<List<MyObjects>>(output);
                m.MyCreate(myObjects);
                foreach (MyObjects obj in myObjects)
                {
                    listBox1.Items.Add(obj);
                }
                ReDraw();
                sr.Close();
            }
        }

        private void SortByLayer()
        {
            MyObjects[] ObjectsArr = new MyObjects[myObjects.Count];
            for (int i = 0; i < myObjects.Count; i++)
            {
                ObjectsArr[i] = myObjects[i];
            }
            bubbleSort(ObjectsArr);
            myObjects.Clear();
            for (int i = 0; i < ObjectsArr.Length; i++)
            {
                myObjects.Add(ObjectsArr[i]);
            }            
        }

        static void bubbleSort(MyObjects[] arr)
        {
            int i, j;
            MyObjects temp;
            bool swapped;
            for (i = 0; i < arr.Length - 1; i++)
            {
                swapped = false;
                for (j = 0; j < arr.Length - i - 1; j++)
                {
                    if (arr[j].Layer > arr[j + 1].Layer)
                    {
                        temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                        swapped = true;
                    }
                }
                if (swapped == false)
                    break;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (MyObjects f in myObjects)
            {
                if (curObj.Type == "Line" && curObj.Vertexes[0].X - 5 < e.X && curObj.Vertexes[0].X + 10 > e.X
                    && curObj.Vertexes[0].Y - 5 < e.Y && curObj.Vertexes[0].Y + 10 > e.Y)
                {
                    scale = true;
                    vertexForScaling = 0;
                    prevP = new Point(e.X, e.Y);
                }
                if (curObj.Type == "Line" && curObj.Vertexes[1].X - 5 < e.X && curObj.Vertexes[1].X + 10 > e.X
                    && curObj.Vertexes[1].Y - 5 < e.Y && curObj.Vertexes[1].Y + 10 > e.Y)
                {
                    scale = true;
                    vertexForScaling = 1;
                    prevP = new Point(e.X, e.Y);
                }
                if (curObj.Type == "Triangle" && curObj.Vertexes[0].X - 5 < e.X && curObj.Vertexes[0].X + 10 > e.X
                    && curObj.Vertexes[0].Y - 5 < e.Y && curObj.Vertexes[0].Y + 10 > e.Y)
                {
                    scale = true;
                    vertexForScaling = 0;
                    prevP = new Point(e.X, e.Y);
                }
                if (curObj.Type == "Triangle" && curObj.Vertexes[1].X - 5 < e.X && curObj.Vertexes[1].X + 10 > e.X
                    && curObj.Vertexes[1].Y - 5 < e.Y && curObj.Vertexes[1].Y + 10 > e.Y)
                {
                    scale = true;
                    vertexForScaling = 1;
                    prevP = new Point(e.X, e.Y);
                }
                if (curObj.Type == "Triangle" && curObj.Vertexes[2].X - 5 < e.X && curObj.Vertexes[2].X + 10 > e.X
                    && curObj.Vertexes[2].Y - 5 < e.Y && curObj.Vertexes[2].Y + 10 > e.Y)
                {
                    scale = true;
                    vertexForScaling = 2;
                    prevP = new Point(e.X, e.Y);
                }

                if (LeftTopPoint(f.Vertexes).X < e.X && LeftTopPoint(f.Vertexes).X + f.size.Width > e.X && LeftTopPoint(f.Vertexes).Y < e.Y && LeftTopPoint(f.Vertexes).Y + f.size.Height > e.Y)
                {
                    listBox1.SelectedIndex = listBox1.Items.IndexOf(f);
                    move = true;
                    prevP = new Point(e.X, e.Y);
                    DrawPoints();
                }
            }
            this.Refresh();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (scale && curObj.Type == "Line")
            {
                switch (vertexForScaling)
                {
                    case 0:
                        int x = curObj.Vertexes[0].X;
                        x += e.X - prevP.X;
                        int y = curObj.Vertexes[0].Y;
                        y += e.Y - prevP.Y;
                        ((MyLine)listBox1.SelectedItem).draw(new Point(x, y), curObj.Vertexes[1], curObj.color, (int)curObj.PenWidth);
                        break;
                    case 1:
                        x = curObj.Vertexes[1].X;
                        x += e.X - prevP.X;
                        y = curObj.Vertexes[1].Y;
                        y += e.Y - prevP.Y;
                        ((MyLine)listBox1.SelectedItem).draw(curObj.Vertexes[0], new Point(x, y), curObj.color, (int)curObj.PenWidth);
                        break;
                    default:
                        break;
                }
                ReDraw();
                prevP = e.Location;
                textBox1.Text = curObj.size.Width.ToString();
                textBox2.Text = curObj.size.Height.ToString();
                textBox3.Text = curObj.Text;
                button13.Image = curObj.Visible ? Properties.Resources.visibility_off : Properties.Resources.visibility;
                DrawPoints();
            }
            if (scale && curObj.Type == "Triangle")
            {
                switch (vertexForScaling)
                {
                    case 0:
                        int x = curObj.Vertexes[0].X;
                        x += e.X - prevP.X;
                        int y = curObj.Vertexes[0].Y;
                        y += e.Y - prevP.Y;
                        ((MyTriangle)listBox1.SelectedItem).draw(new Point(x, y), curObj.Vertexes[1], curObj.Vertexes[2], curObj.color, (int)curObj.PenWidth); ;
                        break;
                    case 1:
                        x = curObj.Vertexes[1].X;
                        x += e.X - prevP.X;
                        y = curObj.Vertexes[1].Y;
                        y += e.Y - prevP.Y;
                        ((MyTriangle)listBox1.SelectedItem).draw(curObj.Vertexes[0], new Point(x, y), curObj.Vertexes[2], curObj.color, (int)curObj.PenWidth); ;
                        break;
                    case 2:
                        x = curObj.Vertexes[2].X;
                        x += e.X - prevP.X;
                        y = curObj.Vertexes[2].Y;
                        y += e.Y - prevP.Y;
                        ((MyTriangle)listBox1.SelectedItem).draw(curObj.Vertexes[0], curObj.Vertexes[1], new Point(x, y), curObj.color, (int)curObj.PenWidth); ;
                        break;
                    default:
                        break;
                }
                ReDraw();
                prevP = e.Location;
                textBox1.Text = curObj.size.Width.ToString();
                textBox2.Text = curObj.size.Height.ToString();
                textBox3.Text = curObj.Text;
                button13.Image = curObj.Visible ? Properties.Resources.visibility_off : Properties.Resources.visibility;
                DrawPoints();
            }
            if (move)
            {
                ((MyObjects)listBox1.SelectedItem).move(e.X - prevP.X, e.Y - prevP.Y);
                prevP = e.Location;
                ReDraw();
                DrawPoints();
            }
            this.Refresh();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                move = false;
                DrawPoints();
                scale = false;
                vertexForScaling = -1;
            }
            this.Refresh();
        }

        Point LeftTopPoint(List<Point> Vertexes)
        {
            Point p = new Point();
            for (int i = 0; i < Vertexes.Count; i++)
            {
                p = Vertexes[0];
                if (p.X > Vertexes[i].X)
                {
                    p.X = Vertexes[i].X;
                }
                if (p.Y > Vertexes[i].Y)
                {
                    p.Y = Vertexes[i].Y;
                }
            }
            return p;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            foreach (MyObjects obj in myObjects)
            {
                if (obj == listBox1.SelectedItem)
                {
                    curObj = (MyObjects)listBox1.SelectedItem;
                }
            }
            textBox1.Text = curObj.size.Width.ToString();
            textBox2.Text = curObj.size.Height.ToString();
            textBox3.Text = curObj.Text;
            button13.Image = curObj.Visible ? Properties.Resources.visibility_off : Properties.Resources.visibility;
            DrawPoints();
            this.Refresh();
        }

        private void DrawPoints()
        {
            if (curObj.Type == "Triangle" && curObj.Visible == true)
            {
                MyTriangle T = (MyTriangle)curObj;
                T.drawPoints();
            }
            if (curObj.Type == "Line" && curObj.Visible == true)
            {
                MyLine L = (MyLine)curObj;
                L.drawPoints();
            }
            this.Refresh();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            curObj.size.Width = Convert.ToInt32(textBox1.Text);
            curObj.size.Height = Convert.ToInt32(textBox2.Text);
            curObj.Text = textBox3.Text;
            ReDraw();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsNumber(e.KeyChar) ||
            (!string.IsNullOrEmpty(((TextBox)sender).Text)) || char.IsControl(e.KeyChar))
            {
                return;
            }
            e.Handled = true;
        }

        private void ReDraw()
        {
            MyObjects.g.Clear(pictureBox1.BackColor);
            foreach (MyObjects m in myObjects)
            {
                if (m.Visible)
                {
                    switch (m.Type)
                    {
                        case "Image":
                            MyImage i = (MyImage)m;
                            i.draw(i.Vertexes[0], i.size, i.image);
                            break;
                        case "Text":
                            MyText t = (MyText)m;
                            t.draw(t.Vertexes[0], t.font, t.Text, t.color);
                            break;
                        case "Triangle":
                            MyTriangle tr = (MyTriangle)m;
                            tr.draw(tr.Vertexes[0], tr.Vertexes[1], tr.Vertexes[2], tr.color, (int)tr.PenWidth);
                            break;
                        case "Rectangle":
                            MyRectangle r = (MyRectangle)m;
                            r.draw(r.Vertexes[0], r.color, (int)r.PenWidth, r.size);
                            break;
                        case "Ellipse":
                            MyEllipse el = (MyEllipse)m;
                            el.draw(el.Vertexes[0], el.color, (int)el.PenWidth, el.size);
                            break;
                        case "Line":
                            MyLine L = (MyLine)m;
                            L.draw(L.Vertexes[0], L.Vertexes[1], L.color, (int)L.PenWidth);
                            break;
                        default:
                            break;
                    }
                }
            }
            this.Refresh();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            curObj.Visible = curObj.Visible ? false : true;
            button13.Image = curObj.Visible ? Properties.Resources.visibility_off : Properties.Resources.visibility;
            ReDraw();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (curObj.Layer != 0)
            {
                curObj.Layer--;
                myObjects.ElementAt(myObjects.IndexOf(curObj) - 1).Layer++;
                SortByLayer();
                ReDraw();
                listBox1.Items.Clear();
                foreach (MyObjects m in myObjects)
                {
                    listBox1.Items.Add(m);
                }
            }
            this.Refresh();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (curObj.Layer != MyObjects.LayerCount -1)
            {
                curObj.Layer++;
                myObjects.ElementAt(myObjects.IndexOf(curObj) + 1).Layer--;
                SortByLayer();
                ReDraw();
                listBox1.Items.Clear();
                foreach (MyObjects m in myObjects)
                {
                    listBox1.Items.Add(m);
                }
            }
            this.Refresh();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {              
                myObjects.Remove((MyObjects)listBox1.SelectedItem);
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                ReDraw();
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
            }
            this.Refresh();
        }
    }
}
