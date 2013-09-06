using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Studio.Infrastructure.Commands
{
    public static class CommandBehaviors
    {
        #region Execute

        public static string GetExecute(CommandBinding binding) { return ""; }

        public static void SetExecute(CommandBinding binding, string methodName)
        {
            var canMethodName = "Can" + methodName;

            binding.Executed += (o, e) =>
            {
                var viewModel = (e.Source as FrameworkElement).DataContext;

                var commandExecutionInfo = new CommandExecutionInfo(viewModel, methodName);
                var firstExecutable = commandExecutionInfo.FullHierarchy.FirstOrDefault(i => i.CanExecute());
                if (firstExecutable != null)
                {
                    e.Handled = true;
                    firstExecutable.Execute();
                }
            };

            binding.CanExecute += (o, e) =>
            {
                var viewModel = (e.Source as FrameworkElement).DataContext;

                var commandExecutionInfo = new CommandExecutionInfo(viewModel, methodName);
                e.CanExecute = commandExecutionInfo.FullHierarchy.Any(i => i.CanExecute());
            };
        }

        public static readonly DependencyProperty ExecuteProperty =
            DependencyProperty.RegisterAttached("Execute", typeof(string), typeof(CommandBehaviors), new UIPropertyMetadata(""));

        #endregion

        #region Handles Command

        public static readonly DependencyProperty HandlesCommandProperty =
            DependencyProperty.RegisterAttached("HandlesCommand", typeof(RoutedCommand), typeof(CommandBehaviors), new UIPropertyMetadata(OnHandlesCommandChanged));

        private static void OnHandlesCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;
            var command = e.NewValue as RoutedCommand;

            SetHandlesCommand(element, command);
        }

        public static RoutedCommand GetHandlesCommand(UIElement element) { return null; }

        public static void SetHandlesCommand(UIElement element, RoutedCommand command)
        {
            var binding = new CommandBinding(command);
            binding.CanExecute += (o, e) => e.CanExecute = true;
            binding.Executed += (o, e) => MessageBox.Show("Renamed");
            element.CommandBindings.Add(binding);
        }

        #endregion
    }
}
