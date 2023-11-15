using AutoMapper;
using MappingObjects.Mappers;

namespace MappingObjects.Tests;

public class UnitTest1
{
    [Fact]
    public void TestSummaryMapping()
    {
        MapperConfiguration config = CartToSummaryMapper.GetMapperConfiguration();

        config.AssertConfigurationIsValid();
    }
}