using Stories.Query;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stories
{
    public static class DI
    {
        public static Container Container;
        static DI()
        {
            Container = new Container(_ =>
            {
                _.Scan(aAssemblyScanner =>
                {
                    aAssemblyScanner.AssembliesAndExecutablesFromApplicationBaseDirectory();
                  //  aAssemblyScanner.AddAllTypesOf(typeof(IExecutor<>));
                    aAssemblyScanner.WithDefaultConventions();
                    aAssemblyScanner.ConnectImplementationsToTypesClosing(typeof(IExecutor<>));
                });
            });
        }
    }
}
