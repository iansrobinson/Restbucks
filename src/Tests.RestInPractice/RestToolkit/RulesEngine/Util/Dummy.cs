using System.Net.Http;
using Restbucks.RestToolkit.RulesEngine;
using Rhino.Mocks;

namespace Tests.RestInPractice.RestToolkit.RulesEngine.Util
{
    public static class Dummy
    {
        public static readonly HttpResponseMessage PreviousResponse = new HttpResponseMessage();
        public static readonly HttpResponseMessage NewResponse = new HttpResponseMessage();
        public static readonly ApplicationStateVariables StateVariables = new ApplicationStateVariables();
        public static readonly IClientCapabilities ClientCapabilities = MockRepository.GenerateStub<IClientCapabilities>();
        public static readonly IState NewState = MockRepository.GenerateStub<IState>();
        
        public static ICondition Condition(bool evaluatesTo)
        {
            var dummyCondition = MockRepository.GenerateStub<ICondition>();
            dummyCondition.Expect(c => c.IsApplicable(null, null)).IgnoreArguments().Return(evaluatesTo);
            return dummyCondition;
        }

        public static IRequestAction GenerateNextRequest()
        {
            var dummyGenerateNextRequest = MockRepository.GenerateStub<IRequestAction>();
            dummyGenerateNextRequest.Expect(g => g.Execute(null, null, null)).IgnoreArguments().Return(NewResponse);
            return dummyGenerateNextRequest;
        }

        public static ICreateNextState CreateNextState()
        {
            var dummyCreateNextState = MockRepository.GenerateStub<ICreateNextState>();
            dummyCreateNextState.Expect(c => c.Execute(null, null, null)).IgnoreArguments().Return(NewState);
            return dummyCreateNextState;
        }
    }
}