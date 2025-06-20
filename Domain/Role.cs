namespace Domain;

public sealed class Role
{
    public static Role MemberRole = new Role(0, "Member");
    public static Role AdministratorRole = new Role(1, "Admin");

    public int Id { get; private set; }
    public string Name { get; private set; }

    private Role(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Role? FromId(int id)
    {
        if (id ==  MemberRole.Id) return MemberRole;
        if (id == AdministratorRole.Id) return AdministratorRole;
        
        return null;
    }
}