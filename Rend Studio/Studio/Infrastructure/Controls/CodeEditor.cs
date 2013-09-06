using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.Scripting;

namespace Studio.Infrastructure.Controls
{
    public class CodeEditor : TextEditor
    {
        private static IList<Rect> _errorRectangles = new List<Rect>();
        private ToolTip _toolTip = new ToolTip { Placement = PlacementMode.Mouse };

        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(string), typeof(CodeEditor), new UIPropertyMetadata("", new PropertyChangedCallback(OnCodeChanged)));

        private static void OnCodeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // We only update this when loading existing code to begin with
            var editor = sender as CodeEditor;
            if (editor.Text == "" && (e.NewValue as string) != "")
                editor.Text = (string)e.NewValue;
        }

        public SyntaxErrorException SyntaxError
        {
            get { return (SyntaxErrorException)GetValue(SyntaxErrorProperty); }
            set { SetValue(SyntaxErrorProperty, value); }
        }

        public static readonly DependencyProperty SyntaxErrorProperty =
            DependencyProperty.Register("SyntaxError", typeof(SyntaxErrorException), typeof(CodeEditor), new UIPropertyMetadata(null, new PropertyChangedCallback(OnSyntaxErrorChanged)));

        private static void OnSyntaxErrorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var editor = sender as CodeEditor;
            var syntaxError = e.NewValue as SyntaxErrorException;
            if (syntaxError != null)
            {
                _errorRectangles.Clear();
                editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
            }
        }

        public bool AutoFocus { get; set; }

        public CodeEditor()
        {
            Initialized += CodeEditor_Initialized;
            TextChanged += CodeEditor_TextChanged;
            MouseHover += CodeEditor_MouseHover;

            TextArea.TextView.BackgroundRenderers.Add(new SyntaxErrorHighlighter(this));
        }

        private void CodeEditor_MouseHover(object sender, MouseEventArgs e)
        {
            if (SyntaxError == null)
                return;

            if (IsMouseOverErrorRectangles(e))
            {
                e.Handled = true;
                _toolTip.Content = "Syntax error: " + SyntaxError.Message;
                _toolTip.IsOpen = true;
                MouseMove += CodeEditor_MouseMove;
            }
        }

        void CodeEditor_MouseMove(object sender, MouseEventArgs e)
        {
            if (_toolTip == null || !_toolTip.IsOpen)
                return;

            if (!IsMouseOverErrorRectangles(e))
            {
                _toolTip.IsOpen = false;
                MouseMove -= CodeEditor_MouseMove;
            }
        }

        private bool IsMouseOverErrorRectangles(MouseEventArgs e)
        {
            var position = e.GetPosition(this.TextArea.TextView);
            return _errorRectangles.Any(rect => 
                position.X >= rect.Left && 
                position.Y >= rect.Top && 
                position.X <= rect.X + rect.Width && 
                position.Y <= rect.Y + rect.Height
            );
        }

        void CodeEditor_TextChanged(object sender, EventArgs e)
        {
            Code = Text;
            SyntaxError = null;
        }

        private void CodeEditor_Initialized(object sender, EventArgs e)
        {
            SetEditorOptions();
            LoadPythonSyntaxtHighlighting();

            if (AutoFocus)
                Focus();
        }

        public new void Focus()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                Keyboard.Focus(this);
                TextArea.Caret.BringCaretToView();
            }));
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (AutoFocus && e.Property == VisibilityProperty && (Visibility)e.NewValue == Visibility.Visible)
                Focus();
        }

        private void SetEditorOptions()
        {
            TextArea.Options.EnableHyperlinks = false;
            TextArea.Options.EnableEmailHyperlinks = false;
            TextArea.Options.EnableRectangularSelection = true;
            TextArea.Options.EnableTextDragDrop = true;
            TextArea.Options.IndentationSize = 4;
        }

        private void LoadPythonSyntaxtHighlighting()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("Studio.Resources.IDE.Python.xshd"))
            using (XmlReader reader = XmlReader.Create(stream))
            {
                SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }

        public static void RegisterSyntaxErrorRectangle(Rect rectangle)
        {
            _errorRectangles.Add(rectangle);
        }
    }
}
