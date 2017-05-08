﻿using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace SearchEverything.Options
{
    public class GeneralOptionPage : DialogPage
    {
        [Category("Search Everything")]
        [DisplayName("Maximum number of results")]
        [Description("Maximum number of results to return on search request")]
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public uint MaxNumberOfResults { get; set; } = 100;
    }
}
