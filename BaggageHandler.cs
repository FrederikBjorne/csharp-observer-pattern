using System;
using System.Diagnostics;
using System.Collections.Generic;


/// <summary>
/// This class represents the baggage handler who receives baggage status information and updates
/// all registered listeners. It holds all arriving flights and removes a flight when its dedicated
/// baggage carousel is emptied.
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
    /// </summary>
    /// <param name="info">The baggage info for a certained arrived flight.</param>
    public void Update(BaggageInfo info)
    {
        Func<bool> isFlightNotKnown = () => !flights.Contains(info);

        if (!info.IsBaggageClaimAssigned())  // this is strange! Shouldn't it be evaluated when creating object?
        {
            // Baggage claim for flight is done
            BaggageInfo flightToRemove = null;
            foreach (var flight in flights)
            {
                if (info.FlightNumber.Equals(flight.FlightNumber))
                {
                    foreach (var observer in observers)
                        observer.OnNext(info);
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

    public void LastBaggageClaimed()
    {
        foreach (var observer in observers)
            observer.OnCompleted();

        observers.Clear(); // remove all observers
    }
}
