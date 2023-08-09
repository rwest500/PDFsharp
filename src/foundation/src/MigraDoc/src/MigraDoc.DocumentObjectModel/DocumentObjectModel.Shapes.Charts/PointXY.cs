// MigraDoc - Creating Documents on the Fly
// See the LICENSE file in the solution root for more information.

namespace MigraDoc.DocumentObjectModel.Shapes.Charts
{
    /// <summary>
    /// Represents a formatted value on the data series.
    /// </summary>
    public class PointXY : ChartObject
    {
        /// <summary>
        /// Initializes a new instance of the Point class.
        /// </summary>
        public PointXY()
        {
            BaseValues = new PointXValues(this);
        }

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
            Point point = (Point)base.DeepCopy();
            if (point.Values.LineFormat is not null)
            {
                point.Values.LineFormat = point.Values.LineFormat.Clone();
                point.Values.LineFormat.Parent = point;
            }
            if (point.Values.FillFormat is not null)
            {
                point.Values.FillFormat = point.Values.FillFormat.Clone();
                point.Values.FillFormat.Parent = point;
            }
            return point;
        }

        /// <summary>
        /// Gets the line format of the data point's border.
        /// </summary>
        public LineFormat LineFormat
        {
            get => Values.LineFormat ??= new LineFormat(this);
            set
            {
                SetParent(value);
                Values.LineFormat = value;
            }
        }

        /// <summary>
        /// Gets the filling format of the data point.
        /// </summary>
        public FillFormat FillFormat
        {
            get => Values.FillFormat ??= new FillFormat(this);
            set
            {
                SetParent(value);
                Values.FillFormat = value;
            }
        }

        /// <summary>
        /// The actual value of X for the data point.
        /// </summary>
        public double ValueX
        {
            get => Values.ValueX ?? 0;
            set => Values.ValueX = value;
        }
        /// <summary>
        /// The actual value of Y for the data point.
        /// </summary>
        public double ValueY
        {
            get => Values.ValueY ?? 0;
            set => Values.ValueY = value;
        }

        /// <summary>
        /// Converts Point into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            if (!Values.LineFormat.IsValueNullOrEmpty() || !Values.FillFormat.IsValueNullOrEmpty())
            {
                serializer.WriteLine("");
                serializer.WriteLine("\\point");
                int pos = serializer.BeginAttributes();

                if (!Values.LineFormat.IsValueNullOrEmpty())
                    Values.LineFormat!.Serialize(serializer);
                if (!Values.FillFormat.IsValueNullOrEmpty())
                    Values.FillFormat!.Serialize(serializer);

                serializer.EndAttributes(pos);
                serializer.BeginContent();
                serializer.WriteLine(ValueX.ToString(System.Globalization.CultureInfo.InvariantCulture));
                serializer.Write(",");
                serializer.WriteLine(ValueY.ToString(System.Globalization.CultureInfo.InvariantCulture));
                serializer.EndContent();
            }
            else
            {
                serializer.Write(ValueX.ToString(System.Globalization.CultureInfo.InvariantCulture));
                serializer.Write(",");
                serializer.Write(ValueY.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            serializer.Write(", ");
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta => TheMeta;

        static readonly Meta TheMeta = new(typeof(Point));

        /// <summary>
        /// Gets the underlying internal nullable values for the properties of this document object.
        /// </summary>
        public new PointXValues Values => (PointXValues)BaseValues;

        /// <summary>
        /// Contains the internal nullable values for the properties of the enclosing document object class.
        /// </summary>
        public class PointXValues : ChartObjectValues
        {
            internal PointXValues(DocumentObject owner) : base(owner)
            { }

            /// <summary>
            /// Gets or sets the internal nullable implementation value of the enclosing document object property.
            /// See enclosing document object class for documentation of this property.
            /// </summary>
            public LineFormat? LineFormat { get; set; }

            /// <summary>
            /// Gets or sets the internal nullable implementation value of the enclosing document object property.
            /// See enclosing document object class for documentation of this property.
            /// </summary>
            public FillFormat? FillFormat { get; set; }

            /// <summary>
            /// Gets or sets the internal nullable implementation value of the enclosing document object property.
            /// See enclosing document object class for documentation of this property.
            /// </summary>
            public Double? ValueX { get; set; }
            public Double? ValueY { get; set; }
        }
    }
}
