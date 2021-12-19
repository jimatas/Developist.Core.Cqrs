// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Cqrs.DependencyInjection;
using Developist.Core.Cqrs.Events;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class EventDispatcherTests
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
                .AddDispatcher()
                .AddHandlersFromAssembly(Assembly.GetExecutingAssembly());

            serviceProvider = services.BuildServiceProvider();
        }

        [TestCleanup]
        public void CleanUp() => (serviceProvider as IDisposable)?.Dispose();

        [TestMethod]
        public async Task DispatchAsync_GivenNull_ThrowsArgumentNullException()
        {
            // Arrange
            var eventDispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();
            MessageCreated nullEvent = null;

            // Act
            async Task action() => await eventDispatcher.DispatchAsync(nullEvent).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(action).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DispatchAsync_ByDefault_DispatchesToAllHandlers()
        {
            // Arrange
            var eventDispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

            var messageId = Guid.NewGuid();
            var messageCreated = new MessageCreated { Id = messageId };

            // Act
            await eventDispatcher.DispatchAsync(messageCreated).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(output.Contains($"{nameof(MessageCreatedDefaultHandler)}.{nameof(MessageCreatedDefaultHandler.HandleAsync)}"));
            Assert.IsTrue(output.Contains($"{nameof(MessageCreatedSecondaryHandler)}.{nameof(MessageCreatedDefaultHandler.HandleAsync)}"));
            Assert.IsTrue(output.Contains($"{nameof(ExceptionThrowingMessageCreatedHandler)}.{nameof(ExceptionThrowingMessageCreatedHandler.HandleAsync)}"));
        }

        [TestMethod]
        public async Task DispatchAsync_DespiteException_DispatchesToAllHandlers()
        {
            // Arrange
            var dispatcher = serviceProvider.GetRequiredService<IEventDispatcher>();

            var messageId = Guid.NewGuid();
            var messageCreated = new MessageCreated { Id = messageId };

            ExceptionThrowingMessageCreatedHandler.ThrowException = true;

            // Act
            try
            {
                await dispatcher.DispatchAsync(messageCreated);
            }
            catch
            {
                // Expected.
            }

            // Assert
            Assert.IsTrue(output.Contains($"{nameof(MessageCreatedDefaultHandler)}.{nameof(MessageCreatedDefaultHandler.HandleAsync)}"));
            Assert.IsTrue(output.Contains($"{nameof(MessageCreatedSecondaryHandler)}.{nameof(MessageCreatedDefaultHandler.HandleAsync)}"));
            Assert.IsFalse(output.Contains($"{nameof(ExceptionThrowingMessageCreatedHandler)}.{nameof(ExceptionThrowingMessageCreatedHandler.HandleAsync)}"));
        }
    }
}
