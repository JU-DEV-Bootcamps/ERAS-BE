namespace Eras.Application.Tests.TestUtils;
public class SqlInjectionTestData : TheoryData<string>
{
    public SqlInjectionTestData()
    {
        Add("'; DROP TABLE Students; --");
        Add("/* comment */ SELECT");
        Add("' OR '1'='1");
    }
}

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
    }
}

public class URLFormatTestData : TheoryData<string>
{
    public URLFormatTestData()
    {
        Add("htt/test.a/");
        Add("http//test");
        Add("http//test.sql/api");
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