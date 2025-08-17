namespace Day08.Core.Tests.Logging;

/// <summary>
/// class NoOpDisposable - 無操作的 Disposable 實作
/// </summary>
public class NoOpDisposable : IDisposable
{
    public void Dispose()
    {
        // 無操作
    }
}