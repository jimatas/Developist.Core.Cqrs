// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Cqrs.Commands;
using Developist.Core.Cqrs.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class CommandDispatcherTests
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
            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            CreateMessage nullCommand = null;

            // Act
            async Task action() => await commandDispatcher.DispatchAsync(nullCommand).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(action).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DispatchAsync_GivenCreateMessageCommand_CreatesMessage()
        {
            // Arrange
            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            var messageId = Guid.NewGuid();
            var messageText = "Hello world!";

            // Act
            await commandDispatcher.DispatchAsync(new CreateMessage()
            {
                Id = messageId,
                Text = messageText
            }).ConfigureAwait(false);

            database.TryGetValue(messageId, out Message message);

            // Assert
            Assert.IsNotNull(message);
            Assert.AreEqual(messageText, message.Text);
        }

        [TestMethod]
        public async Task DispatchAsync_GivenCreateMessageCommand_RunsWrappersInOrder()
        {
            // Arrange
            var commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();

            var messageId = Guid.NewGuid();
            var messageText = "Hello world!";

            // Act
            await commandDispatcher.DispatchAsync(new CreateMessage()
            {
                Id = messageId,
                Text = messageText
            }).ConfigureAwait(false);

            // Assert
            var queue = new Queue<string>(output);

            Assert.AreEqual($"{nameof(OuterCommandHandlerWrapper<CreateMessage>)}.{nameof(OuterCommandHandlerWrapper<CreateMessage>.HandleAsync)}_Before", queue.Dequeue());
            Assert.AreEqual($"{nameof(InnerCommandHandlerWrapper<CreateMessage>)}.{nameof(InnerCommandHandlerWrapper<CreateMessage>.HandleAsync)}_Before", queue.Dequeue());
            Assert.AreEqual($"{nameof(CreateMessageHandlerWrapper)}.{nameof(CreateMessageHandlerWrapper.HandleAsync)}_Before", queue.Dequeue());
            Assert.AreEqual($"{nameof(CreateMessageHandler)}.{nameof(CreateMessageHandler.HandleAsync)}", queue.Dequeue());
            Assert.AreEqual($"{nameof(CreateMessageHandlerWrapper)}.{nameof(CreateMessageHandlerWrapper.HandleAsync)}_After", queue.Dequeue());
            Assert.AreEqual($"{nameof(InnerCommandHandlerWrapper<CreateMessage>)}.{nameof(InnerCommandHandlerWrapper<CreateMessage>.HandleAsync)}_After", queue.Dequeue());
            Assert.AreEqual($"{nameof(OuterCommandHandlerWrapper<CreateMessage>)}.{nameof(OuterCommandHandlerWrapper<CreateMessage>.HandleAsync)}_After", queue.Dequeue());
        }
    }
}
