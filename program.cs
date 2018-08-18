using System;
using System.Collections.Generic;


/// <summary>
/// This main program shows how to use the observer pattern examplified by the baggage application.
///
/// The observer pattern defines a provider (also known as a subject or an observable) and zero, one, or more
/// observers. Observers register with the provider, and whenever a predefined condition, event, or state change
/// occurs, the provider automatically notifies all observers by calling one of their methods. In this method call,
/// the provider can also provide current state information to observers. In the .NET Framework, the observer
/// design pattern is applied by implementing the generic System.IObservable<T> and System.IObserver<T> interfaces.
/// </summary>
public class Example
{
    public static void Main()
    {
        BaggageHandler provider = new BaggageHandler();
        ArrivalsMonitor BaggageClaimMonitor = new ArrivalsMonitor("BaggageClaimMonitor");
        ArrivalsMonitor securityExitMonitor = new ArrivalsMonitor("SecurityExit");

        provider.Update(new BaggageInfo(712, "Detroit", 3));
        BaggageClaimMonitor.Subscribe(provider);
        provider.Update(new BaggageInfo(712, "Kalamazoo", 3));
        provider.Update(new BaggageInfo(400, "New York-Kennedy", 1));
        provider.Update(new BaggageInfo(712, "Detroit", 3));
        securityExitMonitor.Subscribe(provider);
        provider.Update(new BaggageInfo(511, "San Francisco", 2));
        provider.Update(712);
        securityExitMonitor.Unsubscribe();
        provider.Update(400);
        provider.LastBaggageClaimed();
    }
}
// The example displays the following output:
//      Arrivals information from BaggageClaimMonitor
//      Detroit                712    3
//
//      Arrivals information from BaggageClaimMonitor
//      Detroit                712    3
//      Kalamazoo              712    3
//
//      Arrivals information from BaggageClaimMonitor
//      Detroit                712    3
//      Kalamazoo              712    3
//      New York-Kennedy       400    1
//
//      Arrivals information from SecurityExit
//      Detroit                712    3
//
//      Arrivals information from SecurityExit
//      Detroit                712    3
//      Kalamazoo              712    3
//
//      Arrivals information from SecurityExit
//      Detroit                712    3
//      Kalamazoo              712    3
//      New York-Kennedy       400    1
//
//      Arrivals information from BaggageClaimMonitor
//      Detroit                712    3
//      Kalamazoo              712    3
//      New York-Kennedy       400    1
//      San Francisco          511    2
//
//      Arrivals information from SecurityExit
//      Detroit                712    3
//      Kalamazoo              712    3
//      New York-Kennedy       400    1
//      San Francisco          511    2
//
//      Arrivals information from BaggageClaimMonitor
//      New York-Kennedy       400    1
//      San Francisco          511    2
//
//      Arrivals information from SecurityExit
//      New York-Kennedy       400    1
//      San Francisco          511    2
//
//      Arrivals information from BaggageClaimMonitor
//      San Francisco          511    2
