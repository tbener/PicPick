using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPick.Models;

namespace PicPick.UnitTests.Models
{
    [TestClass]
    public class CreateNew
    {
        [TestMethod]
        public void CreateNew_Project()
        {
            // arrange
            string projectName = "projectName";
            string activityName = "activityName";

            // act
            PicPickProject project = PicPickProject.CreateNew(projectName, activityName);

            // assert
            Assert.AreEqual(projectName, project.Name, $"The new Project was expected to be Named: {projectName}");
            Assert.IsNotNull(project.ActivityList, "The new Project was expected to have one Activity");
            Assert.AreEqual(1, project.ActivityList.Count(), "The new Project was expected to have one Activity");
            Assert.AreEqual(activityName, project.ActivityList.First().Name, $"The new Project was expected to have an Activity named: {activityName}");
        }

        [TestMethod]
        public void CreateNew_Activity_CreatedSuccefully()
        {
            // arrange
            string activityName = "activityName";

            // act
            PicPickProjectActivity activity = PicPickProjectActivity.CreateNew(activityName);

            // assert
            Assert.AreEqual(activityName, activity.Name, $"The new Activity was expected to be Named: {activityName}");
        }

        [TestMethod]
        public void CreateNew_Activity_HasSource()
        {
            // arrange
            string activityName = "activityName";

            // act
            PicPickProjectActivity activity = PicPickProjectActivity.CreateNew(activityName);

            // assert
            Assert.IsNotNull(activity.Source, "The new Activity was expected to have a Source instance");
        }

        [TestMethod]
        public void CreateNew_Activity_HasDestination()
        {
            // arrange
            string activityName = "activityName";

            // act
            PicPickProjectActivity activity = PicPickProjectActivity.CreateNew(activityName);

            // assert
            Assert.IsNotNull(activity.DestinationList, "The new Activity was expected to have one Destination");
            Assert.AreEqual(1, activity.DestinationList.Count(), "The new Activity was expected to have one Destination");
        }
    }
}
