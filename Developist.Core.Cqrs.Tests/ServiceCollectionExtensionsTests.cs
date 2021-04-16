﻿// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        private readonly Dictionary<Guid, Message> database = new();
        private readonly List<string> output = new();

        private IServiceProvider serviceProvider;

        [TestInitialize]
        public void Initialize()
        {
            var services = new ServiceCollection()
                .AddScoped<IDictionary<Guid, Message>>(_ => database)
                .AddScoped<IList<string>>(_ => output)
                .AddCqrs();

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
        public void AddCqrs_ByDefault_RegistersDispatcherParentInterfacesAsSelf()
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
        public void AddCqrs_ByDefault_RegistersHandlers()
        {
            // Arrange

            // Act
            var commandHandler = serviceProvider.GetService<ICommandHandler<CreateMessage>>();
            var queryHandler = serviceProvider.GetService<IQueryHandler<GetMessageById, Message>>();
            var eventHandlers = serviceProvider.GetServices<IEventHandler<MessageCreated>>();

            // Assert
            Assert.IsNotNull(commandHandler);
            Assert.IsNotNull(queryHandler);
            Assert.IsTrue(eventHandlers.Any());
        }

        [TestMethod]
        public void AddCqrs_ByDefault_RegistersWrappers()
        {
            // Arrange

            // Act
            var commandHandlerWrappers = serviceProvider.GetServices<ICommandHandlerWrapper<CreateMessage>>();
            var queryHandlerWrappers = serviceProvider.GetServices<IQueryHandlerWrapper<GetMessageById, Message>>();

            // Assert
            Assert.IsTrue(commandHandlerWrappers.Any());
            Assert.IsTrue(queryHandlerWrappers.Any());
        }
    }
}
