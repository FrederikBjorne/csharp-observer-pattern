using System;
using System.Collections.Generic;

public class BaggageInfo
{
    /// <summary>
    /// This class represnts the baggage status information for a new flight arrival
    /// all registered listeners. Creating a baggage info object with carousel zero (default)
    /// means that the carousel is empty and all baggages has been collected.
    /// </summary>
    private int flightNo, location;
    private string origin;

    internal BaggageInfo(int flightNo, string from = "", int carousel = 0)
    {
        this.flightNo = flightNo;
        this.origin = from;
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

    public bool IsBaggageClaimAssigned()
    {
        return this.location != 0;
    }
}