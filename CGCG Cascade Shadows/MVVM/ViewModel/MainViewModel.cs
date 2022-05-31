﻿using FR.Core;

using SharpDX;

using System;
using System.IO;
using System.Diagnostics;
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
    public ICommand ChangeSamplerCommand { get; init; }
    public ICommand ReloadShaderCommand { get; init; }
    public ICommand ToggleOnCommand { get; init; }
    public ICommand ToggleOffCommand { get; init; }
    public ICommand OpenFile { get; init; }
    public ICommand SetPcfModeCommand { get; init; }
    public ICommand SetDepthBiasCommand { get; init; }
    public ICommand SetVisualiseCommand { get; init; }
    public ICommand SetCascade1Command { get; init; }
    public ICommand SetCascade2Command { get; init; }
    public ICommand SetCascade3Command { get; init; }

    public event Action<Vector3>? MoveCamera;
    public event Action<Vector2>? RotateCamera;
    public event Action<int>? OutputChanged;
    public event Action<string>? SamplerChanged;
    public event Action<string>? ReloadShader;
    public event Action<string>? Error;
    public event Action<string, bool>? Toggle;
    public event Action<string, object>? SetValue;

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

        ChangeSamplerCommand = new RelayCommand(o =>
        {
            SamplerChanged?.Invoke(o?.ToString() ?? "null");
        });

        ReloadShaderCommand = new RelayCommand(o => ReloadShader?.Invoke(o?.ToString() ?? ""));

        ToggleOnCommand = new RelayCommand(o => Toggle?.Invoke(o?.ToString() ?? "", true));

        ToggleOffCommand = new RelayCommand(o => Toggle?.Invoke(o?.ToString() ?? "", false));

        OpenFile = new RelayCommand(o =>
        {
            using Process p = new()
            {
                StartInfo =
                {
                    FileName = o?.ToString() ?? "",
                    UseShellExecute = true
                }
            };
            p.Start();
        });

        SetPcfModeCommand = new RelayCommand(o =>
        {
            SetValue?.Invoke("pcf", int.Parse(o?.ToString() ?? "0"));
        });

        SetDepthBiasCommand = new RelayCommand(o =>
        {
            SetValue?.Invoke("depthBias", float.Parse(o?.ToString() ?? "0"));
        });

        SetVisualiseCommand = new RelayCommand(o =>
        {
            SetValue?.Invoke("visualise", int.Parse(o?.ToString() ?? "0"));
        });

        SetCascade1Command = new RelayCommand(o =>
        {
            SetValue?.Invoke("cascade1", int.Parse(o?.ToString() ?? "0"));
        });

        SetCascade2Command = new RelayCommand(o =>
        {
            SetValue?.Invoke("cascade2", int.Parse(o?.ToString() ?? "0"));
        });

        SetCascade3Command = new RelayCommand(o =>
        {
            SetValue?.Invoke("cascade3", int.Parse(o?.ToString() ?? "0"));
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

    public void ShowError(string error)
        => Error?.Invoke(error);
}
