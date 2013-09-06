using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Caliburn.Micro;
using MethodInfoCache = System.Collections.Generic.Dictionary<System.Type, System.Collections.Generic.Dictionary<string, System.Reflection.MethodInfo>>;

namespace Studio.Infrastructure.Commands
{
    public class CommandExecutionInfo
    {
        private static MethodInfoCache _methodCache = new MethodInfoCache();

        public string MethodName { get; private set; }
        public object CommandTarget { get; private set; }
        public Func<bool> CanExecute { get; private set; }
        public System.Action Execute { get; private set; }
        public CommandExecutionInfo ParentExecutionInfo
        {
            get
            {
                var screen = CommandTarget as Screen;
                if (screen != null && screen.Parent != null)
                    return new CommandExecutionInfo(screen.Parent, MethodName);
                return null;
            }
        }
        public IEnumerable<CommandExecutionInfo> FullHierarchy
        {
            get
            {
                yield return this;
                if (ParentExecutionInfo != null)
                {
                    foreach (var info in ParentExecutionInfo.FullHierarchy)
                        yield return info;
                }
            }
        }

        public CommandExecutionInfo(object commandTarget, string methodName)
        {
            CommandTarget = commandTarget;
            MethodName = methodName;

            CanExecute = GetCanExecuteMethod(CommandTarget);
            Execute = GetExecuteMethod();
        }

        private Func<bool> GetCanExecuteMethod(object commandTarget)
        {
            var canMethodName = "Can" + MethodName;
            var method = GetMethod(MethodName, CommandTarget, CreateExecuteMethod);
            var canMethod = GetMethod(canMethodName, CommandTarget, CreateCanExecuteMethod);
            if (method != null && canMethod == null)
                return () => true;
            if (method == null || canMethod == null)
                return () => false;
            return () => (bool)canMethod.Invoke(commandTarget, null);
        }

        private System.Action GetExecuteMethod()
        {
            var method = GetMethod(MethodName, CommandTarget, CreateExecuteMethod);
            if (method == null)
                return null;
            return () => method.Invoke(CommandTarget, null);
        }

        private static MethodInfo GetMethod(string methodName, object viewModel, Func<string, Type, MethodInfo> createMethod)
        {
            var modelType = viewModel.GetType();

            if (!_methodCache.ContainsKey(modelType))
                _methodCache.Add(modelType, new Dictionary<string, MethodInfo>());
            if (!_methodCache[modelType].ContainsKey(methodName))
                _methodCache[modelType].Add(methodName, createMethod(methodName, modelType));

            return _methodCache[modelType][methodName];
        }

        private static MethodInfo CreateExecuteMethod(string methodName, Type modelType)
        {
            return
                modelType.GetMethods(BindingFlags.Public | BindingFlags.Instance).
                FirstOrDefault(m => m.Name == methodName && !m.GetParameters().Any());
        }

        private static MethodInfo CreateCanExecuteMethod(string methodName, Type modelType)
        {
            return
                modelType.GetMethods(BindingFlags.Public | BindingFlags.Instance).
                FirstOrDefault(m => m.Name == methodName && m.ReturnType == typeof(bool) && !m.GetParameters().Any());
        }
    }
}
