﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Http;
using HttpCommanding.Model.Commands;
using HttpCommanding.Model.Results;
using Microsoft.Extensions.DependencyInjection;

namespace HttpCommanding.Client
{
    class Program
    {
        private static IServiceProvider _serviceProvider = null;

        static void Main(string[] args)
        {
            ICommandDispatcher dispatcher = Configure();
            Console.WriteLine("Press a key to execute HTTP command");
            Console.ReadKey();
#pragma warning disable 4014
            ExecuteHttpCommand(dispatcher);
#pragma warning restore 4014
            Console.ReadKey();            
        }

        static ICommandDispatcher Configure()
        {
            Uri uri = new Uri("http://localhost:52933/api/personalDetails");
            ServiceCollection serviceCollection = new ServiceCollection();
            CommandingDependencyResolverAdapter dependencyResolver = serviceCollection.GetCommandingDependencyResolver(() => _serviceProvider);
            
            ICommandRegistry registry = dependencyResolver.AddCommanding();
            dependencyResolver.AddHttpCommanding();
            _serviceProvider = serviceCollection.BuildServiceProvider();

            registry.Register<UpdatePersonalDetailsCommand, UpdateResult>(HttpCommandDispatcherFactory.Create(uri, HttpMethod.Put));

            ICommandDispatcher dispatcher = _serviceProvider.GetService<ICommandDispatcher>();
            return dispatcher;
        }

        static async Task ExecuteHttpCommand(ICommandDispatcher dispatcher)
        {            
            UpdateResult result = await dispatcher.DispatchAsync(
                new UpdatePersonalDetailsCommand
                {
                    Age = 10,
                    Forename = "Jim",
                    Surname = "McCoy",
                    Id = Guid.NewGuid()
                });
            Console.WriteLine(result.DidUpdate);
            Console.WriteLine(result.ValidationMessage);
        }
    }
}