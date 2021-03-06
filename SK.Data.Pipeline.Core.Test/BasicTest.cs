﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SK.Data.Pipeline.Core.Test
{
    [TestClass]
    public class BasicTest
    {
        const string SimpleSource = "SimpleSource";
        const string SimpleSourceT = "SimpleSourceT";
        const string SimpleFileOutput = "SimpleFileOutput";
        const string TemplateFileOutput = "TemplateFileOutput";
        const string SampleTemplateFileOutput = "SampleTemplateFileOutput";

        [TestMethod]
        public void FileSerilization()
        {
            PipelineTask.Create(new SingleLineFileSourceNode(SimpleSource))
                    .Spilt(Entity.DefaultColumn)
                    .ToFile(SimpleFileOutput)
                    .Start();

            PipelineTask.Create(new FileSourceNode(SimpleFileOutput))
                    .ToTextFile(SimpleSourceT)
                    .Start();

            Assert.IsTrue(TestHelper.CompareTwoFile(SimpleSource, SimpleSourceT));
        }

        [TestMethod]
        public void FromTextFileToTextFile()
        {
            PipelineTask.Create(new SingleLineFileSourceNode(SimpleSource))
                    .Spilt(Entity.DefaultColumn)
                    .To(new TextFileConsumer(SimpleFileOutput))
                    .Start();

            Assert.IsTrue(TestHelper.CompareTwoFile(SimpleSource, SimpleFileOutput));
        }

        [TestMethod]
        public void SpiltByT()
        {
            PipelineTask.Create(new SingleLineFileSourceNode(SimpleSourceT))
                    .Spilt(Entity.DefaultColumn, separator: "\t")
                    .To(new TextFileConsumer(SimpleFileOutput))
                    .Start();

            Assert.IsTrue(TestHelper.CompareTwoFile(SimpleSource, SimpleFileOutput));
        }

        [TestMethod]
        public void AddTemplateColumn()
        {
            PipelineTask.Create(new SingleLineFileSourceNode(SimpleSourceT))
                    .Spilt(Entity.DefaultColumn, separator: "\t")
                    .AddTemplateColumn("Template", "##col1## ##col2")
                    .ToTextFile(TemplateFileOutput)
                    .Start();

            Assert.IsTrue(TestHelper.CompareTwoFile(SampleTemplateFileOutput, TemplateFileOutput));
        }

        [TestMethod]
        public void FromFileToTemplateFile()
        {
            PipelineTask.Create(new SingleLineFileSourceNode(SimpleSourceT))
                    .Spilt(Entity.DefaultColumn, separator: "\t")
                    .ToTemplateFile(TemplateFileOutput, "##col1## dddd ##col2##")
                    .Start();

            Assert.IsTrue(TestHelper.CompareTwoFile(SimpleSource, SimpleFileOutput));
        }

        [TestMethod]
        public void MonitorConsumer()
        {
            int count = 0;
            PipelineTask.Create(new SingleLineFileSourceNode(SimpleSource))
                    .AddMonitor((sender, args) =>
                                {
                                    count++;
                                })
                    .Start();

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void WebSource()
        {
            int count = 0;
            string content = null;
            PipelineTask.Create(new WebSourceNode(@"http://www.bing.com"))
                    .AddMonitor((sender, args) =>
                    {
                        count++;
                        content = args.CurrentEntity.GetValue<string>(Entity.DefaultColumn);
                    })
                    .Start();

            Assert.AreEqual(1, count);
            Assert.IsNotNull(content);
        }

        [TestMethod]
        public void EntityModelTest()
        {
            EntityModel model = new EntityModel("col1", "col2");
            model.AddColumn("col3", "col3");
            model.AddColumn("col4", typeof(int));

            var testEntity = new Entity();
            testEntity.SetValue("col5", "test");

            var clonedEntity = testEntity.Clone();
            clonedEntity.AddDefaultInfo(model);
            Assert.IsTrue(clonedEntity.Columns.Length == 5);

            clonedEntity = testEntity.Clone();
            clonedEntity.ToStandradEntity(model);
            Assert.IsTrue(clonedEntity.Columns.Length == 4);
            Assert.IsTrue(!clonedEntity.Values.ContainsKey("col5"));
        }
    }
}
