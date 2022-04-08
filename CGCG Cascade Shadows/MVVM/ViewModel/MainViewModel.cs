using FR.Core;

using SharpDX;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FR.CascadeShadows;

public class MainViewModel : ObservableObject
{
    public ICommand LoadDirectXTargetCommand { get; init; }
    public ICommand MoveCameraCommand { get; init; }
    public ICommand RotateCameraCommand { get; init; }
    public ICommand ChangeOutputCommand { get; init; }

    public event Action<Vector3>? MoveCamera;
    public event Action<Vector2>? RotateCamera;
    public event Action<int>? OutputChanged;

    readonly CancellationTokenSource loadDirectXTargetCancellationSource = new();

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

        MoveCameraCommand = new RelayCommand(o =>
        {
            if (o is Vector3 vec)
                MoveCamera?.Invoke(vec);
        });

        RotateCameraCommand = new RelayCommand(o =>
        {
            if (o is Vector2 vec)
                RotateCamera?.Invoke(vec);
        });

        ChangeOutputCommand = new RelayCommand(o =>
        {
            if (o is int idx)
                OutputChanged?.Invoke(idx);
        });
    }

    public async Task<DirectXPresenter> GetDirectXPresenter()
    {
        if (presenter != null)
            return presenter;

        await Task.Delay(5000, loadDirectXTargetCancellationSource.Token).ContinueWith(_ => { });

        if (presenter != null)
            return presenter;
        else
            throw new Exception($"No DirectXPresenter loaded. Make sure to call {nameof(LoadDirectXTargetCommand)} with the parameter of {nameof(ContentPresenter)} after the {nameof(ContentPresenter)} is loaded.");
    }
}
