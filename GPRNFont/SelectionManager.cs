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
        private Rectangle selectionRect;
        private bool isMovingRect;

        private Rectangle[] littleSquares = new Rectangle[8];
        private readonly int littleSquareSize = 6;
        private int selectedLittleSquare = -1;

        private Rectangle SelectedArea
        { 
            get
            {
                if (this.selectionRect.Width > 0 && this.selectionRect.Height > 0)
                    return this.selectionRect;

                return Rectangle.Empty;
            }
            set
            {
                this.selectionRect = value;
            }
        }

        private bool IsSelecting()
        {
            return this.startPoint.HasValue;
        }

        private Point LockEndPointByLittleSquare(Point endPoint)
        {
            var p = new Point(endPoint.X, endPoint.Y);

            if (this.selectedLittleSquare == SQUARE_TOP_CENTER)
                p.X = this.SelectedArea.X;
            else if (this.selectedLittleSquare == SQUARE_CENTER_LEFT)
                p.Y = this.SelectedArea.Y;
            else if (this.selectedLittleSquare == SQUARE_CENTER_RIGHT)
                p.Y = this.SelectedArea.Y + this.SelectedArea.Height;
            else if(this.selectedLittleSquare == SQUARE_BOTTOM_CENTER)
                p.X = this.SelectedArea.X + this.SelectedArea.Width;

            return p;
        }

        private Point LockEndPointByScreenSize(Point endPoint, Size imgSize)
        {
            var p = new Point(endPoint.X, endPoint.Y);

            p.X = Math.Min(Math.Max(0, p.X), imgSize.Width);
            p.Y = Math.Min(Math.Max(0, p.Y), imgSize.Height);

            return p;
        }

        private void UpdateSelectionRect(Point endPoint, Size imgSize)
        {
            var lockedEndPoint0 = LockEndPointByLittleSquare(endPoint);
            var lockedEndPoint = LockEndPointByScreenSize(lockedEndPoint0, imgSize);

            this.selectionRect.X = startPoint.Value.X > lockedEndPoint.X ? lockedEndPoint.X : startPoint.Value.X;
            this.selectionRect.Y = startPoint.Value.Y > lockedEndPoint.Y ? lockedEndPoint.Y : startPoint.Value.Y;
            this.selectionRect.Width = startPoint.Value.X > lockedEndPoint.X ? startPoint.Value.X - lockedEndPoint.X : lockedEndPoint.X - startPoint.Value.X;
            this.selectionRect.Height = startPoint.Value.Y > lockedEndPoint.Y ? startPoint.Value.Y - lockedEndPoint.Y : lockedEndPoint.Y - startPoint.Value.Y;
        }

        private void DrawSelectionRectBorder(Graphics g)
        {
            using (var pen = new Pen(Color.Black, 1F)) 
            {
                pen.DashStyle = DashStyle.Dash;
                g.DrawRectangle(pen, this.SelectedArea);
            }
        }

        private void DrawSelectionRectFill(Graphics g)
        {
            using (var brush = new SolidBrush(Color.FromArgb(128, SystemColors.Highlight)))
            {
                g.FillRectangle(brush, this.SelectedArea);
            }
        }

        private void ClearLittleSquares()
        {
            for (var i = 0; i < this.littleSquares.Length; i++)
                this.littleSquares[i] = Rectangle.Empty;

            this.selectedLittleSquare = -1;
        }

        private void UpdateLittleSquares()
        {
            if (this.SelectedArea.IsEmpty)
                this.ClearLittleSquares();
            else
            {
                var half = this.littleSquareSize / 2;

                this.littleSquares[SQUARE_TOP_LEFT] = new Rectangle(
                    this.SelectedArea.X - half,
                    this.SelectedArea.Y - half,
                    this.littleSquareSize, this.littleSquareSize);

                this.littleSquares[SQUARE_TOP_CENTER] = new Rectangle(
                    this.SelectedArea.X - half + this.SelectedArea.Width / 2,
                    this.SelectedArea.Y - half,
                    this.littleSquareSize, this.littleSquareSize);

                this.littleSquares[SQUARE_TOP_RIGHT] = new Rectangle(
                    this.SelectedArea.X - half + this.SelectedArea.Width,
                    this.SelectedArea.Y - half,
                    this.littleSquareSize, this.littleSquareSize);


                this.littleSquares[SQUARE_CENTER_LEFT] = new Rectangle(
                    this.SelectedArea.X - half,
                    this.SelectedArea.Y - half + this.SelectedArea.Height / 2,
                    this.littleSquareSize, this.littleSquareSize);

                this.littleSquares[SQUARE_CENTER_RIGHT] = new Rectangle(
                    this.SelectedArea.X - half + this.SelectedArea.Width,
                    this.SelectedArea.Y - half + this.SelectedArea.Height / 2,
                    this.littleSquareSize, this.littleSquareSize);


                this.littleSquares[SQUARE_BOTTOM_LEFT] = new Rectangle(
                    this.SelectedArea.X - half,
                    this.SelectedArea.Y - half + this.SelectedArea.Height,
                    this.littleSquareSize, this.littleSquareSize);

                this.littleSquares[SQUARE_BOTTOM_CENTER] = new Rectangle(
                    this.SelectedArea.X - half + this.SelectedArea.Width / 2,
                    this.SelectedArea.Y - half + this.SelectedArea.Height,
                    this.littleSquareSize, this.littleSquareSize);

                this.littleSquares[SQUARE_BOTTOM_RIGHT] = new Rectangle(
                    this.SelectedArea.X - half + this.SelectedArea.Width,
                    this.SelectedArea.Y - half + this.SelectedArea.Height,
                    this.littleSquareSize, this.littleSquareSize);
            }  
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
            this.SelectedArea = Rectangle.Empty;
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

            if (this.SelectedArea.Contains(mousePoint))
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
            if (this.SelectedArea.X < 0)
            {
                this.selectionRect.Width += this.SelectedArea.X;
                this.selectionRect.X = 0;
            }
            if (this.SelectedArea.Y < 0)
            {
                this.selectionRect.Height += this.SelectedArea.Y;
                this.selectionRect.Y = 0;
            }
            if (this.SelectedArea.X + this.SelectedArea.Width > imgSize.Width)
                this.selectionRect.Width = imgSize.Width - this.SelectedArea.X;
            if (this.SelectedArea.Y + this.SelectedArea.Height > imgSize.Height)
                this.selectionRect.Height = imgSize.Height - this.SelectedArea.Y;
        }

        private void StopMoveRect(Size imgSize)
        {
            this.isMovingRect = false;
            this.startPoint = null;

            this.AdjustSelectedArea(imgSize);

            this.UpdateLittleSquares();
        }

        public Rectangle StartSelection(Point startPoint)
        {
            this.selectedLittleSquare = GetLittleSquareIndex(startPoint);

            if (this.IsUsingLittleSquare())
                this.startPoint = this.GetStartPointByLittleSquare();
            else
            {
                this.startPoint = startPoint;

                if (this.SelectedArea.Contains(startPoint))
                    this.isMovingRect = true;
                else
                    this.SelectedArea = Rectangle.Empty;
            }

            return this.SelectedArea;
        }

        public void UpdateSelection(Point endPoint, Size imgSize)
        {
            if (this.isMovingRect)
                this.MoveRect(endPoint);
            else
            {
                if (this.IsSelecting())
                    this.UpdateSelectionRect(endPoint, imgSize);

                if (this.IsUsingLittleSquare())
                    this.UpdateLittleSquares();
            }
        }

        public Rectangle EndSelection(Point endPoint, Size imgSize)
        {
            if (this.isMovingRect)
                this.StopMoveRect(imgSize);
            else if (this.IsSelecting())
            {
                this.UpdateSelectionRect(endPoint, imgSize);
                this.UpdateLittleSquares();
                this.startPoint = null;
            }

            return this.SelectedArea;
        }

        public void DrawSelectionRect(Graphics g)
        {
            if (!this.SelectedArea.IsEmpty)
            {
                this.DrawSelectionRectBorder(g);
                if (!this.IsSelecting() || this.IsUsingLittleSquare())
                {
                    this.DrawSelectionRectFill(g);
                    this.DrawLittleSquares(g);
                }
            }
        }

        public void ChangeSelectionRect(Rectangle newSelectedArea)
        {
            this.SelectedArea = newSelectedArea;
            this.UpdateLittleSquares();
        }
    }
}