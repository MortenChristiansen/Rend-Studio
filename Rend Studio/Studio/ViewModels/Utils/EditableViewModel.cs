using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Studio.ViewModels.Utils
{
    public class EditableViewModel<TViewModel> : INotifyPropertyChanged
    {
        private bool _isEditing;
        public bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                _isEditing = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsEditing"));
            }
        }
        public TViewModel ViewModel { get; private set; }

        public EditableViewModel(TViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}

// I may need an EditableView that can contain arbitrary content. It must handle the editing interactions such as enter and escape
