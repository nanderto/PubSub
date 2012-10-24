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
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Finds Subscribers of the type T in the bin and adds them to a collection.
    /// </summary>
    /// <typeparam name="T">The type of message that we are looking for subscribers for</typeparam>
    public static class AssemblyLocator<T>
    {
        /// <summary>
        /// Read Only collection of subscribers that were found in the executing directory (bin)
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Static members are to interact with data specific to the type of the member")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Need to create ReadOnlyType in constructor")]
        public static readonly ReadOnlyCollection<Type> SubscribersInBin = null;

        /// <summary>
        /// Read Only Collection of all Dlls that were found in the Executing directory(bin). does not include exe's
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Need to create ReadOnlyType in constructor")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Static members are to interact with data specific to the type of the member")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dlls", Justification = "Nameing is correct")]
        public static readonly ReadOnlyCollection<string> AllDlls = null;

        /// <summary>
        /// Initializes static members of the <see cref="AssemblyLocator{T}" /> class.
        /// The class finds assemblies and processes them to find classes that implement the <see cref="ISubscriber {T}"/> interface. These classes will be automatically added to the subscriptions
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Need one operation to occur after the other")]
        static AssemblyLocator()
        {
            AllDlls = GetAllDlls();
            SubscribersInBin = GetSubscribersInBin();
        }

        /// <summary>
        /// Returns enumerable of types that implement the interface specified
        /// </summary>
        /// <param name="assemblies">Array of assemblies</param>
        /// <param name="desiredType">The type you are looking for</param>
        /// <returns>enumerable of types that have implemented the interface.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Static members are to interact with data specific to the type of the member")]
        public static IEnumerable<Type> TypesImplementingInterface(Assembly[] assemblies, Type desiredType)
        {
            var returnAssemblies = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => IsAssignableToGenericType(type, desiredType));

            return returnAssemblies;
        }

        /// <summary>
        /// Determines whether the Type passes in is assignable to generic type passed in.
        /// </summary>
        /// <param name="givenType">Type to check.</param>
        /// <param name="genericType">Type of the generic to check against.</param>
        /// <returns><c>true</c> if [is assignable to generic type] [the specified given type]; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">Will throw System.ArgumentNullException for null values</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Static members are to interact with data specific to the type of the member")]
        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            if (givenType == null)
            {
                throw new ArgumentNullException("givenType");
            }

            if (genericType == null)
            {
                throw new ArgumentNullException("genericType");
            }

            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType)
                {
                    if (it.GetGenericArguments()[0].Name.Equals(genericType.GetGenericArguments()[0].Name))
                    {
                        return true;
                    }
                }
            }

            Type baseType = givenType.BaseType;
            if (baseType == null)
            {
                return false;
            }

            return (baseType.IsGenericType &&
                baseType.GetGenericTypeDefinition() == genericType) ||
                IsAssignableToGenericType(baseType, genericType);
        }

        /// <summary>
        /// Gets all DLLS found in executing directory.
        /// </summary>
        /// <returns>Read Only Collection of dll names</returns>
        private static ReadOnlyCollection<string> GetAllDlls()
        {
            string binFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            IList<string> dllFiles = Directory.GetFiles(binFolder, "*.dll", SearchOption.TopDirectoryOnly).ToList();
            return new ReadOnlyCollection<string>(dllFiles);
        }

        /// <summary>
        /// Gets the subscribers in bin.
        /// </summary>
        /// <returns>Read Only collection of types found in the executing directory</returns>
        private static ReadOnlyCollection<Type> GetSubscribersInBin()
        {
            IList<Assembly> assembliesFoundInBin = new List<Assembly>();
            foreach (var item in AllDlls)
            {
                var assembly = System.Reflection.Assembly.LoadFrom(item);
                assembliesFoundInBin.Add(assembly);
            }

            var typesInBin = TypesImplementingInterface(assembliesFoundInBin.ToArray(), typeof(ISubscriber<T>));
            var result = new ReadOnlyCollection<Type>(typesInBin.ToList<Type>());
            return result;
        }
    }
}
