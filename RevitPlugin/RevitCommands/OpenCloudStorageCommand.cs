using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitPlugin.Services;
using RevitPlugin.ViewModels;
using RevitPlugin.Views;

namespace RevitPlugin.RevitCommands
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class OpenCloudStorageCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var cloudStorage = new GoogleDriveService();
            CloudStorageWindow cloudStorageView = new CloudStorageWindow();
            CloudStorageViewModel storageViewModel = new CloudStorageViewModel(cloudStorage);
            cloudStorageView.DataContext = storageViewModel;
            cloudStorageView.ShowDialog();

            return Result.Succeeded;
        }
    }
}
