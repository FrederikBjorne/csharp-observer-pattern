using System;
using System.Diagnostics;
using System.Collections.Generic;


/// <summary>
/// This class is a provider of arriving flights and baggage claim carousels updates.
/// Internally, it maintains two collections:
///     observers - A collection of clients that will receive updated information
///     flights - A collection of flights and their assigned carousels
///
/// An observer may subscribe for baggage claim information updates by registering with the method Subscribe.
/// The provider is updated by calling method Update with baggage info.
/// A flight is removed when its dedicated baggage claim carousel is unassigned.
/// <example>For example:
/// <code>
///     BaggageHandler provider = new BaggageHandler();
///     provider.Update(new BaggageInfo(712, "Detroit", 3));
///     provider.LastBaggageClaimed();
/// </code>
/// </example>
/// </summary>
public class BaggageHandler : IObservable<BaggageInfo>
{
    private HashSet<IObserver<BaggageInfo>> observers; // all observers for baggage info updates
    private HashSet<BaggageInfo> flights;

    public BaggageHandler()
    {
        observers = new HashSet<IObserver<BaggageInfo>>();
        flights = new HashSet<BaggageInfo>();
    }

    /// <summary>When the object's Dispose method is called, it checks whether the observer still exists in the
    /// observers collection, and, if it does, removes the observer.</summary>
    internal class Unsubscriber<BaggageInfo> : IDisposable
    {
        private HashSet<IObserver<BaggageInfo>> _observers;
        private IObserver<BaggageInfo> _observer;

        internal Unsubscriber(HashSet<IObserver<BaggageInfo>> observers, IObserver<BaggageInfo> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }

    /// <summary>Called by observers to subscribe for baggage info updates.</summary>
    /// <param name="observer">The subscribing observer object.</param>
    /// <returns>returns an IDisposable implementation that enables observers to stop receiving notifications
    /// before the OnCompleted method is called.</returns>
    public IDisposable Subscribe(IObserver<BaggageInfo> observer)
    {
        // Check whether observer is already registered. If not, add it
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
            // Provide observer with latest status.
            foreach (var item in flights)
                observer.OnNext(item);
        }
        return new Unsubscriber<BaggageInfo>(observers, observer);  // mark for deletion when not referenced
    }

    /// <summary>Called to indicate all baggage is now picked from the assigned carousel.</summary>
    /// <param name="flightNo">The arriving flight number.</param>
    public void Update(int flightNo)
    {
        Update(new BaggageInfo(flightNo));
    }

    /// <summary>Update baggage status for a certain flight. If carousel is not claimed, then baggage info
    /// is removed.
    /// <param name="info">The baggage info for a certained arrived flight.</param>
    /// </summary>
    public void Update(BaggageInfo info)
    {
        Func<bool> isFlightNotKnown = () => !flights.Contains(info);

        if (!info.IsBaggageClaimAssigned())  // If baggage claim is unassigned, the flight is removed
        {
            // Baggage claim for flight is done
            BaggageInfo flightToRemove = null;
            foreach (var flight in flights)
            {
                if (info.FlightNumber.Equals(flight.FlightNumber))
                {
                    foreach (var observer in observers)
                        observer.OnNext(info);  // update observers
                    flightToRemove = flight;
                }
            }
            if (flightToRemove != null)  // If flight was found, remove it
            {
                flights.Remove(flightToRemove);
                Debug.WriteLine("flight {0} is removed!", flightToRemove.FlightNumber);
            }
        }
        else if (isFlightNotKnown())  // Carousel is assigned, so add new baggage info.
        {
            flights.Add(info);
            Debug.WriteLine("flight {0} is added!", info.FlightNumber);
            foreach (var observer in observers)
                observer.OnNext(info);  // update observers
        }
    }

    /// <summary>Called when the last flight of the day has landed and its baggage has been processed.true</summary>
    public void LastBaggageClaimed()
    {
        foreach (var observer in observers)
            observer.OnCompleted(); // the provider has finished sending notifications

        observers.Clear(); // remove all observers
    }
}
