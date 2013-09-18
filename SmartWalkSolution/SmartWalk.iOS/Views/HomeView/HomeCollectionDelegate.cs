using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Controls;

namespace SmartWalk.iOS.Views.HomeView
{
    public class HomeCollectionDelegate : UICollectionViewDelegate
    {
        private readonly HomeViewModel _viewModel;
        private readonly HomeCollectionSource _collectionSource;

        public HomeCollectionDelegate(HomeViewModel viewModel, HomeCollectionSource collectionSource)
        {
            _viewModel = viewModel;
            _collectionSource = collectionSource;
        }

        public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath);
            cell.ContentView.BackgroundColor = ThemeColors.CellHighlight;
        }

        public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath);
            cell.ContentView.BackgroundColor = ThemeColors.CellBackground;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var org = _collectionSource.ItemsSource.Cast<EntityInfo>().ElementAt(indexPath.Row);

            if (_viewModel.NavigateOrgViewCommand.CanExecute(org))
            {
                _viewModel.NavigateOrgViewCommand.Execute(org);
            }

            collectionView.DeselectItem(indexPath, false);
        }
    }
}