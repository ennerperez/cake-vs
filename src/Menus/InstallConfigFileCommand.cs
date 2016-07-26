﻿//------------------------------------------------------------------------------
// <copyright file="InstallConfigFileCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Cake.VisualStudio.Helpers;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Constants = Cake.VisualStudio.Helpers.Constants;

namespace Cake.VisualStudio.Menus
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class InstallConfigFileCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = PackageIds.cmdidInstallConfigFileCommand;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6003519f-6876-4db3-ad29-8d5379949869");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallConfigFileCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private InstallConfigFileCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static InstallConfigFileCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new InstallConfigFileCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var dte = CakePackage.Dte;
            if (dte == null) return;
            if (dte.Solution == null || dte.Solution.Count == 0)
            {
                ServiceProvider.ShowMessageBox("No solution opened");
            }
            else
            {
                if (MenuHelpers.DownloadFileToProject(Constants.ConfigFilePath, "cake.config"))
                {
                    VsShellUtilities.LogMessage(Constants.PackageName, "Cake configuraiton file installed into solution", __ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION);
                    ServiceProvider.ShowMessageBox("Cake configuration file successfully downloaded.");
                }
            }
        }
    }
}
