using System.Threading.Tasks;
using Windows.UI.Popups;

namespace RemoteController.Services.DialogServices
{
    public class DialogService : IDialogService
    {
        DialogHelper _helper = new DialogHelper();

        public async Task ShowAsync(string content, string title = default(string))
        {
            await this._helper.ShowAsync(content, title);
        }

        public async Task ShowAsync(string content, string title, params UICommand[] commands)
        {
            await this._helper.ShowAsync(content, title, commands);
        }
    }
}
