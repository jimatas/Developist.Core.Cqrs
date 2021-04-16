// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Func<Task<Message>> action = async () => await queryDispatcher.DispatchAsync(nullQuery).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        }
    }
}
