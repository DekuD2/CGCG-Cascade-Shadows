using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FR.Core;

// https://www.youtube.com/watch?v=PzP8mw7JUzI
public class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
