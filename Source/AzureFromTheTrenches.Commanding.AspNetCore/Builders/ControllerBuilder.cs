﻿using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.AspNetCore.Model;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Builders
{
    internal class ControllerBuilder : IControllerBuilder
    {
        private readonly Dictionary<string, ControllerDefinition> _controllers = new Dictionary<string, ControllerDefinition>();

        IControllerBuilder IControllerBuilder.Controller(string controller, Action<IActionBuilder> actionBuilder)
        {
            return ((IControllerBuilder)this).Controller(controller, null, null, actionBuilder);
        }

        IControllerBuilder IControllerBuilder.Controller(string controller, Action<IAttributeBuilder> attributeBuilder, Action<IActionBuilder> actionBuilder)
        {
            return ((IControllerBuilder)this).Controller(controller, null, attributeBuilder, actionBuilder);
        }

        IControllerBuilder IControllerBuilder.Controller(string controller,
            string route,
            Action<IAttributeBuilder> attributeBuilder,
            Action<IActionBuilder> actionBuilder)
        {
            if (_controllers.ContainsKey(controller))
            {
                throw new ArgumentException(nameof(controller), $"The controller {controller} has already been configured.");
            }

            string resolvedName =
                controller.EndsWith("Controller") ? controller : string.Concat(controller, "Controller");
            ActionBuilder actionBuilderInstance = new ActionBuilder();
            actionBuilder(actionBuilderInstance);
            ControllerDefinition definition = new ControllerDefinition
            {
                Actions = actionBuilderInstance.Actions,
                Name = resolvedName,
                Route = route
            };
            _controllers[resolvedName] = definition;

            attributeBuilder?.Invoke(new AttributeBuilder(definition));

            return this;
        }

        IControllerBuilder IControllerBuilder.Controller(string controller, string route, Action<IActionBuilder> actionBuilder) // TODO: we need to allow attributes to be specified
        {
            return ((IControllerBuilder)this).Controller(controller, route, null, actionBuilder);
        }

        public void SetDefaults(string defaultNamespace, string defaultControllerRoute)
        {
            foreach (ControllerDefinition definition in _controllers.Values)
            {
                if (string.IsNullOrWhiteSpace(definition.Route))
                {
                    definition.Route = defaultControllerRoute;
                }
                if (string.IsNullOrWhiteSpace(definition.Namespace))
                {
                    definition.Namespace = defaultNamespace;
                }
            }
        }

        public IDictionary<string, ControllerDefinition> Controllers => _controllers;

        public IReadOnlyCollection<Type> GetRegisteredCommandTypes()
        {
            HashSet<Type> commandTypes = new HashSet<Type>();
            foreach (ControllerDefinition definition in _controllers.Values)
            {
                foreach (ActionDefinition action in definition.Actions)
                {
                    commandTypes.Add(action.CommandType);
                }
            }

            return commandTypes;
        }
    }
}
