using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Studio.Infrastructure.Commands
{
    public static class ProjectCommands
    {
        public static readonly RoutedCommand NewProject = new RoutedCommand("New Project", typeof(ProjectCommands));
        public static readonly RoutedCommand OpenProject = new RoutedCommand("Open Project", typeof(ProjectCommands));
        public static readonly RoutedCommand AddNewScene = new RoutedCommand("Add New Scene", typeof(ProjectCommands));
    }
}
