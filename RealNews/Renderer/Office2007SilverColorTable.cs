using System.Drawing;
using System.Collections.Generic;
using System;

namespace BSE.Windows.Forms
{
    /// <summary>
    /// Provides colors used for Microsoft Office 2007 silver display elements.
    /// </summary>
    public class Office2007SilverColorTable : BSE.Windows.Forms.OfficeColorTable
    {
        //#region FieldsPrivate
        //private PanelColors m_panelColorTable;
        //#endregion

        //#region Properties
        ///// <summary>
        ///// Gets the associated ColorTable for the XPanderControls
        ///// </summary>
        //public override PanelColors PanelColorTable
        //{
        //    get
        //    {
        //        if (this.m_panelColorTable == null)
        //        {
        //            this.m_panelColorTable = new PanelColorsOffice2007Silver();
        //        }
        //        return this.m_panelColorTable;
        //    }
        //}
        //#endregion

        #region MethodsProtected
        /// <summary>
        /// Initializes a color dictionary with defined colors
        /// </summary>
        /// <param name="rgbTable">Dictionary with defined colors</param>
        protected override void InitColors(Dictionary<ProfessionalColorTable.KnownColors, Color> rgbTable)
        {
            rgbTable[KnownColors.ButtonPressedBorder] = Color.FromArgb(255, 189, 105);
            rgbTable[KnownColors.ButtonPressedGradientBegin] = Color.FromArgb(248, 181, 106);
            rgbTable[KnownColors.ButtonPressedGradientEnd] = Color.FromArgb(255, 208, 134);
            rgbTable[KnownColors.ButtonPressedGradientMiddle] = Color.FromArgb(251, 140, 60);
            rgbTable[KnownColors.ButtonSelectedBorder] = Color.FromArgb(255, 189, 105);
            rgbTable[KnownColors.ButtonSelectedGradientBegin] = Color.FromArgb(255, 245, 204);
            rgbTable[KnownColors.ButtonSelectedGradientEnd] = Color.FromArgb(255, 219, 117);
            rgbTable[KnownColors.ButtonSelectedGradientMiddle] = Color.FromArgb(255, 232, 116);
            rgbTable[KnownColors.ButtonSelectedHighlightBorder] = Color.FromArgb(255, 189, 105);
            rgbTable[KnownColors.CheckBackground] = Color.FromArgb(255, 227, 149);
            rgbTable[KnownColors.CheckSelectedBackground] = Color.FromArgb(254, 128, 62);
            rgbTable[KnownColors.GripDark] = Color.FromArgb(84, 84, 117);
            rgbTable[KnownColors.GripLight] = Color.FromArgb(255, 255, 255);
            rgbTable[KnownColors.ImageMarginGradientBegin] = Color.FromArgb(239, 239, 239);
            rgbTable[KnownColors.MenuBorder] = Color.FromArgb(124, 124, 148);
            rgbTable[KnownColors.MenuItemBorder] = Color.FromArgb(255, 189, 105);
            rgbTable[KnownColors.MenuItemPressedGradientBegin] = Color.FromArgb(232, 233, 241);
            rgbTable[KnownColors.MenuItemPressedGradientEnd] = Color.FromArgb(186, 185, 205);
            rgbTable[KnownColors.MenuItemPressedGradientMiddle] = Color.FromArgb(209, 209, 223);
            rgbTable[KnownColors.MenuItemSelected] = Color.FromArgb(255, 238, 194);
            rgbTable[KnownColors.MenuItemSelectedGradientBegin] = Color.FromArgb(255, 245, 204);
            rgbTable[KnownColors.MenuItemSelectedGradientEnd] = Color.FromArgb(255, 223, 132);
            rgbTable[KnownColors.MenuItemText] = Color.FromArgb(0, 0, 0);
            rgbTable[KnownColors.MenuStripGradientBegin] = Color.FromArgb(215, 215, 229);
            rgbTable[KnownColors.MenuStripGradientEnd] = Color.FromArgb(243, 243, 247);
            rgbTable[KnownColors.OverflowButtonGradientBegin] = Color.FromArgb(179, 178, 200);
            rgbTable[KnownColors.OverflowButtonGradientEnd] = Color.FromArgb(118, 116, 146);
            rgbTable[KnownColors.OverflowButtonGradientMiddle] = Color.FromArgb(152, 151, 177);
            rgbTable[KnownColors.RaftingContainerGradientBegin] = Color.FromArgb(215, 215, 229);
            rgbTable[KnownColors.RaftingContainerGradientEnd] = Color.FromArgb(243, 243, 247);
            rgbTable[KnownColors.SeparatorDark] = Color.FromArgb(110, 109, 143);
            rgbTable[KnownColors.SeparatorLight] = Color.FromArgb(255, 255, 255);
            rgbTable[KnownColors.StatusStripGradientBegin] = Color.FromArgb(235, 238, 250);
            rgbTable[KnownColors.StatusStripGradientEnd] = Color.FromArgb(197, 199, 209);
            rgbTable[KnownColors.StatusStripText] = Color.FromArgb(0, 0, 0);
            rgbTable[KnownColors.ToolStripBorder] = Color.FromArgb(124, 124, 148);
            rgbTable[KnownColors.ToolStripContentPanelGradientBegin] = Color.FromArgb(207, 211, 220);
            rgbTable[KnownColors.ToolStripContentPanelGradientEnd] = Color.FromArgb(155, 159, 166);
            rgbTable[KnownColors.ToolStripDropDownBackground] = Color.FromArgb(250, 250, 250);
            rgbTable[KnownColors.ToolStripGradientBegin] = Color.FromArgb(243, 244, 250);
            rgbTable[KnownColors.ToolStripGradientEnd] = Color.FromArgb(153, 151, 181);
            rgbTable[KnownColors.ToolStripGradientMiddle] = Color.FromArgb(218, 219, 231);
            rgbTable[KnownColors.ToolStripPanelGradientBegin] = Color.FromArgb(215, 215, 229);
            rgbTable[KnownColors.ToolStripPanelGradientEnd] = Color.FromArgb(243, 243, 247);
            rgbTable[KnownColors.ToolStripText] = Color.FromArgb(0, 0, 0);

        }
        #endregion
    }

    public class Office2007CustomColorTable : BSE.Windows.Forms.OfficeColorTable
    {
        //#region FieldsPrivate
        //private PanelColors m_panelColorTable;
        //#endregion

        //#region Properties
        ///// <summary>
        ///// Gets the associated ColorTable for the XPanderControls
        ///// </summary>
        //public override PanelColors PanelColorTable
        //{
        //    get
        //    {
        //        if (this.m_panelColorTable == null)
        //        {
        //            this.m_panelColorTable = new PanelColorsOffice2007Silver();
        //        }
        //        return this.m_panelColorTable;
        //    }
        //}
        //#endregion

        public Office2007CustomColorTable(Color tint)
        {
            _tint = tint;
        }

        private Color _tint = Color.Silver;

        #region [  hsb color change  ]
        /// <summary>
        /// Provides Round-trip conversion from RGB to HSB and back
        /// </summary>
        public struct HSBColor
        {
            float h;
            float s;
            float b;
            int a;

            public HSBColor(float h, float s, float b)
            {
                this.a = 0xff;
                this.h = Math.Min(Math.Max(h, 0), 255);
                this.s = Math.Min(Math.Max(s, 0), 255);
                this.b = Math.Min(Math.Max(b, 0), 255);
            }

            public HSBColor(int a, float h, float s, float b)
            {
                this.a = a;
                this.h = Math.Min(Math.Max(h, 0), 255);
                this.s = Math.Min(Math.Max(s, 0), 255);
                this.b = Math.Min(Math.Max(b, 0), 255);
            }

            public HSBColor(Color color)
            {
                HSBColor temp = FromColor(color);
                this.a = temp.a;
                this.h = temp.h;
                this.s = temp.s;
                this.b = temp.b;
            }

            public float H
            {
                get { return h; }
                set { h = value; }
            }

            public float S
            {
                get { return s; }
                set { s = value; }
            }

            public float B
            {
                get { return b; }
                set { b = value; }
            }

            public int A
            {
                get { return a; }
            }

            public Color Color
            {
                get
                {
                    return FromHSB(this);
                }
            }

            public static Color ShiftHue(Color c, float hueDelta)
            {
                HSBColor hsb = HSBColor.FromColor(c);
                hsb.h += hueDelta;
                hsb.h = Math.Min(Math.Max(hsb.h, 0), 255);
                return FromHSB(hsb);
            }

            public static Color ShiftSaturation(Color c, float saturationDelta)
            {
                HSBColor hsb = HSBColor.FromColor(c);
                hsb.s += saturationDelta;
                hsb.s = Math.Min(Math.Max(hsb.s, 0), 255);
                return FromHSB(hsb);
            }


            public static Color ShiftBrighness(Color c, float brightnessDelta)
            {
                HSBColor hsb = HSBColor.FromColor(c);
                hsb.b += brightnessDelta;
                hsb.b = Math.Min(Math.Max(hsb.b, 0), 255);
                return FromHSB(hsb);
            }

            public static Color FromHSB(HSBColor hsbColor)
            {
                float r = hsbColor.b;
                float g = hsbColor.b;
                float b = hsbColor.b;
                if (hsbColor.s != 0)
                {
                    float max = hsbColor.b;
                    float dif = hsbColor.b * hsbColor.s / 255f;
                    float min = hsbColor.b - dif;

                    float h = hsbColor.h * 360f / 255f;

                    if (h < 60f)
                    {
                        r = max;
                        g = h * dif / 60f + min;
                        b = min;
                    }
                    else if (h < 120f)
                    {
                        r = -(h - 120f) * dif / 60f + min;
                        g = max;
                        b = min;
                    }
                    else if (h < 180f)
                    {
                        r = min;
                        g = max;
                        b = (h - 120f) * dif / 60f + min;
                    }
                    else if (h < 240f)
                    {
                        r = min;
                        g = -(h - 240f) * dif / 60f + min;
                        b = max;
                    }
                    else if (h < 300f)
                    {
                        r = (h - 240f) * dif / 60f + min;
                        g = min;
                        b = max;
                    }
                    else if (h <= 360f)
                    {
                        r = max;
                        g = min;
                        b = -(h - 360f) * dif / 60 + min;
                    }
                    else
                    {
                        r = 0;
                        g = 0;
                        b = 0;
                    }
                }

                return Color.FromArgb
                    (
                        hsbColor.a,
                        (int)Math.Round(Math.Min(Math.Max(r, 0), 255)),
                        (int)Math.Round(Math.Min(Math.Max(g, 0), 255)),
                        (int)Math.Round(Math.Min(Math.Max(b, 0), 255))
                        );
            }

            public static HSBColor FromColor(Color color)
            {
                HSBColor ret = new HSBColor(0f, 0f, 0f);
                ret.a = color.A;

                float r = color.R;
                float g = color.G;
                float b = color.B;

                float max = Math.Max(r, Math.Max(g, b));

                if (max <= 0)
                {
                    return ret;
                }

                float min = Math.Min(r, Math.Min(g, b));
                float dif = max - min;

                if (max > min)
                {
                    if (g == max)
                    {
                        ret.h = (b - r) / dif * 60f + 120f;
                    }
                    else if (b == max)
                    {
                        ret.h = (r - g) / dif * 60f + 240f;
                    }
                    else if (b > g)
                    {
                        ret.h = (g - b) / dif * 60f + 360f;
                    }
                    else
                    {
                        ret.h = (g - b) / dif * 60f;
                    }
                    if (ret.h < 0)
                    {
                        ret.h = ret.h + 360f;
                    }
                }
                else
                {
                    ret.h = 0;
                }

                ret.h *= 255f / 360f;
                ret.s = (dif / max) * 255f;
                ret.b = max;

                return ret;
            }
        } 
        #endregion


        #region MethodsProtected
        /// <summary>
        /// Initializes a color dictionary with defined colors
        /// </summary>
        /// <param name="rgbTable">Dictionary with defined colors</param>
        protected override void InitColors(Dictionary<ProfessionalColorTable.KnownColors, Color> rgbTable)
        {
            rgbTable[KnownColors.ButtonPressedBorder] = Tint(Color.FromArgb(255, 189, 105));
            rgbTable[KnownColors.ButtonPressedGradientBegin] = Tint(Color.FromArgb(248, 181, 106));
            rgbTable[KnownColors.ButtonPressedGradientEnd] = Tint(Color.FromArgb(255, 208, 134));
            rgbTable[KnownColors.ButtonPressedGradientMiddle] = Tint(Color.FromArgb(251, 140, 60));
            rgbTable[KnownColors.ButtonSelectedBorder] = Tint(Color.FromArgb(255, 189, 105));
            rgbTable[KnownColors.ButtonSelectedGradientBegin] = Tint(Color.FromArgb(255, 245, 204));
            rgbTable[KnownColors.ButtonSelectedGradientEnd] = Tint(Color.FromArgb(255, 219, 117));
            rgbTable[KnownColors.ButtonSelectedGradientMiddle] = Tint(Color.FromArgb(255, 232, 116));
            rgbTable[KnownColors.ButtonSelectedHighlightBorder] = Tint(Color.FromArgb(255, 189, 105));
            rgbTable[KnownColors.CheckBackground] = Tint(Color.FromArgb(255, 227, 149));
            rgbTable[KnownColors.CheckSelectedBackground] = Tint(Color.FromArgb(254, 128, 62));
            rgbTable[KnownColors.GripDark] = Tint(Color.FromArgb(84, 84, 117));
            rgbTable[KnownColors.GripLight] = Tint(Color.FromArgb(255, 255, 255));
            rgbTable[KnownColors.ImageMarginGradientBegin] = Tint(Color.FromArgb(239, 239, 239));
            rgbTable[KnownColors.MenuBorder] = Tint(Color.FromArgb(124, 124, 148));
            rgbTable[KnownColors.MenuItemBorder] = Tint(Color.FromArgb(255, 189, 105));
            rgbTable[KnownColors.MenuItemPressedGradientBegin] = Tint(Color.FromArgb(232, 233, 241));
            rgbTable[KnownColors.MenuItemPressedGradientEnd] = Tint(Color.FromArgb(186, 185, 205));
            rgbTable[KnownColors.MenuItemPressedGradientMiddle] = Tint(Color.FromArgb(209, 209, 223));
            rgbTable[KnownColors.MenuItemSelected] = Tint(Color.FromArgb(255, 238, 194));
            rgbTable[KnownColors.MenuItemSelectedGradientBegin] = Tint(Color.FromArgb(255, 245, 204));
            rgbTable[KnownColors.MenuItemSelectedGradientEnd] = Tint(Color.FromArgb(255, 223, 132));
            rgbTable[KnownColors.MenuItemText] = Tint(Color.FromArgb(0, 0, 0));
            rgbTable[KnownColors.MenuStripGradientBegin] = Tint(Color.FromArgb(215, 215, 229));
            rgbTable[KnownColors.MenuStripGradientEnd] = Tint(Color.FromArgb(243, 243, 247));
            rgbTable[KnownColors.OverflowButtonGradientBegin] = Tint(Color.FromArgb(179, 178, 200));
            rgbTable[KnownColors.OverflowButtonGradientEnd] = Tint(Color.FromArgb(118, 116, 146));
            rgbTable[KnownColors.OverflowButtonGradientMiddle] = Tint(Color.FromArgb(152, 151, 177));
            rgbTable[KnownColors.RaftingContainerGradientBegin] = Tint(Color.FromArgb(215, 215, 229));
            rgbTable[KnownColors.RaftingContainerGradientEnd] = Tint(Color.FromArgb(243, 243, 247));
            rgbTable[KnownColors.SeparatorDark] = Tint(Color.FromArgb(110, 109, 143));
            rgbTable[KnownColors.SeparatorLight] = Tint(Color.FromArgb(255, 255, 255));
            rgbTable[KnownColors.StatusStripGradientBegin] = Tint(Color.FromArgb(235, 238, 250));
            rgbTable[KnownColors.StatusStripGradientEnd] = Tint(Color.FromArgb(197, 199, 209));
            rgbTable[KnownColors.StatusStripText] = Tint(Color.FromArgb(0, 0, 0));
            rgbTable[KnownColors.ToolStripBorder] = Tint(Color.FromArgb(124, 124, 148));
            rgbTable[KnownColors.ToolStripContentPanelGradientBegin] = Tint(Color.FromArgb(207, 211, 220));
            rgbTable[KnownColors.ToolStripContentPanelGradientEnd] = Tint(Color.FromArgb(155, 159, 166));
            rgbTable[KnownColors.ToolStripDropDownBackground] = Tint(Color.FromArgb(250, 250, 250));
            rgbTable[KnownColors.ToolStripGradientBegin] = Tint(Color.FromArgb(243, 244, 250));
            rgbTable[KnownColors.ToolStripGradientEnd] = Tint(Color.FromArgb(153, 151, 181));
            rgbTable[KnownColors.ToolStripGradientMiddle] = Tint(Color.FromArgb(218, 219, 231));
            rgbTable[KnownColors.ToolStripPanelGradientBegin] = Tint(Color.FromArgb(215, 215, 229));
            rgbTable[KnownColors.ToolStripPanelGradientEnd] = Tint(Color.FromArgb(243, 243, 247));
            rgbTable[KnownColors.ToolStripText] = Tint(Color.FromArgb(0, 0, 0));

        }


        public Color Tint(Color color)
        {
            HSBColor h = HSBColor.FromColor(color);
            HSBColor t = HSBColor.FromColor(_tint);
            h.H = t.H;
            //h.B -= 50;

            return HSBColor.FromHSB(h);
        }

        #endregion
    }
}
