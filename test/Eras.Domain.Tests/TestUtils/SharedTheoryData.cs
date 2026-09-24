namespace Eras.Domain.Tests.TestUtils;
public class RequiredStringTestData : TheoryData<string?>
{
    public RequiredStringTestData()
    {
        Add(null);
        Add("");
    }
}

public class UuidFormatTestData : TheoryData<string>
{
    public UuidFormatTestData()
    {
        Add("7gd01d8z-37fh-4b4a-9537-03epa0916f25");
        Add("7cd01d8a_37fc_4b4a_9537_03eaa0916f25");
        Add("7cd01d8a-3#fc-4b4a-9537-03eaa0916f25");
    }
}

public class EmailFormatTestData : TheoryData<string>
{
    public EmailFormatTestData()
    {
        Add("plainaddress");
        Add("@domain.com");
        Add("user@");
        Add("user@.com");
        Add("user@domain.co1");
        Add("user @test.com");
    }
}

public class ValidErasRolesTestData : TheoryData<string>
{
    public ValidErasRolesTestData()
    {
        Add("ERAS Administrator");
        Add("ERAS Student Services Officer");
        Add("ERAS Professional");
        Add("ERAS Guest");
    }
}
