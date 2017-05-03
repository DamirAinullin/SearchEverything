//------------------------------------------------------------------------------
// <copyright file="CommandPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using SearchEverything.EverythingApi;

namespace SearchEverything
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string)]
    [PackageRegistration(UseManagedResourcesOnly = false)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideToolWindow(typeof(SearchWindow), Style = VsDockStyle.Tabbed, Window = Constants.vsWindowKindSolutionExplorer)]
    [ProvideOptionPage(typeof(GeneralOptionPage), "Search Everything", "General", 0, 0, true)]
    public sealed class CommandPackage : Package
    {
        public int MaxNumberOfResults
        {
            get
            {
                GeneralOptionPage page = (GeneralOptionPage)GetDialogPage(typeof(GeneralOptionPage));
                return (int)page.MaxNumberOfResults;
            }
        }

        /// <summary>
        /// CommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "b4f3731f-969e-475d-a681-61e553b15a9c";

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        public CommandPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Command.Initialize(this);
            base.Initialize();

            OpenFileManager.CreateInstance(this);

            var everythingServiceManager = new EverythingServiceManager();

            if (!everythingServiceManager.IsServiceInstalled())
            {
                everythingServiceManager.InstallService();
            }
            else if (!everythingServiceManager.IsServiceRunning())
            {
                everythingServiceManager.StartService();
            }

            everythingServiceManager.StartClientInBackgroundMode();

            var everythingManager = new EverythingManager();
            everythingManager.Init();
            EverythingSearchTask.CommandPackage = this;
        }

        #endregion
    }
}
