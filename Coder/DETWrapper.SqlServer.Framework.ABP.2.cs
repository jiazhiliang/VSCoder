using EnvDTE;

using ISoft.Metabase;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ISoft.Coder
{
    public partial class SqlServerClassGenWrapper
    {
        /// <summary>
        /// Write an entity file
        /// </summary>
        private void _Do_4(Func<string, bool> doneToConfirmContinue = null)
        {
            if (string.IsNullOrEmpty(_Namespace))
            {
                MessageBox.Show("Please provide a namespace");
                return;
            }
        }

    }
}
