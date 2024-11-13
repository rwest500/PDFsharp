// PDFsharp - A .NET library for processing PDF
// See the LICENSE file in the solution root for more information.

namespace PdfSharp.Charting
{
    /// <summary>
    /// Represents a formatted value on the data series.
    /// </summary>
    public class PointXY : ChartObject
    {
        /// <summary>
        /// Initializes a new instance of the Point class.
        /// </summary>
        internal PointXY()
        { }

        /// <summary>
        /// Initializes a new instance of the Point class with a real value.
        /// </summary>
        public PointXY(double x, double y) : this()
        {
            ValueX = x;
            ValueY = y;
        }

        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new PointXY Clone() 
            => (PointXY)DeepCopy();

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            var point = (PointXY)base.DeepCopy();
            if (point._lineFormat != null)
            {
                point._lineFormat = point._lineFormat.Clone();
                point._lineFormat.Parent = point;
            }
            if (point._fillFormat != null)
            {
                point._fillFormat = point._fillFormat.Clone();
                point._fillFormat.Parent = point;
            }
            return point;
        }

        /// <summary>
        /// Gets the line format of the data point's border.
        /// </summary>
        public LineFormat LineFormat => _lineFormat ??= new LineFormat(this);
        // ReSharper disable once InconsistentNaming because this is old code
        internal LineFormat? _lineFormat;

        /// <summary>
        /// Gets the filling format of the data point.
        /// </summary>
        public FillFormat FillFormat => _fillFormat ??= new FillFormat(this);
        // ReSharper disable once InconsistentNaming because this is old code
        internal FillFormat? _fillFormat;

        /// <summary>
        /// The actual value of the data point.
        /// </summary>
        public double ValueX { get; set; }
        public double ValueY { get; set; }
    }
}
