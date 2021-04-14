# Developist.Core.Cqrs
CQRS pattern implementation using an in-process message dispatcher.
The messages that can be dispatched are either command, query or event (notification). 
Supports wrapping of message handlers to create processing pipelines for easy extensibility.
