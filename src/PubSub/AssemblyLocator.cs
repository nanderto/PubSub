//-----------------------------------------------------------------------
// <copyright file="AssemblyLocator.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Finds Subscribers of the type T in the bin and adds them to a collection.
    /// </summary>
    /// <typeparam name="T">The type of message that we are looking for subscribers for</typeparam>
    public static class AssemblyLocator<T>
    {
        ////public static readonly ReadOnlyCollection<Assembly> AllAssemblies = null;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Static members are to interact with data specific to the type of the member")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Need to create ReadOnlyType in constructor")]
        public static readonly ReadOnlyCollection<Type> SubscribersInBin = null;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Need to create ReadOnlyType in constructor")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Static members are to interact with data specific to the type of the member")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dlls", Justification = "Nameing is correct")]
        public static readonly ReadOnlyCollection<string> AllDlls = null;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Need one operation to occur after the other")]
        static AssemblyLocator()
        {
            AllDlls = GetAllDlls();
            SubscribersInBin = GetSubscribersInBin();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Static members are to interact with data specific to the type of the member")]
        public static IEnumerable<Type> TypesImplementingInterface(Assembly[] assemblies, Type desiredType)
        {
            return assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => IsAssignableToGenericType(type, desiredType));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Static members are to interact with data specific to the type of the member")]
        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            if (givenType == null) throw new ArgumentNullException("givenType");
            if (genericType == null) throw new ArgumentNullException("genericType");

            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType)
                {
                    if (it.GetGenericArguments()[0].Name.Equals(genericType.GetGenericArguments()[0].Name))
                        return true;
                }
            }

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return (baseType.IsGenericType &&
                baseType.GetGenericTypeDefinition() == genericType) ||
                IsAssignableToGenericType(baseType, genericType);
        }

        private static ReadOnlyCollection<string> GetAllDlls()
        {
            string binFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            IList<string> dllFiles = Directory.GetFiles(binFolder, "*.dll", SearchOption.TopDirectoryOnly).ToList();
            return new ReadOnlyCollection<string>(dllFiles);
        }

        private static ReadOnlyCollection<Type> GetSubscribersInBin()
        {
            IList<Assembly> assembliesFoundInBin = new List<Assembly>();
            foreach (var item in AllDlls)
            {
                var assembly = System.Reflection.Assembly.LoadFrom(item);
                assembliesFoundInBin.Add(assembly);
    }

            var typesInBin = TypesImplementingInterface(assembliesFoundInBin.ToArray(), typeof(ISubscriber<T>));
            return new ReadOnlyCollection<Type>(typesInBin.ToList<Type>());
        }
    }
}
