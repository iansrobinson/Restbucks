using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.ServiceModel.Http;
using NUnit.Framework;
using Restbucks.Quoting.Service.Old;
using Restbucks.Quoting.Service.Old.Processors;
using Restbucks.Quoting.Service.Old.Resources;

namespace Tests.Restbucks.Old.Quoting.Service.Old
{
    [TestFixture]
    public class ConfigTests
    {
        [Test]
        public void ShouldAddFormsIntegrityResponseProcessorToOrderFormResponses()
        {
            var operation = new HttpOperationDescription
                                {
                                    DeclaringContract = new ContractDescription("OrderForm", "http://tempuri.org/")
                                                            {
                                                                ContractType = typeof (OrderForm)
                                                            }
                                };
            var processors = new List<Processor>();

            var processor = new FormsIntegrityResponseProcessor(new DummyFormsSigner());
            var config = new Config(CreateContainer(processor));

            config.RegisterResponseProcessorsForOperation(operation, processors, MediaTypeProcessorMode.Response);

            Assert.IsTrue(processors.Contains(processor));
        }

        [Test]
        public void ShouldNotAddFormsIntegrityResponseProcessorForResponsesToResourcesOtherThanOrderForm()
        {
            var operation = new HttpOperationDescription
            {
                DeclaringContract = new ContractDescription("EntryPoint", "http://tempuri.org/")
                {
                    ContractType = typeof(EntryPoint)
                }
            };
            var processors = new List<Processor>();

            var processor = new FormsIntegrityResponseProcessor(new DummyFormsSigner());
            var config = new Config(CreateContainer(processor));

            config.RegisterResponseProcessorsForOperation(operation, processors, MediaTypeProcessorMode.Response);

            Assert.IsFalse(processors.Contains(processor));
        }

        [Test]
        public void AddsRestbucksMediaTypeProcessorToAllResponses()
        {
            var operation = new HttpOperationDescription
            {
                DeclaringContract = new ContractDescription("AnyResource", "http://tempuri.org/")
                {
                    ContractType = typeof(AnyResource)
                }
            };
            var processors = new List<Processor>();
            
            var config = new Config(new WindsorContainer());
            config.RegisterResponseProcessorsForOperation(operation, processors, MediaTypeProcessorMode.Response);

            Assert.IsNotNull(processors.Find(p => p.GetType().Equals(typeof(RestbucksMediaTypeProcessor))));
        }

        private static WindsorContainer CreateContainer(FormsIntegrityResponseProcessor processor)
        {
            var container = new WindsorContainer();
            container.Register(Component.For(typeof(ISignForms)).ImplementedBy(typeof(DummyFormsSigner)).LifeStyle.Singleton);
            container.Register(Component.For(typeof(FormsIntegrityResponseProcessor)).Instance(processor).LifeStyle.Singleton);
            return container;
        }

        private class DummyFormsSigner : ISignForms
        {
            public void SignForms(Stream streamIn, Stream streamOut)
            {
                throw new NotImplementedException();
            }
        }

        private class AnyResource
        {
        }
    }
}