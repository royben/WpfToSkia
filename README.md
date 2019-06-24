# WpfToSkia
This is an attempt of replacing WPF's rendering engine using Skia.

### Why
The benefits of using this library are mainly performance improvements while rendering many elements on the screen.
This library also adds the possibility for fast real-time recording of anything that happens in the window for video editing/streaming purposes. Of course, this can also be achieved by using RenderTargetBitmap, but will be very slow.


### How it works
The library contains a **SkiaHost** WPF container element that can be declared anywhere in your xaml.
Once the SkiaHost element is loaded, the child element and all its descendants are made invisible by settings their Opacity to zero. Settings the opacity to zero bypasses all rendering of those elements while preserving hit testing, in contrasts to settings the Visibility to “Hidden”. This also lets WPF measure and arrange pass to work as expected.

Once WPF’s Visual Tree is loaded properly the library starts to traverse the tree and map it to its own **SkiaTree**.
The process of mapping elements is basically measuring each element to understand its bounds and registering for property changed that might affect its size, position or visual representation.

All WPF elements are mapped by default to the basic **SkiaFrameworkElement** unless a specific mapping was registered through the **SkiaElementResolver**.

Most elements do not require any mapping as they do not have any visual representation and do not require any custom rendering or layout. Elements like Grid, StackPanel, Canvas, ContentPresenter do not have any mappings.

The below are elements that currently have mappings.
-	**Border**
-	**Ellipse**
-	**Image**
-	**ItemsControl**
-	**Line**
-	**Path**
-	**Polygon**
-	**Rectangle**
-	**ScrollViewer**
-	**TextBlock**


### Property Changes and Animations
Each Skia element defines a set of dependency properties which might affect its size, position or visual representation. Each property definition specifies whether it affects the bounds or visual of the element.
Once the SkiaTree is finished loading, the library starts to traverse the tree and create an event notification container for each element property definition.

**Layout Properties**
<br/>
A property defined as “Layout” will trigger the invalidation of its element parent bounds and drawings.

**Render Properties**
<br/>
A property defined as “Render” will only trigger its element re-drawings. 

Redrawing of elements is optimized to by using clipped dirty rects on the source bitmap.

### Scrolling and Virtualization
By default, the Skia Renderer will draw all the elements to the source WriteableBitmap, unless the size of the required bitmap exceeds the size defined by the “MaximumBitmapSize” property which is set to 4000 * 4000 by default.

Once the required bitmap size exceeds the maximum size, which usually occurs when the SkiaHost element is inside a ScollViewer and contains many elements, the Renderer will enter a virtualization mode where it renders only the content visible inside the parent ScrollViewer viewport. 

The Skia Renderer is very fast at rendering many elements but they might take a lot of time to be measured by WPF’s measuring pass before they are loaded. That is why WPF has a virtualization technique where a VirtualizedStackPanel can be placed as an ItemsSource control panel template and a ScrollViewer as a parent. SkiaItemsControl element can detects this kind of behavior and notify the renderer when child elements are added/removed (virtualized) and a Tree injection/ejection/render call is required.

### Usage Example

```xaml
  
<skia:SkiaHost>
    <Border>
        <Canvas>
            <Ellipse Canvas.Left="100" Canvas.Top="100" Width="100" Height="100" Fill="Red" Stroke="Black" StrokeThickness="2" RenderTransformOrigin="0.5,0.5">
                <Ellipse.RenderTransform>
                    <ScaleTransform ScaleX="1" ScaleY="1" />
                </Ellipse.RenderTransform>
                <Ellipse.Triggers>
                    <EventTrigger RoutedEvent="MouseEnter">
                        <BeginStoryboard>
                            <Storyboard>
                                <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleX" To="1.5" Duration="00:00:0.3"></DoubleAnimation>
                                <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleY" To="1.5" Duration="00:00:0.3"></DoubleAnimation>
                            </Storyboard>
                        </BeginStoryboard>
                    </EventTrigger>
                    <EventTrigger RoutedEvent="MouseLeave">
                        <BeginStoryboard>
                            <Storyboard>
                                <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleX" To="1" Duration="00:00:0.3"></DoubleAnimation>
                                <DoubleAnimation Storyboard.TargetProperty="RenderTransform.ScaleY" To="1" Duration="00:00:0.3"></DoubleAnimation>
                            </Storyboard>
                        </BeginStoryboard>
                    </EventTrigger>
                </Ellipse.Triggers>
            </Ellipse>
        </Canvas>
    </Border>
</skia:SkiaHost>
  
  ```
