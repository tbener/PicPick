using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPick.Project;
using TalUtils;

namespace PicPick.UnitTests
{
    [TestClass]
    public class IsDirty_LoadedProject
    {
        int _isDirtyEventsCount;
        PicPickProject _project;

        [TestInitialize]
        public void LoadProject()
        {
            ProjectLoader.Load(PathHelper.GetFullPath(PathHelper.ExecutionPath(), "test.picpick"));
            _project = ProjectLoader.Project;
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
        public void IsDirty_ChangeExistingActivity_GotDirty()
        {
            // Arrenge
            bool expectedIsDirty = true;

            // Act
            var act = _project.ActivityList[0];
            act.Name = "Change";

            // Assert
            Assert.AreEqual(expectedIsDirty, _project.IsDirty, "The activity name within the project was changed but the project didn't get dirty");
        }

        [TestMethod]
        public void IsDirty_ChangeNewActivity_GotDirty()
        {
            // Arrenge
            bool expectedIsDirty = true;

            // Act
            var newAct = new PicPickProjectActivity();
            _project.ActivityList.Add(newAct);
            _project.IsDirty = false;
            newAct.Name = "Change";

            // Assert
            Assert.AreEqual(expectedIsDirty, _project.IsDirty, "A new activity was added and then its name was changed but the project didn't get dirty");
        }

        [TestMethod]
        public void IsDirty_ChangeActivitySourceProperty_GotDirty()
        {
            // Arrenge
            bool expectedIsDirty = true;

            // Act
            _project.ActivityList[0].Source.Path = "Changed";

            // Assert
            Assert.AreEqual(expectedIsDirty, _project.IsDirty, 
                "The Source path was changed but the project didn't get dirty");
        }

        [TestMethod]
        public void IsDirty_IsDirtyIgnoreAttributePropertyChange_NotDirty()
        {
            // Arrenge
            bool expectedIsDirty = false;

            // Act
            _project.ActivityList[0].DestinationList[0].Mapping = 
                new System.Collections.Generic.Dictionary<string, Core.CopyFilesHandler>();

            // Assert
            Assert.AreEqual(expectedIsDirty, _project.IsDirty,
                "Changed property with [IsDirtyIgnore] attribute and the object got dirty.");
        }


    }
}
