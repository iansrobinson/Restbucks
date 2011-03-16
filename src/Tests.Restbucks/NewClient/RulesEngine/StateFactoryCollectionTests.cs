using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using Restbucks.NewClient.RulesEngine;
using Rhino.Mocks;

namespace Tests.Restbucks.NewClient.RulesEngine
{
    [TestFixture]
    public class StateFactoryCollectionTests
    {
        private static readonly HttpResponseMessage Response = new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted};
        private static readonly ApplicationContext Context = new ApplicationContext();
        private static readonly IState DummyState = MockRepository.GenerateStub<IState>();
        private static readonly IStateFactory DummyWorker = MockRepository.GenerateStub<IStateFactory>();
        
        [Test]
        public void ShouldInvokeCorrectWorkerBasedOnHttpStatusCode()
        {
            var mockWorker = MockRepository.GenerateMock<IStateFactory>();   
            mockWorker.Expect(w => w.Create(Response, Context)).Return(DummyState);

            var factory = new StateFactoryCollection(new Dictionary<HttpStatusCode, IStateFactory> {{HttpStatusCode.Accepted, mockWorker}});

            factory.Create(Response, Context);

            mockWorker.VerifyAllExpectations();
        }

        [Test]
        public void ShouldInvokeDefaultWorkerIfStatusCodeIsNotRecognized()
        {
            var mockDefaultWorker = MockRepository.GenerateMock<IStateFactory>();
            mockDefaultWorker.Expect(w => w.Create(Response, Context)).Return(DummyState);

            var factory = new StateFactoryCollection(new Dictionary<HttpStatusCode, IStateFactory> { { HttpStatusCode.OK, DummyWorker } }, mockDefaultWorker);

            factory.Create(Response, Context);

            mockDefaultWorker.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnUnsuccesfulStateIfStatusCodeIsNotRecognizedAndDefaultWorkerIsNotSupplied()
        {
            var factory = new StateFactoryCollection(new Dictionary<HttpStatusCode, IStateFactory> { { HttpStatusCode.OK, DummyWorker } });
            Assert.IsInstanceOf(typeof(UnsuccessfulState), factory.Create(Response, Context));
        }
    }
}