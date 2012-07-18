using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project
{
    public class ProjectFolder : Folder
    {
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new ProjectFolder();
        }
    }
}
