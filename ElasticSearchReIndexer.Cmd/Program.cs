using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using DbDataFlow;
using ElasticSearchReIndexer.Installers;
using ElasticSearchReIndexer.Models;

namespace ElasticSearchReIndexer.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            // application root
            var container = new WindsorContainer();

            // install everything in the directory
            container.Install(new ElasticSearchReIndexerInstaller());

            // install the cascading provider
            // install the command line config provider

            DbDataFlow<EsDocument, List<EsDocument>> reindexer;

            reindexer = container.Resolve<DbDataFlow<EsDocument, List<EsDocument>>>();
            reindexer.StartFlowAsync().Wait();
        }
    }
}
