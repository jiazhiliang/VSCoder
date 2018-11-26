using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;

using System.Data.Entity;
using CF = System.Configuration;
using FM = System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading.Tasks;
namespace ISoft.Coder
{
    /// <summary>
    /// Hosting object
    /// </summary>
    public static class Host
    {
        private static Forms.frmMain _MainForm = null;

        /// <summary>
        /// Factory run (async)
        /// </summary>
        public static async void Go(DTE2 app)
        {
            if (_MainForm != null) _MainForm.Close();
            _MainForm = new Forms.frmMain(app);
            await Task.Run(() =>
            {
                _MainForm.ShowDialog();
            });
            
        }

        private static CF.Configuration _Configuration = null;
        public static CF.Configuration Configuration
        {
            get
            {
                var map = new CF.ExeConfigurationFileMap();
                map.ExeConfigFilename = @"c:\t.config";
                _Configuration = CF.ConfigurationManager.OpenMappedExeConfiguration(map, CF.ConfigurationUserLevel.None);
                return _Configuration;
            }
        }

    }

}
