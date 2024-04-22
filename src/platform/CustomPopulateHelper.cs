using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Sitecore.ContentSearch.SolrProvider.Pipelines.PopulateSolrSchema;
using SolrNet.Schema;

namespace XmCloudSXAStarter
{
    public class CustomPopulateHelper: SchemaPopulateHelper
    {
        public CustomPopulateHelper(SolrSchema schema) : base(schema)
        {
        }

        public override IEnumerable<XElement> GetAllFields()
        {
            return base.GetAllFields().Union(GetAddCustomFields());
        }
        public override IEnumerable<XElement> GetAllFieldTypes()
        {
            return base.GetAllFieldTypes().Union(GetAddCustomFieldTypes());
        }
        private IEnumerable<XElement> GetAddCustomFields()
        {
            yield return CreateField("*_t_sr",
                "text_general",
                isDynamic: true,
                required: false,
                indexed: true,
                stored: true,
                multiValued: false,
                omitNorms: false,
                termOffsets: false,
                termPositions: false,
                termVectors: false);
        }
        private IEnumerable<XElement> GetAddCustomFieldTypes()
        {
            var fieldType = CreateFieldType("custom_text", "solr.TextField",
                new Dictionary<string, string>
                {
                    { "positionIncrementGap", "100" },
                    { "multiValued", "false" },
                });
            var indexAnalyzer = new XElement("indexAnalyzer");
            indexAnalyzer.Add(new XElement("tokenizer", new XElement("class", "solr.StandardTokenizerFactory")));
            indexAnalyzer.Add(new XElement("filters", new XElement("class", "solr.StopFilterFactory"), new XElement("ignoreCase", "true"), new XElement("words", "stopwords.txt")));
            indexAnalyzer.Add(new XElement("filters", new XElement("class", "solr.LowerCaseFilterFactory")));
            fieldType.Add(indexAnalyzer);
            var queryAnalyzer = new XElement("queryAnalyzer");
            queryAnalyzer.Add(new XElement("tokenizer", new XElement("class", "solr.StandardTokenizerFactory")));
            queryAnalyzer.Add(new XElement("filters", new XElement("class", "solr.StopFilterFactory"), new XElement("ignoreCase", "true"), new XElement("words", "stopwords.txt")));
            queryAnalyzer.Add(new XElement("filters", new XElement("class", "solr.SynonymFilterFactory"), new XElement("synonyms", "synonyms.txt"), new XElement("ignoreCase", "true"), new XElement("expand", "true")));
            queryAnalyzer.Add(new XElement("filters", new XElement("class", "solr.LowerCaseFilterFactory")));
            fieldType.Add(queryAnalyzer);
            yield return fieldType;
        }
    }
}