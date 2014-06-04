using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchReIndexer.Config;
using Newtonsoft.Json.Linq;
using ElasticSearchReIndexer.Clients;

using Xunit;
using Moq;
using FluentAssertions;
using ElasticSearchReIndexer.Models;
using ElasticSearchReIndexer.Workers;

namespace ElasticSearchReIndexer.Tests.Unit
{
    public class IndexWorkerFixture
    {
        private const string TEST_INDEX = "testindex";
        private const string TEST_TYPE = "testtype";
        private const string TEST_ID = "1";

        private readonly IndexWorker _testWorker;
        private readonly Mock<IEsIndexClient> _mockEsClient;
        private readonly Mock<ITargetIndexingConfig> _mockConfig;
        private readonly MockRepository _mockRepo = 
            new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
        
        public IndexWorkerFixture()
        {
            _mockConfig = _mockRepo.Create<ITargetIndexingConfig>();
            
            _mockEsClient = _mockRepo.Create<IEsIndexClient>();

            _testWorker = new IndexWorker(_mockConfig.Object, _mockEsClient.Object);
        }

        [Fact]
        public void EmptyDocumentList_Indexing_NothingHappens()
        {
            // arrange
            var testDocs = new List<EsDocument>();

            // act
            _testWorker.Index(testDocs).Should().BeTrue();

            _mockEsClient.Verify(c => c.Bulk(It.IsAny<string>()), Times.Never);
            _mockRepo.VerifyAll();
        }

        [Fact]
        public void OneDocumentBatch_Indexed_Correctly()
        {
            // arrange
            _mockConfig.Setup(c => c.Index).Returns("testindex");
            _mockConfig.Setup(c => c.Type).Returns("testtype");
            var testDoc = GenerateTestDoc("1");

            var testDocs = new List<EsDocument>()
            {
                testDoc
            };

            var expectedBody = BuildDocIndexJsonChrome(testDoc);
            _mockEsClient.Setup(c => c.Bulk(expectedBody)).Returns(true);

            // act
            _testWorker.Index(testDocs).Should().BeTrue();

            // assert
            _mockRepo.VerifyAll();
        }

        [Fact]
        public void MultipleDocumentBatch_Indexed_Correctly()
        {
            // arrange
            _mockConfig.Setup(c => c.Index).Returns("testindex");
            _mockConfig.Setup(c => c.Type).Returns("testtype");

            var testDoc1 = GenerateTestDoc("1");
            var testDoc2 = GenerateTestDoc("2");

            var testDocs = new List<EsDocument>()
            {
                testDoc1,
                testDoc2
            };

            var expectedBody = string.Format("{0}{1}",
                BuildDocIndexJsonChrome(testDoc1),
                BuildDocIndexJsonChrome(testDoc2));

            _mockEsClient.Setup(c => c.Bulk(expectedBody)).Returns(true);

            // act
            _testWorker.Index(testDocs).Should().BeTrue();

            // assert
            _mockRepo.VerifyAll();
        }

        private EsDocument GenerateTestDoc(string id)
        {
            var testDoc = 
                JObject.Parse(
                    string.Format("{{ \"_id\" : \"{0}\", \"dude\" : true }}", id));

            return new EsDocument(TEST_INDEX, TEST_TYPE, testDoc);
        }

        private string BuildDocIndexJsonChrome(EsDocument doc)
        {
            return string.Format(
@"{{""index"":{{""_index"":""{0}"",""_type"":""{1}"",""_id"":""{2}""}}}}
{3}
",
 _mockConfig.Object.Index, _mockConfig.Object.Type, doc.Id, doc.Data.ToString(Newtonsoft.Json.Formatting.None));
        }
    }
}
