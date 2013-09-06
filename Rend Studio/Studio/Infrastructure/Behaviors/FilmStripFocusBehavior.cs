using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit.Editing;
using Studio.Infrastructure.Controls;

namespace Studio.Infrastructure.Behaviors
{
    /// <summary>
    /// A behavior that highjacks the keyboard focus of an
    /// element to the FilmStrip instance the behavior is
    /// attached to.
    /// </summary>
    public class FilmStripFocusBehavior : Behavior<FilmStrip>
    {
        private FilmStrip _associatedButton;

        protected override void OnAttached()
        {
            _associatedButton = AssociatedObject;

            FocusElement.GotKeyboardFocus += FocusElement_GotKeyboardFocus;
        }

        void FocusElement_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(e.NewFocus is TextArea))
                _associatedButton.Focus();
        }

        protected override void OnDetaching()
        {
            FocusElement.GotKeyboardFocus -= FocusElement_GotKeyboardFocus;
        }

        /// <summary>
        /// The FrameworkElement instance to highjack the keyboard
        /// focus from.
        /// </summary>
        public FrameworkElement FocusElement
        {
            get { return (FrameworkElement)GetValue(FocusElementProperty); }
            set { SetValue(FocusElementProperty, value); }
        }

        public static readonly DependencyProperty FocusElementProperty =
            DependencyProperty.Register("FocusElement", typeof(FrameworkElement), typeof(FilmStripFocusBehavior), new UIPropertyMetadata());
    }
}
