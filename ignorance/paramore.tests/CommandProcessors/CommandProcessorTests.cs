﻿using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Machine.Specifications;
using Paramore.Services.CommandHandlers;
using Paramore.Services.CommandProcessors;
using Paramore.Tests.CommandProcessors.TestDoubles;

namespace Paramore.Tests.CommandProcessors
{
    [Subject("Basic send of a command")]
    public class When_sending_a_command_to_the_processor
    {
        static CommandProcessor commandProcessor;
        static MyCommand myCommand;

        Establish context = () =>
        {
            MyCommandHandler.SetUp();
            myCommand = new MyCommand(Guid.NewGuid()); 
            
            var container = new WindsorContainer();
            container.Register(Component.For<IHandleRequests<MyCommand>>().ImplementedBy<MyCommandHandler>());
            
            commandProcessor = new CommandProcessor(container);

        };

        Because of = () => commandProcessor.Send(myCommand);

        It should_send_the_command_to_the_command_handler = () => MyCommandHandler.ShouldRecieve(myCommand).ShouldBeTrue();
    }

    [Subject("Commands should only have one handler")]
    public class When_there_are_multiple_possible_command_handlers
    {
        static CommandProcessor commandProcessor;
        static MyCommand myCommand;
        private static Exception exception;

        Establish context = () =>
        {
            MyCommandHandler.SetUp();
            myCommand = new MyCommand(Guid.NewGuid()); 
            
            var container = new WindsorContainer();
            container.Register(Component.For<IHandleRequests<MyCommand>>().ImplementedBy<MyCommandHandler>());
            container.Register(Component.For<IHandleRequests<MyCommand>>().ImplementedBy<MyImplicitHandler>());
            
            commandProcessor = new CommandProcessor(container);

        };

        Because of = () =>  exception = Catch.Exception(() => commandProcessor.Send(myCommand));

        It should_fail_because_multiple_recievers_found = () => exception.ShouldBeOfType(typeof (ArgumentException));
        It should_have_an_error_message_that_tells_you_why = () => exception.ShouldContainErrorMessage("More than one handler was found for the typeof command Paramore.Tests.CommandProcessors.TestDoubles.MyCommand - a command should only have one handler.");
    }

    [Subject("Commands should have at least one handler")]
    public class When_there_are_no_command_handlers
    {
        static CommandProcessor commandProcessor;
        static MyCommand myCommand;
        private static Exception exception;

        Establish context = () =>
        {
            MyCommandHandler.SetUp();
            myCommand = new MyCommand(Guid.NewGuid()); 
            
            var container = new WindsorContainer();
            
            commandProcessor = new CommandProcessor(container);

        };

        Because of = () =>  exception = Catch.Exception(() => commandProcessor.Send(myCommand));

        It should_fail_because_multiple_recievers_found = () => exception.ShouldBeOfType(typeof (ArgumentException));
        It should_have_an_error_message_that_tells_you_why = () => exception.ShouldContainErrorMessage("No command handler was found for the typeof command Paramore.Tests.CommandProcessors.TestDoubles.MyCommand - a command should have only one handler."); 
    }
}