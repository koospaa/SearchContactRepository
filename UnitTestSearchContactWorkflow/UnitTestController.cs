using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace UnitTestSearchContactWorkflow
{
    public class UnitTestController
    {
        public List<Entity> ContactsList = new List<Entity>();
        IOrganizationService _service;

        public UnitTestController(IOrganizationService service)
        {
            _service = service;
            InitContacts();
        }

        private void InitContacts()
        {
            CreateContact(new Dictionary<string, object>
            {
                { "nickname", "koospaa" },
                { "firstname", "Pavlo" },
                { "lastname", "Kostanian" },
                { "mobilephone", "0997777777" }
            });

            CreateContact(new Dictionary<string, object>
            {
                { "nickname", "testNick" },
                { "firstname", "John" },
                { "lastname", "Smith" },
                { "mobilephone", "0930000011" }
            });

            CreateContact(new Dictionary<string, object>
            {
                { "nickname", "koospaa2" },
                { "firstname", "Pavlo" },
                { "lastname", "Kostanian" },
                { "mobilephone", "0635555555" }
            });

            CreateContact(new Dictionary<string, object>
            {
                { "governmentid", "88878" },
                { "firstname", "Tom" },
                { "lastname", "Grant" },
                { "mobilephone", "0967770077" }
            });
        }

        private void CreateContact(Dictionary<string, object> keyValuePairs)
        {
            Entity newContact = new Entity("contact");

            foreach (var item in keyValuePairs)
            {
                newContact.Attributes.Add(item.Key, item.Value);                
            }

            Guid contactID = _service.Create(newContact);
            newContact.Id = contactID;
            
            ContactsList.Add(newContact);
        }

        public void GetExpectedValues(Dictionary<string, object> inputs, 
                                      out List<EntityReference> expectedList, out int expectedStatus)
        {
            List<EntityReference> resultList = new List<EntityReference>();

            string paramValue = null;
            string paramName = null;

            for (int i = 0; i < inputs.Count; i++)
            {
                if (i % 2 == 0)
                    paramValue = inputs.ElementAt(i).Value.ToString();
                else
                {
                    paramName = inputs.ElementAt(i).Value.ToString();

                    var contact = (from c in ContactsList
                                   where c.Attributes.Any(a => a.Key == paramName && a.Value.ToString() == paramValue)
                                   && !resultList.Select(e => e.Id).Contains(c.Id)
                                   select c).FirstOrDefault();

                    if (contact != null)
                        resultList.Add(new EntityReference(contact.LogicalName, contact.Id));
                }
            }

            expectedList = resultList;
            expectedStatus = expectedList.Count == 0 ? 2 : (expectedList.Count == 1 ? 1 : 3);
        }
    }
}
