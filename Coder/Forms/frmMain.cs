using EnvDTE80;
using ISoft.Metabase;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using CF = System.Configuration;
namespace ISoft.Coder.Forms
{
    public partial class frmMain : Form
    {
        private DTE2 _App;
        private string _ConnectionSection = null;
        private int _ObjectIndex = 0;

        public frmMain(DTE2 app)
        {
            _App = app;
            InitializeComponent();
            _InitializeControls();
            _InitializeEvents();
        }

        public frmMain()
        {
            InitializeComponent();
            _InitializeControls();
            _InitializeEvents();
        }

        protected void _InitializeControls()
        {
            _ResetControls();
            var connectionSettings = Host.Configuration.ConnectionStrings.ConnectionStrings;
            if (connectionSettings.Count == 0)
            {
                cb_Connections.Items.Add("—— Please add connection string in t.config --");
            }
            else
            {
                foreach (CF.ConnectionStringSettings
                    c in connectionSettings) cb_Connections.Items.Add(c.Name);
            }
        }

        protected void _InitializeEvents()
        {
            cb_Connections.SelectedIndexChanged += (s, e) =>
            {
                switch (cb_Connections.SelectedIndex)
                {
                    case 0:
                        _ResetControls();
                        break;
                    default:
                        _ConnectionSection = cb_Connections.SelectedItem.ToString();
                        break;
                }
            };

            cb_Objects.SelectedIndexChanged += (s, e) =>
            {
                _ObjectIndex = cb_Objects.SelectedIndex;
                _ResetControls(1);
                if (cb_Objects.SelectedIndex == 0) return;
                BaseDTEWrapper wrapper = null;
                switch (_ObjectIndex)
                {
                    case 1:
                        wrapper = new SqlServerClassGenWrapper(_App, null);
                        break;
                    case 2:
                        wrapper = new MySqlClassGenWrapper(_App, null);
                        break;
                }

                foreach (var o in wrapper.Operations) { cb_Operations.Items.Add(o); }
                if (cb_Operations.Items.Count > 0)
                {
                    cb_Operations.Enabled = true;
                    cb_Operations.Focus();
                }
            };

            cb_Operations.SelectedIndexChanged += (s, e) =>
            {
                b_Go.Enabled = (cb_Operations.SelectedIndex > 0);
            };

            b_Go.Click += (s, e) =>
            {
                // MessageBox.Show(Assembly.GetExecutingAssembly().Location);

                BaseDTEWrapper wrapper = null;
                DbConnection connection = null;
                var connectionSection = Host.Configuration
                    .ConnectionStrings.ConnectionStrings[_ConnectionSection];
                switch (connectionSection.ProviderName)
                {
                    default:
                        connection = new SqlConnection(connectionSection.ConnectionString);
                        break;
                    case "mysql":
                        //connection = new MySqlConnection(connectionSection.ConnectionString);
                        break;
                }

                switch (cb_Objects.SelectedIndex)
                {
                    case 1:
                        wrapper = new SqlServerClassGenWrapper(
                        new SqlServerEFContext(connection, new Metabase.SqlServerPropertyResolver()), _App,
                        tb_Namespace.Text.Trim(), t =>
                            !new string[]
                            { 
                                //"____table", "____column", "____property"
                                "sysdiagrams"
                            }.Contains(t.Name.ToLower())
                        );
                        break;
                    case 2:
                        wrapper = new MySqlClassGenWrapper(
                        new MySqlEFContext(connection, new Metabase.MySqlPropertyResolver()), _App,
                        tb_Namespace.Text.Trim(), t =>
                            !new string[]
                            { 
                                //"____table", "____column", "____property"
                                "sysdiagrams"
                            }.Contains(t.Name.ToLower())
                        );
                        break;
                }

                try
                {
                    gb_Startup.Enabled = false;
                    gb_Console.Text = "Running, please wait ..";
                    l_Console.Items.Clear();
                    Refresh();
                    var cache = new List<string>();
                    if (cb_Operations.SelectedIndex > 0)
                    {
                        Visible = false;
                        wrapper.Do(cb_Operations.SelectedIndex - 1);
                    }
                    else
                    {
                        wrapper.Do(cb_Operations.SelectedIndex - 1,
                            msg =>
                            {

                                cache.Add(DateTime.Now.ToString("hh:mm:ss") + " - " + msg);
                                if (cache.Count >= 5)
                                {
                                    l_Console.Items.AddRange(cache.ToArray());
                                    l_Console.SelectedIndex = l_Console.Items.Count - 1;
                                    l_Console.Refresh();
                                    cache.Clear();
                                }
                                return true;
                            },
                            ex =>
                            {
                                MessageBox.Show(ex.Message);
                            });
                    }

                    Visible = true;
                    if (cache.Count > 0) l_Console.Items.AddRange(cache.ToArray());
                    l_Console.Items.Add("Done");
                    l_Console.SelectedIndex = l_Console.Items.Count - 1;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    gb_Startup.Enabled = true;
                    gb_Console.Text = "Log Info ..";
                    BringToFront();
                }
            };

            b_Close.Click += (s, e) => Close();
            Shown += (s, e) => { cb_Connections.Focus(); };
        }

        private void _ResetControls(int flag = 0)
        {
            if (flag == 1)
            {
                cb_Operations.Items.Clear();
                cb_Operations.Items.Add("-- Please Select --");
                cb_Operations.SelectedIndex = 0;
                b_Go.Enabled = false;
                return;
            }

            cb_Connections.Items.Clear();
            cb_Connections.Items.Add("-- Please Select --");

            cb_Objects.Items.Clear();
            cb_Objects.Items.Add("-- Please Select --");
            cb_Objects.Items.Add("MS SQLServer");
            cb_Objects.Items.Add("MySQL");

            cb_Operations.Items.Clear();
            cb_Operations.Items.Add("-- Please Select --");

            cb_Connections.SelectedIndex = 0;
            cb_Objects.SelectedIndex = 0;
            cb_Operations.SelectedIndex = 0;

            cb_Operations.Enabled = false;
            b_Go.Enabled = false;

            _ConnectionSection = null;
            _ObjectIndex = 0;
        }

    }
}
