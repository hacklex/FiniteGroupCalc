using System.ComponentModel;
using System.Reflection;

namespace FiniteGroupCalc.Widgets
{
    /// <summary>
    /// A PropertyGrid that automatically refreshes when the selected object implements INotifyPropertyChanged,
    /// and allows setting the width of the label column.
    /// </summary>
    public partial class PropertyGridEx : PropertyGrid
    {
        private const BindingFlags FindAmongPrivates = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        private INotifyPropertyChanged? _selectedObject;

        protected override void OnSelectedObjectsChanged(EventArgs e)
        {
            base.OnSelectedObjectsChanged(e);

            if (_selectedObject != null) _selectedObject.PropertyChanged -= SelectedObject_PropertyChanged;
            _selectedObject = SelectedObject as INotifyPropertyChanged;
            if (_selectedObject != null) _selectedObject.PropertyChanged += SelectedObject_PropertyChanged;
        }
        public PropertyGridEx()
        {
        }
        private void SelectedObject_PropertyChanged(object? sender, PropertyChangedEventArgs? e)
        {
            Refresh();
        }
        public static void SetLabelColumnWidth(PropertyGrid grid, int width)
        {
            if (grid == null) return;
            if (typeof(PropertyGrid).GetField("_gridView", FindAmongPrivates)?.GetValue(grid) is not Control view) return;
            view.GetType().GetMethod("MoveSplitterTo", FindAmongPrivates)?.Invoke(view, [width]);
        }
    }
}