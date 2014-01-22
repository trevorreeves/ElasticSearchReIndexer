using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchReIndexer.Workers
{
    public class EsDocumentBatcher
    {
        private readonly int _batchSize;

        public EsDocumentBatcher(int batchSize)
        {
            _batchSize = batchSize;
        }

        public BlockingCollection<List<EsDocument>> StartBatching(
            BlockingCollection<EsDocument> source)
        {
            // watch the source

            // when batch complete

            // add to the result

            // when source complete, add an any orphans to final batch.
        }
    }
}
