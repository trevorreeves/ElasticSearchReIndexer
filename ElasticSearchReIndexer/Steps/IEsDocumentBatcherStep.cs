using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Models;

namespace ElasticSearchReIndexer.Steps
{
    public interface IEsDocumentBatcherStepFactory
    {
        IEsDocumentBatcherStep Create(int batchSize);
    }

    public interface IEsDocumentBatcherStep
    {
        BlockingCollection<List<EsDocument>> StartBatching(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<EsDocument> source);
    }
}
