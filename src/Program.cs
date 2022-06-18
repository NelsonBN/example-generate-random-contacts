using System.Globalization;
using Bogus;
using Bogus.DataSets;
using CsvHelper;
using static System.Console;


var totalContacts = args.Length > 0 ? Convert.ToInt32(args[0]) : 10;

WriteLine($"Generate {totalContacts} contacts");
WriteLine();


var contacts = new Dictionary<string, Contact>();


while(contacts.Count < totalContacts)
{
    var gender = new Faker().PickRandom<Name.Gender>();

    var dataFake = new Faker<Contact>()
        .CustomInstantiator(f => new()
        {
            Name = f.Name.FullName(gender),
            Gender = gender.ToString().ToUpper(),
            DateOfBirth = f.Date.Past(100, DateTime.UtcNow.AddYears(-18)).ToString("yyyy-MM-dd"),
            Country = f.Address.CountryCode(),
            Address = f.Address.FullAddress()
        })
        .RuleFor(r => r.Email, (f, c) => f.Internet.Email(c.Name)?.ToLower());

    var contact = dataFake.Generate();


    if(contacts.ContainsKey(contact.Email))
    { // Prevent duplicated emails
        WriteLine($"DUPLICATED: {contact.Email}");
        continue;
    }


    contacts.Add(contact.Email, contact);
    WriteLine($"ADDED: {contact} - TOTAL: {contacts.Count}");
}


using(var writer = new StreamWriter("contacts.csv"))
using(var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    csv.WriteRecords(contacts.Values);
}

WriteLine();
WriteLine("***** END *****");



public record Contact
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Gender { get; set; }
    public string? DateOfBirth { get; set; }

    public string? Country { get; set; }
    public string? Address { get; set; }
}
