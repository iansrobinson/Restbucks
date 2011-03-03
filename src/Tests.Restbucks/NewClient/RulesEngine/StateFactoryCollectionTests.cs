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
        [Test]
        public void ShouldInvokeCorrectWorkerBasedOnHttpStatusCode()
        {
            var worker = MockRepository.GenerateMock<IStateFactory>();
            var state = MockRepository.GenerateStub<IState>();
            var response = new HttpResponseMessage {StatusCode = HttpStatusCode.Accepted};

            worker.Expect(w => w.Create(response)).Return(state);

            var factory = new StateFactoryCollection(new Dictionary<HttpStatusCode, IStateFactory> {{HttpStatusCode.Accepted, worker}});

            factory.Create(response);

            worker.VerifyAllExpectations();
        }

        [Test]
        public void ShouldInvokeDefaultWorkerIfStatusCodeIsNotRecognized()
        {
            var defaultWorker = MockRepository.GenerateMock<IStateFactory>();
            var worker = MockRepository.GenerateStub<IStateFactory>();
            var state = MockRepository.GenerateStub<IState>();
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted };

            defaultWorker.Expect(w => w.Create(response)).Return(state);

            var factory = new StateFactoryCollection(new Dictionary<HttpStatusCode, IStateFactory> { { HttpStatusCode.OK, worker } }, defaultWorker);

            factory.Create(response);

            defaultWorker.VerifyAllExpectations();
        }

        [Test]
        public void ShouldReturnUnsuccesfulStateIfStatusCodeIsNotRecognizedAndDefaultWorkerIsNotSupplied()
        {
            var worker = MockRepository.GenerateStub<IStateFactory>();
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted };

            var factory = new StateFactoryCollection(new Dictionary<HttpStatusCode, IStateFactory> { { HttpStatusCode.OK, worker } });

            Assert.IsInstanceOf(typeof(UnsuccessfulState), factory.Create(response));
        }
    }
}