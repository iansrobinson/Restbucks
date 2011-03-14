using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using NUnit.Framework;
using Restbucks.MediaType;
using Restbucks.NewClient.RulesEngine;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class ApplicationContextTests
    {
        private static readonly ExampleObject FirstValue = new ExampleObject();
        private static readonly ExampleObject SecondValue = new ExampleObject();
        
        private static readonly ApplicationContext Context = new ApplicationContext(
                new KeyValuePair<IKey, object>(new StringKey("key"), FirstValue),
                new KeyValuePair<IKey, object>(new EntityBodyKey(new MediaTypeHeaderValue(RestbucksMediaType.Value), new Uri("http://schemas/shop")), SecondValue));
        
        [Test]
        public void ShouldReturnDataByKey()
        {
            Assert.AreEqual(FirstValue, Context.Get<ExampleObject>(new StringKey("key")));
            Assert.AreEqual(SecondValue, Context.Get<ExampleObject>(new EntityBodyKey(new MediaTypeHeaderValue(RestbucksMediaType.Value), new Uri("http://schemas/shop"))));
        }

        [Test]
        [ExpectedException(typeof(InvalidCastException))]
        public void ThrowsExceptionIfReturnValueIsDifferentTypeFromGenericParameter()
        {   
            Context.Get<DifferentObject>(new StringKey("key"));
        }

        private class ExampleObject
        {
        }

        private class DifferentObject
        {           
        }
    }
}