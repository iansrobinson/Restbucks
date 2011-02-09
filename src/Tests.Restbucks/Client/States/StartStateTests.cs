using System;
using System.Collections.Generic;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.States;
using Restbucks.MediaType;
using Restbucks.RestToolkit.Http;
using Rhino.Mocks;
using Tests.Restbucks.Client.States.Helpers;
using Tests.Restbucks.MediaType.Helpers;

namespace Tests.Restbucks.Client.States
{
    [TestFixture]
    public class StartStateTests
    {
        private static readonly Uri EntryPointUri = new Uri("http://localhost/shop/");
     
        [Test]
        public void IsNotATerminalState()
        {
            var state = new StartState(new ApplicationContext(), null);
            Assert.IsFalse(state.IsTerminalState);
        }

        [Test]
        public void IfContextNameIsEmptyShouldCallEntryPointUri()
        {
            var mocks = new MockRepository();
            var userAgent = mocks.StrictMock<IUserAgent>();

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, EntryPointUri);

            var state = new StartState(context, null);

            using (mocks.Record())
            {
                Expect.Call(userAgent.Invoke<Shop>(EntryPointUri, null)).Return(CreateResponse());
            }
            mocks.Playback();

            state.Apply(userAgent);

            mocks.VerifyAll();
        }

        [Test]
        public void IfContextNameIsEmptyShouldReturnNewStateStateWithResponse()
        {
            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, EntryPointUri);

            var response = CreateResponse();

            var state = new StartState(context, null);           
            var newState = state.Apply(CreateStubUserAgent(response));

            Assert.IsInstanceOf<StartState>(newState);
            Assert.AreNotEqual(state, newState);
            Assert.AreEqual(response, PrivateField.GetValue<Response<Shop>>("response", newState));
        }

        private static Response<Shop> CreateResponse()
        {
            var entity = new ShopBuilder().Build();
            return new Response<Shop>(200, new Dictionary<string, IEnumerable<string>>(), entity);
        }

        private static IUserAgent CreateStubUserAgent(Response<Shop> response)
        {
            var userAgent = MockRepository.GenerateStub<IUserAgent>();
            userAgent.Stub(ua => ua.Invoke<Shop>(null, null)).IgnoreArguments().Return(response);
            return userAgent;
        }
    }
}