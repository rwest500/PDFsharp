// PDFsharp - A .NET library for processing PDF
// See the LICENSE file in the solution root for more information.

using PdfSharp.Diagnostics;
using PdfSharp.Drawing;

namespace PdfSharp.Charting.Renderers
{
    /// <summary>
    /// Represents an axis renderer used for charts of type Column2D or Line.
    /// </summary>
    class HorizontalXAxisRenderer : XAxisRenderer
    {
        /// <summary>
        /// Initializes a new instance of the HorizontalXAxisRenderer class with the specified renderer parameters.
        /// </summary>
        internal HorizontalXAxisRenderer(RendererParameters parms) : base(parms)
        { }

        /// <summary>
        /// Returns an initialized rendererInfo based on the X axis.
        /// </summary>
        internal override RendererInfo Init()
        {
            var chart = (Chart)_rendererParms.DrawingItem;
            
            var xari = new AxisRendererInfo
            {
                Axis = chart._xAxis
            };
            if (xari.Axis != null)
            {
                var cri = (ChartRendererInfo)_rendererParms.RendererInfo;

                bool isxy = false;
                Series s = chart.SeriesCollection[0];
                if (s != null && s.Elements.GetPointXY(0) != null)
                {
                    isxy = true;
                    InitScale(xari);
                    if (xari.Axis != null)
                    {
                        InitTickLabels(xari, cri.DefaultFont);
                    }
                }

                if( ! isxy )
                {
                    CalculateXAxisValues(xari);
                    InitTickLabels(xari, cri.DefaultFont);
                    InitXValues(xari);
                }
                InitAxisTitle(xari, cri.DefaultFont);
                InitAxisLineFormat(xari);
                InitGridlines(xari);
            }
            return xari;
        }

        /// <summary>
        /// Calculates the space used for the X axis.
        /// </summary>
        internal override void Format()
        {
            var xari = ((ChartRendererInfo)_rendererParms.RendererInfo).XAxisRendererInfo;
            if (xari?.Axis != null)
            {
                var atri = xari.AxisTitleRendererInfo;

                // Calculate space used for axis title.
                XSize titleSize = new XSize(0, 0);

                //if (atri is { AxisTitleText: { Length: > 0 } }) better readyble?
                if (atri != null && atri.AxisTitleText != null && atri.AxisTitleText.Length > 0)
                {
                    titleSize = _rendererParms.Graphics.MeasureString(atri.AxisTitleText, atri.AxisTitleFont);
                    atri.AxisTitleSize = titleSize;
                }

                // Calculate space used for tick labels.
                Chart chart = (Chart)this._rendererParms.DrawingItem;

                var size = new XSize(0, 0);
                bool isxy = false;
                Series s = chart.SeriesCollection[0];
                if (s != null && s.Elements.GetPointXY(0) != null)
                {
                    isxy = true;
                    // width of all ticklabels
                    double xMin = xari.MinimumScale;
                    double xMax = xari.MaximumScale;
                    double xMajorTick = xari.MajorTick;
                    double lineHeight = Double.MinValue;
                    XSize labelSize = new XSize(0, 0);
                    XGraphics gfx = this._rendererParms.Graphics;

                    for (double x = xMin; x <= xMax; x += xMajorTick)
                    {
                        string str = x.ToString(xari.TickLabelsFormat);
                        labelSize = gfx.MeasureString(str, xari.TickLabelsFont);
                        size.Width += labelSize.Width;
                        size.Height = Math.Max(size.Height, labelSize.Height);
                        lineHeight = Math.Max(lineHeight, labelSize.Height);
                    }

                    // add space for tickmarks
                    size.Width += xari.MajorTickMarkWidth * 1.5;
                }

                if( ! isxy )
                {
                    if (xari.XValues?.Count > 0)
                    {
                        XSeries xs = xari.XValues[0];
                        foreach (XValue xv in xs)
                        {
                            if (xv != null)
                            {
                                string tickLabel = xv.ValueField;
                                XSize valueSize = _rendererParms.Graphics.MeasureString(tickLabel, xari.TickLabelsFont);
                                size.Height = Math.Max(valueSize.Height, size.Height);
                                size.Width += valueSize.Width;
                            }
                        }
                    }

                    // Remember space for later drawing.
                    xari.TickLabelsHeight = size.Height;
                    xari.Height = titleSize.Height + size.Height + xari.MajorTickMarkWidth;
                    xari.Width = Math.Max(titleSize.Width, size.Width);
                }
            }
        }

        /// <summary>
        /// Draws the horizontal X axis.
        /// </summary>
        internal override void Draw()
        {
            var gfx = _rendererParms.Graphics;
            var cri = (ChartRendererInfo)_rendererParms.RendererInfo;
            var xari = cri.XAxisRendererInfo;
            if (xari == null)
                return;

            double xMin = xari.MinimumScale;
            double xMax = xari.MaximumScale;
            double xMajorTick = xari.MajorTick;
            double xMinorTick = xari.MinorTick;
            double xMaxExtension = xari.MajorTick;

            // Draw tick labels. Each tick label will be aligned centered.
            int countTickLabels = (int)xMax;
            double tickLabelStep = xari.Width;
            if (countTickLabels != 0)
                tickLabelStep = xari.Width / countTickLabels;

            //XPoint startPos = new XPoint(xari.X + tickLabelStep / 2, xari.Y + /*xari.TickLabelsHeight +*/ xari.MajorTickMarkWidth);
            XPoint startPos = new XPoint(xari.X + tickLabelStep / 2, xari.Y + xari.TickLabelsHeight);

            // Draw axis.
            // First draw tick marks, second draw axis.
            double majorTickMarkStart = 0, majorTickMarkEnd = 0,
                   minorTickMarkStart = 0, minorTickMarkEnd = 0;
            GetTickMarkPos(xari, ref majorTickMarkStart, ref majorTickMarkEnd, ref minorTickMarkStart, ref minorTickMarkEnd);

            bool isxy = false;
            var lineFormatRenderer = new LineFormatRenderer(gfx, xari.LineFormat);
            XPoint[] points = new XPoint[2];
            Chart chart = (Chart)this._rendererParms.DrawingItem;
            Series s = chart.SeriesCollection[0];

            if (s != null && s.Elements.GetPointXY(0) != null)
            {
                isxy = true;
                isxy = true;
                countTickLabels = (int)((xMax - xMin) / xMajorTick) + 1;
                if (countTickLabels != 0)
                    tickLabelStep = xari.Width / countTickLabels;
                startPos = new XPoint(0, xari.Y + xari.TickLabelsHeight);

                XMatrix matrix = new XMatrix();  //XMatrix.Identity;
                                                 //matrix.TranslatePrepend(xari.X, xari.InnerRect.Y ); //-xari.InnerRect.X, xMax);
                matrix.Scale(xari.InnerRect.Width / (xMax - xMin), 1, XMatrixOrder.Append);
                //matrix.ScalePrepend( 1, 1); // mirror Vertical
                matrix.Translate(xari.InnerRect.X, xari.InnerRect.Y, XMatrixOrder.Append);


                if (xari.MajorTickMark != TickMarkType.None)
                    startPos.Y += xari.MajorTickMarkWidth;

                //matrix.OffsetX = startPos.X;

                // Draw axis.
                // First draw tick marks, second draw axis.
                LineFormatRenderer minorTickMarkLineFormat = new LineFormatRenderer(gfx, xari.MinorTickMarkLineFormat);
                LineFormatRenderer majorTickMarkLineFormat = new LineFormatRenderer(gfx, xari.MajorTickMarkLineFormat);

                // Draw minor tick marks.
                if (xari.MinorTickMark != TickMarkType.None)
                {
                    for (double x = xMin + xMinorTick; x < xMax; x += xMinorTick)
                    {
                        points[0].X = x;
                        points[0].Y = minorTickMarkStart;
                        points[1].X = x;
                        points[1].Y = minorTickMarkEnd;
                        matrix.TransformPoints(points);
                        minorTickMarkLineFormat.DrawLine(points[0], points[1]);
                    }
                }

                double lineSpace = xari.TickLabelsFont.GetHeight(gfx);
                int cellSpace = xari.TickLabelsFont.FontFamily.GetLineSpacing(xari.TickLabelsFont.Style);
                double xHeight = xari.TickLabelsFont.Metrics.XHeight;

                XSize labelSize = new XSize(0, 0);
                labelSize.Height = lineSpace * xHeight / cellSpace;

                for (int i = 0; i < countTickLabels; ++i)
                {
                    double x = xMin + xMajorTick * i;
                    string str = x.ToString(xari.TickLabelsFormat);
                    labelSize = gfx.MeasureString(str, xari.TickLabelsFont);

                    // Draw major tick marks.
                    if (xari.MajorTickMark != TickMarkType.None)
                    {
                        labelSize.Width += xari.MajorTickMarkWidth * 1.5;
                        points[0].X = x;
                        points[0].Y = 0; // majorTickMarkStart;
                        points[1].X = x;
                        points[1].Y = 0; // majorTickMarkEnd;
                        matrix.TransformPoints(points);
                        points[1].Y += xari.MajorTickMarkWidth;

                        majorTickMarkLineFormat.DrawLine(points[0], points[1]);
                    }
                    else
                        labelSize.Height += SpaceBetweenLabelAndTickmark;

                    // Draw label text.
                    XPoint[] layoutText = new XPoint[1];
                    layoutText[0].X = x;
                    layoutText[0].Y = 0;
                    matrix.TransformPoints(layoutText);
                    layoutText[0].Y += labelSize.Height;
                    layoutText[0].X -= labelSize.Width / 2; // Center text horizontally
                    gfx.DrawString(str, xari.TickLabelsFont, xari.TickLabelsBrush, layoutText[0]);
                }

            }


            if (!isxy)
            {
                if (xari.MajorTickMark != TickMarkType.None)
                    startPos.Y += xari.MajorTickMarkWidth;
                foreach (var xs in (xari.XValues ?? throw new InvalidOperationException()).Cast<XSeries>()) // BUG???
                {
                    for (int idx = 0; idx < countTickLabels && idx < xs.Count; ++idx)
                    {
                        var xv = xs[idx];
                        if (xv != null!)
                        {
                            string tickLabel = xv.ValueField;
                            XSize size = gfx.MeasureString(tickLabel, xari.TickLabelsFont);
                            gfx.DrawString(tickLabel, xari.TickLabelsFont, xari.TickLabelsBrush, startPos.X - size.Width / 2, startPos.Y);
                        }
                        startPos.X += tickLabelStep;
                    }
                }

                // Minor ticks.
                if (xari.MinorTickMark != TickMarkType.None)
                {
                    int countMinorTickMarks = (int)(xMax / xMinorTick);
                    double minorTickMarkStep = xari.Width / countMinorTickMarks;
                    startPos.X = xari.X;
                    for (int x = 0; x <= countMinorTickMarks; x++)
                    {
                        points[0].X = startPos.X + minorTickMarkStep * x;
                        points[0].Y = minorTickMarkStart;
                        points[1].X = points[0].X;
                        points[1].Y = minorTickMarkEnd;
                        lineFormatRenderer.DrawLine(points[0], points[1]);
                    }
                }

                // Major ticks.
                if (xari.MajorTickMark != TickMarkType.None)
                {
                    int countMajorTickMarks = (int)(xMax / xMajorTick);
                    double majorTickMarkStep = xari.Width;
                    if (countMajorTickMarks != 0)
                        majorTickMarkStep = xari.Width / countMajorTickMarks;
                    startPos.X = xari.X;
                    for (int x = 0; x <= countMajorTickMarks; x++)
                    {
                        points[0].X = startPos.X + majorTickMarkStep * x;
                        points[0].Y = majorTickMarkStart;
                        points[1].X = points[0].X;
                        points[1].Y = majorTickMarkEnd;
                        lineFormatRenderer.DrawLine(points[0], points[1]);
                    }
                }
            }

            // Axis.
            if (xari.LineFormat != null)
            {
                points[0].X = xari.X;
                points[0].Y = xari.Y;
                points[1].X = xari.X + xari.Width;
                points[1].Y = xari.Y;
                if (xari.MajorTickMark != TickMarkType.None)
                {
                    points[0].X -= xari.LineFormat.Width / 2;
                    points[1].X += xari.LineFormat.Width / 2;
                }
                lineFormatRenderer.DrawLine(points[0], points[1]);
            }

            // Draw axis title.
            var atri = xari.AxisTitleRendererInfo;
            if (atri != null && atri.AxisTitleText != null && atri.AxisTitleText.Length > 0)
            {
                XRect rect = new XRect(xari.Rect.Right / 2 - atri.AxisTitleSize.Width / 2, xari.Rect.Bottom,
                                       atri.AxisTitleSize.Width, 0);
                gfx.DrawString(atri.AxisTitleText, atri.AxisTitleFont, atri.AxisTitleBrush, rect);
            }
        }

        /// <summary>
        /// Calculates the X axis describing values like minimum/maximum scale, major/minor tick and
        /// major/minor tick mark width.
        /// </summary>
        void CalculateXAxisValues(AxisRendererInfo rendererInfo)
        {
            // Calculates the maximum number of data points over all series.
            var seriesCollection = ((Chart?)rendererInfo.Axis?.Parent)?._seriesCollection ?? NRT.ThrowOnNull<SeriesCollection>();
            int count = 0;
            bool hasXY = false;
            double xMin = double.MaxValue;
            double xMax= double.MinValue;

            // Any series contain PointXY ?
            foreach (Series series in seriesCollection)
            {
                if (series.Elements.Count > 0)
                {
                    if ((series.Elements.GetPointXY(0)) != null)
                    {
                        hasXY = true;
                        foreach (PointXY xp in series.Elements)
                        {
                            if (!double.IsNaN(xp.ValueY))
                            {
                                xMin = Math.Min(xMin, xp.ValueX);
                                xMax = Math.Max(xMax, xp.ValueX);
                            }
                        }
                    }
                }
            }
            if (hasXY)
            {
                //double xWidth = .5;
                if (xMin != Double.MaxValue &&
                    xMax != Double.MinValue)
                {
                    FineTuneXAxis(rendererInfo, xMin, xMax); //Sets: Major/Minor Tick,Min/Max Scale
                }
            }
            else
            {
                xMin = 0; xMax = 0;
                foreach (Series series in seriesCollection)
                    count = Math.Max(count, series.Count);
                rendererInfo.MajorTick = 1;
                rendererInfo.MinorTick = 0.5;
                rendererInfo.MinimumScale = xMin;
                rendererInfo.MaximumScale = xMax; // At least 0
            }

            rendererInfo.MajorTickMarkWidth = DefaultMajorTickMarkWidth;
            rendererInfo.MinorTickMarkWidth = DefaultMinorTickMarkWidth;
        }

        /// <summary>
        /// Initializes the rendererInfo's xvalues. If not set by the user xvalues will be simply numbers
        /// from minimum scale + 1 to maximum scale.
        /// </summary>
        void InitXValues(AxisRendererInfo rendererInfo)
        {
            rendererInfo.XValues = ((Chart?)rendererInfo.Axis?.Parent)?._xValues;
            if (rendererInfo.XValues == null)
            {
                rendererInfo.XValues = new XValues();
                XSeries xs = rendererInfo.XValues.AddXSeries();

                for (double i = rendererInfo.MinimumScale + 1; i <= rendererInfo.MaximumScale; ++i)
                    xs.Add(i.ToString(rendererInfo.TickLabelsFormat));
            }
        }

        /// <summary>
        /// Calculates the starting and ending y position for the minor and major tick marks.
        /// </summary>
        void GetTickMarkPos(AxisRendererInfo rendererInfo,
                            ref double majorTickMarkStart, ref double majorTickMarkEnd,
                            ref double minorTickMarkStart, ref double minorTickMarkEnd)
        {
            double majorTickMarkWidth = rendererInfo.MajorTickMarkWidth;
            double minorTickMarkWidth = rendererInfo.MinorTickMarkWidth;
            XRect rect = rendererInfo.Rect;

            switch (rendererInfo.MajorTickMark)
            {
                case TickMarkType.Inside:
                    majorTickMarkStart = rect.Y;
                    majorTickMarkEnd = rect.Y - majorTickMarkWidth;
                    break;

                case TickMarkType.Outside:
                    majorTickMarkStart = rect.Y;
                    majorTickMarkEnd = rect.Y + majorTickMarkWidth;
                    break;

                case TickMarkType.Cross:
                    majorTickMarkStart = rect.Y + majorTickMarkWidth;
                    majorTickMarkEnd = rect.Y - majorTickMarkWidth;
                    break;

                case TickMarkType.None:
                    majorTickMarkStart = 0;
                    majorTickMarkEnd = 0;
                    break;
            }

            switch (rendererInfo.MinorTickMark)
            {
                case TickMarkType.Inside:
                    minorTickMarkStart = rect.Y;
                    minorTickMarkEnd = rect.Y - minorTickMarkWidth;
                    break;

                case TickMarkType.Outside:
                    minorTickMarkStart = rect.Y;
                    minorTickMarkEnd = rect.Y + minorTickMarkWidth;
                    break;

                case TickMarkType.Cross:
                    minorTickMarkStart = rect.Y + minorTickMarkWidth;
                    minorTickMarkEnd = rect.Y - minorTickMarkWidth;
                    break;

                case TickMarkType.None:
                    minorTickMarkStart = 0;
                    minorTickMarkEnd = 0;
                    break;
            }
        }
        /// <summary>
        /// Calculates all values necessary for scaling the axis like minimum/maximum scale or
        /// minor/major tick.
        /// </summary>
        private void InitScale(AxisRendererInfo rendererInfo)
        {
            double xMin, xMax;
            CalcXAxis(out xMin, out xMax);
            FineTuneXAxis(rendererInfo, xMin, xMax);

            rendererInfo.MajorTickMarkWidth = DefaultMajorTickMarkWidth;
            rendererInfo.MinorTickMarkWidth = DefaultMinorTickMarkWidth;
        }
        protected virtual void CalcXAxis(out double xMin, out double xMax)
        {
            xMin = double.MaxValue;
            xMax = double.MinValue;

            foreach (Series series in ((Chart)this._rendererParms.DrawingItem).SeriesCollection)
            {
                if (series.Elements.Count > 0)
                {
                    if ((series.Elements.GetPointXY(0)) != null)
                    {
                        foreach (PointXY xp in series.Elements)
                        {
                            if (!double.IsNaN(xp.ValueY))
                            {
                                xMin = Math.Min(xMin, xp.ValueX);
                                xMax = Math.Max(xMax, xp.ValueX);
                            }
                        }
                    }
                }
            }
        }

    }
}
