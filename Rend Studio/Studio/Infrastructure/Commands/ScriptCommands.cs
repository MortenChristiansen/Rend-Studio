using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Studio.Infrastructure.Commands
{
    public static class ScriptCommands
    {
        public static readonly RoutedCommand Render = new RoutedCommand("Render scene", typeof(EditorCommands), new InputGestureCollection(new List<InputGesture> { new KeyGesture(Key.F5, ModifierKeys.None, "F5") }));
        public static readonly RoutedCommand ShowEditor = new RoutedCommand("Code", typeof(EditorCommands), new InputGestureCollection(new List<InputGesture> { new KeyGesture(Key.F11, ModifierKeys.None, "F11") }));
        public static readonly RoutedCommand ShowRenderings = new RoutedCommand("Rendered images", typeof(EditorCommands), new InputGestureCollection(new List<InputGesture> { new KeyGesture(Key.F12, ModifierKeys.None, "F12") }));
    }
}
