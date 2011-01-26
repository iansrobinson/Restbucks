using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Restbucks.Quoting;
using Restbucks.Quoting.Implementation;

namespace Tests.Restbucks.Quoting.Implementation
{
    [TestFixture]
    public class QuotationEngineTests
    {
        private static readonly QuotationRequest QuotationRequest = new QuotationRequest(new[]
                                                    {
                                                        new QuotationRequestItem("Costa Rica Tarrazu", new Quantity("g", 250)),
                                                        new QuotationRequestItem("Elephant Beans", new Quantity("g", 300))
                                                    });

        private static readonly Guid Id = Guid.NewGuid();
        private static readonly DateTimeOffset CreatedDateTime = new DateTimeOffset(DateTime.Now);
        
        [Test]
        public void CanCreateQuote()
        {
            var quote = new QuotationEngine(new StubDateTimeProvider(CreatedDateTime), new StubGuidProvider(Id))
                .CreateQuote(QuotationRequest);

            AssertQuoteIsCorrect(quote);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(KeyNotFoundException))]
        public void ThrowsExceptionIfAttemptingToRetrieveNonExistentQuote()
        {
            new QuotationEngine(new DateTimeProvider(), new GuidProvider()).GetQuote(Guid.NewGuid());
        }

        [Test]
        public void CanRetrievePreviouslyCreatedQuote()
        {
            var quoteEngine = new QuotationEngine(new StubDateTimeProvider(CreatedDateTime), new StubGuidProvider(Id));

            try
            {
                quoteEngine.GetQuote(Id);
                Assert.Fail("Expected quote to not exist.");
            }
            catch (KeyNotFoundException)
            {
            }

            quoteEngine.CreateQuote(QuotationRequest);

            var quote = quoteEngine.GetQuote(Id);

            AssertQuoteIsCorrect(quote);
        }

        private static void AssertQuoteIsCorrect(Quotation quotation)
        {
            Assert.AreEqual(Id, quotation.Id);
            Assert.AreEqual(CreatedDateTime, quotation.CreatedDateTime);

            Assert.AreEqual(2, quotation.LineItems.Count());

            var firstItem = quotation.LineItems.First();
            Assert.AreEqual("Costa Rica Tarrazu", firstItem.Description);
            Assert.AreEqual("g", firstItem.Quantity.Measure);
            Assert.AreEqual(250, firstItem.Quantity.Value);
            Assert.AreEqual(2.50, firstItem.Price.Value);
            Assert.AreEqual("GBP", firstItem.Price.Currency);

            var secondItem = quotation.LineItems.Last();
            Assert.AreEqual("Elephant Beans", secondItem.Description);
            Assert.AreEqual("g", secondItem.Quantity.Measure);
            Assert.AreEqual(300, secondItem.Quantity.Value);
            Assert.AreEqual(3.00, secondItem.Price.Value);
            Assert.AreEqual("GBP", secondItem.Price.Currency);
        }

        
    }
}