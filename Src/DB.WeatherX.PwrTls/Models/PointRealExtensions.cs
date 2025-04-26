// New file to add to your project
// filepath: c:\g\CtrsPoc\RealTimeChangeTracking\DbEditorWpfApp\Models\PointRealExtensions.cs
using DB.WeatherX.PwrTls.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DB.WeatherX.PwrTls.Models
{
    // Extend the auto-generated class with notification capabilities
    public partial class PointReal : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Override setters for key properties you want to monitor
        // This assumes your class has Note64 property with a regular setter - adjust as needed
        private string? _note64;
        public string? Note64
        {
            get => _note64;
            set
            {
                if (_note64 != value)
                {
                    _note64 = value;
                    OnPropertyChanged();
                }
            }
        }

        // Add similar overrides for other properties you need to track
    }
}