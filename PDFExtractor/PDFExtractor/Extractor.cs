using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Analytics.Interfaces;
using System.Collections.Generic;

namespace PDFExtractor
{
    [SqlUserDefinedExtractor(AtomicFileProcessing = true)]
    public class Extractor : IExtractor
    {
        public override IEnumerable<IRow> Extract(IUnstructuredReader input, IUpdatableRow output)
        {
            var reader = new PdfReader(input.BaseStream);
            for (var page = 1; page <= reader.NumberOfPages; page++)
            {
                output.Set(0, page);
                output.Set(1, ExtractText(reader, page));
                yield return output.AsReadOnly();
            }
        }

        public string ExtractText(PdfReader pdfReader, int pageNum)
        {
            var text = PdfTextExtractor.GetTextFromPage(pdfReader, pageNum, new LocationTextExtractionStrategy());
            // Encode new lines to prevent from line breaking in text editors,
            // I want nice line after line files
            return text.Replace("\r", "\\r").Replace("\n", "\\n");
        }
    }

}