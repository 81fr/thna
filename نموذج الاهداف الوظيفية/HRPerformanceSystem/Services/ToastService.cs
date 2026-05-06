namespace HRPerformanceSystem.Services;

public enum ToastType { Success, Error, Info, Warning }

public class ToastMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public ToastType Type { get; set; } = ToastType.Info;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class ToastService
{
    public event Action? OnChange;
    private List<ToastMessage> _toasts = new();

    public List<ToastMessage> GetToasts() => _toasts;

    public void Show(string title, string message, ToastType type = ToastType.Info)
    {
        var toast = new ToastMessage { Title = title, Message = message, Type = type };
        _toasts.Add(toast);
        NotifyStateChanged();
        
        // Auto-remove after 5 seconds
        _ = RemoveAfterDelay(toast.Id);
    }

    public void ShowSuccess(string msg) => Show("نجاح", msg, ToastType.Success);
    public void ShowError(string msg) => Show("خطأ", msg, ToastType.Error);
    public void ShowInfo(string msg) => Show("تنبيه", msg, ToastType.Info);

    public void Remove(string id)
    {
        var toast = _toasts.FirstOrDefault(t => t.Id == id);
        if (toast != null)
        {
            _toasts.Remove(toast);
            NotifyStateChanged();
        }
    }

    private async Task RemoveAfterDelay(string id)
    {
        await Task.Delay(5000);
        Remove(id);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
