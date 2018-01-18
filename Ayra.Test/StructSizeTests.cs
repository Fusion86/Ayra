using System.Runtime.InteropServices;
using Xunit;

namespace Ayra.Test
{
    public class StructSizeTests
    {
        #region WUP

        [Fact]
        public void WUP__TMD_Header()
        {
            Assert.Equal(0x1E4, Marshal.SizeOf(new Core.Structs.WUP._TMD_Header()));
        }

        #endregion WUP
       
        #region CTR

        [Fact]
        public void CTR__TMD_Header()
        {
            Assert.Equal(0xC4, Marshal.SizeOf(new Core.Structs.CTR._TMD_Header()));
        }

        [Fact]
        public void CTR__TMD_ContentInfoRecord()
        {
            Assert.Equal(0x24, Marshal.SizeOf(new Core.Structs.CTR._TMD_ContentInfoRecord()));
        }

        #endregion CTR
    }
}
