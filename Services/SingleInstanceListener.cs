using System.IO.Pipes;

namespace TeclaLibre.Services;

internal sealed class SingleInstanceListener : IDisposable
{
    private readonly string _pipeName;
    private readonly Action _showRequested;
    private readonly Action<Exception> _errorOccurred;

    private readonly CancellationTokenSource _cancellation = new();

    private bool _started;
    private bool _disposed;

    public SingleInstanceListener(
        string pipeName,
        Action showRequested,
        Action<Exception> errorOccurred)
    {
        _pipeName = pipeName;
        _showRequested = showRequested;
        _errorOccurred = errorOccurred;
    }

    public void Start()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_started)
        {
            return;
        }

        _started = true;

        _ = Task.Run(ListenAsync);
    }

    private async Task ListenAsync()
    {
        try
        {
            while (!_cancellation.IsCancellationRequested)
            {
                using NamedPipeServerStream server = new(
                    pipeName: _pipeName,
                    direction: PipeDirection.In,
                    maxNumberOfServerInstances: 1,
                    transmissionMode: PipeTransmissionMode.Byte,
                    options:
                        PipeOptions.Asynchronous |
                        PipeOptions.CurrentUserOnly);

                await server.WaitForConnectionAsync(
                    _cancellation.Token);

                if (!_cancellation.IsCancellationRequested)
                {
                    _showRequested();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Cierre normal de la aplicación.
        }
        catch (Exception ex) when (
            ex is IOException or
            UnauthorizedAccessException or
            InvalidOperationException)
        {
            if (!_cancellation.IsCancellationRequested)
            {
                _errorOccurred(ex);
            }
        }
        /*
        finally
        {
            _cancellation.Dispose();
        }
        */
    }

    /*
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_started)
        {
            _cancellation.Cancel();
        }
        else
        {
            _cancellation.Dispose();
        }
    }
    */

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_started)
        {
            try { _cancellation.Cancel(); }
            catch (ObjectDisposedException) { }
        }
        _cancellation.Dispose(); // el doble Dispose es seguro
    }
}