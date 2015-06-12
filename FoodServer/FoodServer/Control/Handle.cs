using System;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace WindowsApplication1
{
    [Serializable]
    public abstract class Handle:Ele
    {
        public string op;

        public Handle(Ele e, string o)
        {
            op = o;
            this.rePosition(e);        
        }

        public string isOver(int x, int y)
        {
            Rectangle r;
            r = new Rectangle(this.X, this.Y, this.X1 - this.X, this.Y1 - this.Y);
            if (r.Contains(x, y))
                return this.op;
            else
                return "";
        }

        public virtual void rePosition(Ele e)
        { }
    }

    /// <summary>
    /// 
    /// Handle object for redim/move/rotate shapes
    /// </summary>
    [Serializable]
    public class RedimHandle : Handle
    {
        
        public RedimHandle(Ele e, string o) : base(e,o) 
        {
            this.fillColor = Color.Black;
        }

        public override void rePosition(Ele e)
        {
            switch (this.op)
            {
                case "NW":
                    this.X = e.getX() - 2;
                    this.Y = e.getY() - 2;   
                    break;
                case "N":
                    this.X = e.getX() - 2 + ((e.getX1() - e.getX()) / 2);
                    this.Y = e.getY() - 2;
                    break;
                case "NE":
                    this.X = e.getX1() - 2;
                    this.Y = e.getY() - 2;
                    break;
                case "E":
                    this.X = e.getX1() - 2;
                    this.Y = e.getY() - 2 + (e.getY1() - e.getY()) / 2;                    
                    break;
                case "SE":
                    this.X = e.getX1() - 2;
                    this.Y = e.getY1() - 2;
                    break;
                case "S":
                    this.X = e.getX() - 2 + (e.getX1() - e.getX()) / 2;
                    this.Y = e.getY1() - 2;                    
                    break;
                case "SW":
                    this.X = e.getX() - 2;
                    this.Y = e.getY1() - 2;
                    break;
                case "W":
                    this.X = e.getX() - 2;
                    this.Y = e.getY() - 2 + (e.getY1() - e.getY()) / 2;
                    break;
                default: 
                    break;
            }
            this.X1 = this.X + 5;
            this.Y1 = this.Y + 5;

        }

        
        public override void Draw(Graphics g, int dx, int dy, float zoom)
        {
            //System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(this.fillColor);
            myBrush.Color = this.Trasparency(Color.Black, 80);
            System.Drawing.Pen whitePen = new System.Drawing.Pen(System.Drawing.Color.White);            
            g.FillRectangle(myBrush, new RectangleF((this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom));
            g.DrawRectangle(whitePen, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);
            myBrush.Dispose();
            whitePen.Dispose();
        }
    }

    [Serializable]
    public class ZoomHandle : Handle
    { 
        public ZoomHandle(Ele e, string o)
            : base(e, o)
        {
            this.fillColor = Color.Red;
        }

        public override void rePosition(Ele e)
        {
            float zx = (e.Width - (e.Width * e.getGprZoomX()))/2 ;
            float zy = (e.Height - (e.Height * e.getGprZoomY())) / 2;
            this.X = (int)((e.getX1() - 2) - zx);
            this.Y = (int)((e.getY1() - 2) - zy);
            this.X1 = this.X + 5;
            this.Y1 = this.Y + 5;
            //this._zoomX = ((SelRect)e).zoomX;
            //this._zoomY = ((SelRect)e).zoomY;

        }
        public override void Draw(Graphics g, int dx, int dy, float zoom)
        {
            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(this.fillColor);
            myBrush.Color = this.Trasparency(myBrush.Color, 80);
            System.Drawing.Pen whitePen = new System.Drawing.Pen(System.Drawing.Color.White);
            System.Drawing.Pen fillPen = new System.Drawing.Pen(this.fillColor);

            g.FillRectangle(myBrush, new RectangleF((this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom));
            g.DrawRectangle(whitePen, (this.X + dx) * zoom , (this.Y + dy) * zoom , (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);
            
            g.DrawRectangle(fillPen, (this.X + dx -1 ) * zoom, (this.Y + dy -1) * zoom, (this.X1 - this.X +2) * zoom, (this.Y1 - this.Y +2) * zoom);
            
            myBrush.Dispose();
            whitePen.Dispose();
            fillPen.Dispose();
        }



    }

    [Serializable]
    public  class RotHandle : Handle 
    {

        public RotHandle(Ele e, string o)
            : base(e, o)
        { }

        public override void rePosition(Ele e)
        {
            float midX, midY = 0;
            midX = (e.getX1() - e.getX()) / 2;
            midY = (e.getY1() - e.getY()) / 2;
            PointF Hp = new PointF(0, -25);
            PointF RotHP = this.rotatePoint(Hp, e.getRotation());
            midX += RotHP.X;
            midY += RotHP.Y;

            this.X = e.getX() + (int)midX - 2;
            this.Y = e.getY() + (int)midY - 2;
            this._rotation = e.getRotation();

            this.X1 = this.X + 5;
            this.Y1 = this.Y + 5;


        }

        public override void Draw(Graphics g, int dx, int dy, float zoom)
        {
            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            myBrush.Color = this.Trasparency(Color.Black, 80);
            System.Drawing.Pen whitePen = new System.Drawing.Pen(System.Drawing.Color.White);
            System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Blue, 1.5f);
            myPen.DashStyle = DashStyle.Dash;

            g.FillRectangle(myBrush, new RectangleF((this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom));
            g.DrawRectangle(whitePen, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);

            //CENTER POINT
            float midX, midY = 0;
            midX = (this.X1 - this.X) / 2;
            midY = (this.Y1 - this.Y) / 2;

            PointF Hp = new PointF(0, 25);

            PointF RotHP = this.rotatePoint(Hp, this._rotation);
            
            RotHP.X += this.X;
            RotHP.Y += this.Y;
            g.FillEllipse(myBrush, (RotHP.X + midX + dx - 3) * zoom, (RotHP.Y + dy - 3 + midY) * zoom, 6 * zoom, 6 * zoom);
            g.DrawEllipse(whitePen, (RotHP.X + midX + dx - 3) * zoom, (RotHP.Y + dy - 3 + midY) * zoom, 6 * zoom, 6 * zoom);
            g.DrawLine(myPen, (this.X +midX+ dx) * zoom, (this.Y + midY + dy) * zoom, ( RotHP.X+midX + dx) * zoom, ( RotHP.Y+midY + dy) * zoom);

            myPen.Dispose();
            myBrush.Dispose();
            whitePen.Dispose();
        }

    }


    /// <summary>
    /// 
    /// Abstract Handle collection for redim/move/rotate shapes
    /// </summary>
    [Serializable]
    public abstract class AbstractSel : Ele
    {
        //public float zoomX=1;
        //public float zoomY=1;
        protected ArrayList handles;

        public AbstractSel(Ele el)
        {
            this.X = el.getX();
            this.Y = el.getY();
            this.X1 = el.getX1(); ;
            this.Y1 = el.getY1();
            this.Selected = false;
            this.rot = el.canRotate();// RotAllowed;
            this._rotation = el.getRotation();
            this.gprZoomX = el.getGprZoomX();
            this.gprZoomY = el.getGprZoomY();
            this.sonoUnaLinea = el.sonoUnaLinea;
            this.IamGroup = el.AmIaGroup();
            handles = new ArrayList();
            this.endMoveRedim();
        }

        public override void endMoveRedim()
        {
            base.endMoveRedim();
            foreach (Handle h in handles)
            {
                h.endMoveRedim();
            }
        }


        public void setZoom(float x, float y)
        {
            this.gprZoomX = x;
            this.gprZoomY = y;
            foreach (Handle h in handles)
            {
                h.rePosition(this);
            }
        }

        public override void Rotate(float x, float y)
        {
            base.Rotate(x, y);
            foreach (Handle h in handles)
            {
                h.rePosition(this);
            }            
        }

        public override void move(int x, int y)
        {
            base.move(x, y);
            //
            foreach (Handle h in handles)
            {
                //h.move(x, y);
                h.rePosition(this);
            }
        }

        public void showHandles(bool i)
        {
            this.IamGroup = i;
        }

        /// <summary>
        /// Su quale maniglia cade il punto x,y? 
        /// </summary>
        public string isOver(int x, int y)
        {
            string ret;
            foreach (Handle h in handles)
            {
                ret = h.isOver(x, y);
                if (ret != "")
                    return ret;
            }

            if (this.contains(x, y))
                return "C";

            return "NO";

        }

        public override void Select()
        {
            this.undoEle = this.Copy();
        }

        public override void redim(int x, int y, string redimSt)
        {
            base.redim(x, y, redimSt); //
            foreach (Handle h in handles)
            {
                h.rePosition(this);
            }

        }

        public override void Draw(Graphics g, int dx, int dy, float zoom)
        {
            foreach (Handle h in handles)
            {
                h.Draw(g, dx, dy, zoom);
            }
        }

    }


    /// <summary>
    /// Handle tool for redim/move/rotate shapes
    /// </summary>
    [Serializable]
    public class SelRect : AbstractSel
    {
        public SelRect(Ele el)
            : base(el)
        {
            setup();
        }

        /// <summary>
        ///set ups handles
        /// </summary>
        public void setup()
        {
            if (!this.AmIaGroup())
            {
                //NW
                this.handles.Add(new RedimHandle(this, "NW"));
                //SE
                this.handles.Add(new RedimHandle(this, "SE"));
                if (!sonoUnaLinea)
                {
                    //N
                    this.handles.Add(new RedimHandle(this, "N"));
                    if (rot)
                    {
                        //ROT
                        this.handles.Add(new RotHandle(this, "ROT"));
                    }
                    //NE
                    this.handles.Add(new RedimHandle(this, "NE"));
                    //E
                    this.handles.Add(new RedimHandle(this, "E"));
                    //S
                    this.handles.Add(new RedimHandle(this, "S"));
                    //SW
                    this.handles.Add(new RedimHandle(this, "SW"));
                    //W
                    this.handles.Add(new RedimHandle(this, "W"));
                }
            }
            else
            {
                //N
                this.handles.Add(new RedimHandle(this, "N"));
                if (rot)
                {
                    //ROT
                    this.handles.Add(new RotHandle(this, "ROT"));
                }
                //E
                this.handles.Add(new RedimHandle(this, "E"));
                //S
                this.handles.Add(new RedimHandle(this, "S"));
                //W
                this.handles.Add(new RedimHandle(this, "W"));
                //ZOOM
                this.handles.Add(new ZoomHandle(this, "ZOOM"));
            }

        }

        public override void Draw(Graphics g, int dx, int dy, float zoom)
        {
            //TEST
            //Matrix precMx = g.Transform.Clone();//store previos trasformation
            //Matrix mx = g.Transform; // get previous trasformation
            //PointF p = new PointF(zoom * (this.X + dx + (this.X1 - this.X) / 2), zoom * (this.Y + dy + (this.Y1 - this.Y) / 2));
            //mx.RotateAt(this.getRotation(), p, MatrixOrder.Append); //add a trasformation
            //g.Transform = mx;

            base.Draw(g, dx, dy, zoom);   
            System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Blue, 1.5f);
            myPen.DashStyle = DashStyle.Dash;
            if (this.sonoUnaLinea)
                g.DrawLine(myPen, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 + dx) * zoom, (this.Y1 + dy) * zoom);
            else
                g.DrawRectangle(myPen, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);
            myPen.Dispose();

            //TEST
            //g.Transform = precMx;
            //precMx.Dispose();
            //mx.Dispose();
        }

    }


}
