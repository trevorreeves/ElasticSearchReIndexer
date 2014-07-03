using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Config;
using ElasticSearchReIndexer.Clients;
using ElasticSearchReIndexer.Workers;
using ElasticSearchReIndexer.Models;

namespace ElasticSearchReIndexer.Steps
{
    public class EsScrollerStep : IEsScrollerStep
    {
        private readonly ISourceScrollConfig _config;

        public EsScrollerStep(ISourceScrollConfig config)
        {
            _config = config;
        }

        public BlockingCollection<EsDocument> StartScrollingToEnd(
            JobCancellationUnit cancellationUnit)
        {
            var scrolledDocs = new BlockingCollection<EsDocument>();

            var scrollingTask = new Task(
                () => this.ScheduleScrollingWorkers(cancellationUnit, scrolledDocs),
                cancellationUnit.Token);

            scrollingTask.Start();

            return scrolledDocs;
        }

        private void ScheduleScrollingWorkers(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<EsDocument> scrolledDocs)
        {
            try
            {
                var worker = new ScrollWorker(new EsScrollClient(_config));

                // single task / synchronous workers pattern...
                this.ThrowIfSuccessorCancelled(cancellationUnit);

                var docs = worker.ScrollPage();
                while (docs.Any())
                {
                    foreach (var doc in docs)
                    {
                        this.ThrowIfSuccessorCancelled(cancellationUnit);
                        scrolledDocs.Add(doc);
                    }
                    docs = worker.ScrollPage();
                }
            }
            catch (TaskCanceledException)
            {
                // our sucessors should have stopped, but lets dot the i's...
                throw;
            }
            catch (Exception ex)
            {
                // because we're the first in the job, don't cause a cancel if we
                // fail, just say we're finished.  This gives everything further down
                // stream a chance to process all the data we've gathered so far.

                // TODO: logging

                throw; // currently throwing.  though I think this will cause a cancel 
                // on the token.  but we do want to report the exceptioned status of this
                // task, but just not cause everything else to stop immediately.
            }
            finally
            {
                scrolledDocs.CompleteAdding();
            }
        }

        private void ThrowIfSuccessorCancelled(
            JobCancellationUnit cancellationUnit)
        {
            // if something cancelled the job, and we have no predecessor, so we can only
            // assume that is must have been a successor, so we must end.
            if (cancellationUnit.IsCancellationRequested)
            {
                cancellationUnit.ThrowIfCancelled();
            }
        }
    }
}
