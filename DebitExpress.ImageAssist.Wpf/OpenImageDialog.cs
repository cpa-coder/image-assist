using Microsoft.Win32;
using System.Windows;

namespace DebitExpress.ImageAssist.Wpf
{
    public interface IOpenImageDialog
    {
        /// <summary>
        /// Get a string containing the full path of the selected image in a dialog.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Shows image selection dialog.
        /// </summary>
        /// <param name="title">
        /// Dialog title.
        /// </param>
        /// <param name="window">
        /// Window owner of the dialog.
        /// <br/>
        /// When set to null, the default window owner is set to the application's main window.
        /// </param>
        /// <returns></returns>
        bool ShowDialog(string title = "Select image", Window window = null);
    }

    /// <summary>
    /// Concrete implementation of <see cref="IOpenImageDialog"/>
    /// </summary>
    public sealed class OpenImageDialog : IOpenImageDialog
    {
        /// <summary>
        /// Get a string containing the full path of the selected image in a dialog.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Shows image selection dialog.
        /// </summary>
        /// <param name="title">
        /// Dialog title.
        /// </param>
        /// <param name="window">
        /// Window owner of the dialog.
        /// <br/>
        /// When set to null, the default window owner is set to the application's main window.
        /// </param>
        /// <returns></returns>
        public bool ShowDialog(string title = "Select image", Window window = null)
        {
            window ??= Application.Current.MainWindow;

            var dialog = new OpenFileDialog
            {
                Title = title,
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png",
                Multiselect = false,
            };

            var result = dialog.ShowDialog(window) == true;
            FileName = dialog.FileName;

            return result;
        }
    }
}