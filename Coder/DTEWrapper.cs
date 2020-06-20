using EnvDTE;

using EnvDTE80;

using System;
using System.Collections.Generic;
namespace ISoft.Coder
{
    /// <summary>
    /// Abstract class for worker based on IDE automation
    /// </summary>
    public abstract class BaseDTEWrapper
    {
        protected List<string> _Operations = new List<string>();
        protected string _Identifier = string.Empty;
        protected string _Namespace = string.Empty;
        protected DTE2 _App = null;
        public BaseDTEWrapper(DTE2 app, string ns)
        {
            _Namespace = ns;
            _App = app;
        }

        /// <summary>
        /// Template file location (local drive)
        /// </summary>
        public string TemplateFile
        {
            get
            {
                string key = string.Format("{0}", _Identifier);
                return Host.Configuration.AppSettings.Settings[key].Value;
            }
        }

        /// <summary>
        /// Supprting operations 
        /// </summary>
        public List<string> Operations { get { return _Operations; } }

        /// <summary>
        /// All solution projects
        /// </summary>
        /// <returns></returns>
        protected List<Project> _Projects()
        {
            List<Project> results = new List<Project>();
            Action<ProjectItems> _dig = items => { };
            _dig = items =>
            {
                foreach (ProjectItem i in items)
                {
                    if (i.SubProject != null) results.Add(i.SubProject);
                    if (i.ProjectItems != null) _dig(i.ProjectItems);
                }
            };
            foreach (Project p in _App.Solution.Projects)
            {
                if (p.Kind != ProjectKinds.vsProjectKindSolutionFolder)
                {
                    results.Add(p);
                }
                else
                {
                    _dig(p.ProjectItems);
                }
            }
            return results;
        }

        /// <summary>
        /// Command implementation
        /// </summary>
        /// <param name="section"></param>
        public void Do(int operation,
            Func<string, bool> doneToConfirmContinue = null, Action<Exception> handleError = null)
        {
            try
            {
                _Do(operation, doneToConfirmContinue);
            }
            catch (Exception ex)
            {
                if (handleError == null) throw ex;
                handleError(ex);
            }
        }

        protected abstract void _Do(int operation, Func<string, bool> doneToConfirmContinue = null);

    }
}
