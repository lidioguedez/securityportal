namespace SecurityPortal.Domain.ValueObjects;

public sealed class Address : IEquatable<Address>
{
    public string Street { get; private init; }
    public string City { get; private init; }
    public string State { get; private init; }
    public string PostalCode { get; private init; }
    public string Country { get; private init; }

    private Address(string street, string city, string state, string postalCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    public static Address Create(string street, string city, string state, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));

        return new Address(street, city, state, postalCode, country);
    }

    public override string ToString()
    {
        return $"{Street}, {City}, {State} {PostalCode}, {Country}";
    }

    public bool Equals(Address? other)
    {
        return other is not null &&
               Street == other.Street &&
               City == other.City &&
               State == other.State &&
               PostalCode == other.PostalCode &&
               Country == other.Country;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Address);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, City, State, PostalCode, Country);
    }

    public static bool operator ==(Address? left, Address? right)
    {
        return EqualityComparer<Address>.Default.Equals(left, right);
    }

    public static bool operator !=(Address? left, Address? right)
    {
        return !(left == right);
    }
}