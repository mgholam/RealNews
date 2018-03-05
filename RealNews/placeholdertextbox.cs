using System;
using System.Drawing;
using System.Windows.Forms;

public class PlaceHolderTextBox : TextBox
{

    bool isPlaceHolder = true;
    string _placeHolderText;
    public string PlaceHolderText
    {
        get { return _placeHolderText; }
        set
        {
            _placeHolderText = value;
            setPlaceholder();
        }
    }

    public new string Text
    {
        get => isPlaceHolder ? string.Empty : base.Text;
        set => base.Text = value;
    }

    //when the control loses focus, the placeholder is shown
    private void setPlaceholder()
    {
        if (string.IsNullOrEmpty(base.Text))
        {
            base.Text = PlaceHolderText;
            this.ForeColor = Color.Gray;
            this.Font = new Font(this.Font, FontStyle.Italic);
            isPlaceHolder = true;
        }
    }

    //when the control is focused, the placeholder is removed
    private void removePlaceHolder()
    {

        if (isPlaceHolder)
        {
            base.Text = "";
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Font = new Font(this.Font, FontStyle.Regular);
            isPlaceHolder = false;
        }
    }
    public PlaceHolderTextBox()
    {
        GotFocus += removePlaceHolder;
        LostFocus += setPlaceholder;
    }

    private void setPlaceholder(object sender, EventArgs e)
    {
        setPlaceholder();
    }

    private void removePlaceHolder(object sender, EventArgs e)
    {
        removePlaceHolder();
    }
}