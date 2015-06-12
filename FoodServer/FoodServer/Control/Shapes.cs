using System;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace WindowsApplication1
{

    //Grouped objs properties. When you decide to create a group with the
    // selected objs, U can preset these pprops in the objs:
    // see: 
        //_onGroupXRes 
        //_onGroupX1Res
        //_onGroupYRes 
        //_onGroupY1Res 
    public enum OnGroupResize { Move, Resize, Nothing};


    #region Events & Delegates (Used by the controls vectShape & toolBox )
    
    public delegate void OptionChanged(object sender, OptionEventArgs e);

    public delegate void ObjectSelected(object sender, PropertyEventArgs e);

    public class PropertyEventArgs : EventArgs
    {
        public Ele[] ele;
        public bool Undoable; //for enable/disable udo/redoBtn
        public bool Redoable; //for enable/disable udo/redoBtn
        //Constructor.
        public PropertyEventArgs(Ele[] a,bool r, bool u)
        {
            this.ele = a;
            this.Undoable = u;
            this.Redoable = r;
        }
    }

    public class OptionEventArgs : EventArgs 
    {  
      public string option ;
      
      //Constructor.
      //
        public OptionEventArgs(string s) 
      {
         this.option = s;
      }
    }

    #endregion

    #region Base element Ele and its derived class

    // base Element
    [Serializable]
    public abstract class Ele: Object
    {
        protected bool IamGroup = false;
        protected bool rot; // Can Rotate
        
        //Start point
        protected int X; 
        protected int Y;
        //End point
        protected int X1; 
        protected int Y1;

        //Grouped objs properties. When you decide to create a group with the
        // selected objs, U can preset these pprops in the objs: Move,Resize,Nothing
        // Move: On the resize of the group the grouped obj moves
        // Resize: On the resize of the group the grouped obj resizes
        // Nothing: On the resize of the group the grouped obj mantains its position and dimension.
        // When you resize the group using the X handle (West):
        protected OnGroupResize _onGroupXRes = OnGroupResize.Resize;
        // When you resize the group using the X1 handle (East):
        protected OnGroupResize _onGroupX1Res = OnGroupResize.Resize;
        // When you resize the group using the Y handle (North):
        protected OnGroupResize _onGroupYRes = OnGroupResize.Resize;
        // When you resize the group using the Y handle (South):
        protected OnGroupResize _onGroupY1Res = OnGroupResize.Resize;

        // When double click on Group obj, forward it to sub-obj
        protected bool _onGroupDoubleClick = true;

        // Store start position during moving/resizing
        protected int startX; 
        protected int startY;
        protected int startX1; 
        protected int startY1; 
        
        // some pen properties
        protected LineCap start;
        protected LineCap end;
        protected DashStyle dashstyle;

        protected int _rotation; 

        private Color _penColor;
        private float _penWidth;
        private Color _fillColor;
        private bool _filled;
        private bool _showBorder;
        private DashStyle _dashStyle;
        private int _alpha;

        //LINEAR GRADIENT
        private bool _useGradientLine = false;
        private Color _endColor=Color.White;
        private int _endalpha = 255;
        private int _gradientLen = 0;
        private int _gradientAngle = 0;
        private float _endColorPos = 1f;

        //Group obj zoom 
        protected float gprZoomX = 1f;
        protected float gprZoomY = 1f;

        public bool sonoUnaLinea; //I am a Line

        public bool Selected;

        public bool Deleted;

        public Ele undoEle;

        public Ele()
        {
         penColor=Color.Black;
         penWidth= 1f;
         fillColor=Color.Black;
         filled=false;
         showBorder = true;
         this.dashstyle = DashStyle.Solid;
         this.alpha = 255;
        }

        ~Ele()
        {
           // System.Console.WriteLine("Destrojed obj:  {0}/{1}", this.getX().ToString(), this.getY().ToString());
        }

     #region GET/SET
        public bool canRotate()
        {            
                return rot;            
        }
        public int getRotation()
        {
                if (canRotate())
                    return _rotation;
                else
                    return 0;
        }

        public void addRotation(int a)
        {
            //if (canRotate())
                this._rotation += a;
        }

        public bool AmIaGroup()
        {
            return this.IamGroup;
            //return false;
        }

        [CategoryAttribute("Position"), DescriptionAttribute("X ")]
        public int PosX
        {
            get
            {
                return this.X;
            }
        }
        [CategoryAttribute("Position"), DescriptionAttribute("Y ")]
        public int PosY
        {
            get
            {
                return this.Y;
            }
        }
        [CategoryAttribute("Dimention"), DescriptionAttribute("Width ")]
        public int Width
        {
            get
            {
                return System.Math.Abs( this.X1-this.X);
            }
        }
        [CategoryAttribute("Dimention"), DescriptionAttribute("Height ")]
        public int Height
        {
            get
            {
                return System.Math.Abs(this.Y1 - this.Y);
            }
        }
        [CategoryAttribute("Group Behav"), DescriptionAttribute("On Group Resize X")]
        public OnGroupResize OnGrpXRes
        {
            set 
            {
                _onGroupXRes = value;
            }
            get
            {
                return _onGroupXRes;
            }
        }
        [CategoryAttribute("Group Behav"), DescriptionAttribute("On Group Resize X1")]
        public OnGroupResize OnGrpX1Res
        {
            set
            {
                _onGroupX1Res = value;
            }
            get
            {
                return _onGroupX1Res;
            }
        }
        [CategoryAttribute("Group Behav"), DescriptionAttribute("On Group Resize Y")]
        public OnGroupResize OnGrpYRes
        {
            set
            {
                _onGroupYRes = value;
            }
            get
            {
                return _onGroupYRes;
            }
        }
        [CategoryAttribute("Group Behav"), DescriptionAttribute("On Group Resize Y1")]
        public OnGroupResize OnGrpY1Res
        {
            set
            {
                _onGroupY1Res = value;
            }
            get
            {
                //return System.Math.Abs(this.Y1 - this.Y);
                return _onGroupY1Res;
            }
        }
        [CategoryAttribute("Group Behav"), DescriptionAttribute("Manage On Group Double Click")]
        public bool OnGrpDClick
        { 
            set
            {
                _onGroupDoubleClick = value;
            }
            get
            {
                return _onGroupDoubleClick;
            }
        }

        [CategoryAttribute("Appearance"), DescriptionAttribute("Set Border Dash Style ")]
        public virtual DashStyle dashStyle
        {
            get
            {
                return _dashStyle;
            }
            set
            {
                _dashStyle = value;
            }
        }
        [CategoryAttribute("Appearance"), DescriptionAttribute("Show border when filled or contains Text")]
        public virtual bool showBorder
        {
            get
            {
                return _showBorder;
            }
            set
            {
                _showBorder = value;
            }
        }
        [CategoryAttribute("Appearance"), DescriptionAttribute("Pen Color")]
        public virtual Color penColor
        {
            get
            {
                return _penColor;
            }
            set
            {
                _penColor = value;
            }
        }
        [CategoryAttribute("Appearance"), DescriptionAttribute("Fill Color")]
        public virtual Color fillColor
        {
            get
            {
                return _fillColor;
            }
            set
            {
                _fillColor = value;
            }
        }
        [CategoryAttribute("Appearance"), DescriptionAttribute("Pen Width")]
        public virtual float penWidth
        {
            get
            {
                return _penWidth;
            }
            set
            {
                _penWidth = value;
            }
        }
        [CategoryAttribute("Appearance"), DescriptionAttribute("Filled/Unfilled")]
        public virtual bool filled
        {
            get
            {
                return _filled;
            }
            set
            {
                _filled = value;
            }
        }
        [CategoryAttribute("Appearance"), DescriptionAttribute("Trasparency")]
        public virtual int alpha
        {
            get
            {
                return _alpha;
            }
            set
            {
                if (value < 0)
                    _alpha = 0;
                else
                    if (value > 255)
                        _alpha = 255;
                    else
                        _alpha = value;
            }
        }

        [CategoryAttribute("GradientBrush"), DescriptionAttribute("True: use gradient fill color")]
        public virtual bool UseGradientLineColor
        {
            get
            {
                return _useGradientLine;
            }
            set
            {
                _useGradientLine = value;
            }
        }

        [CategoryAttribute("GradientBrush"), DescriptionAttribute("End Color Position [0..1])")]
        public virtual float EndColorPosition
        {
            get
            {
                return _endColorPos;
            }
            set
            {
                if (value > 1)
                    value = 1;
                if (value < 0)
                    value = 0;
                _endColorPos = value;
            }
        }


        [CategoryAttribute("GradientBrush"), DescriptionAttribute("Gradient End Color")]
        public virtual Color EndColor
        {
            get
            {
                return _endColor;
            }
            set
            {
                _endColor = value;
            }
        }


        [CategoryAttribute("GradientBrush"), DescriptionAttribute("End Color Trasparency")]
        public virtual int EndAlpha
        {
            get
            {
                return _endalpha;
            }
            set
            {
                if (value < 0)
                    _endalpha = 0;
                else
                    if (value > 255)
                        _endalpha = 255;
                    else
                        _endalpha = value;
            }
        }

        [CategoryAttribute("GradientBrush"), DescriptionAttribute("Gradient Dimension")]
        public virtual int GradientLen
        {
            get
            {
                return _gradientLen;
            }
            set
            {
                if (value >= 0)
                    _gradientLen = value;
                else
                    _gradientLen = 0;
            }
        }

        [CategoryAttribute("GradientBrush"), DescriptionAttribute("Gradient Angle")]
        public virtual int GradientAngle
        {
            get
            {
                return _gradientAngle;
            }
            set
            {
                    _gradientAngle = value;
            }
        }


        public int getX()
        {
            return X;
        }
        public int getY()
        {
            return Y;
        }
        public int getX1()
        {
            return X1;
        }
        public int getY1()
        {
            return Y1;
        }

        public float getGprZoomX()
        {
            return gprZoomX;
        }
        public float getGprZoomY()
        {
            return gprZoomY;
        }


        #endregion

        #region virtual metods
        public virtual void Draw(Graphics g,int dx,int dy, float zoom)
        { }
        public virtual void AddGp(GraphicsPath gp, int dx, int dy, float zoom)
        {}
        public virtual ArrayList deGroup()
        {
            return null;
        }
        public virtual void Select()
        { }
        public virtual void Select(RichTextBox r)
        { }
       // public virtual void ShowEditor(richForm2 f)
       // { }
        public virtual void AfterLoad()
        { }
        public virtual void CopyFrom(Ele ele)
        { }
        public virtual Ele Copy()
        {
            return null;
        }
        protected void copyGradprop(Ele ele)
        { 
            _useGradientLine = ele._useGradientLine;
            _endColor = ele._endColor;
            _endalpha = ele._endalpha;
            _gradientLen = ele._gradientLen;
            _gradientAngle = ele._gradientAngle;
            _endColorPos = ele._endColorPos;
        }

        //public virtual void Rotate(float x, float y)
        //{ }

#endregion

        protected float scaledPenWidth(float zoom)
        {
            if (zoom < 0.1f)
                zoom = 0.1f;
            return this.penWidth * zoom;
        }

        public void Fit2grid(int gridsize)
        {
            this.startX = gridsize * (int)(this.startX / gridsize);
            this.startY = gridsize * (int)(this.startY / gridsize);
            this.startX1 = gridsize * (int)(this.startX1 / gridsize);
            this.startY1 = gridsize * (int)(this.startY1 / gridsize);
            
        }


        public virtual void Rotate(float x, float y)
        {
            float tmp = _rotate(x, y);
            this._rotation = (int)tmp;
        }

        protected PointF rotatePoint(PointF p, int RotAng)
        { 
            double RotAngF = RotAng * Math.PI/180;
            double SinVal = Math.Sin(RotAngF);
            double CosVal = Math.Cos(RotAngF);
            float Nx = (float)(p.X * CosVal - p.Y * SinVal);
            float Ny = (float)(p.Y * CosVal + p.X * SinVal);
            return new PointF(Nx, Ny);
        }

        protected float _rotate(float x, float y)
        { 
			//
            Point c = new Point((int)(this.X + (this.X1 - this.X) / 2), (int)(this.Y+(this.Y1 - this.Y) / 2));
			float dx = x - c.X;
			float dy = y - c.Y;
			float b= 0f;
			float alpha= 0f;
			float f= 0f;
			if ((dx>0)&(dy>0))
			{
				b= 90;
				alpha= (float)Math.Abs((Math.Atan((double)(dy/dx)) * (180/Math.PI)));
			}
			else
				if ((dx<=0)&(dy>=0))
				{
					b= 180;
					if (dy > 0)
						alpha= (float)Math.Abs((Math.Atan((double)(dx/dy)) * (180/Math.PI)));
				}
				else
					if ((dx<0)&(dy<0))
					{
						b= 270;
						alpha= (float)Math.Abs((Math.Atan((double)(dy/dx)) * (180/Math.PI)));
					}
					else
					{
						b= 0;
						alpha= (float)Math.Abs((Math.Atan((double)(dx/dy)) * (180/Math.PI)));
					}
			f = (b+ alpha);
            return f;
			//

        }

        private float getDimX()
        {
            return (float)(System.Math.Sqrt( System.Math.Pow(this.Width, 2) + System.Math.Pow(this.Height, 2)) - this.Width )/2;
        }
        private float getDimY()
        {
            return (float)(System.Math.Sqrt(System.Math.Pow(this.Width, 2) + System.Math.Pow(this.Height, 2)) - this.Height) / 2;
        }


        protected Brush getBrush(int dx,int dy, float zoom)
        {
            if (this.filled)
            {
                if (this.UseGradientLineColor)
                {
                    float wid;
                    float hei;
                    if (this.GradientLen > 0)
                    {
                        wid = this.GradientLen;
                        hei = this.GradientLen;
                    }
                    else
                    {
                        wid = ((this.X1 - this.X) + 2 * getDimX()) * zoom;
                        hei = ((this.Y1 - this.Y) + 2 * getDimY()) * zoom;
                    }
                    LinearGradientBrush br=new LinearGradientBrush(
                        new RectangleF((this.X - getDimX() + dx) * zoom, (this.Y - getDimY() + dy) * zoom, wid, hei)
                        , this.Trasparency(this.fillColor, this.alpha)
                        , this.Trasparency(this.EndColor, this.EndAlpha)
                        , this.GradientAngle
                        ,true);
                    br.SetBlendTriangularShape(this.EndColorPosition, 0.95f);
                    br.WrapMode = WrapMode.TileFlipXY;
                    return br;
                }
                else
                {
                    return new SolidBrush(this.Trasparency(this.fillColor, this.alpha));
                }
            }
            else
            {
                return null;
            }
        }

        protected void copyStdProp(Ele from, Ele to)
        {
            to.X = from.X;
            to.X1 = from.X1;
            to.Y = from.Y;
            to.Y1 = from.Y1;
            to.start = from.start;
            to.startX = from.startX;
            to.startX1 = from.startX1;
            to.startY = from.startY;
            to.startY1 = from.startY1;
            to.sonoUnaLinea = from.sonoUnaLinea;
            to.alpha = from.alpha;
            to.dashstyle = from.dashstyle;
            to.fillColor = from.fillColor;
            to.filled = from.filled;
            to.penColor = from.penColor;
            to._penWidth = from.penWidth;
            to.showBorder = from.showBorder;
            to._onGroupX1Res = from._onGroupX1Res;
            to._onGroupXRes = from._onGroupXRes;
            to._onGroupY1Res = from._onGroupY1Res;
            to._onGroupYRes = from._onGroupYRes;

            to._useGradientLine = from._useGradientLine;
            to._endColor=from._endColor;
            to._endalpha = from._endalpha;
            to._gradientLen = from._gradientLen;
            to._gradientAngle = from._gradientAngle;
            to._endColorPos = from._endColorPos;

        }

        /// <summary>
        /// 2 points distance
        /// </summary>
        protected int Dist(int x, int y, int x1, int y1)
        {
            return (int)System.Math.Sqrt(System.Math.Pow((x - x1),2) + System.Math.Pow((y - y1),2) );
        }

        /// <summary>
        /// Make a color darker or lighter
        /// </summary>
        protected Color dark(Color c, int v, int a)
        {

            int r = c.R;
            r = r - v;
            if (r < 0)
                r = 0;
            if (r >255)
                r = 255;
            int green = c.G;
            green = green - v;
            if (green < 0)
                green = 0;
            if (green > 255)
                green = 255;
            int b = c.B;
            b = b - v;
            if (b < 0)
                b = 0;
            if (b > 255)
                b = 255;
            if (a > 255)
                a = 255;
            if (a <0)
                a = 0;

            return Color.FromArgb(a,r, green, b);

        }

        /// <summary>
        /// Make a color Tresparent/Solid
        /// </summary>
        protected Color Trasparency(Color c, int v)
        {
            if (v < 0)
                v = 0;
            if (v > 255)
                v = 255;

            return Color.FromArgb(v, c);
        }

        public bool contains(int x, int y)
        {
            if (sonoUnaLinea)
            {
                int appo = Dist(x, y, this.X, this.Y) + Dist(x, y, this.X1, this.Y1);
                int appo1 = Dist(this.X1, this.Y1, this.X, this.Y) + 7;

                return appo < appo1;
            }
            else 
            {
                //Rectangle r = new Rectangle(this.X, this.Y, this.X1 - this.X, this.Y1 - this.Y);
                //return r.Contains(x, y);
                return new Rectangle(this.X, this.Y, this.X1 - this.X, this.Y1 - this.Y).Contains(x, y);
            }
            
        }

        public virtual void move(int x, int y)
        {            
            this.X = this.startX - x;
            this.Y = this.startY - y;
            this.X1 = this.startX1 - x;
            this.Y1 = this.startY1 - y;
        }
        
        /* 
        public void move(int x, int startx,int y, int starty)
        {
            int dx = startx - x;
            int dy = starty - y;
            this.X = this.X - dx;
            this.Y = this.Y - dy;
            this.X1 = this.X1 - dx;
            this.Y1 = this.Y1 - dy;
        }
        */

        public virtual void redim(int x, int y, string redimSt)
        {
            switch (redimSt)
            {
                case "NW":
                    this.X = this.startX + x;
                    this.Y = this.startY + y;
                    break;
                case "N":
                    this.Y = this.startY + y;
                    break;
                case "NE":
                    this.X1 = this.startX1 + x;
                    this.Y = this.startY + y;
                    break;
                case "E":
                    this.X1 = this.startX1 + x;                    
                    break;
                case "SE":
                    this.X1 = this.startX1 + x;
                    this.Y1 = this.startY1 + y;
                    break;
                case "S":
                    this.Y1 = this.startY1 + y;
                    break;
                case "SW":
                    this.X = this.startX + x;
                    this.Y1 = this.startY1 + y;
                    break;
                case "W":
                    this.X = this.startX + x;
                    break;
                default:
                    break;
            }

            if (!this.sonoUnaLinea)
            {   // manage redim limits
                if (this.X1 <= this.X)
                    this.X1 = this.X + 10;
                if (this.Y1 <= this.Y)
                    this.Y1 = this.Y + 10;
            }

        }

        public virtual void endMoveRedim()
        {
            this.startX = this.X;
            this.startY = this.Y;
            this.startX1 = this.X1;
            this.startY1 = this.Y1;
        }

    }




    /// <summary>
    /// 
    /// Handle tool for redim/move/rotate shapes
    /// </summary>
    [Serializable]
    public class AbSel : Ele
    {
        public AbSel(Ele EL)
        {
            this.X = EL.getX();
            this.Y = EL.getY();
            this.X1 = EL.getX1();
            this.Y1 = EL.getY1();
            this.Selected = false;
            this.endMoveRedim();
            this.rot = EL.canRotate();
            this._rotation = EL.getRotation();
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
            if (this.contains(x, y))
                return "C";
            return "NO";
        }

        public override void Select()
        {
            this.undoEle = this.Copy();
        }

        public override void Draw(Graphics g, int dx, int dy, float zoom)
        {
            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            myBrush.Color = this.Trasparency(Color.Black, 80);

            System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Blue, 1.5f);
            myPen.DashStyle = DashStyle.Dash;

            System.Drawing.Pen whitePen = new System.Drawing.Pen(System.Drawing.Color.White);
                
            g.DrawRectangle(myPen, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);
            
            myPen.Dispose();
            myBrush.Dispose();
        }


    }



    /// <summary>
    /// 
    /// Handle tool for redim/move/rotate shapes
    /// </summary>
    [Serializable]
    public class SelRectBK : Ele
    {
        public SelRectBK(Ele EL)
        {
            this.X = EL.getX();
            this.Y = EL.getY();
            this.X1 = EL.getX1();
            this.Y1 = EL.getY1();
            this.Selected = false;
            this.endMoveRedim();
            this.rot = EL.canRotate();
            this._rotation = EL.getRotation();
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
            Rectangle r;
            r = new Rectangle(this.X - 2, this.Y - 2, 5, 5);
                if (!this.AmIaGroup())
                {
                    //NW
                    if (r.Contains(x, y))
                        return "NW";
                    //SE
                    r.X = this.X1 - 2;
                    r.Y = this.Y1 - 2;
                    if (r.Contains(x, y))
                        return "SE";
                }
                if (!sonoUnaLinea)
                {
                    //N
                    r.X = this.X - 2 + (this.X1 - this.X) / 2;
                    r.Y = this.Y - 2;
                    if (r.Contains(x, y))
                        return "N";
                    if (rot)
                    {
                        //ROT
                        float midX, midY = 0;
                        midX = (this.X1 - this.X) / 2;
                        midY = (this.Y1 - this.Y) / 2;
                        PointF Hp = new PointF(0, -25);
                        PointF RotHP = this.rotatePoint(Hp, this._rotation);
                        midX += RotHP.X;
                        midY += RotHP.Y;

                        r.X = this.X + (int)midX - 2;
                        r.Y = this.Y + (int)midY - 2;
                        if (r.Contains(x, y))
                            return "ROT";
                    }
                    if (!this.AmIaGroup())
                    {
                        //NE
                        r.X = this.X1 - 2;
                        r.Y = this.Y - 2;
                        if (r.Contains(x, y))
                            return "NE";
                    }
                    //E
                    r.X = this.X1 - 2;
                    r.Y = this.Y - 2 + (this.Y1 - this.Y) / 2;
                    if (r.Contains(x, y))
                        return "E";
                    //S
                    r.X = this.X - 2 + (this.X1 - this.X) / 2;
                    r.Y = this.Y1 - 2;
                    if (r.Contains(x, y))
                        return "S";
                    if (!this.AmIaGroup())
                    {

                        //SW
                        r.X = this.X - 2;
                        r.Y = this.Y1 - 2;
                        if (r.Contains(x, y))
                            return "SW";
                    }
                    //W
                    r.X = this.X - 2;
                    r.Y = this.Y - 2 + (this.Y1 - this.Y) / 2;
                    if (r.Contains(x, y))
                        return "W";
                }
            
            if (this.contains(x, y))
                return "C";

            return "NO";

        }

        public override void Select()
        {
            this.undoEle = this.Copy();
        }

        public override void Draw(Graphics g, int dx, int dy, float zoom)
        {
            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            myBrush.Color = this.Trasparency(Color.Black, 90);

            System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Blue, 1.5f);
            myPen.DashStyle = DashStyle.Dash;

            System.Drawing.Pen whitePen = new System.Drawing.Pen(System.Drawing.Color.White);
            if (!this.AmIaGroup())
            {
                //NW
                g.FillRectangle(myBrush, new RectangleF((int)((this.X + dx - 2)) * zoom, (int)((this.Y + dy - 2)) * zoom, (int)5 * zoom, (int)5 * zoom));
                g.DrawRectangle(whitePen, (this.X + dx - 2) * zoom, (this.Y + dy - 2) * zoom, 5 * zoom, 5 * zoom);
                //SE
                g.FillRectangle(myBrush, new RectangleF((this.X1 + dx - 2) * zoom, (this.Y1 + dy - 2) * zoom, (5) * zoom, (5) * zoom));
                g.DrawRectangle(whitePen, (this.X1 + dx - 2) * zoom, (this.Y1 + dy - 2) * zoom, (5) * zoom, (5) * zoom);
            }
            if (!sonoUnaLinea)
            {
                    g.DrawRectangle(myPen, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);
                    //N
                    g.FillRectangle(myBrush, (this.X + dx - 2 + (this.X1 - this.X) / 2) * zoom, (this.Y + dy - 2) * zoom, 5 * zoom, 5 * zoom);
                    g.DrawRectangle(whitePen, (this.X + dx - 2 + (this.X1 - this.X) / 2) * zoom, (this.Y + dy - 2) * zoom, 5 * zoom, 5 * zoom);
                if (!this.AmIaGroup())
                {
                    //NE
                    g.FillRectangle(myBrush, (this.X1 + dx - 2) * zoom, (this.Y + dy - 2) * zoom, 5 * zoom, 5 * zoom);
                    g.DrawRectangle(whitePen, (this.X1 + dx - 2) * zoom, (this.Y + dy - 2) * zoom, 5 * zoom, 5 * zoom);
                }
                    //E
                    g.FillRectangle(myBrush, (this.X1 + dx - 2) * zoom, (this.Y + dy - 2 + (this.Y1 - this.Y) / 2) * zoom, 5 * zoom, 5 * zoom);
                    g.DrawRectangle(whitePen, (this.X1 + dx - 2) * zoom, (this.Y + dy - 2 + (this.Y1 - this.Y) / 2) * zoom, 5 * zoom, 5 * zoom);
                    //S
                    g.FillRectangle(myBrush, (this.X + dx - 2 + (this.X1 - this.X) / 2) * zoom, (this.Y1 + dy - 2) * zoom, 5 * zoom, 5 * zoom);
                    g.DrawRectangle(whitePen, (this.X + dx - 2 + (this.X1 - this.X) / 2) * zoom, (this.Y1 + dy - 2) * zoom, 5 * zoom, 5 * zoom);
                if (!this.AmIaGroup())
                {
                    //SW
                    g.FillRectangle(myBrush, (this.X + dx - 2) * zoom, (this.Y1 + dy - 2) * zoom, 5 * zoom, 5 * zoom);
                    g.DrawRectangle(whitePen, (this.X + dx - 2) * zoom, (this.Y1 + dy - 2) * zoom, 5 * zoom, 5 * zoom);
                }
                    //W
                    g.FillRectangle(myBrush, (this.X + dx - 2) * zoom, (this.Y + dy - 2 + (this.Y1 - this.Y) / 2) * zoom, 5 * zoom, 5 * zoom);
                    g.DrawRectangle(whitePen, (this.X + dx - 2) * zoom, (this.Y + dy - 2 + (this.Y1 - this.Y) / 2) * zoom, 5 * zoom, 5 * zoom);

                    //TEST                
                    if (rot)
                    {
                        //C
                        float midX, midY = 0;
                        midX = (this.X1 - this.X) / 2;
                        midY = (this.Y1 - this.Y) / 2;
                        g.FillEllipse(myBrush, (this.X + midX + dx - 3) * zoom, (this.Y + dy - 3 + midY) * zoom, 6 * zoom, 6 * zoom);
                        g.DrawEllipse(whitePen, (this.X + midX + dx - 3) * zoom, (this.Y + dy - 3 + midY) * zoom, 6 * zoom, 6 * zoom);
                        // ROT
                        PointF Hp = new PointF(0, -25);
                        PointF RotHP = this.rotatePoint(Hp, this._rotation);
                        RotHP.X += midX;
                        RotHP.Y += midY;
                        g.FillRectangle(myBrush, (this.X + RotHP.X + dx - 3) * zoom, (this.Y + dy - 3 + RotHP.Y) * zoom, 6 * zoom, 6 * zoom);
                        g.DrawRectangle(whitePen, (this.X + RotHP.X + dx - 3) * zoom, (this.Y + dy - 3 + RotHP.Y) * zoom, 6 * zoom, 6 * zoom);
                        g.DrawLine(myPen, (this.X + midX + dx) * zoom, (this.Y + midY + dy) * zoom, (this.X + RotHP.X + dx) * zoom, (this.Y + RotHP.Y + dy) * zoom);
                    }
                }
                else
                {
                    g.DrawLine(myPen, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 + dx) * zoom, (this.Y1 + dy) * zoom);
                }

            
           // else
           // {
           //     g.DrawRectangle(myPen, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);
                // }
            myPen.Dispose();
            myBrush.Dispose();
        }


    }

    /// <summary>
    /// Linea ( estende Ele ) 
    /// </summary>
    [Serializable]
    public class Linea : Ele
    {
        private LineCap _starCap;
        private LineCap _endCap;

        public Linea(int x, int y, int x1, int y1)
        {
            this.X = x;
            this.Y = y;
            this.X1 = x1;
            this.Y1 = y1;
            this.sonoUnaLinea = true;
            this.Selected = true;
            this.starCap = LineCap.Custom;
            this.endCap = LineCap.Custom;
            this.endMoveRedim();
            this.rot = false; //can rotate?
        }

        [CategoryAttribute("Line Appearance"), DescriptionAttribute("Line Start Cap")]
        public LineCap starCap
        {
            get
            {
                return _starCap;
            }
            set
            {
                _starCap = value;
            }
        }

        [CategoryAttribute("Line Appearance"), DescriptionAttribute("Line End Cap")]
        public LineCap endCap
        {
            get
            {
                return _endCap;
            }
            set
            {
                _endCap = value;
            }
        }

        [CategoryAttribute("1"), DescriptionAttribute("Line")]
        public string ObjectType
        {
            get
            {
                return "Line";
            }
        }


        public override Ele Copy()
        {
            Linea newE = new Linea(this.X, this.Y, this.X1, this.Y1);
            newE.penColor = this.penColor;
            newE.penWidth = this.penWidth;
            newE.fillColor = this.fillColor;
            newE.filled = this.filled;
            newE.dashstyle = this.dashstyle;
            newE.sonoUnaLinea = this.sonoUnaLinea;
            newE.alpha = this.alpha;
            //
            newE.starCap = this.starCap;
            newE.endCap = this.endCap;

            newE.OnGrpXRes = this.OnGrpXRes;
            newE.OnGrpX1Res = this.OnGrpX1Res;
            newE.OnGrpYRes = this.OnGrpYRes;
            newE.OnGrpY1Res = this.OnGrpY1Res;

            newE.gprZoomX = this.gprZoomX;
            newE.gprZoomY = this.gprZoomY;
            

            return newE;
        }

        public override void CopyFrom(Ele ele)
        {
            this.copyStdProp(ele, this);
            this.endCap = ((Linea)ele).endCap;
            this.starCap = ((Linea)ele).starCap;
        }        

        public override void Select()
        { 
            this.undoEle = this.Copy();        
        }


        public override void AddGp(GraphicsPath gp,int dx , int dy , float zoom)
        {
            gp.AddLine((this.getX() + dx) * zoom, (this.getY() + dy) * zoom, (this.getX1() + dx) * zoom, (this.getY1() + dy) * zoom);
        }

        public override void Draw(Graphics g,int dx,int dy, float zoom)
        {
            System.Drawing.Pen myPen = new System.Drawing.Pen(this.penColor, scaledPenWidth(zoom));
            myPen.DashStyle = this.dashStyle;
            myPen.StartCap = this.starCap;
            myPen.EndCap = this.endCap;

            myPen.Color = this.Trasparency(this.penColor, this.alpha);
            if (this.Selected)
            {
                myPen.Color = Color.Red;
                myPen.Color = this.Trasparency(myPen.Color, 120);
                myPen.Width = myPen.Width + 1;
                g.DrawEllipse(myPen,(this.X + dx) * zoom, (this.Y + dy) * zoom,3,3);
            }

            if (this.X==this.X1 && this.Y==this.Y1)
                g.DrawEllipse(myPen, (this.X + dx) * zoom, (this.Y + dy) * zoom, 3, 3);
            else
                g.DrawLine(myPen, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 + dx) * zoom, (this.Y1 + dy) * zoom );

            myPen.Dispose();
           
        }

    }

    /// <summary>
    /// Arc 
    /// </summary>
    [Serializable]
    public class Arc : Ele
    {
        private int _startAng;
        private int _lenAng;
        private LineCap _starCap;
        private LineCap _endCap;


        public Arc(int x, int y, int x1, int y1)
        {

            this.X = x;
            this.Y = y;
            this.X1 = x1;
            this.Y1 = y1;
            this.Selected = true;
            this.endMoveRedim();
            this.StartAng = 0;
            this.LenAng = 90;
            this.startCap = LineCap.Custom;
            this.endCap = LineCap.Custom;
        }

        [CategoryAttribute("1"), DescriptionAttribute("Rectangle")]
        public string ObjectType
        {
            get
            {
                return "Arc";
            }
        }

        [CategoryAttribute("Line Appearance"), DescriptionAttribute("Line Start Cap")]
        public LineCap startCap
        {
            get
            {
                return _starCap;
            }
            set
            {
                _starCap = value;
            }
        }

        [CategoryAttribute("Line Appearance"), DescriptionAttribute("Line End Cap")]
        public LineCap endCap
        {
            get
            {
                return _endCap;
            }
            set
            {
                _endCap = value;
            }
        }


        [DescriptionAttribute("Start angle")]
        public int StartAng
        {
            get
            {
                return _startAng;
            }
            set
            {
                _startAng = value;
            }
        }

        [DescriptionAttribute("Angle length")]
        public int LenAng
        {
            get
            {
                return _lenAng;
            }
            set
            {
                _lenAng = value;
            }
        }


        public override Ele Copy()
        {
            Arc newE = new Arc(this.X, this.Y, this.X1, this.Y1);
            newE.penColor = this.penColor;
            newE.penWidth = this.penWidth;
            newE.fillColor = this.fillColor;
            newE.filled = this.filled;
            newE.dashStyle = this.dashStyle;
            newE.alpha = this.alpha;
            newE.sonoUnaLinea = this.sonoUnaLinea;
            newE.StartAng = this.StartAng;
            newE.LenAng = this.LenAng;
            newE.showBorder = this.showBorder;
            newE.endCap = this.endCap;
            newE.startCap = this.startCap;

            newE.OnGrpXRes = this.OnGrpXRes;
            newE.OnGrpX1Res = this.OnGrpX1Res;
            newE.OnGrpYRes = this.OnGrpYRes;
            newE.OnGrpY1Res = this.OnGrpY1Res;

            newE.copyGradprop(this);

            return newE;
        }


        public override void CopyFrom(Ele ele)
        {
            this.copyStdProp(ele, this);
            this.StartAng = ((Arc)ele).StartAng;
            this.LenAng = ((Arc)ele).LenAng;
            this.startCap = ((Arc)ele).startCap;
            this.endCap = ((Arc)ele).endCap;
        }


        public override void Select()
        {
            this.undoEle = this.Copy();
        }


        public override void AddGp(GraphicsPath gp, int dx, int dy, float zoom)
        {
            gp.AddArc((this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom, this.StartAng, this.LenAng);
        }


        public override void Draw(Graphics g,int dx,int dy, float zoom)
        {

            //System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(this.fillColor);
            Brush myBrush = getBrush(dx, dy, zoom);
            //myBrush.Color = this.Trasparency(this.fillColor, this.alpha);
            System.Drawing.Pen myPen = new System.Drawing.Pen(this.penColor, scaledPenWidth(zoom));
            myPen.DashStyle = this.dashStyle;
            myPen.EndCap = this.endCap;
            myPen.StartCap = this.startCap;

            //myBrush.Color = this.Trasparency(this.fillColor, this.alpha);

            if (this.Selected)
            {
                System.Drawing.Pen myPen1 = new System.Drawing.Pen(this.penColor, scaledPenWidth(zoom));
                myPen1.Width = 0.5f;
                myPen1.DashStyle = DashStyle.Dot;
                g.DrawEllipse(myPen1, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);
                myPen1.Dispose();
                //myBrush.Color = this.dark(this.fillColor, 5, this.alpha);
                myPen.Color = Color.Red;
                myPen.Color = this.Trasparency(myPen.Color, 120);
                myPen.Width = myPen.Width + 1;
            }

            // Create a path and add the object.
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddArc((this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom, this.StartAng, this.LenAng);
            //Matrix translateMatrix = new Matrix();
            //translateMatrix.RotateAt(this.Rotation, new Point(this.X + (int)(this.X1 - this.X) / 2, this.Y + (int)(this.Y1 - this.Y) / 2));
            //myPath.Transform(translateMatrix);

            // Draw the transformed ellipse to the screen.
            if (this.filled)
            {
                g.FillPath(myBrush, myPath);
                if (this.showBorder)
                    g.DrawPath(myPen, myPath);
            }
            else
                g.DrawPath(myPen, myPath);

            myPath.Dispose();
            myPen.Dispose();
            if (myBrush != null)
                myBrush.Dispose();
        }
    }


  

    /// <summary>
    /// Group ( extends Ele ) 
    /// </summary>
    [Serializable]
    public class Group : Ele
    {
        // sub objects contained in group
        ArrayList objs;

        // Manage the gropu likr a Graphic path 
        bool _grapPath = false;

        // the name of the group
        string _name = "";
        public static int Ngrp; // used to generate names

        public Group(ArrayList a)
        {
            this.IamGroup = true;
            objs = a;

            int minX=+32000;
            int maxX=-32000;
            int minY=+32000;
            int maxY=-32000;

            foreach (Ele e in objs)
            {
                if (e.getX() < minX)
                    minX = e.getX();
                if (e.getY() < minY)
                    minY = e.getY();
                if (e.getX1() > maxX)
                    maxX = e.getX1();
                if (e.getY1() > maxY)
                    maxY = e.getY1();
                e.Selected = false;
            }

            this.X = minX;
            this.Y = minY;
            this.X1 = maxX;
            this.Y1 = maxY;
            this.Selected = true;
            this.endMoveRedim();
            this.Rotation = 0;
            this.rot = true; //can rotate?
            this.Name = "Itm" + Ngrp.ToString();
            Ngrp++;
        }

        #region OVERRIDEN PROPERIES

        #region GET/SET

        [CategoryAttribute("GroupZoom"), DescriptionAttribute("X Scale Zoom ")]
        public float GrpZoomX
        {
            get
            {
                return this.gprZoomX;
            }
            set
            {
                if (value >0)
                  this.gprZoomX = value; 
            }
        }

        [CategoryAttribute("GroupZoom"), DescriptionAttribute("Y Scale Zoom ")]
        public float GrpZoomY
        {
            get
            {
                return this.gprZoomY;
            }
            set
            {
                if (value > 0)
                    this.gprZoomY = value;
            }
        }



        [CategoryAttribute("GradientBrush"), DescriptionAttribute("True: use gradient fill color")]
        public override bool UseGradientLineColor
        {
            get
            {
                return base.UseGradientLineColor;
            }
            set
            {
                base.UseGradientLineColor = value;
                if (this.objs != null)
                    foreach (Ele e in this.objs)
                    {
                        e.UseGradientLineColor = value;
                    }
            }
        }

        [CategoryAttribute("GradientBrush"), DescriptionAttribute("End Color Position [0..1])")]
        public override float EndColorPosition
        {
            get
            {
                return base.EndColorPosition;
            }
            set
            {
                base.EndColorPosition = value;
                if (this.objs != null)
                    foreach (Ele e in this.objs)
                    {
                        e.EndColorPosition = value;
                    }
            }
        }


        [CategoryAttribute("GradientBrush"), DescriptionAttribute("Gradient End Color")]
        public override Color EndColor
        {
            get
            {
                return base.EndColor;
            }
            set
            {
                base.EndColor = value;
                if (this.objs != null)
                    foreach (Ele e in this.objs)
                    {
                        e.EndColor = value;
                    }
            }
        }


        [CategoryAttribute("GradientBrush"), DescriptionAttribute("End Color Trasparency")]
        public override int EndAlpha
        {
            get
            {
                return base.EndAlpha;
            }
            set
            {
                base.EndAlpha = value;
                if (this.objs != null)
                    foreach (Ele e in this.objs)
                    {
                        e.EndAlpha = value;
                    }
            }
        }

        [CategoryAttribute("GradientBrush"), DescriptionAttribute("Gradient Dimension")]
        public override int GradientLen
        {
            get
            {
                return base.GradientLen;
            }
            set
            {
                base.GradientLen = value;
                if (this.objs != null)
                    foreach (Ele e in this.objs)
                    {
                        e.GradientLen = value;
                    }
            }
        }

        [CategoryAttribute("GradientBrush"), DescriptionAttribute("Gradient Angle")]
        public override int GradientAngle
        {
            get
            {
                return base.GradientAngle;
            }
            set
            {
                base.GradientAngle = value;
                if (this.objs != null)
                    foreach (Ele e in this.objs)
                    {
                        e.GradientAngle = value;
                    }
            }
        }

        public override int alpha
        {
            get
            {
                return base.alpha;
            }
            set
            {
                base.alpha = value;
                if (this.objs!=null)
                    foreach (Ele e in this.objs)
                    {
                        e.alpha = value;
                    }
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != "")
                    _name = value;
            }
        }


        [DescriptionAttribute("Manege group as a graphic path.")]
        public  bool graphPath
        {
            get
            {
                return _grapPath;
            }
            set
            {
                
                _grapPath = value;
            }
        }


        [DescriptionAttribute("Rotation Angle.")]
        public int Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }

        public override Color fillColor
        {
            get
            {
                return base.fillColor;
            }
            set
            {
                base.fillColor = value;
                if (this.objs != null)
                    foreach (Ele e in this.objs)
                    {
                        e.fillColor = value;
                    }
            }
        }

        public override bool filled
        {
            get
            {
                return base.filled;
            }
            set
            {
                base.filled = value;
                if (this.objs != null)
                    foreach (Ele e in this.objs)
                    {
                        e.filled = value;
                    }
            }
        }

        public override Color penColor
        {
            get
            {
                return base.penColor;
            }
            set
            {
                base.penColor = value;
                if (this.objs != null)
                    foreach (Ele e in this.objs)
                    {
                        e.penColor = value;
                    }
            }
        }

        public override float penWidth
        {
            get
            {
                return base.penWidth;
            }
            set
            {
                base.penWidth = value;
                if (this.objs != null)
                    foreach (Ele e in this.objs)
                    {
                        e.penWidth = value;
                    }
            }
        }

        public override bool showBorder
        {
            get
            {
                return base.showBorder;
            }
            set
            {
                base.showBorder = value;
                if (this.objs != null)
                    foreach (Ele e in this.objs)
                    {
                        e.showBorder = value;
                    }
            }
        }

        public override DashStyle dashStyle
        {
            get
            {
                return base.dashStyle;
            }
            set
            {
                base.dashStyle = value;
                if (this.objs != null)
                    foreach (Ele e in this.objs)
                    {
                        e.dashStyle = value;
                    }
            }
        }
        #endregion

        #endregion

        public override void AfterLoad()
        {
            base.AfterLoad();
            foreach (Ele e in objs)
            {
                e.AfterLoad();
            }
        }

        public override void  endMoveRedim()
        {
 	        base.endMoveRedim();
            foreach (Ele e in objs)
            {
                e.endMoveRedim();
            }
        }

        public override ArrayList deGroup()
        {
            return this.objs;
        }

        public override void move(int x, int y)
        {
            foreach (Ele e in objs)
            {
                e.move(x, y);
            }
            this.X = this.startX - x;
            this.Y = this.startY - y;
            this.X1 = this.startX1 - x;
            this.Y1 = this.startY1 - y;
        }

        [CategoryAttribute("1"), DescriptionAttribute("GroupObj")]
        public string ObjectType
        {
            get
            {
                return "Group";
            }
        }

//         public override void ShowEditor(richForm2 f)
//         {
//             foreach (Ele e in this.objs)
//             {
//                 if (e.OnGrpDClick)
//                 {
//                     e.ShowEditor(f);
//                 }
//             }
//         }

        public void Load_IMG()
        {
            foreach (Ele e in this.objs)
            {
                if (e.OnGrpDClick)
                {
                    if (e is ImgBox)
                    {
                        ((ImgBox)e).Load_IMG();
                    }
                    if (e is Group)
                    {
                        ((Group)e).Load_IMG();
                    }
                }
            }            
        }

        public void setZoom(int x, int y)
        {
            float dx = (this.X1-x)*2;
            float dy = (this.Y1-y)*2;

            this.GrpZoomX = (this.Width - dx) / this.Width;
            this.GrpZoomY = (this.Height - dy) / this.Height;            
        }

        public override Ele Copy()
        {            
            //Copy chils
            ArrayList l1 = new ArrayList();
            foreach (Ele e in this.objs)
            {
                Ele e1 = e.Copy();
                l1.Add(e1);
            }

            Group newE = new Group(l1);
            /*
            newE.penColor = this.penColor;
            newE.penWidth = this.penWidth;
            newE.fillColor = this.fillColor;
            newE.filled = this.filled;
            newE.dashStyle = this.dashStyle;
            newE.alpha = this.alpha;
            newE.sonoUnaLinea = this.sonoUnaLinea;
            //newE.Rotation = this.Rotation;
            newE.showBorder = this.showBorder;
            */
            newE.Rotation = this.Rotation;
            newE._grapPath = this._grapPath;
            newE.gprZoomX = this.gprZoomX;
            newE.gprZoomY = this.gprZoomY;
            newE.IamGroup = this.IamGroup;
            newE._name = this.Name + "_" + Group.Ngrp.ToString();
            newE.OnGrpDClick = this.OnGrpDClick;
            newE.OnGrpXRes = this.OnGrpXRes;
            newE.OnGrpX1Res = this.OnGrpX1Res;
            newE.OnGrpYRes = this.OnGrpYRes;
            newE.OnGrpY1Res = this.OnGrpY1Res;


            if (newE._grapPath)
            {
                newE.penColor = this.penColor;
                newE.penWidth = this.penWidth;
                newE.fillColor = this.fillColor;
                newE.filled = this.filled;
                newE.dashStyle = this.dashStyle;
                newE.alpha = this.alpha;
                newE.sonoUnaLinea = this.sonoUnaLinea;
                newE.Rotation = this.Rotation;
                newE.showBorder = this.showBorder;

                newE.UseGradientLineColor = this.UseGradientLineColor;
                newE.GradientAngle =this.GradientAngle;
                newE.GradientLen = this.GradientLen;
                newE.EndAlpha = this.EndAlpha;
                newE.EndColor = this.EndColor;
                newE.EndColorPosition = this.EndColorPosition;

            }

            return newE;
            
        }

        public override void CopyFrom(Ele ele)
        {
            this.copyStdProp(ele, this);
            //
            //this._grapPath = ele._grapPath;
        }

        public override void Select()
        {
            this.undoEle = this.Copy();
        }

        public override void redim(int x, int y, string redimSt)
        {            
            foreach (Ele e in objs)
            {
                switch (redimSt)
                {
                    case "N":
                        base.redim(x, y, redimSt);
                        if (e.OnGrpYRes != OnGroupResize.Nothing)
                        {
                            if (e.OnGrpYRes == OnGroupResize.Move)
                            {
                                e.move(0, -y);
                            }
                            else
                            {
                                e.redim(0, y, redimSt);
                            }
                        }
                        break;
                    case "E":
                        base.redim(x, y, redimSt);
                        if (e.OnGrpX1Res != OnGroupResize.Nothing)
                        {
                            if (e.OnGrpX1Res == OnGroupResize.Move)
                            {
                                e.move(-x, 0);
                            }
                            else
                            {
                                e.redim(x, 0, redimSt);
                            }
                        }
                        break;
                    case "S":
                        base.redim(x, y, redimSt);
                        if (e.OnGrpY1Res != OnGroupResize.Nothing)
                        {
                            if (e.OnGrpY1Res == OnGroupResize.Move)
                            {
                                e.move(0, -y);
                            }
                            else
                            {
                                e.redim(0, y, redimSt);
                            }
                        }
                        break;
                    case "W":
                        base.redim(x, y, redimSt);
                        if (e.OnGrpXRes != OnGroupResize.Nothing)
                        {
                            if (e.OnGrpXRes == OnGroupResize.Move)
                            {
                                e.move(-x, 0);
                            }
                            else
                            {
                                e.redim(x, 0, redimSt);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }



        public override void AddGp(GraphicsPath gp, int dx, int dy, float zoom)
        {
            foreach (Ele e in this.objs)
            {
                e.AddGp(gp, dx, dy, zoom);
            }            
        }


        public override void Draw(Graphics g, int dx, int dy, float zoom)
        {
            Matrix precMx = g.Transform.Clone();//store previos trasformation
            Matrix mx = g.Transform; // get previous trasformation
            PointF p = new PointF(zoom * (this.X + dx + (this.X1 - this.X) / 2), zoom * (this.Y + dy + (this.Y1 - this.Y) / 2));
            if (this.Rotation>0)
                mx.RotateAt(this.Rotation, p, MatrixOrder.Append); //add a trasformation

            if (this.GrpZoomX > 0 && this.GrpZoomY > 0)
            {
                mx.Translate((-1) * zoom * (this.X + dx + (this.X1 - this.X) / 2), (-1) * zoom * (this.Y + dy + (this.Y1 - this.Y) / 2), MatrixOrder.Append);
                mx.Scale(this.GrpZoomX, this.GrpZoomY, MatrixOrder.Append);
                mx.Translate(zoom * (this.X + dx + (this.X1 - this.X) / 2), zoom * (this.Y + dy + (this.Y1 - this.Y) / 2), MatrixOrder.Append);
            }
            g.Transform = mx;

            //g.ResetTransform();
            //The next drawn objs are translated over origin , rotated and then traslated again.
            //g.TranslateTransform((-1) * zoom * (this.X + dx + (this.X1 - this.X) / 2), (-1) * zoom * (this.Y + dy + (this.Y1 - this.Y) / 2), MatrixOrder.Append);
            //g.RotateTransform(this.Rotation, MatrixOrder.Append);
            //g.TranslateTransform(zoom * (this.X + dx + (this.X1 - this.X) / 2), zoom * (this.Y + dy + (this.Y1 - this.Y) / 2), MatrixOrder.Append);
            
            if (this._grapPath)
            #region path
            {
                //System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(this.fillColor);
                Brush myBrush = getBrush(dx, dy, zoom);
                //myBrush.Color = this.Trasparency(this.fillColor, this.alpha);

                System.Drawing.Pen myPen = new System.Drawing.Pen(this.penColor, scaledPenWidth(zoom));
                myPen.DashStyle = this.dashStyle;
                
                if (this.Selected)
                {
                    //myBrush.Color = this.dark(this.fillColor, 5, this.alpha);
                    myPen.Color = Color.Red;
                    myPen.Color = this.Trasparency(myPen.Color, 120);
                    myPen.Width = myPen.Width + 1;
                }

                GraphicsPath gp = new GraphicsPath();
                foreach (Ele e in objs)
                {
                    /*
                    if (e.sonoUnaLinea)
                    {
                        gp.AddLine((e.getX() + dx) * zoom, (e.getY() + dy) * zoom, (e.getX1() + dx) * zoom, (e.getY1() + dy) * zoom);
                    }
                    else
                    {
                        if (e is Arc)
                        {
                            Arc a = (Arc)e;
                            gp.AddArc((a.getX() + dx) * zoom, (a.getY() + dy) * zoom, (getX1() - a.getX()) * zoom, (a.getY1() - a.getY()) * zoom, a.StartAng, a.LenAng);
                        }
                        //else
                        //{ 
                        //    gp.AddString("Only Arcs and Lines in "
                        //}
                    }
                    */
                    e.AddGp(gp, dx, dy, zoom);
                }
                if (this.filled)
                {
                    g.FillPath(myBrush, gp);
                    if (this.showBorder)
                        g.DrawPath(myPen, gp);
                }
                else
                {
                    g.DrawPath(myPen, gp);
                }
                if (myBrush != null)
                    myBrush.Dispose();
                myPen.Dispose();
            }
#endregion
            else
            {
                foreach (Ele e in objs)
                {
                    /*
                    Matrix precMx = g.Transform.Clone();//store previos trasformation
                    Matrix mx = g.Transform; // get previous trasformation
                    PointF p = new PointF(zoom * (this.X + dx + (this.X1 - this.X) / 2), zoom * (this.Y + dy + (this.Y1 - this.Y) / 2));
                    mx.RotateAt(this.Rotation, p, MatrixOrder.Append); //add a trasformation
                    g.Transform = mx;*/
            
                    e.Draw(g, dx, dy, zoom);

                    /*
                    g.Transform = precMx; //restor previos trasformation
                    */
                }
            }
            // reset the trasformation added above
            //g.ResetTransform();
            g.Transform = precMx; //restore previos trasformation

            if (this.Selected)
            {
                //System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(this.fillColor);
                Brush myBrush = getBrush(dx, dy, zoom);
                //myBrush.Color = this.Trasparency(this.fillColor, this.alpha);

                System.Drawing.Pen myPen = new System.Drawing.Pen(this.penColor, scaledPenWidth(zoom));
                myPen.DashStyle = this.dashStyle;

                //myBrush.Color = this.dark(this.fillColor, 5, this.alpha);
                myPen.Color = Color.Red;
                myPen.Color = this.Trasparency(myPen.Color, 120);
                myPen.Width = myPen.Width + 1;
                g.DrawRectangle(myPen, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);
                if (myBrush != null)
                    myBrush.Dispose();
                myPen.Dispose();

            }

            mx.Dispose();
            precMx.Dispose();

        }
    }

    /// <summary>
    /// Rectangle ( extends Ele ) 
    /// </summary>
 			[Serializable]
    public class Rect : Ele 
    {
        //private int _rotation;

        public Rect(int x, int y,int x1, int y1)
        {
            
            this.X = x;
            this.Y = y;
            this.X1 = x1;
            this.Y1 = y1;
            this.Selected = true;
            this.endMoveRedim();
            this.Rotation = 0;
            this.rot = true; //can rotate?
        }

        [CategoryAttribute("1"), DescriptionAttribute("Rectangle")]
        public string ObjectType
        {
            get
            {
                return "Rectangle";
            }
        }

        [ DescriptionAttribute("Rotation angle")]
        public int Rotation
        {
            get
            {
                return _rotation;
            }
            set 
            {
                _rotation = value;            
            }
        }

        public override Ele Copy()
        {
            Rect newE = new Rect(this.X, this.Y, this.X1, this.Y1);
            
            newE.penColor = this.penColor;
            newE.penWidth = this.penWidth;
            newE.fillColor = this.fillColor;
            newE.filled = this.filled;
            newE.dashStyle = this.dashStyle;
            newE.alpha = this.alpha;
            newE.sonoUnaLinea = this.sonoUnaLinea;
            newE.Rotation = this.Rotation;            
            newE.showBorder = this.showBorder;

            newE.OnGrpXRes = this.OnGrpXRes;
            newE.OnGrpX1Res = this.OnGrpX1Res;
            newE.OnGrpYRes = this.OnGrpYRes;
            newE.OnGrpY1Res = this.OnGrpY1Res;

            newE.copyGradprop(this);

            return newE;
        }


        public override void CopyFrom(Ele ele)
        {
            this.copyStdProp(ele, this);
            this.Rotation = ((Rect)ele).Rotation;
        }        


        public override void Select()
        {
            this.undoEle = this.Copy();        
        }

        public override void AddGp(GraphicsPath gp, int dx, int dy, float zoom)
        {
            gp.AddRectangle(new RectangleF((this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom));
        }


        public override void Draw(Graphics g,int dx , int dy, float zoom)
        {

            //System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(this.fillColor);
            Brush myBrush = getBrush(dx,dy,zoom);
            //myBrush.Color = this.Trasparency(this.fillColor, this.alpha);
            System.Drawing.Pen myPen = new System.Drawing.Pen(this.penColor, scaledPenWidth(zoom));
            myPen.DashStyle = this.dashStyle;
                     
            if (this.Selected)
            {
                //myBrush.Color = this.dark(this.fillColor, 5,this.alpha);
                myPen.Color = Color.Red;
                myPen.Color = this.Trasparency(myPen.Color, 120);
                myPen.Width = myPen.Width + 1;

            }

            // Create a path and add the object.
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddRectangle( new RectangleF((this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom));
            Matrix translateMatrix = new Matrix();
            translateMatrix.RotateAt(this.Rotation, new PointF((this.X + dx + (int)(this.X1 - this.X) / 2) * zoom, (this.Y + dy + (int)(this.Y1 - this.Y) / 2) * zoom));
            myPath.Transform(translateMatrix);

            // Draw the transformed ellipse to the screen.
            if (this.filled)
            {
                g.FillPath(myBrush, myPath);
                //g.FillPath(br, myPath);
                if (this.showBorder)
                    g.DrawPath(myPen, myPath);
            }
            else
                g.DrawPath(myPen, myPath);

            myPath.Dispose();                        
            myPen.Dispose();
            if (myBrush!=null)
                myBrush.Dispose();
        }
    }

    /// <summary>
    /// Box Immagine ( estende Ele ) 
    /// </summary>
  		[Serializable]
    public class ImgBox : Ele
    {
        private Bitmap _img;
        //private int _rotation;
        private bool _trasparent = false;

        public ImgBox(int x, int y, int x1, int y1)
        {

            this.X = x;
            this.Y = y;
            this.X1 = x1;
            this.Y1 = y1;
            this.Selected = true;
            this.endMoveRedim();
            this.rot = true; //can rotate
        }
        
        [CategoryAttribute("Image"), DescriptionAttribute("File image")]            
        public Bitmap img
        {
            get
            {
                return _img;
            }
            set
            {
                _img = value;
            }
        }

        [CategoryAttribute("Image"), DescriptionAttribute("Trasparent")]
            public bool Trasparent
        {
            get
            {
                return _trasparent;
            }
            set
            {
                _trasparent = value;
            }
        }

        [DescriptionAttribute("Rotation angle")]
        public int Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }

        [CategoryAttribute("1"), DescriptionAttribute("Image Box")]
        public string ObjectType
        {
            get
            {
                return "Image Box";
            }
        }


        public override Ele Copy()
        {
            ImgBox newE = new ImgBox(this.X,this.Y,this.X1,this.Y1);
            newE.penColor = this.penColor;
            newE.penWidth = this.penWidth;
            newE.fillColor = this.fillColor;
            newE.filled = this.filled;
            newE.sonoUnaLinea = this.sonoUnaLinea;
            newE.alpha = this.alpha;
            newE.dashStyle = this.dashStyle;
            newE.showBorder = this.showBorder;
            newE.Trasparent = this.Trasparent;
            newE.Rotation = this.Rotation;

            newE.OnGrpXRes = this.OnGrpXRes;
            newE.OnGrpX1Res = this.OnGrpX1Res;
            newE.OnGrpYRes = this.OnGrpYRes;
            newE.OnGrpY1Res = this.OnGrpY1Res;


            newE.img = this.img;

            newE.copyGradprop(this);

            return newE;
        }

        public override void CopyFrom(Ele ele)
        {
            this.copyStdProp(ele, this);
            this.img = ((ImgBox)ele).img;
        }        

        public override void Select()
        {
            this.undoEle = this.Copy();        
        }

        public void Load_IMG()
        {
            string f_name = this.imgLoader();
                if (f_name != null)
                {
                    try
                    {
                        Bitmap loadTexture = new Bitmap(f_name);
                        this.img = loadTexture;
                    }
                    catch { }
               }
        }

        private string imgLoader()
        {
            try
            {
                OpenFileDialog DialogueCharger = new OpenFileDialog();
                DialogueCharger.Title = "Load background image";
                DialogueCharger.Filter = "jpg files (*.jpg)|*.jpg|bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";
                //DialogueCharger.DefaultExt = "frame";
                if (DialogueCharger.ShowDialog() == DialogResult.OK)
                {
                    return (DialogueCharger.FileName);
                }
            }
            catch { }
            return null;
        }



        public override void Draw(Graphics g,int dx , int dy, float zoom)
        {            
            System.Drawing.Pen myPen = new System.Drawing.Pen(this.penColor, scaledPenWidth(zoom));
            myPen.DashStyle = this.dashStyle;

            if (this.Selected)
            {
                myPen.Color = Color.Red;
                myPen.Color = this.Trasparency(myPen.Color, 120);
                myPen.Width = myPen.Width + 1;
            }

            if (img != null)
            {
                Color backColor = this.img.GetPixel(0, 0); //get the back color from the first pixel (UP-LEFT)
                //Create a tmp Bitmap and a graphic object
                // the dimension of the tmp bitmap must permit the rotation of img
                int dim = (int)System.Math.Sqrt(img.Width * img.Width + img.Height * img.Height);
                Bitmap curBitmap = new Bitmap(dim, dim);
                Graphics curG = Graphics.FromImage(curBitmap);
                
                if (this.Rotation > 0)
                {
                    // activate the rotation on the graphic obj
                    Matrix X = new Matrix();
                    X.RotateAt(this.Rotation, new PointF(curBitmap.Width / 2, curBitmap.Height / 2));
                    curG.Transform = X;                                        
                    X.Dispose();
                }
                // I draw img over the tmp bitmap 
                curG.DrawImage(img, (dim - img.Width) / 2, (dim - img.Height) / 2, img.Width, img.Height);
                
                if (this.Trasparent)
                    curBitmap.MakeTransparent(backColor); // here I perform trasparency

                curG.Save();
                // I draw the tmp bitmap on canvas
                g.DrawImage(curBitmap, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);
                    
                curG.Dispose();
                curBitmap.Dispose();

            }

            if (this.showBorder)
                g.DrawRectangle(myPen, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);
            
            myPen.Dispose();
            
        }
    }



    /// <summary>
    /// Text Box ( estende Ele ) 
    /// </summary>
    [Serializable]
    public class BoxTesto : Ele
    {
        public string rtf; // save rtf text

        // placed here to not overload draw metod
       // [NonSerialized()] 
       // private RichTextBoxPrintCtrl.RichTextBoxPrintCtrl tmpR = new RichTextBoxPrintCtrl.RichTextBoxPrintCtrl();

        public BoxTesto(int x, int y, int x1, int y1)
        {

            this.X = x;
            this.Y = y;
            this.X1 = x1;
            this.Y1 = y1;
            this.Selected = true;
            this.endMoveRedim();
            rtf = "";
            //tBox = new TxtBox();
        }

        public override  void AfterLoad()
        { 
//         // tmpR is not serialized, I must recreate after Load
//         if (this.tmpR == null)
//              tmpR = new RichTextBoxPrintCtrl.RichTextBoxPrintCtrl();
        }

        [CategoryAttribute("1"), DescriptionAttribute("Text Rectangle")]
        public string ObjectType
        {
            get
            {
                return "Text Rectangle";
            }
        }


        public override Ele Copy()
        {
            BoxTesto newE = new BoxTesto(this.X, this.Y, this.X1, this.Y1);
            newE.penColor = this.penColor;
            newE.penWidth = this.penWidth;
            newE.fillColor = this.fillColor;
            newE.filled = this.filled;
            newE.sonoUnaLinea = this.sonoUnaLinea;
            newE.alpha = this.alpha;
            newE.dashStyle = this.dashStyle;
            newE.showBorder = this.showBorder;

            newE.OnGrpXRes = this.OnGrpXRes;
            newE.OnGrpX1Res = this.OnGrpX1Res;
            newE.OnGrpYRes = this.OnGrpYRes;
            newE.OnGrpY1Res = this.OnGrpY1Res;

            newE.copyGradprop(this);

            newE.rtf = this.rtf;

            return newE;
        }


        public override void CopyFrom(Ele ele)
        {
            this.copyStdProp(ele, this);
            this.rtf = ((BoxTesto)ele).rtf;
        }   

        public override void Select()
        {
            this.undoEle = this.Copy();
        }

        public override void Select(RichTextBox r)
        {
             r.Rtf=rtf;    
        }

//         public override void ShowEditor(richForm2 f)
//         {
//             f.richTextBox1.Rtf = rtf;
//             f.ShowDialog();
//             if (f.confermato)
//                 this.rtf = f.richTextBox1.Rtf;
// 
//         }

        public override void Draw(Graphics g,int dx , int dy, float zoom)
        {
            //System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(this.fillColor);
            Brush myBrush = getBrush(dx, dy, zoom);
            //myBrush.Color = this.Trasparency(this.fillColor, this.alpha);
            System.Drawing.Pen myPen = new System.Drawing.Pen(this.penColor, scaledPenWidth(zoom));
            myPen.DashStyle = this.dashStyle;
            if (this.Selected)
            {
                //myBrush.Color = this.dark(this.fillColor, 5, this.alpha);
                myPen.Color = Color.Red;
                myPen.Color = this.Trasparency(myPen.Color, 120);
                myPen.Width = myPen.Width + 1;
            }

            if (this.filled)
            {
                g.FillRectangle(myBrush, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);
            }
            if (this.showBorder || this.Selected)
                g.DrawRectangle(myPen, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);

//             tmpR.BorderStyle = BorderStyle.None;
//             tmpR.ScrollBars = RichTextBoxScrollBars.None;
// 
//             tmpR.Rtf = rtf;

            /*
            // TO ENABLE TEST ROTATION / ZOOM
            //TEST START
                Bitmap curBitmap = new Bitmap((int)(this.Width * zoom),(int)( this.Height * zoom));
                curBitmap.SetResolution(this.Width , this.Height );
                Graphics curG = Graphics.FromImage(curBitmap);
                curG.PageUnit = GraphicsUnit.Point;
                if (this._rotation > 0)
                {
                    // activate the rotation on the graphic obj
                    Matrix X = new Matrix();
                    X.RotateAt(this._rotation, new PointF(curBitmap.Width / 2, curBitmap.Height / 2));
                    curG.Transform = X;
                    X.Dispose();
                }
                // I draw img over the tmp bitmap 
                if (curG.DpiX < 600)
                {
                    //tmpR.Draw(0, tmpR.TextLength, curG, (int)((this.X + dx) * zoom), (int)((this.Y + dy) * zoom), (int)((dx + this.X1 - (int)((this.X1 - this.X) * .08)) * zoom), (int)((dy + this.Y1 - (int)((this.Y1 - this.Y) * .08)) * zoom), 15);
                    tmpR.Draw(0, tmpR.TextLength, curG, 0, 0, (int)((this.Width * 1) * zoom), (int)(( this.Height * 1) * zoom), 15);
                }
                else
                {
                    //tmpR.Draw(0, tmpR.TextLength, curG, (int)((this.X + dx) * zoom), (int)((this.Y + dy) * zoom), (int)((this.X1 + dx) * zoom), (int)((this.Y1 + dy) * zoom), 14.4);
                    tmpR.Draw(0, tmpR.TextLength, curG, 0, 0, (int)((this.Width * 1) * zoom), (int)((this.Width * 1) * zoom), 14.4);
                }


                //if (this._tra .Trasparent)
                    //curBitmap.MakeTransparent(backColor); // here I perform trasparency

                curG.Save();
                // I draw the tmp bitmap on canvas
                g.DrawImage(curBitmap, (this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);

                curG.Dispose();
                curBitmap.Dispose();

            // END
            */

//             if (g.DpiX < 600)
//             {
//                 tmpR.Draw(0, tmpR.TextLength, g, (int)((this.X + dx) * zoom), (int)((this.Y + dy) * zoom), (int)((dx + this.X1 - (int)((this.X1 - this.X) * .08)) * zoom),(int)( (dy + this.Y1 - (int)((this.Y1 - this.Y) * .08)) * zoom), 15);
//             }
//             else
//                 tmpR.Draw(0, tmpR.TextLength, g, (int)((this.X + dx) * zoom), (int)((this.Y + dy) * zoom),(int)( (this.X1 + dx) * zoom), (int)((this.Y1 + dy) * zoom), 14.4);


            //tmpR.Dispose();
            myPen.Dispose();
            if (myBrush != null)
                myBrush.Dispose();
        }

    }

    /// <summary>
    /// Ellisse ( estende Ele ) 
    /// </summary>
    [Serializable]
    public class Ell : Ele
    {
        //int _rotation;
        public Ell(int x, int y, int x1, int y1)
        {

            this.X = x;
            this.Y = y;
            this.X1 = x1;
            this.Y1 = y1;
            this.Selected = true;
            this.endMoveRedim();
            this.Rotation = 0;
            this.rot = true; //can rotate
        }

        public override Ele Copy()
        {
            Ell newE = new Ell(this.X, this.Y, this.X1, this.Y1);
            newE.penColor = this.penColor;
            newE.penWidth = this.penWidth;
            newE.fillColor = this.fillColor;
            newE.filled = this.filled;
            newE.sonoUnaLinea = this.sonoUnaLinea;
            newE.alpha = this.alpha;
            newE.dashStyle = this.dashStyle;
            newE.showBorder = this.showBorder;
            newE.Rotation = this.Rotation;

            newE.OnGrpXRes = this.OnGrpXRes;
            newE.OnGrpX1Res = this.OnGrpX1Res;
            newE.OnGrpYRes = this.OnGrpYRes;
            newE.OnGrpY1Res = this.OnGrpY1Res;
            
            newE.copyGradprop(this);

            return newE;
        }

        public override void CopyFrom(Ele ele)
        {
            this.copyStdProp(ele, this);
            this.Rotation = ((Ell)ele).Rotation;
        }   


        [DescriptionAttribute("Rotation angle")]
        public int Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }


        [CategoryAttribute("1"), DescriptionAttribute("Ellipse")]
        public string ObjectType
        {
            get
            {
                return "Ellipse";
            }
        }


        public override void Select()
        {
            this.undoEle = this.Copy();        
        
        }

        public override void AddGp(GraphicsPath gp, int dx, int dy, float zoom)
        {
            gp.AddEllipse((this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);
        }


        public override void Draw(Graphics g, int dx, int dy, float zoom)
        {
            //System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(this.fillColor);
            Brush myBrush = getBrush(dx, dy, zoom);
            //myBrush.Color = this.Trasparency(this.fillColor, this.alpha);
            System.Drawing.Pen myPen = new System.Drawing.Pen(this.penColor, scaledPenWidth(zoom));
            myPen.DashStyle = this.dashStyle;
            if (this.Selected)
            {
                //myBrush.Color = this.dark(this.fillColor, 5, this.alpha);
                myPen.Color = Color.Red;
                myPen.Color = this.Trasparency(myPen.Color, 120);
                myPen.Width = myPen.Width + 1;

            }

            // Create a path and add the object.
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddEllipse((this.X + dx) * zoom, (this.Y + dy) * zoom, (this.X1 - this.X) * zoom, (this.Y1 - this.Y) * zoom);
            Matrix translateMatrix = new Matrix();
            translateMatrix.RotateAt(this.Rotation, new PointF((this.X + dx + (int)(this.X1 - this.X) / 2) * zoom, (this.Y + dy + (int)(this.Y1 - this.Y) / 2) * zoom));
            myPath.Transform(translateMatrix);

            // Draw the transformed ellipse to the screen.
            if (this.filled)
            {
                g.FillPath(myBrush, myPath);
                if (this.showBorder)
                    g.DrawPath(myPen, myPath);
            }
            else
                g.DrawPath(myPen, myPath);

            myPath.Dispose();
            myPen.Dispose();
            if (myBrush!=null)
                myBrush.Dispose();
        }
    }




    /// <summary>
    /// Rettangolo smussato ( estende Ele ) 
    /// </summary>
    /// 
    [DescriptionAttribute("Rounded rectangle")]
    [Serializable]
    public class RRect : Ele
    {
        private int _arcsWidth;
        //private int _rotation;

        public RRect(int x, int y, int x1, int y1)
        {

            this.X = x;
            this.Y = y;
            this.X1 = x1;
            this.Y1 = y1;
            this.Selected = true;
            this.arcsWidth = 20;
            this._rotation = 0;
            this.endMoveRedim();
            this.rot = true; //can rotate?
        }

        [CategoryAttribute("1"), DescriptionAttribute("Rounded Rectangle")]
        public string ObjectType
        {
            get
            {
                return "Rounded Rectangle";
            }
        }



        [CategoryAttribute("Vertex Appearance"), DescriptionAttribute("Dimension of vertex arcs.")]
        public int arcsWidth
        {
            get
            {
                return _arcsWidth;
            }
            set
            {
                if (value <= 9 )
                    _arcsWidth = 10;
                else
                    _arcsWidth = value;
            }
        }

        [ DescriptionAttribute("Rotation Angle.")]
        public int Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }


        public override Ele Copy()
        {
            RRect newE = new RRect(this.X, this.Y, this.X1, this.Y1);
            newE.penColor = this.penColor;
            newE.penWidth = this.penWidth;
            newE.fillColor = this.fillColor;
            newE.filled = this.filled;
            newE.sonoUnaLinea = this.sonoUnaLinea;
            newE.alpha = this.alpha;
            newE.dashStyle = this.dashStyle;
            newE.showBorder = this.showBorder;
            newE.arcsWidth = this.arcsWidth;
            newE.Rotation = this.Rotation;

            newE.OnGrpXRes = this.OnGrpXRes;
            newE.OnGrpX1Res = this.OnGrpX1Res;
            newE.OnGrpYRes = this.OnGrpYRes;
            newE.OnGrpY1Res = this.OnGrpY1Res;

            newE.copyGradprop(this);

            return newE;
        }

        public override void CopyFrom(Ele ele)
        {
            this.copyStdProp(ele, this);
            this.arcsWidth = ((RRect)ele).arcsWidth;
            this.Rotation = ((RRect)ele).Rotation;
        }   


        public override void Select()
        {
            this.undoEle = this.Copy();        
        }


        public override void AddGp(GraphicsPath gp, int dx, int dy, float zoom)
        {
            float n = this.arcsWidth;
            gp.AddArc(new RectangleF((this.X + dx) * zoom, (this.Y + dy) * zoom, n * zoom, n * zoom), 180, 90);
            gp.AddLine((this.X + dx + n / 2) * zoom, (this.Y + dy) * zoom, (this.X1 + dx - n / 2) * zoom, (this.Y + dy) * zoom);

            gp.AddArc(new RectangleF((this.X1 + dx - n) * zoom, (this.Y + dy) * zoom, n * zoom, n * zoom), 270, 90);
            gp.AddLine((this.X1 + dx) * zoom, (this.Y + dy + n / 2) * zoom, (this.X1 + dx) * zoom, (this.Y1 + dy - n / 2) * zoom);

            gp.AddArc(new RectangleF((this.X1 + dx - n) * zoom, (this.Y1 + dy - n) * zoom, n * zoom, n * zoom), 0, 90);
            gp.AddLine((this.X + dx + n / 2) * zoom, (this.Y1 + dy) * zoom, (this.X1 + dx - n / 2) * zoom, (this.Y1 + dy) * zoom);

            gp.AddArc(new RectangleF((this.X + dx) * zoom, (this.Y1 + dy - n) * zoom, n * zoom, n * zoom), 90, 90);
            gp.AddLine((this.X + dx) * zoom, (this.Y1 + dy - n / 2) * zoom, (this.X + dx) * zoom, (this.Y + dy + n / 2) * zoom);

        }


        public override void Draw(Graphics g,int dx, int dy, float zoom)
        {
            float n = this.arcsWidth;
            //System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(this.fillColor);
            Brush myBrush = getBrush(dx, dy, zoom);
            //myBrush.Color = this.Trasparency(this.fillColor, this.alpha);
            System.Drawing.Pen myPen = new System.Drawing.Pen(this.penColor, scaledPenWidth(zoom));
            myPen.DashStyle = this.dashStyle;
            
            if (this.Selected)
            {
                //myBrush.Color = this.dark(this.fillColor, 5, this.alpha);
                myPen.Color = Color.Red;
                myPen.Color = this.Trasparency(myPen.Color, 120);
                myPen.Width = myPen.Width + 1;
            }

            // Create a path and add the object.
            GraphicsPath myPath = new GraphicsPath();

            myPath.AddArc(new RectangleF((this.X + dx) * zoom, (this.Y + dy) * zoom, n * zoom, n * zoom), 180, 90);
            myPath.AddLine((this.X + dx + n / 2) * zoom, (this.Y + dy) * zoom, (this.X1 + dx - n / 2) * zoom, (this.Y + dy) * zoom);

            myPath.AddArc(new RectangleF((this.X1 + dx - n) * zoom, (this.Y + dy) * zoom, n * zoom, n * zoom), 270, 90);
            myPath.AddLine((this.X1 + dx) * zoom, (this.Y + dy + n / 2) * zoom, (this.X1 + dx) * zoom, (this.Y1 + dy - n / 2) * zoom);

            myPath.AddArc(new RectangleF((this.X1 + dx - n) * zoom, (this.Y1 + dy - n) * zoom, n * zoom, n * zoom), 0, 90);
            myPath.AddLine((this.X + dx + n / 2) * zoom, (this.Y1 + dy) * zoom, (this.X1 + dx - n / 2) * zoom, (this.Y1 + dy) * zoom);

            myPath.AddArc(new RectangleF((this.X + dx) * zoom, (this.Y1 + dy - n) * zoom, n * zoom, n * zoom), 90, 90);
            myPath.AddLine((this.X + dx) * zoom, (this.Y1 + dy - n / 2) * zoom, (this.X + dx) * zoom, (this.Y + dy + n / 2) * zoom);                

            Matrix translateMatrix = new Matrix();
            translateMatrix.RotateAt(this.Rotation, new PointF((this.X + dx + (int)(this.X1 - this.X) / 2) * zoom, (this.Y + dy + (int)(this.Y1 - this.Y) / 2) * zoom));
            myPath.Transform(translateMatrix);

            // Draw the transformed ellipse to the screen.
            if (this.filled)
            {
                g.FillPath(myBrush, myPath);
                if (this.showBorder)
                    g.DrawPath(myPen, myPath);
            }
            else
                g.DrawPath(myPen, myPath);

            myPath.Dispose();
            myPen.Dispose();
            if (myBrush != null)
                myBrush.Dispose();
        }
    }

#endregion

    #region Main class Shapes: the collection of Ele

    /// <summary>
    /// Gestisce l'insieme degli oggetti vettoriali 
    /// </summary>
    [Serializable]
    public class Shapes
    {
        // List of objects on the canvas
        public ArrayList List;
        // The Handles Obj
        public SelRect sRec;
        //The selected ele
        public Ele selEle;

        public int minDim = 10;

        //the undo/redo buffer 
        [NonSerialized]
            private UndoBuffer undoB;

        public Shapes()
        {
            List = new ArrayList();
            initUndoBuff();
        }

        private void initUndoBuff()
        {
            undoB = new UndoBuffer(20);// set dim of undo buffer
        }

        public bool IsEmpty()
        {
            return ( List.Count>0 );
        }

        public void afterLoad()
        {
            // UndoBuff is not serialized, I must reinit it after LOAD from file
            initUndoBuff();
            foreach (Ele e in this.List)
                e.AfterLoad();
        }

        /// <summary>
        /// Copy all selected Items
        /// </summary>
        public void CopyMultiSelected(int dx, int dy)
        {
            ArrayList tmpList = new ArrayList();
            foreach (Ele elem in this.List)
            {
                if (elem.Selected)
                {
                    Ele eL = elem.Copy();
                    elem.Selected = false;
                    eL.move(dx, dy);
                    tmpList.Add(eL);
                    //
                    sRec = new SelRect(eL);
                    selEle = eL;
                    selEle.endMoveRedim();
                }
            }
            foreach (Ele tmpElem in tmpList)
            {
                this.List.Add(tmpElem);
                // store the operation in undo/redo buffer
                storeDo("I", tmpElem);
            }
                        
        }


        /// <summary>
        /// returns a Copy of selected element
        /// </summary>
        public Ele CpSelected()
        {
            if (this.selEle != null)
            {
                Ele L = selEle.Copy();
                return L;
            }
            return null;
        }

        /// <summary>
        /// Copy selected Item 
        /// </summary>
        public void CopySelected(int dx,int dy)
        {
            if (this.selEle != null)
            {

                Ele L = this.CpSelected();
                L.move(dx, dy);
                this.deSelect();
                this.List.Add(L);
                
                // store the operation in undo/redo buffer
                storeDo("I", L);

                sRec = new SelRect(L);
                //sRec.sonoUnaLinea = L.sonoUnaLinea;
                selEle = L;
                selEle.endMoveRedim();
            }
        }

        /// <summary>
        /// Elimina oggetto selezioanto
        /// </summary>
        public void rmSelected()
        {
            ArrayList tmpList = new ArrayList();
            foreach (Ele elem in this.List)
            {
                if (elem.Selected)
                {
                    tmpList.Add(elem);    
                }
            }

            if (this.selEle != null)
            {
                //this.List.Remove(selEle);
                selEle = null;
                this.sRec = null;
            }

            foreach (Ele tmpElem in tmpList)
            {
                this.List.Remove(tmpElem);
                
                // store the operation in undo/redo buffer
                storeDo("D", tmpElem);
                
            }

        }

        /// <summary>
        /// Grup selected objs
        /// </summary>
        public void groupSelected()
        {
            ArrayList tmpList = new ArrayList();
            foreach (Ele elem in this.List)
            {
                if (elem.Selected)
                {
                    tmpList.Add(elem);
                }
            }

            if (this.selEle != null)
            {
                //this.List.Remove(selEle);
                selEle = null;
                this.sRec = null;
            }

            foreach (Ele tmpElem in tmpList)
            {
                this.List.Remove(tmpElem);

                // store the operation in undo/redo buffer
                //storeDo("D", tmpElem);
            }

            Group g = new Group(tmpList);
            
            this.List.Add(g);
            // store the operation in undo/redo buffer
            //storeDo("I", g);

            sRec = new SelRect(g);
            //sRec.showHandles(true);
            selEle = g;
            selEle.Select();
            
            // when grouping / degrouping reset undoBuffer
            this.undoB = new UndoBuffer(20);
        }


        /// <summary>
        /// Grup selected objs
        /// </summary>
        public void deGroupSelected()
        {
            ArrayList tmpList = new ArrayList();
            foreach (Ele elem in this.List)
            {
                if (elem.Selected)
                {
                    tmpList.Add(elem);
                }
            }

            if (this.selEle != null)
            {
                //this.List.Remove(selEle);
                selEle = null;
                this.sRec = null;
            }
            bool found = false;
            foreach (Ele tmpElem in tmpList)
            {
                ArrayList tmpL = tmpElem.deGroup();
                
                if (tmpL != null)
                {
                    foreach (Ele e1 in tmpL)
                    {
                        this.List.Add(e1);
                    }
                    this.List.Remove(tmpElem);
                    found = true;
                }                
            }
            if (found)
            {
                // when grouping / degrouping reset undoBuffer
                this.undoB = new UndoBuffer(20);
            }


        }



        public void move(int dx, int dy)
        {
            foreach (Ele e in this.List)
            {
                if (e.Selected)
                {
                    e.move(dx, dy);
                }
            }
        }

        public void Fit2grid(int gridsize)
        {
            foreach (Ele e in this.List)
            {
                if (e.Selected)
                {
                    e.Fit2grid(gridsize);
                    
                }
            }
        }


        public void endMove()
        {
            foreach (Ele e in this.List)
            {
                if (e.Selected)
                {
                    e.endMoveRedim();
                    if (!e.AmIaGroup())
                        storeDo("U",e); 
                }
            }
        }

        public void Propertychanged()
        {
            foreach (Ele e in this.List)
            {
                if (e.Selected)
                {
                    storeDo("U",e);
                }
            }

        }

        #region undo/redo 

        public bool UndoEnabled()
        {
            return this.undoB.unDoable();
        }

        public bool RedoEnabled()
        {
            return this.undoB.unRedoable();
        }

        public void storeDo(string option, Ele e)
        {
            Ele olde = null;
            if (e.undoEle != null)
                olde = e.undoEle.Copy();
            Ele newe = e.Copy();
            buffEle buff = new buffEle(e, newe, olde,option);
            this.undoB.add2Buff(buff);
            e.undoEle = e.Copy();
        }

        public void Undo() 
        {
            buffEle buff = (buffEle)this.undoB.Undo();
            if (buff != null)
            {
                switch (buff.op)
                {
                    case "U":
                        buff.objRef.CopyFrom(buff.oldE);
                        break;
                    case "I":
                        //buff.objRef.CopyFrom(buff.oldE);
                        this.List.Remove(buff.objRef);
                        break;
                    case "D":
                        //buff.objRef.CopyFrom(buff.oldE);
                        this.List.Add(buff.objRef);
                        break;
                    default:
                        break;
                }
            }
        }

        public void Redo()
        {
            buffEle buff = (buffEle)this.undoB.Redo();
            if (buff != null)
            {
                switch (buff.op)
                {
                    case "U":
                        buff.objRef.CopyFrom(buff.newE);
                        break;
                    case "I":
                        //buff.objRef.CopyFrom(buff.oldE);
                        this.List.Add(buff.objRef);
                        break;
                    case "D":
                        //buff.objRef.CopyFrom(buff.oldE);
                        this.List.Remove(buff.objRef);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        private int countSelected()
        {
            int i = 0;
            foreach (Ele e in this.List)
            {
                if (e.Selected)
                {
                    i++;
                }
            }
            return i;
        }

        /// <summary>
        /// Returns an array with the selected item. Used for property grid.
        /// </summary>
        public Ele[] getSelectedArray()
        {
            Ele[] myArray = new Ele[this.countSelected()];   
            int i = 0;
            foreach (Ele e in this.List)
            {
                if (e.Selected)
                {
                    myArray[i]=e;
                    i++;
                }
                    
            }
            return myArray;
        }

        /// <summary>
        /// Returns a List with the selected items. Used for SaveObj.
        /// </summary>
        public ArrayList getSelectedList()
        {
            ArrayList tmpL = new ArrayList();
            foreach (Ele e in this.List)
            {
                if (e.Selected)
                {
                    tmpL.Add(e);
                }
            }
            return tmpL;
        }

        /// <summary>
        /// Returns a List with the selected items. Used for SaveObj.
        /// </summary>
        public void setList(ArrayList a)
        {
            foreach (Ele e in a)
            {
                 this.List.Add(e);

            }
        }

        /// <summary>
        /// 2 front
        /// </summary>
        public void primoPiano()
        {
            if (this.selEle != null)
            {
                this.List.Remove(selEle);
                this.List.Add(selEle);
            }
        }

        /// <summary>
        /// 2 back
        /// </summary>
        public void secondoPiano()
        {
            if (this.selEle != null)
            {
                this.List.Remove(selEle);
                this.List.Insert(0,selEle);
                this.deSelect();
            }
        }

        /// <summary>
        /// Deselect 
        /// </summary>
        public void deSelect()
        {
            foreach (Ele obj in this.List)
            {
                obj.Selected = false;
            }
            selEle = null;
            sRec = null;
        }

        #region DRAW

        /// <summary>
        /// Draw all shapes
        /// </summary>
        public void Draw(Graphics g,int dx, int dy, float zoom)
        {
            bool almostOneSelected = false;
            foreach (Ele obj in this.List)
            {
                obj.Draw(g,dx,dy, zoom);
                if (obj.Selected)
                    almostOneSelected = true;                    
            }
            if (almostOneSelected)
                if (sRec !=null)
                    sRec.Draw(g,dx,dy,zoom);
        }


        /// <summary>
        /// Draw all Unselected shapes
        /// </summary>
        public void DrawUnselected(Graphics g, int dx , int dy,float zoom)
        {
            g.PageScale = 10;
            //bool almostOneSelected = false;
            foreach (Ele obj in this.List)
            {

                if (!obj.Selected)
                    obj.Draw(g,dx,dy,zoom);
            }
        }


        /// <summary>
        /// Draw all Unselected shapes
        /// </summary>
        public void DrawUnselected(Graphics g)
        {
            g.PageScale = 10;
            //bool almostOneSelected = false;
            foreach (Ele obj in this.List)
            {

                if (!obj.Selected)
                    obj.Draw(g,0,0,1);
            }
        }

        /// <summary>
        /// Draw all Selected shapes
        /// </summary>
        public void DrawSelected(Graphics g,int dx, int dy, float zoom)
        {
            bool almostOneSelected = false;
            foreach (Ele obj in this.List)
            {
                if (obj.Selected)
                {
                    obj.Draw(g,dx,dy,zoom);
                    almostOneSelected = true;
                }
            }
            if (almostOneSelected)
                if (sRec != null)
                    sRec.Draw(g,dx,dy,zoom);
        }

        /// <summary>
        /// Draw all Selected shapes
        /// </summary>
        public void DrawSelected(Graphics g)
        {
            bool almostOneSelected = false;
            foreach (Ele obj in this.List)
            {
                if (obj.Selected)
                {
                    obj.Draw(g,0,0,1);
                    almostOneSelected = true;
                }
            }
            if (almostOneSelected)
                if (sRec != null)
                    sRec.Draw(g,0,0,1);
        }

        #endregion

        #region ADD ELEMNTS METODS

        /// <summary>
        /// Adds Rect
        /// </summary>
        public void addRect(int x,int y , int x1, int y1, Color penC,Color fillC,float penW,bool filled)
        {
            if (x1 - minDim <= x)
                x1 = x + minDim;
            if (y1 - minDim <= y)
                y1 = y + minDim;             

            this.deSelect();
            Rect r = new Rect(x, y, x1, y1);
            r.penColor = penC;
            r.penWidth = penW;
            r.fillColor = fillC;
            r.filled = filled;
            
            this.List.Add(r);
            // store the operation in undo/redo buffer
            storeDo("I", r);

            sRec = new SelRect(r);
            selEle = r;
            selEle.Select();
        }

        /// <summary>
        /// Adds Arc
        /// </summary>
        public void addArc(int x, int y, int x1, int y1, Color penC, Color fillC, float penW, bool filled)
        {
            if (x1 - minDim <= x)
                x1 = x + minDim;
            if (y1 - minDim <= y)
                y1 = y + minDim;

            this.deSelect();
            Arc r = new Arc(x, y, x1, y1);
            r.penColor = penC;
            r.penWidth = penW;
            r.fillColor = fillC;
            r.filled = filled;

            this.List.Add(r);
            // store the operation in undo/redo buffer
            storeDo("I", r);

            sRec = new SelRect(r);
            selEle = r;
            selEle.Select();
        }
        
        /// <summary>
        /// Adds Rect
        /// </summary>
        public void addLine(int x, int y, int x1, int y1, Color penC, float penW)
        {
            /*
            if ((x1 == x) && (y1 == y))
            {
                x1 = x + minDim;
                y1 = y + minDim;             
            }
             */

            this.deSelect();
            Linea r = new Linea(x, y, x1, y1);

            r.penColor = penC;
            r.penWidth = penW;

            this.List.Add(r);
            // store the operation in undo/redo buffer
            storeDo("I", r);

            sRec = new SelRect(r);
            //sRec.sonoUnaLinea = r.sonoUnaLinea;
            selEle = r;
            selEle.Select();
        }

        /// <summary>
        /// Adds TextBox
        /// </summary>
        public void addTextBox(int x, int y, int x1, int y1, RichTextBox t, Color penC, Color fillC, float penW, bool filled)
        {
            if (x1 - minDim <= x)
                x1 = x + minDim;
            if (y1 - minDim <= y)
                y1 = y + minDim;             

            this.deSelect();
            BoxTesto r = new BoxTesto(x, y, x1, y1);

            r.penColor = penC;
            r.penWidth = penW;
            r.fillColor = fillC;
            r.filled = filled;

            //Queste due riche sono alternative in base al tipo di textbox che si utilizza
            //r.tBox.getString(t);
            r.rtf = t.Rtf;

            this.List.Add(r);
            // store the operation in undo/redo buffer
            storeDo("I", r);

            sRec = new SelRect(r);
            selEle = r;
            selEle.Select();
        }

        /// <summary>
        /// Adds RoundRect
        /// </summary>
        public void addRRect(int x, int y, int x1, int y1, Color penC, Color fillC, float penW, bool filled)
        {
            if (x1 - minDim <= x)
                x1 = x + minDim;
            if (y1 - minDim <= y)
                y1 = y + minDim;             

            this.deSelect();
            RRect r = new RRect(x, y, x1, y1);

            r.penColor = penC;
            r.penWidth = penW;
            r.fillColor = fillC;
            r.filled = filled;

            this.List.Add(r);
            // store the operation in undo/redo buffer
            storeDo("I", r);

            sRec = new SelRect(r);
            selEle = r;
            selEle.Select();
        }

        /// <summary>
        /// Adds ImageBox
        /// </summary>
        public void addImgBox(int x, int y, int x1, int y1, string st, Color penC, float penW)
        {
            if (x1 - minDim <= x)
                x1 = x + minDim;
            if (y1 - minDim <= y)
                y1 = y + minDim;             

            this.deSelect();
            ImgBox r = new ImgBox(x, y, x1, y1);
            r.penColor = penC;
            r.penWidth = penW;

            this.List.Add(r);
            // store the operation in undo/redo buffer
            storeDo("I", r);

            if (!(st == null))
            {
                try
                {
                    Bitmap loadTexture = new Bitmap(st);
                    r.img = loadTexture;
                }
                catch { }
            }


            sRec = new SelRect(r);
            selEle = r;
            selEle.Select();
        }

        /// <summary>
        /// Adds Ellipse
        /// </summary>
        public void addEllipse(int x, int y, int x1, int y1, Color penC, Color fillC, float penW, bool filled)
        {
            if (x1 - minDim <= x)
                x1 = x + minDim;
            if (y1 - minDim <= y)
                y1 = y + minDim;             

            this.deSelect();
            Ell r = new Ell(x, y, x1, y1);
            r.penColor = penC;
            r.penWidth = penW;
            r.fillColor = fillC;
            r.filled = filled;

            this.List.Add(r);
            // store the operation in undo/redo buffer
            storeDo("I", r);

            sRec = new SelRect(r);
            selEle = r;
            selEle.Select();
        }

        #endregion

        /// <summary>
        /// Selects last shape containing x,y
        /// </summary>
        public void click(int x, int y, RichTextBox r)
        {
            sRec = null;
            selEle = null;
            foreach (Ele obj in this.List)
            {
                obj.Selected = false;
                if (obj.contains(x, y))
                {
                    selEle = obj; //salvo il riferimento dell'ogg trovato
                }
            }
            if (selEle != null)
            {
                selEle.Selected = true;
                selEle.Select();
                selEle.Select(r);
                sRec = new SelRect(selEle);//create handling rect
                //sRec.sonoUnaLinea = selEle.sonoUnaLinea;
                //sRec.showHandles(selEle.AmIaGroup());
            }
        }

        /// <summary>
        /// Selects all shapes in imput rectangle 
        /// </summary>
        public void multiSelect(int startX, int startY, int endX, int endY, RichTextBox r)
        {
            sRec = null;
            selEle = null;
            foreach (Ele obj in this.List)
            {

                obj.Selected = false;
                int x = obj.getX();
                int x1 = obj.getX1();
                int y = obj.getY();
                int y1 = obj.getY1();
                int c = 0;
                if (x > x1)
                {
                    c = x;
                    x = x1;
                    x1 = c;
                }
                if (y > y1)
                {
                    c = y;
                    y = y1;
                    y1 = c;
                }
                //if (obj.getX() <= endX & obj.getX1() >= startX & obj.getY() <= endY & obj.getY1() >= startY)
                if (x <= endX & x1 >= startX & y <= endY & y1 >= startY)
                {
                    selEle = obj; //salvo il riferimento dell'ogg trovato
                    obj.Selected = true;//indico l'oggetto trovato come selezionato
                    obj.Select();
                    obj.Select(r);                
                }
            }
            if (selEle != null)
            {
                sRec = new SelRect(selEle);//creo un gestore con maniglie
                //sRec.sonoUnaLinea = selEle.sonoUnaLinea;//indico se il gestore ?per una linea
                //sRec.showHandles(selEle.AmIaGroup());
            }

        }

    }

    #endregion

    #region UNDO BUFFER 

    /// <summary>
    /// Undo buffer Ele element.
    /// </summary>
    public class buffEle
    {

        public Ele objRef;
        public string op; // U:Update, I:Insert, D:Delete
        public Ele oldE; //Start point
        public Ele newE; //Start point
      
        public buffEle(Ele refe,Ele newe, Ele olde, string o)
        {            
            objRef = refe;
            oldE = olde;
            newE = newe;
            op =o;
        }

    }

    /// <summary>
    /// Two Linked List Element
    /// </summary>
    public class buffObj
    {
        public buffObj Next ;
        public buffObj Prec ;
        public object elem;
        
        public buffObj(object o)
        {
            elem=o;
        }
        
    }

    /// <summary>
    /// Undo buffer. (Two Linked List)
    /// </summary>
    //[Serializable]
    public class UndoBuffer
    {
        private buffObj Top;
        private buffObj Bottom;
        private buffObj Current;
        private int _BuffSize;
        private int _N_elem;
        private bool At_Bottom;

        public UndoBuffer(int i)
        {

            this.BuffSize = i;
            this._N_elem = 0;
            Top = null;
            Bottom = null;
            Current = null;
            At_Bottom = true;
        }

        public int BuffSize
        {        
            get
            {
                return _BuffSize;
            }
            set
            {
                _BuffSize = value;
            }                    
        }

        public int N_elem
        {
            get
            {
                return _N_elem;
            }
        }

        public void add2Buff(object o)
        {
            if (o != null)
            {
                buffObj g = new buffObj(o);
                if (this.N_elem == 0)
                {
                    g.Next = null;
                    g.Prec = null;
                    Top = g;
                    Bottom = g;
                    Current = g;
                }
                else 
                {
                    g.Prec = Current;
                    g.Next = null;
                    Current.Next = g;                    
                    Top = g;
                    Current = g;
                    if (this.N_elem == 1)
                    {
                        Bottom.Next = g;
                    }
                }

                //this._N_elem = count();
                this._N_elem++;
                if (this.BuffSize < this.N_elem)
                {
                    this.Bottom = this.Bottom.Next;
                    this.Bottom.Prec = null;
                    this._N_elem--;
                }
                At_Bottom = false;
            }


        }
     
        public object Undo()
        {

            if (Current != null)
            {

                object obj = Current.elem;
                if (Current.Prec != null)
                {
                    Current = Current.Prec;
                    this._N_elem--;
                    this.At_Bottom = false;
                }
                else
                {
                    this.At_Bottom = true;
                }
                return obj;
            }
            //this._N_elem = count();
            return null;
        }

        public object Redo()
        {
            if (Current != null)
            {
                object obj;
                if (!At_Bottom)
                {
                    if (Current.Next != null)
                    {
                        Current = Current.Next;
                        this._N_elem++;
                    }
                }
                else 
                {
                    At_Bottom = false;
                }
                obj = Current.elem;

                return obj;
            }
            //this._N_elem = count();
            return null;
        }

        public bool unDoable()
        {
            return ! this.At_Bottom;
        }

        public bool unRedoable()
        {
            if (this.Current == null)
                return false;
            if (this.Current.Next == null)
                return false;
            return true;
        }
    }

    #endregion

}

