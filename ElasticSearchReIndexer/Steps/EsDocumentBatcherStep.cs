using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbDataFlow;
using ElasticSearchReIndexer.Models;

namespace ElasticSearchReIndexer.Steps
{
    public class EsDocumentBatcherStep : BaseTransformer<EsDocument, List<EsDocument>>
    {
        private readonly int _batchSize;

        public EsDocumentBatcherStep(int batchSize)
        {
            Contract.Requires(batchSize > 0);

            _batchSize = batchSize;
        }

        protected override void Transform(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<EsDocument> source, 
            BlockingCollection<List<EsDocument>> transformations)
        {
            var currentBatch = new List<EsDocument>(_batchSize);

            while (source.PossiblyMoreInStream())
            {
                // close the current batch if its full
                if (currentBatch.Count() == _batchSize)
                {
                    transformations.Add(currentBatch);
                    currentBatch = new List<EsDocument>(_batchSize);
                }

                // add another doc to the current batch
                EsDocument currentDoc;
                if (source.TryTake(out currentDoc, 5 * 1000, cancellationUnit.Token))
                {
                    currentBatch.Add(currentDoc);
                }

                this.ThrowIfSuccessorCancelled(cancellationUnit, source);
            }

            // ensure all docs are pushed when total number is not exact multiple
            // of batch size.
            if (currentBatch.Any())
            {
                transformations.Add(currentBatch);
            }
        }
    }
}
