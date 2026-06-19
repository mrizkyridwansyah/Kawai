using Microsoft.Playwright;

namespace Kawai.Api.Services;

/// <summary>
/// Singleton service to manage a long-running Playwright Chromium instance.
/// This drastically reduces PDF generation overhead by avoiding repeated browser startups.
/// </summary>
public class PlaywrightBrowserService : IAsyncDisposable
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly ILogger<PlaywrightBrowserService> _logger;

    public PlaywrightBrowserService(ILogger<PlaywrightBrowserService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Returns the active browser instance, starting it if necessary.
    /// </summary>
    public async Task<IBrowser> GetBrowserAsync()
    {
        if (_browser != null && _browser.IsConnected)
            return _browser;

        await _lock.WaitAsync();
        try
        {
            // Cek lagi setelah dapat lock untuk mencegah race condition
            if (_browser == null || !_browser.IsConnected)
            {
                _logger.LogInformation("[Playwright] Starting new Chromium instance...");
                
                // Pastikan yang lama di-dispose jika dalam state terputus (disconnected)
                await DisposePlaywrightObjectsAsync();

                _playwright = await Playwright.CreateAsync();
                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    Args = new[] { "--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage" }
                });

                _logger.LogInformation("[Playwright] Chromium instance started successfully.");
            }

            return _browser;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Dipanggil oleh Hangfire setiap jam 4 pagi untuk mencegah memory leak 
    /// dari akumulasi zombie processes Google Chrome.
    /// </summary>
    public async Task RestartBrowserAsync()
    {
        _logger.LogInformation("[Playwright] Scheduled restart triggered (4 AM). Stopping Chromium...");

        await _lock.WaitAsync();
        try
        {
            await DisposePlaywrightObjectsAsync();
            _logger.LogInformation("[Playwright] Chromium stopped. Will start automatically on next request.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Playwright] Error restarting Chromium instance.");
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task DisposePlaywrightObjectsAsync()
    {
        if (_browser != null)
        {
            try { await _browser.DisposeAsync(); } catch { /* abaikan error dispose */ }
            _browser = null;
        }

        if (_playwright != null)
        {
            try { _playwright.Dispose(); } catch { /* abaikan error dispose */ }
            _playwright = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposePlaywrightObjectsAsync();
        _lock.Dispose();
    }
}
