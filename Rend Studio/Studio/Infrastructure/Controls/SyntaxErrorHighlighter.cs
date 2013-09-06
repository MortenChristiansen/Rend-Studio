using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;

namespace Studio.Infrastructure.Controls
{
    public class SyntaxErrorHighlighter : IBackgroundRenderer
    {
        private CodeEditor _editor;

        public SyntaxErrorHighlighter(CodeEditor editor)
        {
            _editor = editor;
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            var syntaxError = _editor.SyntaxError;

            if (_editor.Document == null || syntaxError == null)
                return;

            textView.EnsureVisualLines();

            var fillBrush = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0, 0));
            var strokeBrush = new SolidColorBrush(Color.FromArgb(0x60, 0xFF, 0, 0));
            var strokePen = new Pen(strokeBrush, 1);

            var remainingChars = syntaxError.RawSpan.Length;
            var currentLine = syntaxError.Line;
            while (remainingChars > 0)
            {
                var line = _editor.Document.GetLineByNumber(currentLine++);
                foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, line))
                {
                    var charWidth = rect.Width / line.Length;
                    var offset = remainingChars == syntaxError.RawSpan.Length ? (syntaxError.Column - 1) * charWidth - 1 : 0;
                    var width = Math.Min(line.Length, remainingChars) * charWidth + 1;

                    var errorRectangle = new Rect(new Point(rect.X + offset, rect.Y), new Size(width, rect.Height + 2));
                    CodeEditor.RegisterSyntaxErrorRectangle(errorRectangle);
                    drawingContext.DrawRoundedRectangle(fillBrush, strokePen, errorRectangle, 2, 2);
                }
                remainingChars -= line.Length;
            }
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }
    }
}
