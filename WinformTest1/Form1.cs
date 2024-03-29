﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformTest1
{
    public partial class Calculator : Form
    {
        //drawing values
        Rectangle field = new Rectangle(0, 0, 600, 400); //general rectange for graph use
        Point[] graphPoints = new Point[100]; // store points as entered to draw on graph             
        int lineAmount = 0; //counter to track amount of lines
        int currentPos = 0; //counter to check amount of points entered
        int RelevantPoints = 0; //count the amount of relevant points
        Boolean autoDraw = false;
        int scale = 1;
        int dummy;
        

        //mathemmatic values
        Point[] dataPoints = new Point[100];// store points as entered to calculate on
        int[,] lines = new int[50, 2]; // double array to hold lines in y = mx + b
        double lastAngle = 0; //save last angle found incase it needs to be inverted

        public Calculator()
        {
            InitializeComponent();
         
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //draw graph field on start up
            Graphics g = e.Graphics;
            using (Pen selPen = new Pen(Color.Blue))
            {
                g.FillRectangle(new SolidBrush(Color.White), field);
                g.DrawRectangle(new Pen(Color.Black, 2), field);
                g.DrawLine(new Pen(Brushes.Black, 2), new Point(0, 200), new Point(600, 200));
                g.DrawLine(new Pen(Brushes.Black, 2), new Point(300, 0), new Point(300, 400));
                //add lines to indicate axis lengths
                for(int i = 0; i < 5; i++)
                {
                    g.DrawLine(new Pen(Brushes.Black, 2), new Point(i * 100 + 100, 190), new Point(i * 100 + 100, 210));
                    if(i != 2)
                    {
                        g.DrawString(((i - 2) * 100 / scale).ToString(), new Font(new FontFamily("Arial"), 11, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.Black), i * 100 + 90, 215);
                    }
                    
                }
                for (int i = 0; i < 3; i++)
                {
                    g.DrawLine(new Pen(Brushes.Black, 2), new Point(290, i * 100 + 100), new Point(310 , i * 100 + 100));
                    if (i != 1)
                    {
                        g.DrawString(((1 - i) * 100 / scale).ToString(), new Font(new FontFamily("Arial"), 11, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.Black), 315, i * 100 + 93);
                    }
                    
                }
             
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e) //button to add point to the graph
        {
            int dummy;
            
            //take point given by x and y textfield
            if (Int32.TryParse(IntegerA.Text, out dummy) && Int32.TryParse(IntegerB.Text, out dummy))
            {
                graphPoints[currentPos] = new Point(297 + (Int32.Parse(IntegerA.Text) * scale), 197 - (Int32.Parse(IntegerB.Text) * scale)); // had to set to 297,197 instead of 300,200 because draw object is not accurate
                dataPoints[currentPos] = new Point(Int32.Parse(IntegerA.Text), Int32.Parse(IntegerB.Text));
                this.CreateGraphics().FillEllipse(new SolidBrush(Color.Black), graphPoints[currentPos].X , graphPoints[currentPos].Y, 5, 5);
                this.CreateGraphics().DrawString((currentPos + 1).ToString(), new Font(new FontFamily("Arial"), 11, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.Black), graphPoints[currentPos].X - 3, graphPoints[currentPos].Y - 15);
                currentPos++;

                if(RelevantPoints < 4)
                {
                    RelevantPoints++;
                }
            }
            //draw line for more than 2 points
            if (currentPos == 2 && autoDraw)
            {
                this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), graphPoints[currentPos-2], graphPoints[currentPos-1]);
            }
            //draw triangle for 3 points
            if (currentPos == 3 && autoDraw)
            {
                    this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), graphPoints[currentPos - 2], graphPoints[currentPos - 1]);
                    this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), graphPoints[currentPos - 3], graphPoints[currentPos - 1]);
                    this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), graphPoints[currentPos - 3], graphPoints[currentPos - 2]);  
            }

            if (currentPos >= 4)
            {
                //drawQuad(new Point[4] { points[currentPos - 1], points[currentPos - 2], points[currentPos - 3], points[currentPos - 4] });
                /*
                    this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), points[currentPos - 1], points[currentPos - 2]);
                    this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), points[currentPos - 1], points[currentPos - 3]);
                    this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), points[currentPos - 4], points[currentPos - 2]);
                    this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), points[currentPos - 4], points[currentPos - 3]);
                */
                //this.CreateGraphics().DrawPolygon(new Pen(Brushes.Black, 2), new Point[4] { points[currentPos - 1], points[currentPos - 2], points[currentPos - 3], points[currentPos - 4] });
            }
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        //drawing quadrilateral function must use points and angles amoung points to 
        //determine which points to connect with lines to draw wanted quadrilateral
        public void drawQuad(Point[] p)
        {
            double greatest = 0;// use to save the greatest angle when calculating angles and finding greatest angle amoung all lines
            double greatestDiff = 0; // use to find greatest difference in angles amoung lines
            int[] point = new int[2]; // use to save points when determining where to draw to
            double[] hold = new double[4]; //used to store angles created by each point when drawing out from one point
            for(int i = 0; i < 4; i++) //choose a point to draw lines out from, then cycle through pairs of points to determine which two other points makes larges angle in order to draw proper lines
            {
                for (int j = 0; j < 4; j++) //loop to choose first point to compare
                {
                    for (int k = 0; k < 4; k++)//loop to choose second point to compare
                    {
                        for(int l = 0; l < 4; l++)//loop to choose third point to compare
                        {
                            if (i != j && j != k && k != l)//don't compare the same point to itself
                            {
                                hold[0] = findLineAngle(Math.Abs(p[j].X - p[i].X), Math.Abs(p[j].Y - p[i].Y));//find angle between current point and first point(simplify by translating point to origin in relation to current point)
                                hold[1] = findLineAngle(Math.Abs(p[k].X - p[i].X), Math.Abs(p[k].Y - p[i].Y));//find angle between current point and second point(simplify by translating point to origin in relation to current point)
                                hold[2] = findLineAngle(Math.Abs(p[l].X - p[i].X), Math.Abs(p[l].Y - p[i].Y));//find angle between current point and third point(simplify by translating point to origin in relation to current point)
                                //add 90 degrees to any line that goes beyond y axis
                                
                                if(p[j].X - p[i].X < 0 || p[j].X - p[i].X < 0 && p[j].Y - p[i].Y < 0)
                                {
                                    hold[0] = hold[0] + 90;
                                }
                                if (p[k].X - p[i].X < 0 || p[k].X - p[i].X < 0 && p[k].Y - p[i].Y < 0)
                                {
                                    hold[1] = hold[1] + 90;
                                }
                                if (p[l].X - p[i].X < 0 || p[l].X - p[i].X < 0 && p[l].Y - p[i].Y < 0)
                                {
                                    hold[2] = hold[2] + 90;
                                }
                                
                                //determine max angle for 1. solving first point to draw to. 2. use max to determine largest angle generated to find second point to draw to
                                greatest = hold.Max();
                                //1. find point that gives greatest angle
                                if(hold[0] == hold.ToList().IndexOf(greatest)) {
                                    point[0] = j;
                                }
                                if (hold[1] == hold.ToList().IndexOf(greatest))
                                {
                                    point[0] = k;
                                }
                                if (hold[2] == hold.ToList().IndexOf(greatest))
                                {
                                    point[0] = l;
                                }
                                // 2. compare each angle to max to determine greatest difference to determine which second line to draw to
                                for (int a = 0; a < 3; a++)
                                {
                                    if(hold[a] < greatest)
                                    {
                                        if(greatestDiff < greatest - hold[a])
                                        {
                                            greatestDiff = greatest - hold[a];
                                            if(a == 0)
                                            {
                                                point[1] = j;
                                            }
                                            if (a == 1)
                                            {
                                                point[1] = k;
                                            }
                                            if (a == 2)
                                            {
                                                point[1] = l;
                                            }
                                        }
                                        
                                    }
                                }


                            }
                        }
                        
                    }
                }
                greatest = 0;
                greatestDiff = 0;
                this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), p[i],p[point[0]]); //draw lines to adjacent points based on points found earlier
                this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2),p[i], p[point[1]]);
            }
        }
        public double findLineAngle(int x, int y)
        {
            if(x == 0 && y == 0) // if point is at (0,0) no line exists -> no angle exists
            {
                return 0;
            }
            if(x == 0)  //prevent math error
            {
                return 90;
            }
            else { //otherwise calculate angle
                Console.WriteLine(x);
                Console.WriteLine(y);
                Console.WriteLine(Math.Atan2(y, x));
                Console.WriteLine((180.0 / Math.PI) * Math.Atan2(y, x));   
                return (180.0 / Math.PI) * Math.Atan2(y * 10, x * 10); //need to make numbers larger, math.Atan isn't accurate enough with one digit numbers
            }
            
        }
        /*
        Angle calculation: In order to calculate any given angle between 3 points you must choose one point to be the point where 
        the angle we want is. The other adjacent points will be edge vertices. 
        
        To calculate first I translate the entire target angle to be in relation with the x-axis.

        Next I isolate two lines from the target angle and determine the angle they create with the x-axis.

        Then take the corresponding sum of those angles to determine the overall target angle.
        */
        public double findGraphingAngle(int sel1, int sel2, int sel3) //given three points from graph determine angle
        {
            double angle1 = 0;
            double angle2 = 0;
            int holdx;
            int holdy;
            Point holdP1;
            Point holdP2;
            Point holdP3;

            
                holdP1 = dataPoints[sel2]; //find first angle(angle between line 2-1 and x axis)
                holdP2 = dataPoints[sel1];
                holdx = holdP2.X - holdP1.X; // translate line 2-1 to get in relation with x-axis for angle calculation 
                holdy = holdP2.Y - holdP1.Y;
                //determine first angle 
                angle1 = findLineAngle(holdx, holdy);

                holdP3 = dataPoints[sel3]; //find second angle(angle between line 3-2 and x axis)
                holdx = holdP3.X - holdP1.X; // translate line 3-2 to get in relation with x-axis for angle calculation 
                holdy = holdP3.Y - holdP1.Y;
                //determine second angle 
                angle2 = findLineAngle(holdx, holdy);

            
            return Math.Abs(angle1 - angle2);
        }


        public double convertToAngle(double radians)
        {
            return ((180 / Math.PI) * radians);
        }

        public void redraw()
        {
            //draw all points points
            for (int i = 0; i < currentPos; i++)
            {
                this.CreateGraphics().FillEllipse(new SolidBrush(Color.Black), graphPoints[currentPos - i].X, graphPoints[currentPos - i].Y, 5, 5);
                this.CreateGraphics().DrawString((i+1).ToString(), new Font(new FontFamily("Arial"), 11, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.Black), graphPoints[i].X - 3, graphPoints[i].Y - 15);
            }
        }
        private void button5_Click(object sender, EventArgs e) // button for finding and drawing midpoint between two points
        {
            if (Int32.TryParse(firstPoint.Text, out dummy) && Int32.TryParse(secondPoint.Text, out dummy) && RelevantPoints >= 2)
            {
                if (Int32.Parse(firstPoint.Text) <= 4 && Int32.Parse(secondPoint.Text) <= 4)
                {
                    Point midPoint = getMidpoint(graphPoints[RelevantPoints - Int32.Parse(firstPoint.Text)], graphPoints[RelevantPoints - Int32.Parse(secondPoint.Text)]);

                    this.CreateGraphics().FillEllipse(new SolidBrush(Color.Blue), midPoint.X, midPoint.Y, 5, 5);
                    this.CreateGraphics().DrawString("M", new Font(new FontFamily("Arial"), 11, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.Blue), midPoint.X - 3, midPoint.Y - 15);
                }

            }
        }
        private void button3_Click(object sender, EventArgs e) //button to draw line across graph given by y = mx+b
        {
            int m = 0;
            int b;
            string[] slopeParts;
            bool slopeDecimal = false;


            if (textBox1.Text.Contains("/")) //if slope is fraction is entered then convert
            {
                slopeParts = textBox1.Text.Split('/');
                if (Int32.TryParse(slopeParts[0], out dummy) && Int32.TryParse(slopeParts[1], out dummy))
                {
                    m = Int32.Parse(slopeParts[0]) / Int32.Parse(slopeParts[1]);
                    Console.WriteLine(Int32.Parse(slopeParts[0]));
                    Console.WriteLine(Int32.Parse(slopeParts[1]));
                    Console.WriteLine(m);
                }        
                slopeDecimal = true;
            }    

            if (Int32.TryParse(textBox1.Text, out dummy) && Int32.TryParse(textBox2.Text, out dummy))
            {
                if (!slopeDecimal)//if not a fraction than convert straight to int
                {
                    m = Convert.ToInt32(Convert.ToDouble(textBox1.Text));// take values from text boxes and store as m and b   
                }

                b = Convert.ToInt32(Convert.ToDouble(textBox2.Text));

                Point p1 = new Point(0, 200 - (m * -300 + b)); // set points to draw to
                Point p2 = new Point(600, 200 - (m * 300 + b));

                for (int i = 0; i < 300; i++)  // determine first point to draw to so line is not drawn out of bounds
                {
                    if (200 + (m * i + b) > 400 || 200 + (m * i + b) < 0)
                    {
                        p1 = new Point(300 + i, 200 - (m * i + b));
                        break;  // stop at first instance that bound is broken
                    }
                }

                for (int i = -300; i < 0; i++)// determine second point to draw to so line is not drawn out of bounds
                {
                    if (200 - (m * i + b) > 400 || 200 - (m * i + b) < 0)
                    {
                        p2 = new Point(300 + i, 200 - (m * i + b));
                    }
                }
                lines[lineAmount, 0] = m;
                lines[lineAmount, 1] = b;
                lineAmount++;
                this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), p1, p2);
                this.CreateGraphics().DrawString("Line: " + lineAmount.ToString(), new Font(new FontFamily("Arial"), 11, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.Black), 350 - 3, 200 - (m * 50 + b) - 45);
            }

        }
        private void Calculator_Load(object sender, EventArgs e) // upon running draw the grid
        {
            this.CreateGraphics().FillRectangle(new SolidBrush(Color.White), field);
            this.CreateGraphics().DrawRectangle(new Pen(Color.Black, 2), field);
            this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), new Point(0, 175), new Point(450, 175));
            this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), new Point(225, 0), new Point(225, 350));
        }

        private void label1_Click_2(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            autoDraw = !autoDraw;         
        }

        private void Ycoord_Click(object sender, EventArgs e)
        {

        }

 
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        

        private void button4_Click(object sender, EventArgs e) // this is the button to connect two points with a line
        {
            if (Int32.TryParse(firstPoint.Text, out dummy) && Int32.TryParse(secondPoint.Text, out dummy) && RelevantPoints >= 2)
            {
                if(Int32.Parse(firstPoint.Text) >= 1 && Int32.Parse(secondPoint.Text) >= 1)
                {
                    this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), graphPoints[Int32.Parse(firstPoint.Text) - 1], graphPoints[Int32.Parse(secondPoint.Text) - 1]);
                }
                
            }
            
        }

        public Point getMidpoint(Point a, Point b)
        {
            return new Point((a.X + b.X)/2, (a.Y + b.Y) / 2);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            resetDrawings();
            currentPos = 0;
            RelevantPoints = 0;
        }

        public void resetDrawings()
        {
            this.Invalidate();
            this.CreateGraphics().FillRectangle(new SolidBrush(Color.White), field);
            this.CreateGraphics().DrawRectangle(new Pen(Color.Black, 2), field);
            this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), new Point(0, 175), new Point(450, 175));
            this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), new Point(225, 0), new Point(225, 350));
            //add lines to indicate axis lengths
            for (int i = 0; i < 5; i++)
            {
                this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), new Point(i * 100 + 100, 190), new Point(i * 100 + 100, 210));
                if (i != 2)
                {
                    this.CreateGraphics().DrawString(((i - 2) * 100 / scale).ToString(), new Font(new FontFamily("Arial"), 11, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.Black), i * 100 + 90, 215);
                }

            }
            for (int i = 0; i < 3; i++)
            {
                this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), new Point(290, i * 100 + 100), new Point(310, i * 100 + 100));
                if (i != 1)
                {
                    this.CreateGraphics().DrawString(((1 - i) * 100 / scale).ToString(), new Font(new FontFamily("Arial"), 11, FontStyle.Regular, GraphicsUnit.Pixel), new SolidBrush(Color.Black), 315, i * 100 + 93);
                }

            }
        }

        
        private void button1_Click_1(object sender, EventArgs e) //button to determine angle between three points
        {
            
            double temp;
            if (Int32.TryParse(textBox3.Text, out dummy) && Int32.TryParse(textBox4.Text, out dummy) && Int32.TryParse(textBox5.Text, out dummy))
            {
                temp = findGraphingAngle(Int32.Parse(textBox3.Text) - 1, Int32.Parse(textBox4.Text) - 1, Int32.Parse(textBox5.Text) - 1);
                label10.Text = "Result: " + temp.ToString(); //output total angle which is angle1-angle2
                lastAngle = temp; //save last angle
            }
        }

        private void button8_Click(object sender, EventArgs e) //button to invert last angle
        {
            label10.Text = "Inverted result: " + (360 - Math.Abs(lastAngle)).ToString();
        }


        private void button9_Click(object sender, EventArgs e) //button to find intersection between lines
        {
            if (Int32.TryParse(textBox10.Text, out dummy) && Int32.TryParse(textBox11.Text, out dummy) && lineAmount >= 2)
            {
                int line1 = Int32.Parse(textBox10.Text) - 1;
                int line2 = Int32.Parse(textBox11.Text) - 1;

                if ((lines[line2, 0] - lines[line1, 0]) == 0)
                {
                    label10.Text = "Inconclusive";
                    return;
                }

                int xVal = (lines[line2, 1] - lines[line1, 1])/(lines[line1, 0] - lines[line2, 0]);
                int yVal = lines[line1, 0] * xVal + lines[line1, 1];

                label10.Text = "Intersection point = (" + xVal.ToString() + "," + yVal.ToString() + ")";

            }
        }


        private void textBox4_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)// button for finding segment length
        {
            if (Int32.TryParse(firstPoint.Text, out dummy) && Int32.TryParse(secondPoint.Text, out dummy) && RelevantPoints >= 2)
            {
                if(Int32.Parse(firstPoint.Text) >= 1 && Int32.Parse(secondPoint.Text) >= 1)
                {
                    label10.Text = "Result: " + findLineLength(Int32.Parse(firstPoint.Text), Int32.Parse(firstPoint.Text)).ToString();
                }
                
            }
        }

        //function to determine a length of a line given two points
        //takes in two ints that represent the values entered into the textfield to get a point from graph(data-points)
        private double findLineLength(int p1, int p2) 
        {

            int overallX = dataPoints[p1 - 1].X - dataPoints[p2 - 1].X;
            int overallY = dataPoints[p1 - 1].Y - dataPoints[p2 - 1].Y;

            return Math.Sqrt(Math.Pow(overallX, 2) + Math.Pow(overallY, 2));
        }


        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e) //button to draw polygon given 4 points
        {
            if (Int32.TryParse(textBox6.Text, out dummy) && Int32.TryParse(textBox7.Text, out dummy) && Int32.TryParse(textBox8.Text, out dummy) && Int32.TryParse(textBox9.Text, out dummy))
            {
                this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), graphPoints[Int32.Parse(textBox6.Text) - 1], graphPoints[Int32.Parse(textBox7.Text) - 1]);
                this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), graphPoints[Int32.Parse(textBox7.Text) - 1], graphPoints[Int32.Parse(textBox8.Text) - 1]);
                this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), graphPoints[Int32.Parse(textBox8.Text) - 1], graphPoints[Int32.Parse(textBox9.Text) - 1]);
                this.CreateGraphics().DrawLine(new Pen(Brushes.Black, 2), graphPoints[Int32.Parse(textBox9.Text) - 1], graphPoints[Int32.Parse(textBox6.Text) - 1]);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) //scale down the graph
        {
            scale = 10;
            resetDrawings();
            

            Point holder = new Point();
            int diff1;
            int diff2;

            for(int i = 0; i < currentPos; i++) //loop through all drawing points and convert to new scale
            {
                holder = graphPoints[i];    //hold current point

                diff1 = graphPoints[i].X - 297; //get X&Y value entered by user
                diff2 = graphPoints[i].Y - 197;

                graphPoints[i].X = 297 + diff1 * scale; // convert to use scale then return to array
                graphPoints[i].Y = 197 + diff2 * scale;
            }
            redraw();

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)// set graph to origional scale
        {
            scale = 1;
            resetDrawings();

            Point holder = new Point();
            int diff1;
            int diff2;

            for (int i = 0; i < currentPos; i++) //loop through all drawing points and convert to new scale
            {
                holder = graphPoints[i];    //hold current point

                diff1 = graphPoints[i].X - 297; //get X&Y value entered by user
                diff2 = graphPoints[i].Y - 197;

                graphPoints[i].X = 297 + diff1 * scale; // convert to use scale then return to array
                graphPoints[i].Y = 197 + diff2 * scale;
            }


            redraw();
            
        }

        private void button10_Click(object sender, EventArgs e) // button to determine triangle type
        {
            if (Int32.TryParse(textBox3.Text, out dummy) && Int32.TryParse(textBox4.Text, out dummy) && Int32.TryParse(textBox5.Text, out dummy))
            {

                label10.Text = label10.Text + determmineTriangleType(Int32.Parse(textBox3.Text), Int32.Parse(textBox4.Text), Int32.Parse(textBox5.Text));

            }
        }
    
        private String determmineTriangleType(int p1, int p2, int p3) //function to determine triangle type by angle and edges
        {
            double[] angles = new double[3];
            double[] sideLengths = new double[3];
            
            angles[0] = findGraphingAngle(p1 - 1, p2 - 1, p3 - 1);
            angles[1] = findGraphingAngle(p1 - 1, p3 - 1, p2 - 1);
            angles[2] = findGraphingAngle(p2 - 1, p1 - 1, p3 - 1);

            sideLengths[0] = findLineLength(p1, p2);
            sideLengths[1] = findLineLength(p1, p3);
            sideLengths[2] = findLineLength(p2, p3);

            String result = "";
            
            for(int i = 0; i < 3; i++)
            {
                if (angles[i] > 90)
                {
                    result = result + "Obtuse";
                }
                else if(angles[i] == 90)
                {
                    result = result + "Right Angle";
                }
            }

            result = result + "Acute";

            if (sideLengths[0] == sideLengths[1] && sideLengths[1] == sideLengths[2])
            {
                result = result + "Equilateral Triangle";
            }

            else if (sideLengths[0] == sideLengths[1] && sideLengths[1] != sideLengths[2]  || sideLengths[1] == sideLengths[2] && sideLengths[0] != sideLengths[1] || sideLengths[0] == sideLengths[2] && sideLengths[0] != sideLengths[1])
            {
                result = result + "Isoceles Triangle";
            }
            else
            {
                result = result + "Scalene Triangle";
            }

            return result;
        }
    
    }
}
