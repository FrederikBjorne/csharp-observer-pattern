using System;
using System.Collections.Generic;


/// <summary>
/// This poco class provides information about arriving flights and the carousels where baggage from each flight
/// is available for pickup. Creating a baggage info object with carousel zero (default) means that the carousel
/// is empty and all baggages has been collected.
/// </summary>
public class BaggageInfo
{    private int flightNo, location;
    private string origin;

    /// <summary>Constructor setting its status..</summary>
    /// <param name="flightNo">The flight number.</param>
    /// <param name="origin">The airport from which the flight originated.</param>
    /// <param name="carousel">The assigned carousel for this flight to pick up baggage. Zero value means empty.
    /// </param>    
    internal BaggageInfo(int flightNo, string origin = "", int carousel = 0)
    {
        this.flightNo = flightNo;
        this.origin = origin;
        this.location = carousel;
    }

    public int FlightNumber
    {
        get => this.flightNo;
    }

    public string From
    {
        get => this.origin;
    }

    public int Carousel
    {
        get => this.location;
    }

    /// <summary>Returns true, if a baggage claim has been appointed and false if not.</summary>
    public bool IsBaggageClaimAssigned()
    {
        return this.location != 0;
    }
}
