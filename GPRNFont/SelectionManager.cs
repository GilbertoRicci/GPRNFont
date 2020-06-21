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
        private bool isMovingRect;

        private Rectangle[] littleSquares = new Rectangle[8];
        private readonly int littleSquareSize = 6;
        private int selectedLittleSquare = -1;

        private Rectangle selectionRect;
        public Rectangle SelectionRect
        {
            get
            {
                return this.selectionRect;
            }
            set
            {
                this.selectionRect = value;
                this.UpdateLittleSquares();
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

        private void ClearLittleSquares()
        {
            for (var i = 0; i < this.littleSquares.Length; i++)
                this.littleSquares[i] = Rectangle.Empty;

            this.selectedLittleSquare = -1;
        }

        public bool SelectionRectHasArea()
        {
            return this.selectionRect.Width > 0 && this.selectionRect.Height > 0;
        }

        private void UpdateLittleSquares()
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
            this.ClearLittleSquares();
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

        private bool IsResizing()
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
        }

        private void StopMoveRect(Size imgSize)
        {
            this.isMovingRect = false;
            this.AdjustSelectedArea(imgSize);
        }

        private Point RoundPointByZoom(Point point, int zoomX)
        {
            if (zoomX > 1)
                return new Point((point.X / zoomX) * zoomX, (point.Y / zoomX) * zoomX);

            return point;
        }

        public void StartSelection(Point startPoint, int zoomX)
        {
            this.selectedLittleSquare = GetLittleSquareIndex(startPoint);

            if (this.IsResizing())
                this.startPoint = this.GetStartPointByLittleSquare();
            else
            {
                this.startPoint = this.RoundPointByZoom(startPoint, zoomX);

                if (this.selectionRect.Contains(this.startPoint.Value))
                    this.isMovingRect = true;
                else
                    this.selectionRect = Rectangle.Empty;
            }
        }

        public void UpdateSelection(Point endPoint, Size imgSize, int zoomX)
        {
            var roundedEndPoint = this.RoundPointByZoom(endPoint, zoomX);

            if (this.isMovingRect)
                this.MoveRect(roundedEndPoint);
            else
            {
                if (this.IsSelecting())
                    this.UpdateSelectionRect(roundedEndPoint, imgSize);

                if (this.IsResizing())
                    this.UpdateLittleSquares();
            }
        }

        public void EndSelection(Point endPoint, Size imgSize, int zoomX)
        {
            var roundedEndPoint = this.RoundPointByZoom(endPoint, zoomX);

            if (this.isMovingRect)
                this.StopMoveRect(imgSize);
            else if (this.IsSelecting())
                this.UpdateSelectionRect(roundedEndPoint, imgSize);

            if (this.SelectionRectHasArea())
                this.UpdateLittleSquares();
            else
                this.ClearLittleSquares();

            this.startPoint = null;
        }

        public void DrawSelectionRect(Graphics g)
        {
            if (this.SelectionRectHasArea())
            {
                this.DrawSelectionRectBorder(g);
                if (!this.IsSelecting() || this.IsResizing())
                {
                    this.DrawSelectionRectFill(g);
                    this.DrawLittleSquares(g);
                }
            }
        }
    }
}