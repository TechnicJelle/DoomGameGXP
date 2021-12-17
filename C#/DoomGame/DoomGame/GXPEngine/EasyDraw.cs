using System.Drawing;
using System.Drawing.Text;

namespace GXPEngine 
{
	public enum CenterMode {Min, Center, Max}

	/// <summary>
	/// Creates an easy-to-use layer on top of .NET's System.Drawing methods.
	/// The API is inspired by Processing: internal states are maintained for font, fill/stroke color, etc., 
	/// and everything works with simple methods that have many overloads.
	/// </summary>
	public class EasyDraw : Canvas 
	{
		static Font defaultFont = new Font ("Noto Sans", 15);

		public CenterMode HorizontalTextAlign=CenterMode.Min;
		public CenterMode VerticalTextAlign=CenterMode.Max;
		public CenterMode HorizontalShapeAlign=CenterMode.Center;
		public CenterMode VerticalShapeAlign=CenterMode.Center;
		public Font font		{ get; protected set;}
		public Pen pen			{ get; protected set;}
		public SolidBrush brush	{ get; protected set;}
		protected bool _stroke=true;
		protected bool _fill=true;

		/// <summary>
		/// Creates a new EasyDraw canvas with the given width and height in pixels.
		/// </summary>
		/// <param name="width">width in pixels</param>
		/// <param name="height">height in pixels</param>
		/// <param name="addCollider">whether the canvas should have a collider</param>
		public EasyDraw (int width, int height, bool addCollider=true) : base (new Bitmap (width, height),addCollider)
		{
			Initialize ();
		}

		/// <summary>
		/// Creates a new EasyDraw canvas from a given bitmap.
		/// </summary>
		/// <param name="bitmap">The bitmap (image) that should be on the canvas</param>
		/// <param name="addCollider">whether the canvas should have a collider</param>
		public EasyDraw (System.Drawing.Bitmap bitmap, bool addCollider=true) : base (bitmap,addCollider)
		{
			Initialize ();
		}

		/// <summary>
		/// Creates a new EasyDraw canvas from a file that contains a sprite (png, jpg).
		/// </summary>
		/// <param name="filename">the name of the file that contains a sprite (png, jpg)</param>
		/// <param name="addCollider">whether the canvas should have a collider</param>
		public EasyDraw (string filename, bool addCollider=true) : base(filename,addCollider)
		{
			Initialize ();
		}

		void Initialize() 
		{
			pen = new Pen (Color.White, 1);
			brush = new SolidBrush (Color.White);
			font = defaultFont;
			if (!game.PixelArt) {
				graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit; //AntiAlias;
				graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			} else {
				graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
			}
		}

		//////////// Setting Font

		/// <summary>
		/// Change the font that is used when rendering text (using the Text method).
		/// </summary>
		/// <param name="newFont">The new font (see also Utils.LoadFont)</param>
		public void TextFont(Font newFont) 
		{
			font = newFont;
		}

		/// <summary>
		/// Change the font that is used when rendering text (using the Text method), using one of the available system fonts
		/// </summary>
		/// <param name="fontName">The name of the system font (e.g. "Arial", "Verdana", "Vivaldi")</param>
		/// <param name="pointSize">font size in points</param>
		/// <param name="style">font style (e.g. FontStyle.Italic|FontStyle.Bold )</param>
		public void TextFont(string fontName, float pointSize, FontStyle style = FontStyle.Regular) 
		{
			font = new Font (fontName, pointSize, style);
		}

		/// <summary>
		/// Change the size of the current font.
		/// </summary>
		/// <param name="pointSize">The font size in points</param>
		public void TextSize(float pointSize) 
		{
			font = new Font (font.OriginalFontName, pointSize, font.Style);
		}

		//////////// Setting Alignment for text, ellipses and rects
		
		/// <summary>
		/// Sets the horizontal and vertical alignment of text. 
		/// For instance, when choosing CenterMode.Min for both and calling Text, the x and y coordinates give the top left corner of the
		/// rendered text. 
		/// </summary>
		/// <param name="horizontal">Horizontal alignment</param>
		/// <param name="vertical">Vertical alignment</param>
		public void TextAlign(CenterMode horizontal, CenterMode vertical) 
		{
			HorizontalTextAlign = horizontal;
			VerticalTextAlign = vertical;
		}

		/// <summary>
		/// Sets the horizontal and vertical alignment of shapes.
		/// For instance, when choosing CenterMode.Min for both and calling Ellipse, 
		/// the x and y coordinates give the top left corner of the drawn ellipse.
		/// </summary>
		/// <param name="horizontal">Horizontal alignment</param>
		/// <param name="vertical">Vertical alignment</param>
		public void ShapeAlign(CenterMode horizontal, CenterMode vertical) 
		{
			HorizontalShapeAlign = horizontal;
			VerticalShapeAlign = vertical;
		}

		//////////// Setting Stroke

		/// <summary>
		/// Draw shapes without outline
		/// </summary>
		public void NoStroke() 
		{
			_stroke=false;
		}

		/// <summary>
		/// Set the outline color for drawing shapes
		/// </summary>
		/// <param name="newColor">the color of the outline</param>
		/// <param name="alpha">the opacity of the outline (from 0=transparent to 255=opaque)</param>
		public void Stroke(Color newColor, int alpha=255) 
		{
			pen.Color = Color.FromArgb (alpha, newColor);
			_stroke = true;
		}

		/// <summary>
		/// Set the outline color for drawing shapes to a grayscale value
		/// </summary>
		/// <param name="grayScale">A grayscale value (from 0=black to 255=white)</param>
		/// <param name="alpha">the opacity of the outline (from 0=transparent to 255=opaque)</param>
		public void Stroke(int grayScale, int alpha=255) 
		{
			pen.Color = Color.FromArgb (alpha, grayScale, grayScale, grayScale);
			_stroke = true;
		}

		/// <summary>
		/// Set the outline color for drawing shapes.
		/// </summary>
		/// <param name="red">The red value of the color (from 0 to 255)</param>
		/// <param name="green">The green value of the color (from 0 to 255)</param>
		/// <param name="blue">The blue value of the color (from 0 to 255)</param>
		/// <param name="alpha">The opacity of the outline (from 0=transparent to 255=opaque)</param>
		public void Stroke(int red, int green, int blue, int alpha=255) 
		{
			pen.Color = Color.FromArgb (alpha, red, green, blue);
			_stroke = true;
		}

		/// <summary>
		/// Sets the width of the outline for drawing shapes. (Default value: 1)
		/// </summary>
		/// <param name="width">The width (in pixels)</param>
		public void StrokeWeight(float width) 
		{
			pen.Width = width;
			_stroke = true;
		}

		//////////// Setting Fill

		/// <summary>
		/// Draw shapes without fill color.
		/// </summary>
		public void NoFill() 
		{
			_fill = false;
		}

		/// <summary>
		/// Set the fill color for drawing shapes and text.
		/// </summary>
		/// <param name="newColor">the fill color</param>
		/// <param name="alpha">the fill opacity (from 0=transparent to 255=opaque)</param>
		public void Fill(Color newColor, int alpha=255) 
		{
			brush.Color = Color.FromArgb (alpha, newColor);
			_fill = true;
		}

		/// <summary>
		/// Set the fill color for drawing shapes and text to a gray scale value.
		/// </summary>
		/// <param name="grayScale">gray scale value (from 0=black to 255=white)</param>
		/// <param name="alpha">the fill opacity (from 0=transparent to 255=opaque)</param>
		public void Fill(int grayScale, int alpha=255) 
		{
			brush.Color = Color.FromArgb (alpha, grayScale, grayScale, grayScale);
			_fill = true;
		}

		/// <summary>
		/// Set the fill color for drawing shapes and text.
		/// </summary>
		/// <param name="red">The red value of the color (from 0 to 255)</param>
		/// <param name="green">The green value of the color (from 0 to 255)</param>
		/// <param name="blue">The blue value of the color (from 0 to 255)</param>
		/// <param name="alpha">The fill opacity (from 0=transparent to 255=opaque)</param>
		public void Fill(int red, int green, int blue, int alpha=255) 
		{
			brush.Color = Color.FromArgb (alpha, red, green, blue);
			_fill = true;
		}

		//////////// Clear

		/// <summary>
		/// Clear the canvas with a given color
		/// </summary>
		/// <param name="newColor">the clear color</param>
		public void Clear(Color newColor) 
		{
			graphics.Clear (newColor);
		}

		/// <summary>
		/// Clear the canvas with a grayscale
		/// </summary>
		/// <param name="grayScale">the grayscale value (between 0=black and 255=white)</param>
		public void Clear(int grayScale) 
		{
			graphics.Clear(Color.FromArgb(255, grayScale, grayScale, grayScale));
		}

		/// <summary>
		/// Clear the canvas with a given color.
		/// </summary>
		/// <param name="red">The red value of the clear color (from 0 to 255)</param>
		/// <param name="green">The green value of the clear color (from 0 to 255)</param>
		/// <param name="blue">The blue value of the clear color (from 0 to 255)</param>
		/// <param name="alpha">The opacity of the clear color (from 0=transparent to 255=opaque)</param>
		public void Clear(int red, int green, int blue, int alpha=255) 
		{
			graphics.Clear(Color.FromArgb (alpha, red, green, blue));
		}

		/// <summary>
		/// Clear the canvas with a transparent color. 
		/// Note that this will fully clear the canvas, but will make the sprites behind the canvas visible.
		/// </summary>
		public void ClearTransparent() {
			graphics.Clear(Color.Transparent); // same as Clear(0,0,0,0);
		}

		//////////// Draw & measure Text

		/// <summary>
		/// Draw text on the canvas, using the currently selected font, at position x,y.
		/// This uses the current TextAlign values (e.g. if both are CenterMode.Center, (x,y) will be at the center of the rendered text).
		/// </summary>
		/// <param name="text">The text to be rendered</param>
		/// <param name="x">The x coordinate to draw the text, using canvas (pixel) coordinates</param>
		/// <param name="y">The y coordinate to draw the text, using canvas (pixel) coordinates</param>
		public void Text(string text, float x, float y) 
		{
			float twidth,theight;
			TextDimensions (text, out twidth, out theight);
			if (HorizontalTextAlign == CenterMode.Max) 
			{
				x -= twidth;
			} else if (HorizontalTextAlign == CenterMode.Center) 
			{ 
				x -= twidth / 2;
			}
			if (VerticalTextAlign == CenterMode.Max) 
			{
				y -= theight;
			} else if (VerticalTextAlign == CenterMode.Center) 
			{
				y -= theight / 2;
			}
			graphics.DrawString (text, font, brush, x, y); //left+BoundaryPadding/2,top+BoundaryPadding/2);
		}

		/// <summary>
		/// Draw text on the canvas, using the currently selected font.
		/// The text is aligned on the canvas using the current TextAlign values.
		/// </summary>
		/// <param name="text">The text to be rendered</param>
		/// <param name="clear">Whether the canvas should be cleared before drawing the text</param>
		/// <param name="clearAlpha">The opacity of the clear color (from 0=transparent to 255=opaque)</param>
		/// <param name="clearRed">The red value of the clear color (0-255)</param>
		/// <param name="clearGreen">The green value of the clear color (0-255)</param>
		/// <param name="clearBlue">The blue value of the clear color (0-255)</param>
		public void Text(string text, bool clear=false, int clearAlpha=0, int clearRed=0, int clearGreen=0, int clearBlue=0) {
			if (clear) Clear(clearRed, clearGreen, clearBlue, clearAlpha);
			float tx = 0;
			float ty = 0;
			switch (HorizontalTextAlign) {
				case CenterMode.Center:
					tx = _texture.width/2;
					break;
				case CenterMode.Max:
					tx = _texture.width;
					break;
			}
			switch (VerticalTextAlign) {
				case CenterMode.Center:
					ty = _texture.height / 2;
					break;
				case CenterMode.Max:
					ty = _texture.height;
					break;
			}
			Text(text, tx, ty);
		}

		/// <summary>
		/// Returns the width in pixels of a string, when rendered with the current font.
		/// </summary>
		/// <param name="text">input string</param>
		/// <returns>width in pixels</returns>
		public float TextWidth(string text) 
		{
			SizeF size = graphics.MeasureString (text, font);
			return size.Width;
		}

		/// <summary>
		/// Returns the height in pixels of a string, when rendered with the current font.
		/// </summary>
		/// <param name="text">input string</param>
		/// <returns>height in pixels</returns>
		public float TextHeight(string text) 
		{
			SizeF size = graphics.MeasureString (text, font);
			return size.Height;
		}

		/// <summary>
		/// Returns the width and height in pixels of a string, when rendered with the current font.
		/// </summary>
		/// <param name="text">input string</param>
		/// <param name="width">width in pixels</param>
		/// <param name="height">height in pixels</param>
		public void TextDimensions(string text, out float width, out float height) 
		{
			SizeF size = graphics.MeasureString (text, font);
			width = size.Width;
			height = size.Height;
		}

		//////////// Draw Shapes
		 
		/// <summary>
		/// Draw an (axis aligned) rectangle with given width and height, using the current stroke and fill settings. 
		/// Uses the current ShapeAlign values to position the rectangle relative to the point (x,y)
		/// </summary>
		/// <param name="x">x position in canvas coordinates</param>
		/// <param name="y">y position in canvas coordinates</param>
		/// <param name="width">width in pixels</param>
		/// <param name="height">height in pixels</param>
		public void Rect(float x, float y, float width, float height) {
			ShapeAlign (ref x, ref y, width, height);
			if (_fill) {
				graphics.FillRectangle (brush, x, y, width, height);
			}
			if (_stroke) {
				graphics.DrawRectangle (pen, x, y, width, height);
			}
		}

		/// <summary>
		/// Draw an (axis aligned) ellipse (or circle) with given width and height, using the current stroke and fill settings. 
		/// Uses the current ShapeAlign values to position the rectangle relative to the point (x,y)
		/// </summary>
		/// <param name="x">x position in canvas coordinates</param>
		/// <param name="y">y position in canvas coordinates</param>
		/// <param name="width">width in pixels</param>
		/// <param name="height">height in pixels</param>
		public void Ellipse(float x, float y, float width, float height) {
			ShapeAlign (ref x, ref y, width, height);
			if (_fill) {
				graphics.FillEllipse (brush, x, y, width, height);
			}
			if (_stroke) {
				graphics.DrawEllipse (pen, x, y, width, height);
			}
		}

		/// <summary>
		/// Draws an arc (=segment of an ellipse), where width and height give the ellipse size, and 
		/// start angle and sweep angle can be given (in degrees, clockwise). Uses the current stroke and fill settings.
		/// Uses the current ShapeAlign values to position the ellipse relative to the point (x,y)
		/// </summary>
		/// <param name="x">x position in canvas coordinates</param>
		/// <param name="y">y position in canvas coordinates</param>
		/// <param name="width">width in pixels</param>
		/// <param name="height">height in pixels</param>
		/// <param name="startAngleDegrees">angle in degrees (clockwise) to start drawing</param>
		/// <param name="sweepAngleDegrees">sweep angle in degrees, clockwise. Use e.g. 180 for a half-circle</param>
		public void Arc(float x, float y, float width, float height, float startAngleDegrees, float sweepAngleDegrees) {
			ShapeAlign (ref x, ref y, width, height);
			if (_fill) {
				graphics.FillPie (brush, x, y, width, height, startAngleDegrees, sweepAngleDegrees);
			}
			if (_stroke) {
				graphics.DrawArc (pen, x, y, width, height, startAngleDegrees, sweepAngleDegrees);
			}
		}

		/// <summary>
		/// Draw a line segment between two points, using the current stroke settings.
		/// </summary>
		/// <param name="x1">x coordinate of the start point</param>
		/// <param name="y1">y coordinate of the end point</param>
		/// <param name="x2">x coordinate of the start point</param>
		/// <param name="y2">y coordinate of the end point</param>
		public void Line(float x1, float y1, float x2, float y2) {
			if (_stroke) {
				graphics.DrawLine (pen, x1, y1, x2, y2);
			}
		}

		/// <summary>
		/// Draw a triangle between three points, using the current stroke and fill settings.
		/// </summary>
		public void Triangle(float x1, float y1, float x2, float y2, float x3, float y3) {
			Polygon(x1,y1,x2,y2,x3,y3);
		}

		/// <summary>
		/// Draw a quad (="deformed rectangle") between four points, using the current stroke and fill settings.
		/// </summary>
		public void Quad(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4) {
			Polygon(x1,y1,x2,y2,x3,y3,x4,y4);
		}

		/// <summary>
		/// Draw a polygon shape between any number of points, using the current stroke and fill settings. 
		/// This requires passing in an even number of float coordinates, 
		/// where the odd parameters are x coordinates and even parameters are y coordinates.
		/// </summary>
		public void Polygon(params float[] pt) {
			PointF[] pts = new PointF[pt.Length / 2];
			for (int i = 0; i < pts.Length; i++) {
				pts [i] = new PointF (pt [2 * i], pt [2 * i + 1]);
			}
			Polygon (pts);
		}

		/// <summary>
		/// Draw a polygon shape between any number of points, using the current stroke and fill settings. 
		/// </summary>
		public void Polygon(PointF[] pts) {
			if (_fill) {
				graphics.FillPolygon (brush, pts);
			}
			if (_stroke) {
				graphics.DrawPolygon (pen, pts);
			}
		}

		protected void ShapeAlign(ref float x, ref float y, float width, float height) {
			if (HorizontalShapeAlign == CenterMode.Max) 
			{
				x -= width;
			} else if (HorizontalShapeAlign == CenterMode.Center) 
			{ 
				x -= width / 2;
			}
			if (VerticalShapeAlign == CenterMode.Max) 
			{
				y -= height;
			} else if (VerticalShapeAlign == CenterMode.Center) 
			{
				y -= height / 2;
			}
		}
	}
}