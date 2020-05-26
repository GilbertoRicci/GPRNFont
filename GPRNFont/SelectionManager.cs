using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GPRNFont
{
    public class SelectionManager
    {
        private const int SQUARE_TOP_LEFT = 0;
        private const int SQUARE_TOP_CENTER = 1;
        private const int SQUARE_TOP_RIGHT = 2;
        private const int SQUARE_CENTER_LEFT = 3;
        private const int SQUARE_CENTER_RIGHT = 4;
        private const int SQUARE_BOTTOM_LEFT = 5;
        private const int SQUARE_BOTTOM_CENTER = 6;
        private const int SQUARE_BOTTOM_RIGHT = 7;

        private Point? startPoint;
        private int currentZoom = 100;
        private Rectangle selectionRect;
        private bool isMovingRect;

        private Rectangle[] littleSquares;
        private int littleSquareSize;
        private int selectedLittleSquare;

        public SelectionManager()
        {
            this.littleSquares = new Rectangle[8];
            this.littleSquareSize = 6;
            this.ClearSelectedLittleSquare();
        }

        private void ClearSelectedLittleSquare()
        {
            this.selectedLittleSquare = -1;
        }

        private bool isSelecting()
        {
            return this.startPoint.HasValue;
        }

        private Point LockEndPointByLittleSquare(Point endPoint)
        {
            var p = new Point(endPoint.X, endPoint.Y);

            if (this.selectedLittleSquare == SQUARE_TOP_CENTER)
                p.X = this.selectionRect.X;
            else if (this.selectedLittleSquare == SQUARE_CENTER_LEFT)
                p.Y = this.selectionRect.Y;
            else if (this.selectedLittleSquare == SQUARE_CENTER_RIGHT)
                p.Y = this.selectionRect.Y + this.selectionRect.Height;
            else if(this.selectedLittleSquare == SQUARE_BOTTOM_CENTER)
                p.X = this.selectionRect.X + this.selectionRect.Width;


            return p;
        }

        private Point LockEndPointByScreenSize(Point endPoint, Size imgSize)
        {
            var p = new Point(endPoint.X, endPoint.Y);

            p.X = Math.Min(Math.Max(0, p.X), imgSize.Width);
            p.Y = Math.Min(Math.Max(0, p.Y), imgSize.Height);

            return p;
        }

        private void ClearRectIfIsEmpty()
        {
            if (this.selectionRect.Width <= 0 || this.selectionRect.Height <= 0)
                this.selectionRect = Rectangle.Empty;
        }

        private void UpdateSelectionRect(Point endPoint, Size imgSize)
        {
            var lockedEndPoint0 = LockEndPointByLittleSquare(endPoint);
            var lockedEndPoint = LockEndPointByScreenSize(lockedEndPoint0, imgSize);

            this.selectionRect.X = startPoint.Value.X > lockedEndPoint.X ? lockedEndPoint.X : startPoint.Value.X;
            this.selectionRect.Y = startPoint.Value.Y > lockedEndPoint.Y ? lockedEndPoint.Y : startPoint.Value.Y;
            this.selectionRect.Width = startPoint.Value.X > lockedEndPoint.X ? startPoint.Value.X - lockedEndPoint.X : lockedEndPoint.X - startPoint.Value.X;
            this.selectionRect.Height = startPoint.Value.Y > lockedEndPoint.Y ? startPoint.Value.Y - lockedEndPoint.Y : lockedEndPoint.Y - startPoint.Value.Y;

            this.ClearRectIfIsEmpty();
        }

        private void DrawSelectionRectBorder(Graphics g)
        {
            using (var pen = new Pen(Color.Black, 1F)) 
            {
                pen.DashStyle = DashStyle.Dash;
                g.DrawRectangle(pen, this.selectionRect);
            }
        }

        private void DrawSelectionRectFill(Graphics g)
        {
            using (var brush = new SolidBrush(Color.FromArgb(128, SystemColors.Highlight)))
            {
                g.FillRectangle(brush, this.selectionRect);
            }
        }

        private void CreateLittleSquares()
        {
            var half = this.littleSquareSize / 2;

            this.littleSquares[SQUARE_TOP_LEFT] = new Rectangle(
                this.selectionRect.X - half,
                this.selectionRect.Y - half,
                this.littleSquareSize, this.littleSquareSize);

            this.littleSquares[SQUARE_TOP_CENTER] = new Rectangle(
                this.selectionRect.X - half + this.selectionRect.Width / 2,
                this.selectionRect.Y - half,
                this.littleSquareSize, this.littleSquareSize);

            this.littleSquares[SQUARE_TOP_RIGHT] = new Rectangle(
                this.selectionRect.X - half + this.selectionRect.Width,
                this.selectionRect.Y - half,
                this.littleSquareSize, this.littleSquareSize);


            this.littleSquares[SQUARE_CENTER_LEFT] = new Rectangle(
                this.selectionRect.X - half,
                this.selectionRect.Y - half + this.selectionRect.Height / 2,
                this.littleSquareSize, this.littleSquareSize);

            this.littleSquares[SQUARE_CENTER_RIGHT] = new Rectangle(
                this.selectionRect.X - half + this.selectionRect.Width,
                this.selectionRect.Y - half + this.selectionRect.Height / 2,
                this.littleSquareSize, this.littleSquareSize);


            this.littleSquares[SQUARE_BOTTOM_LEFT] = new Rectangle(
                this.selectionRect.X - half,
                this.selectionRect.Y - half + this.selectionRect.Height,
                this.littleSquareSize, this.littleSquareSize);

            this.littleSquares[SQUARE_BOTTOM_CENTER] = new Rectangle(
                this.selectionRect.X - half + this.selectionRect.Width / 2,
                this.selectionRect.Y - half + this.selectionRect.Height,
                this.littleSquareSize, this.littleSquareSize);

            this.littleSquares[SQUARE_BOTTOM_RIGHT] = new Rectangle(
                this.selectionRect.X - half + this.selectionRect.Width,
                this.selectionRect.Y - half + this.selectionRect.Height,
                this.littleSquareSize, this.littleSquareSize);
        }

        private void DrawLittleSquares(Graphics g)
        {
            foreach (var square in this.littleSquares)
            {
                using (var pen = new Pen(Color.Black, 1F))
                {
                    g.DrawRectangle(pen, square);
                }
                using (var brush = new SolidBrush(SystemColors.HighlightText))
                {
                    g.FillRectangle(brush, square);
                }
            }
        }

        public void ResetSelection()
        {
            this.startPoint = null;
            this.selectionRect = Rectangle.Empty;
        }

        private int GetLittleSquareIndex(Point point)
        {
            for(var i=0; i<this.littleSquares.Length; i++)
            {
                var square = this.littleSquares[i];

                if (square != null && square.Contains(point))
                    return i;
            }

            return -1;
        }

        public Cursor GetCursor(Point mousePoint)
        {
            var littleSquare = this.GetLittleSquareIndex(mousePoint);

            if (littleSquare == SQUARE_TOP_LEFT || littleSquare == SQUARE_BOTTOM_RIGHT)
                return Cursors.SizeNWSE;

            if (littleSquare == SQUARE_TOP_CENTER || littleSquare == SQUARE_BOTTOM_CENTER)
                return Cursors.SizeNS;

            if (littleSquare == SQUARE_TOP_RIGHT || littleSquare == SQUARE_BOTTOM_LEFT)
                return Cursors.SizeNESW;

            if (littleSquare == SQUARE_CENTER_LEFT || littleSquare == SQUARE_CENTER_RIGHT)
                return Cursors.SizeWE;

            if (this.selectionRect.Contains(mousePoint))
                return Cursors.SizeAll;

            return Cursors.Cross;
        }

        private bool IsUsingLittleSquare()
        {
            return this.selectedLittleSquare >= 0;
        }

        private Point? GetStartPointByLittleSquare()
        {
            Point p;

            if (this.selectedLittleSquare == SQUARE_TOP_LEFT || this.selectedLittleSquare == SQUARE_TOP_CENTER || this.selectedLittleSquare == SQUARE_CENTER_LEFT)
                p = this.littleSquares[SQUARE_BOTTOM_RIGHT].Location;
            else if (this.selectedLittleSquare == SQUARE_BOTTOM_RIGHT || this.selectedLittleSquare == SQUARE_BOTTOM_CENTER || this.selectedLittleSquare == SQUARE_CENTER_RIGHT)
                p = this.littleSquares[SQUARE_TOP_LEFT].Location;
            else if (this.selectedLittleSquare == SQUARE_TOP_RIGHT)
                p = this.littleSquares[SQUARE_BOTTOM_LEFT].Location;
            else if (this.selectedLittleSquare == SQUARE_BOTTOM_LEFT)
                p = this.littleSquares[SQUARE_TOP_RIGHT].Location;
            else
                return null;

            p.X += this.littleSquareSize / 2;
            p.Y += this.littleSquareSize / 2;
            return p;
        }

        private void MoveRect(Point endPoint)
        {
            this.selectionRect.X += endPoint.X - this.startPoint.Value.X;
            this.selectionRect.Y += endPoint.Y - this.startPoint.Value.Y;

            this.startPoint = endPoint;
        }

        private void AdjustSelectedArea(Size imgSize)
        {
            if (this.selectionRect.X < 0)
            {
                this.selectionRect.Width += this.selectionRect.X;
                this.selectionRect.X = 0;
            }
            if (this.selectionRect.Y < 0)
            {
                this.selectionRect.Height += this.selectionRect.Y;
                this.selectionRect.Y = 0;
            }
            if (this.selectionRect.X + this.selectionRect.Width > imgSize.Width)
                this.selectionRect.Width = imgSize.Width - this.selectionRect.X;
            if (this.selectionRect.Y + this.selectionRect.Height > imgSize.Height)
                this.selectionRect.Height = imgSize.Height - this.selectionRect.Y;

            this.ClearRectIfIsEmpty();
        }

        private void StopMoveRect(Size imgSize)
        {
            this.isMovingRect = false;
            this.startPoint = null;

            this.AdjustSelectedArea(imgSize);

            if (!this.selectionRect.IsEmpty)
                this.CreateLittleSquares();
        }

        public void StartSelection(Point startPoint)
        {
            this.selectedLittleSquare = GetLittleSquareIndex(startPoint);

            if (this.IsUsingLittleSquare())
                this.startPoint = this.GetStartPointByLittleSquare();
            else
            {
                this.startPoint = startPoint;

                if (this.selectionRect.Contains(startPoint))
                    this.isMovingRect = true;
                else
                    this.selectionRect = Rectangle.Empty;
            }
        }

        public void UpdateSelection(Point endPoint, Size imgSize)
        {
            if (this.isMovingRect)
                this.MoveRect(endPoint);
            else
            {
                if (this.isSelecting())
                    this.UpdateSelectionRect(endPoint, imgSize);

                if (this.IsUsingLittleSquare())
                    this.CreateLittleSquares();
            }
        }

        public void EndSelection(Point endPoint, Size imgSize)
        {
            if (this.isMovingRect)
                this.StopMoveRect(imgSize);
            else
            {
                if (this.isSelecting())
                {
                    this.UpdateSelectionRect(endPoint, imgSize);
                    this.CreateLittleSquares();
                    this.startPoint = null;
                }

                this.ClearSelectedLittleSquare();
            }
        }

        public void DrawSelectionRect(Graphics g)
        {
            if (!this.selectionRect.IsEmpty)
            {
                this.DrawSelectionRectBorder(g);
                if (!this.isSelecting() || this.IsUsingLittleSquare())
                {
                    this.DrawSelectionRectFill(g);
                    this.DrawLittleSquares(g);
                }
            }
        }

        public Rectangle GetSelectedArea()
        {
            if (this.selectionRect.Width > 0 && this.selectionRect.Height > 0)
                return this.selectionRect;

            return Rectangle.Empty;
        }

        public void ChangeSelectionRect(Rectangle newSelectionRect)
        {
            this.selectionRect = newSelectionRect;
            if (!this.selectionRect.IsEmpty)
                this.CreateLittleSquares();
        }

        public void ZoomApply(int newZoom)
        {
            if (!this.GetSelectedArea().IsEmpty)
            {
                double doom = (double)newZoom / (double)this.currentZoom;

                this.selectionRect = new Rectangle(Convert.ToInt32(this.selectionRect.X * doom), Convert.ToInt32(this.selectionRect.Y * doom),
                                       Convert.ToInt32(this.selectionRect.Width * doom), Convert.ToInt32(this.selectionRect.Height * doom));

                this.CreateLittleSquares();
            }

            this.currentZoom = newZoom;

        }
    }
}