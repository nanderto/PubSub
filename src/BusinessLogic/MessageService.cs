using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using System.Data.Objects;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace BusinessLogic
{
    public class MessageService
    {
        public int SaveMessage(Message m)
        {
            m.MessageReadTime = DateTime.Now;
            using (PubSubEntities context = new PubSubEntities())
            {
                int result;
                if (context.Messages.Where(mes => mes.Guid == m.Guid).Count() > 0)
                {
                    context.AttachTo("Messages",m);
                    try
                    {
                        LogEntry log = new LogEntry();
                        log.Message = ("Saving Message to database::Message::Attaching::Guid:" + m.Guid + "::MessageID:" + m.MessageID);
                        Logger.Write(log, "DataLayer");

                        result = context.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException)
                    {
                        LogEntry log = new LogEntry();
                        log.Message = ("Saving Message to database::Message::OptimisticConcurrencyException::Guid:" + m.Guid + "::MessageID:" + m.MessageID);
                        Logger.Write(log);

                        context.Refresh(RefreshMode.ClientWins, m);
                        result = context.SaveChanges();
                    }
                }
                else
                {
                    LogEntry log = new LogEntry();
                    log.Message = ("Saving Message to database::Message::AddObject::Guid:" + m.Guid + "::MessageID:" + m.MessageID);
                    Logger.Write(log);

                    context.Messages.AddObject(m);
                    result = context.SaveChanges();
                }
                return result;
            }
        }

        public Message GetMessage(int ID)
        {
            using (PubSubEntities context = new PubSubEntities())
            {
                var result = context.Messages.Where(m => m.ID == ID);
                if (result.Any())
                {
                    return result.First();
                }
                else
                {
                    return null;
                }
            }
        }

        public bool DeleteMessage(Message m)
        {
            using (PubSubEntities context = new PubSubEntities())
            {
                context.Attach(m);
                context.Refresh(RefreshMode.StoreWins,m);
                context.Messages.DeleteObject(m);
                context.SaveChanges();
            }
            return true;
        }
    }

    public class MessageService2
    {
        public int SaveMessage(Message m)
        {
            Message2 m2 = new Message2();
            m2.BatchNumber = m.BatchNumber;
            m2.Guid = m.Guid;
            m2.MessageID = m.MessageID;
            m2.MessagePutTime = m.MessagePutTime;
            m2.Name = m.Name;
            m2.SubscriptionID = m.SubscriptionID;
            m2.MessageReadTime = DateTime.Now;
            using (PubSubEntities context = new PubSubEntities())
            {
                int result;
                if (context.Message2.Where(mes => mes.Guid == m.Guid).Count() > 0)
                {
                    context.AttachTo("Message2", m2);
                    try
                    {
                        LogEntry log = new LogEntry();
                        log.Message = ("Saving Message to database::Message2::Attaching::Guid:" + m.Guid + "::MessageID:" + m.MessageID);
                        Logger.Write(log);

                        result = context.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException)
                    {
                        LogEntry log = new LogEntry();
                        log.Message = ("Saving Message to database::Message2::OptimisticConcurrencyException::Guid:" + m.Guid + "::MessageID:" + m.MessageID);
                        Logger.Write(log);

                        context.Refresh(RefreshMode.ClientWins, m2);
                        result = context.SaveChanges();
                    }
                }
                else
                {
                    LogEntry log = new LogEntry();
                    log.Message = ("Saving Message to database::Message2::AddObject::Guid:" + m.Guid + "::MessageID:" + m.MessageID);
                    Logger.Write(log);

                    context.Message2.AddObject(m2);
                    result = context.SaveChanges();
                }
                return result;
            }
        }

    }

    public class MessageService3
    {
        public int SaveMessage(Message m)
        {
            Message3 m3 = new Message3();
            m3.BatchNumber = m.BatchNumber;
            m3.Guid = m.Guid;
            m3.MessageID = m.MessageID;
            m3.MessagePutTime = m.MessagePutTime;
            m3.Name = m.Name;
            m3.SubscriptionID = m.SubscriptionID;
            m3.MessageReadTime = DateTime.Now;
            using (PubSubEntities context = new PubSubEntities())
            {
                int result;
                if (context.Message3.Where(mes => mes.Guid == m.Guid).Count() > 0)
                {
                    context.AttachTo("Message3", m3);
                    try
                    {
                        LogEntry log = new LogEntry();
                        log.Message = ("Saving Message to database::Message3::Attaching::Guid:" + m.Guid + "::MessageID:" + m.MessageID);
                        Logger.Write(log);

                        result = context.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException)
                    {
                        LogEntry log = new LogEntry();
                        log.Message = ("Saving Message to database::Message3::OptimisticConcurrencyException::Guid:" + m.Guid + "::MessageID:" + m.MessageID);
                        Logger.Write(log);

                        context.Refresh(RefreshMode.ClientWins, m3);
                        result = context.SaveChanges();
                    }
                }
                else
                {
                    LogEntry log = new LogEntry();
                    log.Message = ("Saving Message to database::Message3::AddObject::Guid:" + m.Guid + "::MessageID:" + m.MessageID);
                    Logger.Write(log);

                    context.Message3.AddObject(m3);
                    result = context.SaveChanges();
                }
                return result;
            }
        }


    }

    public class MessageService4
    {
        public int SaveMessage(Message m)
        {
            Message4 m4 = new Message4();
            m4.BatchNumber = m.BatchNumber;
            m4.Guid = m.Guid;
            m4.MessageID = m.MessageID;
            m4.MessagePutTime = m.MessagePutTime;
            m4.Name = m.Name;
            m4.SubscriptionID = m.SubscriptionID;
            m4.MessageReadTime = DateTime.Now;
            using (PubSubEntities context = new PubSubEntities())
            {
                int result;
                if (context.Message4.Where(mes => mes.Guid == m.Guid).Count() > 0)
                {
                    context.AttachTo("Message4", m4);
                    try
                    {
                        LogEntry log = new LogEntry();
                        log.Message = ("Saving Message to database::Message4::Attaching::Guid:" + m.Guid + "::MessageID:" + m.MessageID);
                        Logger.Write(log);

                        result = context.SaveChanges();
                    }
                    catch (OptimisticConcurrencyException)
                    {
                        LogEntry log = new LogEntry();
                        log.Message = ("Saving Message to database::Message4::OptimisticConcurrencyException::Guid:" + m.Guid + "::MessageID:" + m.MessageID);
                        Logger.Write(log);

                        context.Refresh(RefreshMode.ClientWins, m4);
                        result = context.SaveChanges();
                    }
                }
                else
                {
                    LogEntry log = new LogEntry();
                    log.Message = ("Saving Message to database::Message4::AddObject::Guid:" + m.Guid + "::MessageID:" + m.MessageID);
                    Logger.Write(log);

                    context.Message4.AddObject(m4);
                    result = context.SaveChanges();
                }
                return result;
            }
        }

    }

    public class MessageService5
    {
        public int SaveMessage(Message m)
        {
            Message5 m5 = new Message5();
            m5.BatchNumber = m.BatchNumber;
            m5.Guid = m.Guid;
            m5.MessageID = m.MessageID;
            m5.MessagePutTime = m.MessagePutTime;
            m5.Name = m.Name;
            m5.SubscriptionID = m.SubscriptionID;
            m5.MessageReadTime = DateTime.Now;
            using (PubSubEntities context = new PubSubEntities())
            {
                context.Message5.AddObject(m5);
                return context.SaveChanges();
            }
        }

    }
}
