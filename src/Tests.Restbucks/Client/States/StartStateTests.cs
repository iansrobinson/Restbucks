using System;
using NUnit.Framework;
using Restbucks.Client;
using Restbucks.Client.States;
using Restbucks.MediaType;
using Rhino.Mocks;

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
                Expect.Call(userAgent.Invoke<Shop>(EntryPointUri, null)).Return(null);
            }
            mocks.Playback();

            state.Execute(userAgent);

            mocks.VerifyAll();
        }
    }
}