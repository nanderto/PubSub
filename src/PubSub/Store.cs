//-----------------------------------------------------------------------
// <copyright file="Store.cs" company="The Phantom Coder">
//     Copyright The Phantom Coder. All rights reserved.
// </copyright>
//-------------------------------------------------------------
namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Store provides a object which will guarantee the instanciation of the database including the creation of it if necessary.
    /// used by IoC containers to hold a reference to the Escent store which will ensure that the ESENT instance is not disposed of before the developer has finnished using it
    /// Should be instanciated by IOC and given an application life time.
    /// </summary>
    /// <typeparam name="T">The type that this component handles</typeparam>
    public class Store<T> : IDisposable
    {
        private EsentStore<T> store;

        /// <summary>
        /// Initializes a new instance of the <see cref="Store{T}" /> class.
        /// </summary>
        /// <param name="storeProvider">The store provider.</param>
        /// <exception cref="System.ArgumentNullException">Argument Null Exception</exception>
        public Store(IStoreProvider<T> storeProvider)
        {
            if (storeProvider == null)
            {
                throw new ArgumentNullException("storeProvider");
            }

            if (storeProvider.GetType() == typeof(EsentStoreProvider<T>))
            {
                this.store = new EsentStore<T>(false);
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Store{T}" /> class.
        /// </summary>
        ~Store()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);    
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.store.Dispose();
                EsentInstanceService.Service.DisposeOfEsentInstanceImmediatly();
            }
        }
    }
}
