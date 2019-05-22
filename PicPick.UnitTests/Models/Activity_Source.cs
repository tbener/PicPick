using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PicPick.Models;

namespace PicPick.UnitTests.Models
{
    /// <summary>
    /// To Test:
    /// - Filter
    /// </summary>
    public class Activity_Source
    {
        [TestMethod]
        public void Source_Filter_FileList(string filter, bool includeSubFolders)
        {
            // arrange
            var source = new PicPickProjectActivitySource()
            {
                Path = "",
                Filter = filter,
                IncludeSubFolders = includeSubFolders
            };

            // act
            var files = source.FileList;

            // assert
        }
    }
}
