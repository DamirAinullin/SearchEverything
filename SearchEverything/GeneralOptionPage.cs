using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace SearchEverything
{
    public class GeneralOptionPage : DialogPage
    {
        [Category("Search Everything")]
        [DisplayName("Maximum number of results")]
        [Description("Maximum number of results to return on search request")]
        public uint MaxNumberOfResults { get; set; } = 100;
    }
}
