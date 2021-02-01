namespace WpfCeoSeo.Adorners
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    ///     The adorner control.
    /// </summary>
    public partial class AdornerControl
    {
        /// <summary>
        ///     The _ child placement. 
        /// </summary>
        private readonly ChildPlacement childPlacement;

        /// <summary>
        ///     The child.
        /// </summary>
        private Control child;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AdornerControl" /> class.
        /// </summary>
        /// <param name="adornedElement">
        ///     The adorned element.
        /// </param>
        /// <param name="placement">
        ///     The PE placement.
        /// </param>
        public AdornerControl(UIElement adornedElement, ChildPlacement placement = ChildPlacement.Right)
            : base(adornedElement)
        {
            this.InitializeComponent();

            this.childPlacement = placement;
        }

        /// <summary>
        ///     Gets or sets the child.
        /// </summary>
        public Control Child
        {
            get => this.child;

            set
            {
                if (this.child != null)
                {
                    this.RemoveVisualChild(this.child);
                }

                this.child = value;

                if (this.child != null)
                {
                    this.AddVisualChild(this.child);
                }
            }
        }

        /// <summary>
        ///     Gets the visual children count.
        /// </summary>
        protected override int VisualChildrenCount => 1;

        /// <summary>
        ///     The arrange override.
        /// </summary>
        /// <param name="finalSize">
        ///     The final size.
        /// </param>
        /// <returns>
        ///     The <see cref="Size" />.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var position = new Point();
            position.X = 0;
            position.Y = 0;

            // AdornedElement.DesiredSize.Height / -2
            switch (this.childPlacement)
            {
                case ChildPlacement.Right:
                    position.X = this.AdornedElement.RenderSize.Width;
                    break;
                case ChildPlacement.Below:
                    position.Y = this.AdornedElement.RenderSize.Height;
                    break;
                case ChildPlacement.BelowTop:
                    var topPoint = this.AdornedElement.TranslatePoint(new Point(20, 30), this.AdornedElement);
                    position.X = topPoint.X;
                    position.Y = topPoint.Y;
                    break;
                case ChildPlacement.Middle:
                    position.X = (this.AdornedElement.RenderSize.Width - this.RenderSize.Width) / 2 ;
                    position.Y = (this.AdornedElement.RenderSize.Height - this.RenderSize.Height) / 2;
                    break;
            }

            this.child.Arrange(new Rect(position, finalSize));
            return new Size(this.child.ActualWidth, this.child.ActualHeight);
        }

        /// <summary>
        ///     The get visual child.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <returns>
        ///     The <see cref="Visual" />.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.child;
        }

        /// <summary>
        ///     The measure override.
        /// </summary>
        /// <param name="constraint">
        ///     The constraint.
        /// </param>
        /// <returns>
        ///     The <see cref="Size" />.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.child.Measure(constraint);
            return this.child.DesiredSize;
        }
    }
}