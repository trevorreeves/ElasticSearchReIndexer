using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Config;
using ElasticSearchReIndexer.Workers;

namespace ElasticSearchReIndexer
{
    public class EsScroller
    {
        private readonly ISourceScrollConfig _config;

        public EsScroller(ISourceScrollConfig config)
        {
            _config = config;
        }

        public BlockingCollection<EsDocument> StartScrollingToEnd(
            JobCancellationUnit cancellationUnit)
        {
            var scrolledDocs = new BlockingCollection<EsDocument>();

            try
            {
                var scrollingTask = new Task(
                    () => this.ScheduleScrollingWorkers(cancellationUnit, scrolledDocs),
                    cancellationUnit.Token);

                scrollingTask.Start();
            }
            catch (Exception ex)
            {
                // because we're the first in the job, don't cause a cancel if we
                // fail, just say we're finished.  This gives everything further down
                // stream a chance to process all the data we've gathered so far.
                scrolledDocs.CompleteAdding();
            }
            finally
            {
                
            }

            return scrolledDocs;
        }

        private void ScheduleScrollingWorkers(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<EsDocument> scrolledDocs)
        {
            try
            {
                // single task / synchronous workers pattern...

                while (!cancellationUnit.IsCancellationRequested)
                {
                    // TODO: need to remember the scroll id after each request.
                    var worker = new ScrollWorker(_config);
                    foreach (var doc in worker.ScrollPage())
                    {
                        scrolledDocs.Add(doc);
                    }
                }
            }
            //catch (TaskCanceledException) // not checking/throwing these here
            //{
            //
            //}
            catch (Exception ex)
            {
                // because we're the first in the job, don't cause a cancel if we
                // fail, just say we're finished.  This gives everything further down
                // stream a chance to process all the data we've gathered so far.
                scrolledDocs.CompleteAdding();

                
                // TODO: logging
                
                throw; // currently throwing.  though I think this will cause a cancel 
                // on the token.  but we do want to report the exceptioned status of this
                // task, but just not cause everything else to stop immediately.
            }
        }
    }
}
