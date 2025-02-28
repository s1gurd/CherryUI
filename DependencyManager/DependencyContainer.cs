using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CherryUI.DependencyManager
{
    public sealed class DependencyContainer : IDisposable
    {
        public static DependencyContainer Instance => Lazy.Value;
        private static readonly Lazy<DependencyContainer> Lazy = new(() => new DependencyContainer());

        private DependencyContainer()
        {
        }

        private readonly Dictionary<Type, Dependency> _dependencies = new ();

        public void BindAsSingleton<T>(T instance = null) 
            where T : class
        {
            if (!_dependencies.TryAdd(typeof(T), new Dependency
                {
                    BindedType = typeof(T),
                    BindedInstance = instance,
                    BindType = BindingType.Singleton
                }))
            {
                Debug.LogError($"[Dependency Container] Could not add binding of type {typeof(T)} as it is already installed!");
            }
        }

        public void Bind<T>(BindingType bindType) 
            where T : class
        {
            
            if (!_dependencies.TryAdd(typeof(T), new Dependency
                {
                    BindedType = typeof(T),
                    BindedInstance = null,
                    BindType = bindType
                }))
            {
                Debug.LogError($"[Dependency Container] Could not add binding of type {typeof(T)} as it is already installed!");
            }
        }
        
        public void Bind<T0, T1>(BindingType bindType) 
            where T0 : class 
            where T1 : class
        {
            if (!typeof(T1).IsAssignableFrom(typeof(T0)))
            {
                Debug.LogError($"[Dependency Container] Could not add binding of object with type {typeof(T0)} as it is not assignable from {typeof(T0)}!");
                return;
            }
            
            if (!_dependencies.TryAdd(typeof(T0), new Dependency
                {
                    BindedType = typeof(T1),
                    BindedInstance = null,
                    BindType = bindType
                }))
            {
                Debug.LogError($"[Dependency Container] Could not add binding of type {typeof(T0)} as it is already installed!");
            }
        }
        
        public void BindAsSingleton<T0, T1>(T1 instance = null) 
            where T0 : class 
            where T1 : class
        {
            if (!typeof(T1).IsAssignableFrom(typeof(T0)))
            {
                Debug.LogError($"[Dependency Container] Could not add binding of object with type {typeof(T0)} as it is not assignable from {typeof(T0)}!");
                return;
            }
            
            if (!_dependencies.TryAdd(typeof(T0), new Dependency
                {
                    BindedType = typeof(T1),
                    BindedInstance = instance,
                    BindType = BindingType.Singleton
                }))
            {
                Debug.LogError($"[Dependency Container] Could not add binding of type {typeof(T0)} as it is already installed!");
            }
        }

        public void InjectDependencies(IInjectTarget target)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var fields = target.GetType().GetFields(flags)
                .Where(f => f.GetCustomAttributes(typeof(InjectAttribute)).Any());
            var props = target.GetType().GetProperties(flags)
                .Where(f => f.GetCustomAttributes(typeof(InjectAttribute)).Any());

            foreach (var field in fields)
            {
                InjectFieldValue(field);
            }

            foreach (var prop in props)
            {
                InjectPropValue(prop);
            }

            return;

            void InjectFieldValue(FieldInfo field)
            {
                if (_dependencies.TryGetValue(field.FieldType, out var dep))
                {
                    switch (dep.BindType)
                    {
                        case BindingType.Singleton:
                            dep.BindedInstance ??= Activator.CreateInstance(dep.BindedType);
                            field.SetValue(target, dep.BindedInstance);
                            break;
                        case BindingType.Transient:
                            field.SetValue(target, Activator.CreateInstance(dep.BindedType));
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Debug.LogError($"[Dependency Container] {target.GetType()} tried to receive field injection of type {field.FieldType} which is not registered in the container!");
                }
            }
            
            void InjectPropValue(PropertyInfo prop)
            {
                if (_dependencies.TryGetValue(prop.PropertyType, out var dep))
                {
                    switch (dep.BindType)
                    {
                        case BindingType.Singleton:
                            dep.BindedInstance ??= Activator.CreateInstance(dep.BindedType);
                            prop.SetValue(target, dep.BindedInstance);
                            break;
                        case BindingType.Transient:
                            prop.SetValue(target, Activator.CreateInstance(dep.BindedType));
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Debug.LogError($"[Dependency Container] {target.GetType()} tried to receive property injection of type {prop.PropertyType} which is not registered in the container!");
                }
            }
        }
        
        private class Dependency
        {
            public Type BindedType;
            public object BindedInstance;
            public BindingType BindType;
        }

        public void Dispose()
        {
            foreach (var dep in _dependencies.Values)
            {
                if (dep.BindedInstance is not null && dep.BindedInstance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}