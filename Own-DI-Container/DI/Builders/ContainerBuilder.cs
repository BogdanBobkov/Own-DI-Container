using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DI.Attributes;

namespace DI.Builders
{
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly Dictionary<Type, object> _registrations = new();
        
        public void Register<T>(T @object)
        {
            var type = typeof(T);
            _registrations.Add(type, @object);
        }

        public T Create<T>(params object[] args) where T : new()
        {
            var type = typeof(T);
            T instance;
            
            var constructors = type.GetConstructors();
            var constructor = constructors.FirstOrDefault(t => t.IsDefined(typeof(InjectAttribute), true));

            if (constructor != null)
            {
                var argsTypes = constructor.GetParameters().Select(p => p.ParameterType);
                var dependencies = argsTypes.Select(Get);
                instance = (T) Activator.CreateInstance(type, dependencies);
            }
            else
            {
                if (constructors.FirstOrDefault(t => t.GetParameters().SequenceEqual(args)) == null)
                {
                    throw new ArgumentException($"DI: Object with type {type} does not have a constructor with such arguments");
                }
                
                instance = (T) Activator.CreateInstance(type, args);
            }
            
            var props = type.GetRuntimeProperties().Where(t => Attribute.IsDefined(t, typeof(InjectAttribute)));
            foreach (var prop in props)
            {
                prop.SetValue(instance, Get(prop.PropertyType));
            }

            var fields = type.GetRuntimeFields().Where(t => Attribute.IsDefined(t, typeof(InjectAttribute)));
            foreach (var field in fields)
            {
                field.SetValue(instance, Get(field.FieldType));
            }

            return instance;
        }

        private object Get(Type type)
        {
            if (!_registrations.TryGetValue(type, out var obj))
            {
                throw new KeyNotFoundException($"DI: Object with type {type} is not registered in Container");
            }

            return obj;
        }
    }
}