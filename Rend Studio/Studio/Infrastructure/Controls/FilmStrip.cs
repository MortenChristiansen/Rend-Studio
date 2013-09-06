using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Studio.Infrastructure.Commands;

namespace Studio.Infrastructure.Controls
{
    public class FilmStrip : ListBox
    {
        private static bool _hasOverriddenMetadata = false;

        private double _leftOffset;
        private double? _minOffset;
        private IEnumerable<FrameworkElement> _itemContainers;
        private FrameworkElement _selectedItemContainer;

        private IEnumerable<FrameworkElement> ItemContainers
        {
            get
            {
                if (_itemContainers == null)
                    _itemContainers = Items.Cast<object>().Select(item => ItemContainerGenerator.ContainerFromItem(item)).Cast<FrameworkElement>();
                return _itemContainers;
            }
        }

        private FrameworkElement SelectedItemContainer
        {
            get
            {
                if (_selectedItemContainer == null)
                    _selectedItemContainer = ItemContainerGenerator.ContainerFromItem(SelectedItem) as FrameworkElement;
                return _selectedItemContainer;
            }
        }

        public FilmStrip()
        {
            if (!_hasOverriddenMetadata)
            {
                DefaultStyleKeyProperty.OverrideMetadata(typeof(FilmStrip),
                    new FrameworkPropertyMetadata(typeof(FilmStrip)));
                _hasOverriddenMetadata = true;
            }

            MouseWheel += FilmStrip_MouseWheel;
            SelectionChanged += FilmStrip_SelectionChanged;
            Loaded += FilmStrip_Loaded;
            MouseLeftButtonUp += FilmStrip_MouseLeftButtonUp;
        }

        private void FilmStrip_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // When clicking any thumbnail, activate the ShowRenderings mode
            if (e.OriginalSource is Image)
            {
                if (ScriptCommands.ShowRenderings.CanExecute(null, this))
                    ScriptCommands.ShowRenderings.Execute(null, this);
            }
        }

        private void FilmStrip_Loaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).SizeChanged += FilmStrip_SizeChanged;

            var items = Items.SourceCollection as INotifyCollectionChanged;
            items.CollectionChanged += items_CollectionChanged;
            
            // Start with the list scrolled to the right
            CalculateMinOffset();
            var offset = Math.Min(0, _minOffset.Value);
            Margin = new Thickness(offset, Margin.Top, Margin.Right, Margin.Bottom);

            // Start with the last item selected
            SelectedIndex = Items.Count - 1;
        }

        private void items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Clear items cache when collection changes
            _itemContainers = null;
            CalculateMinOffset();

            // Set last added item, if any, to be selected item
            if (e.NewItems.Count > 0)
            {
                Items.MoveCurrentToLast();
                Focus();
            }
        }

        private void FilmStrip_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            SetOffset(e.Delta);
        }

        private void SetOffset(double delta)
        {
            var maxOffset = 0;
            if (!_minOffset.HasValue)
                CalculateMinOffset();
            var targetOffset = _leftOffset + delta;
            _leftOffset = Math.Min(maxOffset, Math.Max(_minOffset.Value, targetOffset));
            var offset = new Thickness(_leftOffset, Margin.Top, Margin.Right, Margin.Bottom);
            BeginAnimation(MarginProperty, new ThicknessAnimation(offset, new Duration(TimeSpan.FromSeconds(0.6))), HandoffBehavior.Compose);
        }

        private void CalculateMinOffset()
        {
            var itemWidth = ItemContainers.Any() ? ItemContainers.Max(c => c.RenderSize.Width) : 0;
            _minOffset = Math.Min(RenderSize.Width, DesiredSize.Width) - itemWidth * ItemContainers.Count() - 26;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == MarginProperty || e.Property == VisibilityProperty)
                UpdateClipping();
        }

        private void FilmStrip_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                var delta = e.NewSize.Width - e.PreviousSize.Width;
                _minOffset += delta;
            }
            UpdateClipping();
        }

        public new void Focus()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                Keyboard.Focus(this.SelectedItemContainer);
            }));
        }

        private void UpdateClipping()
        {
            Clip = new RectangleGeometry(new Rect(-Margin.Left, 0, RenderSize.Width, RenderSize.Height));
        }

        private void FilmStrip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear cached object
            _selectedItemContainer = null;
            _itemContainers = null;

            if (e.AddedItems.Count > 0 && Items.Contains(e.AddedItems[0]))
                AnimateSelectedItemIntoView();
        }

        private void AnimateSelectedItemIntoView()
        {
            if (SelectedItemContainer == null) return;

            CalculateMinOffset();

            var offset = 12d; // For margin
            var itemWidth = ItemContainers.Max(c => c.RenderSize.Width);
            offset += ItemContainers.TakeWhile(c => c != SelectedItemContainer).Count() * itemWidth;

            if (offset < -_leftOffset)
            {
                SetOffset(-_leftOffset - offset + 12);
            }
            else if (offset + itemWidth > RenderSize.Width)
            {
                SetOffset(RenderSize.Width - (offset + itemWidth) - 12);
            }
        }
    }
}