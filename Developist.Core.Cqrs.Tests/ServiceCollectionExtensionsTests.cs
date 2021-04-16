// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        private IServiceProvider serviceProvider;

        [TestInitialize]
        public void Initialize()
        {
            var services = new ServiceCollection();
            services.AddCqrs();
            serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public void AddCqrs_ByDefault_RegistersDispatchers()
        {
            // Arrange

            // Act
            var dispatcher = serviceProvider.GetService<IDispatcher>();
            var commandDispatcher = serviceProvider.GetService<ICommandDispatcher>();
            var queryDispatcher = serviceProvider.GetService<IQueryDispatcher>();
            var eventDispatcher = serviceProvider.GetService<IEventDispatcher>();

            // Assert
            Assert.IsNotNull(dispatcher);
            Assert.IsNotNull(commandDispatcher);
            Assert.IsNotNull(queryDispatcher);
            Assert.IsNotNull(eventDispatcher);
        }

        [TestMethod]
        public void AddCqrs_ByDefault_RegistersDispatcherSuperTypesAsSelf()
        {
            // Arrange

            // Act
            var dispatcher = serviceProvider.GetService<IDispatcher>();
            var commandDispatcher = serviceProvider.GetService<ICommandDispatcher>();
            var queryDispatcher = serviceProvider.GetService<IQueryDispatcher>();
            var eventDispatcher = serviceProvider.GetService<IEventDispatcher>();

            // Assert
            Assert.AreEqual(dispatcher, commandDispatcher);
            Assert.AreEqual(dispatcher, queryDispatcher);
            Assert.AreEqual(dispatcher, eventDispatcher);
        }

        [TestMethod]
        public void AddCqrs_ByDefault_RegistersCommandHandlers()
        {
            // Arrange

            // Act
            var commandHandler = serviceProvider.GetService<ICommandHandler<CreateMessage>>();

            // Assert
            Assert.IsNotNull(commandHandler);
        }
    }
}
