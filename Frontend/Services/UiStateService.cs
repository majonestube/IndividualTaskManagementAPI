namespace Frontend.Services;

public class UiStateService
{
    public event Action? OnChange;
    public void NotifyStateChanged() => OnChange?.Invoke();
}