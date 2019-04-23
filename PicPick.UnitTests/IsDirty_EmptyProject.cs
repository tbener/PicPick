using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPick.Project;

namespace PicPick.UnitTests
{
    [TestClass]
    public class IsDirty_EmptyProject
    {
        int _isDirtyEventsCount;
        PicPickProject _project;

        [TestInitialize]
        public void SetEmptyProject()
        {
            _project = new PicPickProject();
            _isDirtyEventsCount = 0;
            _project.GetIsDirtyInstance().OnGotDirty += _project_OnGotDirty;
        }

        private void _project_OnGotDirty(object sender, EventArgs e)
        {
            _isDirtyEventsCount++; 
        }

        [TestCleanup]
        public void Cleanup()
        {
            _project.GetIsDirtyInstance().OnGotDirty -= _project_OnGotDirty;
            _project = null;
        }

        [TestMethod]
        public void IsDirty_NoChange_NotDirty()
        {
            // Arrenge
            bool expectedIsDirty = false;

            // Act
            // Do nothing

            // Assert
            Assert.AreEqual(expectedIsDirty, _project.IsDirty);
        }

        [TestMethod]
        public void IsDirty_NoChange_NoEvents()
        {
            // Arrenge
            int expectedEventCount = 0;

            // Act
            // Do nothing

            // Assert
            Assert.AreEqual(expectedEventCount, _isDirtyEventsCount);
        }

        [TestMethod]
        public void IsDirty_OneChangeFirstLevelProperty_IsDirtyTrue()
        {
            // Arrenge
            bool expectedIsDirty = true;

            // Act
            _project.Name = "Changed";

            // Assert
            Assert.AreEqual(expectedIsDirty, _project.IsDirty);
        }

        [TestMethod]
        public void IsDirty_OneChangeFirstLevelProperty_OneEvent()
        {
            // Arrenge
            int expectedEventCount = 1;

            // Act
            _project.Name = "Changed";

            // Assert
            Assert.AreEqual(expectedEventCount, _isDirtyEventsCount);
        }

        /// <summary>
        /// We expect that the event will raise only once, when IsDirty is changed.
        /// </summary>
        [TestMethod]
        public void IsDirty_TwoChangesFirstLevelProperty_OneEvent()
        {
            // Arrenge
            int expectedEventCount = 1;

            // Act
            _project.Name = "Change 1";
            _project.Name = "Change 2";

            // Assert
            Assert.AreEqual(expectedEventCount, _isDirtyEventsCount);
        }

        [TestMethod]
        public void IsDirty_AddActivity_GotDirty()
        {
            // Arrenge
            bool expectedIsDirty = true;

            // Act
            _project.ActivityList.Add(new PicPickProjectActivity());

            // Assert
            Assert.AreEqual(expectedIsDirty, _project.IsDirty);
        }

    }
}
