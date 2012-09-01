namespace Phantom.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public delegate void OnProcessStartedEventHandler(object sender, ProcessStartedEventArgs e);

    public delegate void OnProcessCompletedEventHandler(object sender, ProcessCompletedEventArgs e);

    public enum QueueTransactionOption
    {
        SupportTransactions,
        NoTransactions
    }
}
