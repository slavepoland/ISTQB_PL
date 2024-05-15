using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ISTQB_PL.Services
{
    public class SelectedItemBehavior : Behavior<CollectionView>
    {
        public static readonly BindableProperty HighlightColorProperty =
            BindableProperty.Create(nameof(HighlightColor), typeof(Color), typeof(SelectedItemBehavior), Color.Red);

        public Color HighlightColor
        {
            get { return (Color)GetValue(HighlightColorProperty); }
            set { SetValue(HighlightColorProperty, value); }
        }

        protected override void OnAttachedTo(CollectionView bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.SelectionChanged += OnCollectionViewSelectionChanged;
        }

        protected override void OnDetachingFrom(CollectionView bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.SelectionChanged -= OnCollectionViewSelectionChanged;
        }

        private void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
            {
                foreach (var item in e.CurrentSelection)
                {
                    var container = (sender as CollectionView).ItemTemplate.CreateContent() as View;
                    var viewCell = container as View;

                    if (viewCell != null)
                    {
                        viewCell.BackgroundColor = Color.Red;
                    }
                }
            }
        }
    }

}
