using System;
using System.Drawing;
using System.Windows.Forms;

namespace RealNews
{
    class myListBox : ListBox
    {
        public myListBox()
        {
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.MeasureItem += MyListBox_MeasureItem;
            this.MouseClick += MyListBox_MouseClick;
            this.KeyDown += MyListBox_KeyDown;
            this.DrawItem += MyListBox_DrawItem;
            this.DoubleBuffered = true;
        }
        public Color GroupColor { get; set; }
        public Color HighLightText { get; set; }

        public Action MoveNext { get; set; }

        private void MyListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (DesignMode)
                return;

            var lb = this;
            var fi = lb.Items[e.Index] as FeedItem;
            var t = fi.Title;
            var f = e.Font;
            //if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            //{
            //    e = new DrawItemEventArgs(e.Graphics,
            //                  e.Font,
            //                  e.Bounds,
            //                  e.Index,
            //                  e.State ^ DrawItemState.Selected,
            //                  e.ForeColor,
            //                  Color.DarkOrchid);//Choose the color
            //}

            e.DrawBackground();

            //var ItemMargin = 0;
            Brush myBrush = new SolidBrush(this.ForeColor);
            if (fi.isRead == false)
            {
                myBrush = new SolidBrush(Color.White);
                f = new Font(f, FontStyle.Bold);
            }
            var sf = StringFormat.GenericTypographic;
            sf.Trimming = StringTrimming.EllipsisCharacter;
            sf.LineAlignment = StringAlignment.Center;
            int l = 0;
            int w = 20;
            if (this.RightToLeft == RightToLeft.Yes)
            {
                sf = new StringFormat();
                sf.Alignment = StringAlignment.Far;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                l = 20;
                w = 0;
            }
            if (fi.Id == "")
            {
                var c = GroupColor;
                myBrush = new SolidBrush(c);
                sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawLine(new Pen(c), e.Bounds.Left + l, e.Bounds.Bottom - 5, e.Bounds.Width - w, e.Bounds.Bottom - 5);
            }
            var m = 10;
            var b = new Rectangle(e.Bounds.X + m, e.Bounds.Y, e.Bounds.Width - 2 * m, e.Bounds.Height);
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                // Selected. Draw with the system highlight color.
                e.Graphics.DrawString(t, e.Font, new SolidBrush(HighLightText), b, sf);
            }
            else
            {
                e.Graphics.DrawString(t, f, myBrush, b, sf);
            }

            e.DrawFocusRectangle();
        }

        private void MyListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Down || e.KeyCode == Keys.Up) && e.Shift == false && e.Alt == false)
            {
                var o = Items[SelectedIndex] as FeedItem;
                o.isRead = true;
                //MoveNext();
                //e.SuppressKeyPress = true;
            }

            EnsureVisible(10);
        }

        private void MyListBox_MouseClick(object sender, MouseEventArgs e)
        {
            var o = this.SelectedItem as FeedItem;
            if (o.Id == "")
            {
                // FIX : select all in group
            }
        }

        private void MyListBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (DesignMode)
                return;

            var lb = this;
            var fi = lb.Items[e.Index] as FeedItem;
            if (fi.Id == "")
            {
                e.ItemHeight += 12;
            }
        }

        public void EnsureVisible(int items)
        {
            int row = this.SelectedIndex;
            int visibleItems = this.ClientSize.Height / this.ItemHeight;
            int top = this.TopIndex;
            if (row - top > visibleItems - items)
                this.TopIndex = row - (visibleItems - items);
            this.SelectedIndex = row;
        }

        public void EnsureVisible(int index, int items)
        {
            int row = index;
            int visibleItems = this.ClientSize.Height / this.ItemHeight;
            int top = this.TopIndex;
            if (row - top > visibleItems - items)
                this.TopIndex = row - (visibleItems - items);
            this.SelectedIndex = row;
        }

        //public int FindItemWithText(string text)
        //{

        //}
    }
}
