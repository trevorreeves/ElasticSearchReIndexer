﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Models;

namespace ElasticSearchReIndexer.Steps
{
    public interface IEsIndexerStep
    {
        Task StartIndexingAsync(
            JobCancellationUnit cancellationUnit,
            BlockingCollection<List<EsDocument>> sourceBatches);
    }
}
