using System;
using System.Collections.Generic;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.States;
using Restbucks.MediaType;
using Restbucks.RestToolkit.Http;
using Rhino.Mocks;
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
            var state = new StartState(new ApplicationContext());
            Assert.IsFalse(state.IsTerminalState);
        }

        [Test]
        public void IfContextNameIsEmptyShouldCallEntryPointUri()
        {
            var mocks = new MockRepository();
            var userAgent = mocks.StrictMock<IUserAgent>();

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, EntryPointUri);

            var state = new StartState(context);

            using (mocks.Record())
            {
                Expect.Call(userAgent.Invoke<Shop>(EntryPointUri, null)).Return(CreateResponse());
            }
            mocks.Playback();

            state.Execute(userAgent);

            mocks.VerifyAll();
        }

        [Test]
        public void IfContextNameIsEmptyShouldReturnNewStateState()
        {
            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, EntryPointUri);

            var state = new StartState(context);
            var newState = state.Execute(CreateStubUserAgent(CreateResponse()));

            Assert.IsInstanceOf<StartState>(newState);
            Assert.AreNotEqual(state, newState);
        }

        [Test]
        public void IfContextNameIsEmptyContextCurrentEntityShouldContainFirstResponse()
        {
            var response = CreateResponse();

            var context = new ApplicationContext();
            context.Set(ApplicationContextKeys.EntryPointUri, EntryPointUri);

            var state = new StartState(context);
            var newState = state.Execute(CreateStubUserAgent(response));

            Assert.AreEqual(response.EntityBody, newState.Context.Get<Shop>(ApplicationContextKeys.CurrentEntity));
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