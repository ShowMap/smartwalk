using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;

namespace SmartWalk.Core.ViewModels
{
    public abstract class EntityViewModel : MvxViewModel
    {
        private Entity _entity;
        private bool _isDescriptionExpanded;
        private bool _isDescriptionExpandable;
        private MvxCommand _expandCollapseCommand;

        public Entity Entity
        {
            get
            {
                return _entity;
            }
            protected set
            {
                if (!Equals(_entity, value))
                {
                    _entity = value;
                    RaisePropertyChanged(() => Entity);
                }
            }
        }

        public bool IsDescriptionExpanded
        {
            get
            {
                return _isDescriptionExpanded;
            }
            protected set
            {
                if (_isDescriptionExpanded != value)
                {
                    _isDescriptionExpanded = value;
                    RaisePropertyChanged(() => IsDescriptionExpanded);
                }
            }
        }

        public bool IsDescriptionExpandable
        {
            get
            {
                return _isDescriptionExpandable;
            }
            protected set
            {
                if (_isDescriptionExpandable != value)
                {
                    _isDescriptionExpandable = value;
                    RaisePropertyChanged(() => IsDescriptionExpandable);
                }
            }
        }

        public ICommand ExpandCollapseCommand
        {
            get 
            {
                if (_expandCollapseCommand == null)
                {
                    _expandCollapseCommand = 
                        new MvxCommand(() => IsDescriptionExpanded = !IsDescriptionExpanded);
                }

                return _expandCollapseCommand;
            }
        }
    }
}