using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Common.Tests {
  [TestClass]
  public class ObjectBaseTests {
    internal TestClass ObjTest;

    [TestInitialize]
    public void Setup() {
      ObjTest = new TestClass();
    }

    [TestMethod]
    public void test_clean_property_change() {
      var propertyChanged = false;

      ObjTest.PropertyChanged += (s, e) => {
        if (e.PropertyName == "CleanProp") {
          propertyChanged = true;
        }
      };

      ObjTest.CleanProp = "test value";

      Assert.IsTrue(propertyChanged, "The property should have triggered change notification.");
    }

    [TestMethod]
    public void test_dirty_set() {
      Assert.IsFalse(ObjTest.IsDirty, "Object should be clean");
      ObjTest.DirtyProp = "test value";
      Assert.IsTrue(ObjTest.IsDirty, "Object should be dirty.");
    }

    [TestMethod]
    public void test_property_change_single_subscription() {
      var changeCounter = 0;
      var handler1 = new PropertyChangedEventHandler((s, e) => { changeCounter++; });
      var handler2 = new PropertyChangedEventHandler((s, e) => { changeCounter++; });

      ObjTest.PropertyChanged += handler1;
      ObjTest.PropertyChanged += handler1; // should not duplicate
      ObjTest.PropertyChanged += handler1; // should not duplicate
      ObjTest.PropertyChanged += handler2;
      ObjTest.PropertyChanged += handler2; // should not duplicate
      ObjTest.CleanProp = "test value";

      Assert.IsTrue(changeCounter == 2, "Property change notification should only have been called once.");
    }

    [TestMethod]
    public void test_child_dirty_tracking() {
      Assert.IsFalse(ObjTest.IsAnythingDirty(), "Nothing in the object graph should be dirty.");
      ObjTest.Child.ChildName = "test value";
      Assert.IsTrue(ObjTest.IsAnythingDirty(), "The object graph should be dirty.");
      ObjTest.CleanAll();
      Assert.IsFalse(ObjTest.IsAnythingDirty(), "Nothing in the object graph should be dirty.");
    }

    [TestMethod]
    public void test_dirty_object_aggregating() {
      var dirtyObjects = ObjTest.GetDirtyObjects();

      Assert.IsTrue(dirtyObjects.Count == 0, "There should be no dirty object returned.");

      ObjTest.Child.ChildName = "test value";
      dirtyObjects = ObjTest.GetDirtyObjects();

      Assert.IsTrue(dirtyObjects.Count == 1, "There should be one dirty object.");

      ObjTest.DirtyProp = "test value";
      dirtyObjects = ObjTest.GetDirtyObjects();

      Assert.IsTrue(dirtyObjects.Count == 2, "There should be two dirty object.");

      ObjTest.CleanAll();
      dirtyObjects = ObjTest.GetDirtyObjects();

      Assert.IsTrue(dirtyObjects.Count == 0, "There should be no dirty object returned.");
    }

    [TestMethod]
    public void test_object_validation() {
      Assert.IsFalse(ObjTest.IsValid, "Object should not be valid as one its rules should be broken.");
      ObjTest.StringProp = "Some value";
      Assert.IsTrue(ObjTest.IsValid, "Object should be valid as its property has been fixed.");
    }
  }
}
