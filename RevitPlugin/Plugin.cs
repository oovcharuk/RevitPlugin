using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace RevitPlugin
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Plugin : IExternalApplication
    {
        private UIControlledApplication _application = default;

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            _application = application;

            var ribbonPanel =
                application.GetRibbonPanels()?.FirstOrDefault(r => r.Name == "RDStudioPlugin")
                ?? application.CreateRibbonPanel("RDStudioPlugin");
            var thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            var openCloudStorageButton = (PushButton)
                ribbonPanel.AddItem(
                    new PushButtonData(
                        "cmdOpenCloudStorage",
                        "Open Cloud Storage",
                        thisAssemblyPath,
                        "RevitPlugin.RevitCommands.OpenCloudStorageCommand"
                    )
                );
            openCloudStorageButton.ToolTip = "Shows the Cloud Storage content.";

            Uri uriLargeImage = new Uri("pack://application:,,,/RevitPlugin;component/Resources/CommandLarge.png");
            openCloudStorageButton.LargeImage = new BitmapImage(uriLargeImage);

            Uri uriSmallImage = new Uri("pack://application:,,,/RevitPlugin;component/Resources/CommandSmall.png");
            openCloudStorageButton.Image = new BitmapImage(uriSmallImage);

            return Result.Succeeded;
        }
    }
}
