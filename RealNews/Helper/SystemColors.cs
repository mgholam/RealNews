using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RealNews
{
	public class ColorSchemeController
	{
		public static event Action SystemColorsChanging;
		public static event Action SystemColorsChanged;

		public ColorSchemeController()
		{
			// force init color table
			byte unused = SystemColors.Window.R;

			var systemDrawingAssembly = typeof(Color).Assembly;

			_colorTableField = systemDrawingAssembly.GetType("System.Drawing.KnownColorTable")
				.GetField("colorTable", BindingFlags.Static | BindingFlags.NonPublic);

			_colorTable = readColorTable();

			OriginalColors = _colorTable.ToArray();

			_threadDataProperty = systemDrawingAssembly.GetType("System.Drawing.SafeNativeMethods")
				.GetNestedType("Gdip", BindingFlags.NonPublic)
				.GetProperty("ThreadData", BindingFlags.Static | BindingFlags.NonPublic);

			SystemBrushesKey = typeof(SystemBrushes)
				.GetField("SystemBrushesKey", BindingFlags.Static | BindingFlags.NonPublic)
				.GetValue(null);

			SystemPensKey = typeof(SystemPens)
				.GetField("SystemPensKey", BindingFlags.Static | BindingFlags.NonPublic)
				.GetValue(null);
		}


		private static void fireColorsChangedEvents()
		{
			SystemColorsChanging?.Invoke();
			SystemColorsChanged?.Invoke();
		}

		private int[] readColorTable() =>
			(int[])_colorTableField.GetValue(null);

		public void SetColor(KnownColor knownColor, int argb)
		{
			setColor(knownColor, argb);

			fireColorsChangedEvents();
		}

		private void setColor(KnownColor knownColor, int argb) =>
			_colorTable[(int)knownColor] = argb;

		public int GetOriginalColor(KnownColor knownColor) =>
			OriginalColors[(int)knownColor];

		public int GetColor(KnownColor knownColor)
		{
			if (!KnownColors.Contains(knownColor))
				throw new ArgumentException();

			return _colorTable[(int)knownColor];
		}


		public void Reset(KnownColor color) =>
			SetColor(color, OriginalColors[(int)color]);


		private object SystemBrushesKey { get; }
		private object SystemPensKey { get; }

		public readonly HashSet<KnownColor> KnownColors = new HashSet<KnownColor>(
			new[]
			{
				SystemColors.Control,
				SystemColors.ControlText,

				SystemColors.ButtonFace, // menu gradient
				SystemColors.ButtonShadow, // menu border

				SystemColors.Window,
				SystemColors.WindowText,
				SystemColors.GrayText,

				SystemColors.HotTrack,
				SystemColors.Highlight,
				SystemColors.HighlightText,

				SystemColors.ActiveCaption,
				SystemColors.GradientActiveCaption,

				SystemColors.InactiveCaption,
				SystemColors.GradientInactiveCaption,

				SystemColors.ActiveBorder
			}.Select(_ => _.ToKnownColor())
		);

		private int[] OriginalColors { get; }

		private int[] _colorTable;
		private readonly FieldInfo _colorTableField;
		private readonly PropertyInfo _threadDataProperty;
	}
}
