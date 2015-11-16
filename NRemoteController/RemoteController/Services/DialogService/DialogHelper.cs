using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace RemoteController.Services.DialogService
{
    public class DialogHelper
    {
        public bool IsOpen { get; set; }

        public DialogHelper()
        {
            // not thread safe
        }

        public async void Show(string content, string title = default(string))
        {
            await ShowAsync(content, title);
        }

        public async void Show(string content, string title = default(string), params UICommand[] commands)
        {
            await ShowAsync(content, title, commands);
        }

        public async Task ShowAsync(string content, string title = default(string), params UICommand[] commands)
        {
            while (IsOpen)
            {
                await Task.Delay(1000);
            }
            IsOpen = true;
            var dialog = (title == default(string))? new MessageDialog(content) : new MessageDialog(content, title);
            if (commands != null && commands.Any())
                foreach (var item in commands)
                    dialog.Commands.Add(item);
            await dialog.ShowAsync();
            IsOpen = false;
        }
    }
}
