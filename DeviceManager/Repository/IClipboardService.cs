namespace DeviceManager.Repository
{
    public interface IClipboardService
    {
        Task CopyToClipboard(string text);
    }
}
