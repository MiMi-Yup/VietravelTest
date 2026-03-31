using Application.Common;

namespace Tests.Common;

[TestFixture]
public class PagedListTests
{
    [Test]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var items = new List<string> { "a", "b", "c" };
        var paged = new PagedList<string>(items, totalCount: 30, pageNumber: 2, pageSize: 10);

        Assert.That(paged.Items, Has.Count.EqualTo(3));
        Assert.That(paged.TotalCount, Is.EqualTo(30));
        Assert.That(paged.PageNumber, Is.EqualTo(2));
        Assert.That(paged.PageSize, Is.EqualTo(10));
    }

    [Test]
    public void TotalPages_CalculatesCorrectly()
    {
        var paged = new PagedList<int>(new List<int>(), totalCount: 25, pageNumber: 1, pageSize: 10);
        Assert.That(paged.TotalPages, Is.EqualTo(3)); // ceil(25/10)
    }

    [Test]
    public void TotalPages_ExactDivision()
    {
        var paged = new PagedList<int>(new List<int>(), totalCount: 20, pageNumber: 1, pageSize: 10);
        Assert.That(paged.TotalPages, Is.EqualTo(2));
    }

    [Test]
    public void HasPrevious_FirstPage_ReturnsFalse()
    {
        var paged = new PagedList<int>(new List<int>(), 10, 1, 5);
        Assert.That(paged.HasPrevious, Is.False);
    }

    [Test]
    public void HasPrevious_SecondPage_ReturnsTrue()
    {
        var paged = new PagedList<int>(new List<int>(), 10, 2, 5);
        Assert.That(paged.HasPrevious, Is.True);
    }

    [Test]
    public void HasNext_LastPage_ReturnsFalse()
    {
        var paged = new PagedList<int>(new List<int>(), 10, 2, 5);
        Assert.That(paged.HasNext, Is.False);
    }

    [Test]
    public void HasNext_NotLastPage_ReturnsTrue()
    {
        var paged = new PagedList<int>(new List<int>(), 10, 1, 5);
        Assert.That(paged.HasNext, Is.True);
    }

    [Test]
    public void EmptyList_HasCorrectDefaults()
    {
        var paged = new PagedList<int>(new List<int>(), 0, 1, 10);
        Assert.That(paged.TotalPages, Is.EqualTo(0));
        Assert.That(paged.HasPrevious, Is.False);
        Assert.That(paged.HasNext, Is.False);
    }
}
