// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Developist.Core.Cqrs.Tests
{
    [TestClass]
    public class QueryDispatcherTests
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
        public async Task DispatchAsync_GivenNull_ThrowsArgumentNullException()
        {
            // Arrange
            var queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();
            GetMessageById nullQuery = null;

            // Act
            async Task<Message> action() => await queryDispatcher.DispatchAsync(nullQuery).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(action).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DispatchAsync_GivenGetMessageById_ReturnsMessage()
        {
            // Arrange
            var queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

            var messageId = Guid.NewGuid();
            const string messageText = "Hello world";

            database.Add(messageId, new Message(messageId) { Text = messageText });

            // Act
            var message = await queryDispatcher.DispatchAsync(new GetMessageById { Id = messageId }).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(messageText, message.Text);
        }

        [TestMethod]
        public async Task DispatchAsync_GivenGetMessageById_RunsWrappers()
        {
            // Arrange
            var queryDispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();

            var messageId = Guid.NewGuid();
            const string messageText = "Hello world";

            database.Add(messageId, new Message(messageId) { Text = messageText });

            // Act
            _ = await queryDispatcher.DispatchAsync(new GetMessageById { Id = messageId }).ConfigureAwait(false);

            // Assert
            var queue = new Queue<string>(output);

            Assert.AreEqual($"{nameof(GetMessageByIdHandlerWrapper)}.{nameof(GetMessageByIdHandlerWrapper.HandleAsync)}_Before", queue.Dequeue());
            Assert.AreEqual($"{nameof(GetMessageByIdHandler)}.{nameof(GetMessageByIdHandler.HandleAsync)}", queue.Dequeue());
            Assert.AreEqual($"{nameof(GetMessageByIdHandlerWrapper)}.{nameof(GetMessageByIdHandlerWrapper.HandleAsync)}_After", queue.Dequeue());
        }
    }
}
