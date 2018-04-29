using System;
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
using System.IO;

namespace ISoft.Coder
{
    public class TestWrapper: BaseDTEWrapper
    {
        protected override void _Do(int operation, Func<string, bool> doneToConfirmContinue = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Test1: retrieve all project folder
        /// </summary>
        /// <returns></returns>
        public string Test1()
        {
            StringBuilder sb = new StringBuilder();
            Action<CodeElements> dig = es => { };
            dig = es =>
            {
                foreach (CodeElement e in es)
                {
                    if (e is CodeClass)
                    {
                        CodeClass c = e as CodeClass;
                        sb.AppendLine(c.FullName);
                    }
                    if (e is CodeNamespace) dig(((CodeNamespace)e).Members);
                }
            };
            
            foreach (Project p in _App.Solution.Projects)
            {
                if (p.CodeModel != null) dig(p.CodeModel.CodeElements);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Test2: retrieve all files
        /// </summary>
        /// <returns></returns>
        public void Test2(Action<string> sink)
        {
            Action<ProjectItems> dig = items => { };
            dig = items =>
            {
                foreach (ProjectItem i in items)
                {
                    if (i.ProjectItems != null)
                    {
                        dig(i.ProjectItems);
                    }
                    if (i.SubProject != null && i.SubProject.ProjectItems != null) dig(i.SubProject.ProjectItems);
                    if (i.Name.EndsWith(".aspx") || 
                        i.Name.EndsWith(".ascx") ||
                        i.Name.EndsWith(".cs"))
                    {
                        Window w = i.Open(Constants.vsViewKindCode);
                        w.Activate();
                        TextSelection ts = _App.ActiveDocument.Selection as TextSelection;
                        ts.SelectAll();
                        _App.ExecuteCommand("Edit.FormatDocument");
                        w.Close(vsSaveChanges.vsSaveChangesYes);
                        sink(i.Name);
                    }
                }
            };

            Array projects = (Array)_App.ActiveSolutionProjects;
            if (projects.Length > 0)
            {
                foreach (Project p in projects)
                {
                    if (p.ProjectItems != null)
                    {
                        dig(p.ProjectItems);
                    }
                }
            }
        }

        /// <summary>
        /// Solution root
        /// </summary>
        protected UIHierarchy _SolutionExplorerNode
        {
            get
            {
                return _App.ToolWindows.SolutionExplorer;
            }
        }

        /// <summary>
        /// Project root
        /// </summary>
        public List<UIHierarchyItem> GetProjectNodes(Solution solution)
        {
            string solutionName = solution.Properties.Item("Name").Value.ToString();
            return GetProjectNodes(_SolutionExplorerNode.GetItem(solutionName).UIHierarchyItems);
        }

        /// <summary>
        /// Recursively-loaded project items
        /// </summary>
        public List<UIHierarchyItem> GetProjectNodes(UIHierarchyItems topLevelItems)
        {
            List<UIHierarchyItem> projects = new List<UIHierarchyItem>();
            foreach (UIHierarchyItem item in topLevelItems)
            {
                if (isProjectNode(item))
                {
                    projects.Add(item);
                }
                else if (isSolutionFolder(item))
                {
                    projects.AddRange(_GetProjectNodesInSolutionFolder(item));
                }
            }

            return projects;
        }

        /// <summary>
        /// Get solution items
        /// </summary>
        private List<UIHierarchyItem> _GetProjectNodesInSolutionFolder(UIHierarchyItem item)
        {
            List<UIHierarchyItem> projects = new List<UIHierarchyItem>();

            if (isSolutionFolder(item))
            {
                foreach (UIHierarchyItem subItem in item.UIHierarchyItems)
                {
                    if (isProjectNode(subItem))
                    {
                        projects.Add(subItem);
                    }
                }
            }

            return projects;
        }

        private bool isSolutionFolder(UIHierarchyItem item)
        {
            return ((item.Object is Project) &&
                ((item.Object as Project).Kind == ProjectKinds.vsProjectKindSolutionFolder));
        }

        private bool isProjectNode(UIHierarchyItem item)
        {
            return isDirectProjectNode(item) || isProjectNodeInSolutionFolder(item);
        }

        private bool isDirectProjectNode(UIHierarchyItem item)
        {
            return ((item.Object is Project) && ((item.Object as Project).Kind != ProjectKinds.vsProjectKindSolutionFolder));
        }

        private bool isProjectNodeInSolutionFolder(UIHierarchyItem item)
        {
            return (item.Object is ProjectItem && ((ProjectItem)item.Object).Object is Project &&
                        ((Project)((ProjectItem)item.Object).Object).Kind != ProjectKinds.vsProjectKindSolutionFolder);
        }

        public TestWrapper(DTE2 app)
            : base(app, null)
        {
        }
    }


}
