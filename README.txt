This library is a light weight publish subscribe mechanism...Ha ha just kidding everyone always claims their component is light weight.

This component is a publish subscribe mechanism with guaranteed delivery backed by some form of disk storage. 
Currently it is supporting Msmq and ESENT as storage mechanisms.
It is not a message bus! and does not abstract the transportation of the messages, only a Publish subscribe component that 
guarantees your message will be published to all subscribers.

Usage
It is designed to be used like a generic List, or Stack. In the end I chose not to create a default storage mechanism but thats mainly because I could not decide
what the best default store would be. 

1. If I want to publish a customer object then first I need to create the type of store you would like to use, in this case I have chosen to use the ESENT store.
IEsentStoreProvider<Customer> store = new EsentStoreProvider<Customer>();

2. Create a new subscriber
var pubsub = new PublishSubscribeChannel<Customer>(new EsentStoreProvider<Customer>()); 

3. Add some subscribers, in this case 2 subscribers with the time that the message will expire.
	pubsub.AddSubscriberType(typeof(SpeedySubscriber<Customer>)).WithTimeToExpire(new TimeSpan(0, 1, 0))
              .AddSubscriberType(typeof(SpeedySubscriber2<Customer>)).WithTimeToExpire(new TimeSpan(0, 0, 100));

4. Now just publish your messages.
       pubsub.PublishMessage(new Customer());

Notes:
- The subscribers need to inherit from ISubscriber
- The creation of the store and channels hides alot of complexity. If you are using the MSMQ then it will check if you have queues set up and create them for you, if you dont.
You get a Msmq Queue for each Messsage type.
If you are using Esent it will create the entire database if necessary. Each Message type gets its own database (although I am thinking of changing this implemntation to
housing them all in a single database.
- The PublishMessage method will save the data to disk in a transaction with what ever transaction you establish, so yhou can save the customer object to 
your database and to the pubsub store and guarantee either both or neither will occur.
- after publishing the channel returns to your calling program and then fires off seperate threads (via Task Parallel Library) to run the subscribers.
- The subscribers will not remove the message from the store until each subscriber has succefully completed.
- If the expiration time hits the subscriber will cancell  and a retry mechanism is activated.
- It will retry on a exponentially longer time span 1 minute, 2 minutes, 4 minutes, 8 minutes, 16 minutes, 32 minutes... 
- Most things are not configurable, that is to make it easy to use.
- It is also possible to use a IoC container to wire up you objects. In this case you need to hold a reference to the store for the life of the application. 

Using IoC (Ninject)

for a "User"
IKernel kernel = new StandardKernel();

kernel.Bind<Store<User>>().ToSelf().InSingletonScope();
kernel.Bind<IStoreProvider<User>>().To<EsentStoreProvider<User>>();
kernel.Bind<IEsentStore<User>>().To<EsentStore<User>>();
kernel.Bind<IPublishSubscribeChannel<User>>().To<PublishSubscribeChannel<User>>();
            
var store = kernel.Get<Store<User>>();

var publishSubscribeChannel = kernel.Get<IPublishSubscribeChannel<User>>();

store.Dispose();

Features A couple of additional features
1) You dont need to add subscribers!! 
When a PubSubChannel is instantiated for the first time if you do not specify a subscriber
then it will search through the executing folder for all components that implement the ISubscriber<YourmatchingType>. It will create a list and save the list for building
each PubSubChannel as they are created.
This functionality is intended to enable developers who are working on a product to provide a mechanism for their custmers to integrate via a Pubsub mechanism.
For example you can add this funtionality to the Save of Customer. 
Then a consumer of your product can create implementations of the ISubscriber<Customer>, throw them in the bin and presto they are integrated.