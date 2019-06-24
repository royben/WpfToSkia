using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using WpfToSkia.SkiaElements;
using WpfToSkia.ExtensionsMethods;
using WpfToSkia.Renderers;
using System.Windows.Shapes;

namespace WpfToSkia
{
    /// <summary>
    /// Represents a WPF compatible Skia host element.
    /// The visual tree of this element will be rendered using an alternative rendering system.
    /// </summary>
    /// <seealso cref="System.Windows.FrameworkElement" />
    [ContentProperty(nameof(Child))]
    public class SkiaHost : FrameworkElement
    {
        private Image _image;
        private bool _host_loaded;
        private ScrollViewer _scrollViewer;

        /// <summary>
        /// Gets or sets the first child element which will be used to analyze and map the visual tree.
        /// </summary>
        public FrameworkElement Child
        {
            get { return (FrameworkElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register("Child", typeof(FrameworkElement), typeof(SkiaHost), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the renderer which will be used to render the surface.
        /// </summary>
        public IRenderer Renderer
        {
            get { return (IRenderer)GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }
        public static readonly DependencyProperty RendererProperty =
            DependencyProperty.Register("Renderer", typeof(IRenderer), typeof(SkiaHost), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaHost"/> class.
        /// </summary>
        public SkiaHost()
        {
            Focusable = false;
            FocusVisualStyle = null;

            Renderer = new SkiaRenderer();

            _image = new Image() { Stretch = Stretch.None, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };

            if (!this.DesignMode())
            {
                AddVisualChild(_image);
                AddLogicalChild(_image);

                SkiaElementResolver.Default.Clear();
                SkiaElementResolver.Default.RegisterBinder<Border, SkiaBorder>();
                SkiaElementResolver.Default.RegisterBinder<TextBlock, SkiaTextBlock>();
                SkiaElementResolver.Default.RegisterBinder<ItemsControl, SkiaItemsControl>();
                SkiaElementResolver.Default.RegisterBinder<Rectangle, SkiaRectangle>();
                SkiaElementResolver.Default.RegisterBinder<ScrollViewer, SkiaScrollViewer>();
                SkiaElementResolver.Default.RegisterBinder<Ellipse, SkiaEllipse>();
                SkiaElementResolver.Default.RegisterBinder<Path, SkiaPath>();
                SkiaElementResolver.Default.RegisterBinder<Line, SkiaLine>();
                SkiaElementResolver.Default.RegisterBinder<Polygon, SkiaPolygon>();
                SkiaElementResolver.Default.RegisterBinder<Image, SkiaImage>();
            }

            Loaded += SkiaHost_Loaded;
        }

        /// <summary>
        /// Handles the Loaded event of the SkiaHost control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SkiaHost_Loaded(object sender, RoutedEventArgs e)
        {
            if (_host_loaded || this.ActualWidth == 0 || this.ActualHeight == 0 || Child == null) return;

            if (this.DesignMode())
            {
                AddVisualChild(Child);
                AddLogicalChild(Child);
            }
            else
            {
                _scrollViewer = this.FindAncestor<ScrollViewer>();
                Child.Opacity = 0;
                AddVisualChild(Child);
                AddLogicalChild(Child);

                Child.Loaded += (_, __) =>
                {
                    Renderer.Init(this);
                    Renderer.SourceChanged += (___, ____) => 
                    {
                        _image.Source = Renderer.Source;
                    };
                    _image.Source = Renderer.Source;
                    _host_loaded = true;

                    if (_scrollViewer != null)
                    {
                        _scrollViewer.ScrollChanged += (x, xx) =>
                        {
                            if (Renderer.IsVirtualizing)
                            {
                                _image.Margin = new Thickness(_scrollViewer.HorizontalOffset, _scrollViewer.VerticalOffset, 0, 0);
                            }
                        };
                    }
                };

                _host_loaded = true;
            }
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return this.DesignMode() ? 1 : 2;
            }
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)" />, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return <see langword="null" />; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            if (this.DesignMode())
            {
                return Child;
            }
            else
            {
                if (index == 0)
                {
                    return _image;
                }
                else
                {
                    return Child;
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement" />-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Child != null)
            {
                if (this.DesignMode())
                {
                    return Child.DesiredSize;
                }
                else
                {
                    Child.Measure(availableSize);
                    _image.Measure(availableSize);
                }
                return Child.DesiredSize;
            }
            else
            {
                return availableSize;
            }
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement" /> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Child != null)
            {
                if (this.DesignMode())
                {
                    Child.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
                }
                else
                {
                    Child.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
                    _image.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
                }
            }

            return finalSize;
        }
    }
}
