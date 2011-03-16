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
            var worker = MockRepository.GenerateMock<IStateFactory>();   
            worker.Expect(w => w.Create(Response, Context)).Return(DummyState);

            var factory = new StateFactoryCollection(new Dictionary<HttpStatusCode, IStateFactory> {{HttpStatusCode.Accepted, worker}});

            factory.Create(Response, Context);

            worker.VerifyAllExpectations();
        }

        [Test]
        public void ShouldInvokeDefaultWorkerIfStatusCodeIsNotRecognized()
        {
            var defaultWorker = MockRepository.GenerateMock<IStateFactory>();
            defaultWorker.Expect(w => w.Create(Response, Context)).Return(DummyState);

            var factory = new StateFactoryCollection(new Dictionary<HttpStatusCode, IStateFactory> { { HttpStatusCode.OK, DummyWorker } }, defaultWorker);

            factory.Create(Response, Context);

            defaultWorker.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnUnsuccesfulStateIfStatusCodeIsNotRecognizedAndDefaultWorkerIsNotSupplied()
        {
            var factory = new StateFactoryCollection(new Dictionary<HttpStatusCode, IStateFactory> { { HttpStatusCode.OK, DummyWorker } });
            Assert.IsInstanceOf(typeof(UnsuccessfulState), factory.Create(Response, Context));
        }
    }
}