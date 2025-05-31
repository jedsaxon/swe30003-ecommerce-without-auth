namespace Domain;

public class PhoneNumber
{
    public string Value { get; set; }

    public PhoneNumber(string phoneNumber)
    {
        Value = phoneNumber;
    }

    public override string ToString() => Value;
}