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

    public Color NormalColor;
    public Color HighlightColor;

    public new string Text
    {
        get => isPlaceHolder ? string.Empty : base.Text;
        set => base.Text = value;
    }

    //when the control loses focus, the placeholder is shown
    public void setPlaceholder()
    {
        if (string.IsNullOrEmpty(base.Text))
        {
            base.Text = PlaceHolderText;
            this.ForeColor = NormalColor;
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
            this.ForeColor = NormalColor;
            this.Font = new Font(this.Font, FontStyle.Regular);
            isPlaceHolder = false;
        }
        else
        {
            this.ForeColor = HighlightColor;
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