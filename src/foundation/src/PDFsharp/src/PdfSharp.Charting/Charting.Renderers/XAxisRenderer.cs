// PDFsharp - A .NET library for processing PDF
// See the LICENSE file in the solution root for more information.

namespace PdfSharp.Charting.Renderers
{
    /// <summary>
    /// Represents the base class for all X axis renderer.
    /// </summary>
    abstract class XAxisRenderer : AxisRenderer
    {
        /// <summary>
        /// Initializes a new instance of the XAxisRenderer class with the specified renderer parameters.
        /// </summary>
        internal XAxisRenderer(RendererParameters parameters) : base(parameters)
        { }

        /// <summary>
        /// Returns the default tick labels format string.
        /// </summary>
        protected override string GetDefaultTickLabelsFormat() 
            => "0";

        /// <summary>
        /// Calculates optimal minimum/maximum scale and minor/major tick based on yMin and yMax.
        /// </summary>
        protected void FineTuneXAxis(AxisRendererInfo rendererInfo, double xMin, double xMax)
        {
            if (xMin == double.MaxValue && xMax == double.MinValue)
            {
                // No series data given.
                xMin = 0.0f;
                xMax = 0.9f;
            }

            if (xMin == xMax)
            {
                if (xMin == 0)
                    xMax = 0.9f;
                else if (xMin < 0)
                    xMax = 0;
                else if (xMin > 0)
                    xMax = xMin + 1;
            }

            // If the ratio between xMax to xMin is more than 1.2, the smallest number will be set too zero.
            // It's Excel's behavior.
            if (xMin != 0)
            {
                if (xMin < 0 && xMax < 0)
                {
                    if (xMin / xMax >= 1.2)
                        xMax = 0;
                }
                else if (xMax / xMin >= 1.2)
                    xMin = 0;
            }

            double deltaYRaw = xMax - xMin;

            int digits = (int)(Math.Log(deltaYRaw, 10) + 1);
            double normed = deltaYRaw / Math.Pow(10, digits) * 10;

            double normedStepWidth = 1;
            if (normed < 2)
                normedStepWidth = 0.2f;
            else if (normed < 5)
                normedStepWidth = 0.5f;

            AxisRendererInfo xari = rendererInfo;
            double stepWidth = normedStepWidth * Math.Pow(10.0, digits - 1.0);
            if (xari.Axis == null || double.IsNaN(xari.Axis.MajorTick))
                xari.MajorTick = stepWidth;
            else
                xari.MajorTick = xari.Axis.MajorTick;

            double roundFactor = stepWidth * 0.5;
            if (xari.Axis == null || double.IsNaN(xari.Axis.MinimumScale))
            {
                double signumMin = (xMin != 0) ? xMin / Math.Abs(xMin) : 0;
                xari.MinimumScale = (int)(Math.Abs((xMin - roundFactor) / stepWidth) - (1 * signumMin)) * stepWidth * signumMin;
            }
            else
                xari.MinimumScale = xari.Axis.MinimumScale;

            if (xari.Axis == null || double.IsNaN(xari.Axis.MaximumScale))
            {
                double signumMax = (xMax != 0) ? xMax / Math.Abs(xMax) : 0;
                xari.MaximumScale = (int)(Math.Abs((xMax + roundFactor) / stepWidth) + (1 * signumMax)) * stepWidth * signumMax;
            }
            else
                xari.MaximumScale = xari.Axis.MaximumScale;

            if (xari.Axis == null || double.IsNaN(xari.Axis.MinorTick))
                xari.MinorTick = xari.MajorTick / 5;
            else
                xari.MinorTick = xari.Axis.MinorTick;
        }

    }

}
