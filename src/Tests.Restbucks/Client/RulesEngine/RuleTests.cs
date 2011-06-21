//using System;
//using System.Net.Http;
//using NUnit.Framework;
//using Restbucks.Client;
//using Restbucks.Client.RulesEngine;
//
//namespace Tests.Restbucks.Client.RulesEngine
//{
//    [TestFixture]
//    public class RuleTests
//    {
//        [Test]
//        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: condition")]
//        public void ThrowsExceptionIfConditionIsNull()
//        {
//            new Rule(null, () => new DummyResponseHandler(), c => { }, (r, c) => null);
//        }
//
//        [Test]
//        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: createResponseHandler")]
//        public void ThrowsExceptionIfCreateResponseHandlerFunctionIsNull()
//        {
//            new Rule(() => true, null, c => { }, (r, c) => null);
//        }
//
//        [Test]
//        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: contextAction")]
//        public void ThrowsExceptionIfContextActionIsNull()
//        {
//            new Rule(() => true, () => new DummyResponseHandler(), null, (r, c) => null);
//        }
//
//        [Test]
//        [ExpectedException(ExpectedException = typeof (ArgumentNullException), ExpectedMessage = "Value cannot be null.\r\nParameter name: createState")]
//        public void ThrowsExceptionIfCreateStateFunctionIsNull()
//        {
//            new Rule(() => true, () => new DummyResponseHandler(), c => { }, null);
//        }
//
//        private class DummyResponseHandler : IResponseHandler
//        {
//            public Result<HttpResponseMessage> Handle(HttpResponseMessage response, ApplicationContext context)
//            {
//                throw new NotImplementedException();
//            }
//        }
//    }
//}