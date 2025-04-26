using System;
using SharedKernel.Primitives;

namespace BMKPlace.Domain.Entities;

public sealed class School : AggregateRoot<int>
{
    public string Name {get; private set;} = string.Empty;

    private School() : base(){}
    private School(int id) : base(id){}

    public static School Create(int id, string name){
        if (id <= 0) throw new ArgumentException("School ID must be positive.", nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        // Potentially add name length validation

        var school = new School(id) { Name = name };
        // Add domain event if needed: school.AddDomainEvent(new SchoolCreatedEvent(...));
        return school;
    }
    
    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        if (Name != name)
        {
            Name = name;
            // Add domain event if needed: AddDomainEvent(new SchoolNameUpdatedEvent(...));
        }
    }
}
