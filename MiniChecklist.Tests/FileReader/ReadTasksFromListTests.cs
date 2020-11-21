using FakeItEasy;
using MiniChecklist.FileReader;
using NUnit.Framework;
using Prism.Events;
using System.Collections.Generic;

namespace MiniChecklist.Tests.FileReader
{
    [TestFixture]
    class ReadTasksFromListTests
    {
        private TaskFileReader _sut;

        [SetUp]
        public void SetUpSut()
        {
            _sut = new TaskFileReader(A.Fake<IEventAggregator>());
        }

        [Test]
        public void ProcessEmptyList_NoError()
        {
            var inputList = new List<string>() { };

            Assert.DoesNotThrow(() => _sut.ReadTasksFromList(inputList));
        }
        
        [Test]
        public void ProcessEmptyLine_NoError()
        {
            var inputList = new List<string>() { "" };

            Assert.DoesNotThrow(() => _sut.ReadTasksFromList(inputList));
        }

        [Test]
        public void ProcessNoLine_NoTasksReturned()
        {
            var inputList = new List<string>() { "" };
            
            var output = _sut.ReadTasksFromList(inputList);

            Assert.AreEqual(0, output.Todos.Count);
        }

        [Test]
        public void ProcessNoLines_NoTasksReturned()
        {
            var iinputListput = new List<string>() { "", "", "" };

            var output = _sut.ReadTasksFromList(iinputListput);

            Assert.AreEqual(0, output.Todos.Count);
        }

        [TestCase("Task")]
        [TestCase("  Task  ")]
        [TestCase("Ridiculosly annoingly long tas task")]
        public void ProcessLinesWithoutDescription_OneTaskReturned(string input)
        {
            var inputList = new List<string>() { input};

            var output = _sut.ReadTasksFromList(inputList);

            Assert.AreEqual(1, output.Todos.Count);
            Assert.AreEqual(input.Trim(), output.Todos[0].Task);
        }

        [Test]
        public void ProcessLineWithDescription_InputIsSplitCorrectly()
        {
            var inputList = new List<string>() { "Task # Descirption" };

            var output = _sut.ReadTasksFromList(inputList);

            Assert.AreEqual(1, output.Todos.Count);
            Assert.AreEqual("Task", output.Todos[0].Task);
            Assert.AreEqual("Descirption", output.Todos[0].Description);
        }

        [TestCase("Task # Descirption")]
        [TestCase("  Task    #    Descirption    ")]
        [TestCase("Ridiculosly annoingly long tas task # Ridiculosly annoingly long tas description ")]
        public void ProcessLinesWithDescription_OneTaskReturned(string input)
        {
            var inputList = new List<string>() { input };

            var output = _sut.ReadTasksFromList(inputList);

            Assert.AreEqual(1, output.Todos.Count);
            Assert.IsTrue(input.TrimStart().StartsWith(output.Todos[0].Task));
            Assert.IsTrue(input.TrimEnd().EndsWith(output.Todos[0].Description));
            Assert.AreNotEqual(input, output.Todos[0].Task);
            Assert.AreNotEqual(input, output.Todos[0].Description);
        }


        [Test]
        public void ProcessFlatSeriesOfLines()
        {
            var inputList = new List<string>() { "Task1", "Task2", "Task3" };

            var output = _sut.ReadTasksFromList(inputList);

            Assert.AreEqual(3, output.Todos.Count);
            Assert.AreEqual("Task1", output.Todos[0].Task);
            Assert.AreEqual("Task2", output.Todos[1].Task);
            Assert.AreEqual("Task3", output.Todos[2].Task);
        }

        [Test]
        public void ProcessContinuouslyNestedSeriesOfLines()
        {
            var inputList = new List<string>() { "Task1", "\tTask1.1", "\t\tTask1.1.1", "Task2" };

            var output = _sut.ReadTasksFromList(inputList);

            Assert.AreEqual(2, output.Todos.Count);
            Assert.AreEqual("Task1", output.Todos[0].Task);
            Assert.AreEqual(1, output.Todos[0].SubList.Count);
            Assert.AreEqual("Task1.1", output.Todos[0].SubList[0].Task);
            Assert.AreEqual(1, output.Todos[0].SubList[0].Count);
            Assert.AreEqual("Task1.1.1", output.Todos[0].SubList[0].SubList[0].Task);
            Assert.AreEqual("Task2", output.Todos[1].Task);
        }

        [Test]
        public void ProcessOneNestedElementInSeriesOfLines()
        {
            var inputList = new List<string>() { "Task1", "\tTask1.1", "Task2" };

            var output = _sut.ReadTasksFromList(inputList);

            Assert.AreEqual(2, output.Todos.Count);
            Assert.AreEqual("Task1", output.Todos[0].Task);
            Assert.AreEqual("Task2", output.Todos[1].Task);
            Assert.AreEqual(1, output.Todos[0].SubList.Count);
            Assert.AreEqual("Task1.1", output.Todos[0].SubList[0].Task);
        }

        [Test]
        public void ProcessTwoNestedElementInSeriesOfLines()
        {
            var inputList = new List<string>() { "Task1", "\tTask1.1", "\tTask1.2", "Task2" };

            var output = _sut.ReadTasksFromList(inputList);

            Assert.AreEqual(2, output.Todos.Count);
            Assert.AreEqual("Task1", output.Todos[0].Task);
            Assert.AreEqual("Task2", output.Todos[1].Task);
            Assert.AreEqual(2, output.Todos[0].SubList.Count);
            Assert.AreEqual("Task1.1", output.Todos[0].SubList[0].Task);
            Assert.AreEqual("Task1.2", output.Todos[0].SubList[1].Task);
        }

        [Test]
        public void ProcessTwoRecusivelyNestedElementInSeriesOfLines()
        {
            var inputList = new List<string>() { "Task1", "\tTask1.1", "\t\tTask1.1.1", "\tTask1.2", "Task2" };

            var output = _sut.ReadTasksFromList(inputList);

            Assert.AreEqual(2, output.Todos.Count);
            Assert.AreEqual("Task1", output.Todos[0].Task);
            Assert.AreEqual("Task2", output.Todos[1].Task);
            Assert.AreEqual(2, output.Todos[0].SubList.Count);
            Assert.AreEqual("Task1.1", output.Todos[0].SubList[0].Task);
            Assert.AreEqual("Task1.2", output.Todos[0].SubList[1].Task);
            Assert.AreEqual(1, output.Todos[0].SubList[0].SubList.Count);
            Assert.AreEqual("Task1.1.1", output.Todos[0].SubList[0].SubList[0].Task);
        }
    }
}
