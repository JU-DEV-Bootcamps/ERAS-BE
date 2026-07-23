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