using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Studio.Infrastructure.Commands
{
    public static class EditorCommands
    {
        public static readonly RoutedCommand SaveAll = new RoutedCommand("Save All", typeof(EditorCommands), new InputGestureCollection(new List<InputGesture> { new KeyGesture(Key.S, ModifierKeys.Shift | ModifierKeys.Control, "Ctrl+Shift+S") }));
    }
}
