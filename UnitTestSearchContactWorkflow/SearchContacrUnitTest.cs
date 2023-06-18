using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using SearchContactWorkflowExtension;

namespace UnitTestSearchContactWorkflow
{
    [TestClass]
    public class SearchContacrUnitTest
    {
        [TestMethod]
        [Obsolete]
        public void SearchContact_ResultStatusAndContacts()
        {
            int expectedStatus;
            List<EntityReference> expectedContactsCollection = new List<EntityReference>();
            
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var inputs = new Dictionary<string, object>
            {
                { "Parameter1", "Pavlo" },
                { "FirstParameterName", "firstname" },
                { "Parameter2", "Kostanian" },
                { "SecondParameterName", "lastname" }
            };

            UnitTestController controller = new UnitTestController(service);
            controller.GetExpectedValues(inputs, out expectedContactsCollection, out expectedStatus);

            var result = context.ExecuteCodeActivity<SearchContact>(inputs);

            Assert.AreEqual(expectedStatus, (int)result["Status"]);
            CollectionAssert.AreEqual(expectedContactsCollection, (List<EntityReference>)result["SearchedContacts"]);
        }
    }
}
