using FR.Core;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FR.CascadeShadows;
public class MainViewModel : ObservableObject
{
    public ICommand LoadDirectXTargetCommand { get; init; }
    CancellationTokenSource loadDirectXTargetCancellationSource = new();

    DirectXPresenter? presenter;

    public MainViewModel()
    {
        LoadDirectXTargetCommand = new RelayCommand(o =>
        {
            if (o is not ContentPresenter contentPresenter)
                throw new ArgumentException($"parameter must be of type {nameof(ContentPresenter)}");

            presenter = new DirectXPresenter(contentPresenter);
            loadDirectXTargetCancellationSource.Cancel();
        });
    }

    public async Task<DirectXPresenter> GetDirectXPresenter()
    {
        if (presenter != null)
            return presenter;

        await Task.Delay(5000, loadDirectXTargetCancellationSource.Token);

        if (presenter != null)
            return presenter;
        else
            throw new Exception($"No DirectXPresenter loaded. Make sure to call {nameof(LoadDirectXTargetCommand)} with the parameter of {nameof(ContentPresenter)} after the {nameof(ContentPresenter)} is loaded.");
    }
}
