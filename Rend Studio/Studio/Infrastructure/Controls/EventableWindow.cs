using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Studio.Infrastructure.Commands;
using Studio.ViewModels;
using WpfShell.Commands;
using WpfShell.Controls;

namespace Studio.Infrastructure.Controls
{
    public class EventableWindow : MenuWindowBase
    {
        public EventableWindow()
        {
            AddHandler(CloseableTabItem.CloseTabEvent, new RoutedEventHandler(CloseTab));
            Closing += EventableWindow_Closing;
            Loaded += EventableWindow_Loaded;
        }

        private void CloseTab(object source, RoutedEventArgs args)
        {
            var scene = (args.OriginalSource as FrameworkElement).DataContext as SceneViewModel;
            var sceneEditor = (args.Source as FrameworkElement).DataContext as SceneEditorViewModel;

            sceneEditor.CloseItem(scene);
        }

        private void EventableWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var m = DataContext as ShellViewModel;
            m.SceneEditor.CanClose(canClose => e.Cancel = !canClose);
        }

        private void EventableWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MenuRoots.Add(new CommandRoot("Files", GetFilesCommands()));
        }

        private IEnumerable<IMenuCommandItem> GetFilesCommands()
        {
            yield return new CommandInfo(ProjectCommands.NewProject, BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Icons/box.png", UriKind.RelativeOrAbsolute)));
            yield return new CommandInfo(ProjectCommands.OpenProject, BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Icons/box.png", UriKind.RelativeOrAbsolute)));
            yield return new CommandInfo(ApplicationCommands.Save, BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Icons/save.png", UriKind.RelativeOrAbsolute)));
            yield return new CommandInfo(ApplicationCommands.SaveAs);
            yield return new CommandInfo(EditorCommands.SaveAll, BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Icons/save-all.png", UriKind.RelativeOrAbsolute)));
        }
    }
}
