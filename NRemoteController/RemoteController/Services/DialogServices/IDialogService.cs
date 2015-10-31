using System.Threading.Tasks;
using Windows.UI.Popups;

namespace RemoteController.Services.DialogServices
{
    public interface IDialogService
    {
        Task ShowAsync(string content, string title = default(string));
        Task ShowAsync(string content, string title, params UICommand[] commands);
    }
}
