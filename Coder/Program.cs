﻿using EnvDTE80;

using System.Threading.Tasks;

using CF = System.Configuration;
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
                map.ExeConfigFilename = @"d:\t.config";
                _Configuration = CF.ConfigurationManager.OpenMappedExeConfiguration(map, CF.ConfigurationUserLevel.None);
                return _Configuration;
            }
        }

    }

}
