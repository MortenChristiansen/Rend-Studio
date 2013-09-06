using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Studio.Infrastructure.Commands
{
    public static class AssetCommands
    {
        public static readonly RoutedCommand Rename = new RoutedCommand("Rename", typeof(AssetCommands), new InputGestureCollection(new List<InputGesture> { new KeyGesture(Key.F2, ModifierKeys.None, "F2") }));
    }
}
