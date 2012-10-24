//-----------------------------------------------------------------------
// <copyright file="IUnitOfWork.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//----------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Unit of work interface
    /// </summary>
    public interface IUnitOfWork
    {
        void Commit(); 

        void Rollback();
    }
}
