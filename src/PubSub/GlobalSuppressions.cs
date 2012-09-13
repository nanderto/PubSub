//-----------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Scope = "type", Target = "Phantom.PubSub.RemoveMessageFromQueue`1", Justification = "Does not have incorrect surrix")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Scope = "member", Target = "Phantom.PubSub.AssemblyLocator`1.#GetAssemblies()", Justification = "part of design to allow static members for each specialization of generic class")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dlls", Scope = "member", Target = "Phantom.PubSub.AssemblyLocator`1.#GetDlls()", Justification = "Spelling is correct")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Phantom.PubSub.IQueueProvider`1.#ProcessQueueAsBatch(System.Func`3<Phantom.PubSub.MessagePacket`1<!0>,System.String,System.Boolean>)", Justification = "Required by design")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Msmq", Scope = "type", Target = "Phantom.PubSub.MsmqQueueProvider`1", Justification = "Spelling is correct")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "Phantom.PubSub.IPublishSubscribeChannel`1.#GetSubscriptions()", Justification = "Properties are not appropriate in this case")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom", Scope = "member", Target = "Phantom.PubSub.AssemblyLocator`1.#GetSubscribersInBin()", Justification = "Method is not problematic")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Scope = "member", Target = "Phantom.PubSub.Subscriber`1.#SubscribersForThisMessage", Justification = "Not available for this collection")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Esent", Scope = "type", Target = "Phantom.PubSub.EsentStoreProvider`1")]
