namespace JanHafner.Toolkit.Windows.Tests
{
    using System.Drawing;
    using System.IO;

    public static class TestExtensions
    {
        public static Stream ToStream(this Icon icon)
        {
            var result = new MemoryStream();
            icon.Save(result);

            result.Seek(0, SeekOrigin.Begin);

            return result;
        }
    }
}
